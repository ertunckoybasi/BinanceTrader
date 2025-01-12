using Application.Settings;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Authentication;

namespace Application.Services;

public sealed class BinanceService
{
    private readonly BinanceRestClient _binanceRestClient;

    public BinanceService()
    {
        BinanceRestClient.SetDefaultOptions(options => options.ApiCredentials = new ApiCredentials(AppSettings.apiKey, AppSettings.apiSecret));
        _binanceRestClient = new BinanceRestClient();
    }

    public async Task<double?> GetCurrentPriceAsync(string symbol)
    {
        var tickerResult = await _binanceRestClient.SpotApi.ExchangeData.GetTickerAsync(symbol);
        return (double?)tickerResult.Data?.LastPrice;
    }

    public async Task<decimal?> GetCurrentPriceDecimalAsync(string symbol)
    {
        var tickerResult = await _binanceRestClient.SpotApi.ExchangeData.GetTickerAsync(symbol);
        return tickerResult.Data?.LastPrice;
    }

    public async Task<IEnumerable<IBinanceKline>> GetHistoricalData(string symbol, KlineInterval interval)
    {
        var beginDate = DateTime.Now.AddMinutes(-(((double)interval) / 60) * 500);

        var klinesResult = await _binanceRestClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, interval, beginDate, DateTime.Now);
        if (klinesResult.Success)
        {
            return klinesResult.Data;
        }
        else
        {
            Console.WriteLine($"Error fetching historical data: {klinesResult.Error}");
            return [];
        }
    }

    public async Task<BinanceAccountInfo> GetAccountInfo()
    {
        var accountInfo = await _binanceRestClient.SpotApi.Account.GetAccountInfoAsync();
        return accountInfo.Data;
    }

    public async Task<bool> PlaceBuyOrderAsync(string symbol, decimal quantity)
    {
        decimal adjustedQuantity = Math.Round(quantity / (1 - AppSettings.commissionRate), 5);

        var buyOrderResult = await _binanceRestClient.SpotApi.Trading.PlaceOrderAsync(
            symbol,
            side: Binance.Net.Enums.OrderSide.Buy,
            type: SpotOrderType.Market,
            quantity: adjustedQuantity);

        if (buyOrderResult.Success)
        {
            Console.WriteLine($"Buy order placed successfully. Order ID: {buyOrderResult.Data.Id}");
        }
        else
        {
            Console.WriteLine($"Error placing buy order: {buyOrderResult.Error?.Message}");
        }

        return buyOrderResult.Success;
    }

    public async Task<bool> PlaceBuyStopLossOrderAsync(string symbol, decimal quantity, decimal stopPrice, decimal limitPrice)
    {
        var stopLossOrderResult = await _binanceRestClient.SpotApi.Trading.PlaceOrderAsync(
            symbol,
            side: Binance.Net.Enums.OrderSide.Sell,
            type: SpotOrderType.StopLossLimit,
            quantity: quantity,
            price: limitPrice,
            timeInForce: TimeInForce.GoodTillCanceled,
            stopPrice: stopPrice
            );

        if (stopLossOrderResult.Success)
        {
            Console.WriteLine($"Stop Loss order placed successfully. Order ID: {stopLossOrderResult.Data.Id}");
        }
        else
        {
            Console.WriteLine($"Error placing stop loss order: {stopLossOrderResult.Error?.Message}");
        }

        return stopLossOrderResult.Success;
    }

    public async Task<bool> PlaceBuyTakeProfitOrderAsync(string symbol, decimal quantity, decimal stopPrice, decimal limitPrice)
    {
        var takeProfitOrderResult = await _binanceRestClient.SpotApi.Trading.PlaceOrderAsync(
            symbol,
            side: Binance.Net.Enums.OrderSide.Sell,
            type: SpotOrderType.TakeProfitLimit,
            quantity: quantity,
            price: limitPrice,
            timeInForce: TimeInForce.GoodTillCanceled,
            stopPrice: stopPrice
            );

        if (takeProfitOrderResult.Success)
        {
            Console.WriteLine($"Take Profit order placed successfully. Order ID: {takeProfitOrderResult.Data.Id}");
        }
        else
        {
            Console.WriteLine($"Error placing take profit order: {takeProfitOrderResult.Error?.Message}");
        }

        return takeProfitOrderResult.Success;
    }

    public async Task<bool> PlaceBuyOcoOrderAsync(string symbol)
    {
        var priceResult = await GetCurrentPriceAsync(AppSettings.symbol);
        var price = priceResult.HasValue ? (decimal)priceResult.Value : 0;

        var takeProfitPrice = price * (1 + (AppSettings.tpPercent / 100));
        var stopLossPrice = price * (1 - (AppSettings.slPercent / 100));

        var tpPrice = Math.Floor(takeProfitPrice / AppSettings.tickSize) * AppSettings.tickSize;
        var slPrice = Math.Floor(stopLossPrice / AppSettings.tickSize) * AppSettings.tickSize;

        var ocoOrderResult = await _binanceRestClient.SpotApi.Trading.PlaceOcoOrderAsync(
           symbol: symbol,
           side: OrderSide.Sell,
           quantity: AppSettings.quantity,
           price: tpPrice,
           stopPrice:slPrice,
           stopLimitPrice: slPrice,
           stopLimitTimeInForce: TimeInForce.GoodTillCanceled
        );

        if (!ocoOrderResult.Success)
        {
            Console.WriteLine("OCO emri başarısız oldu: " + ocoOrderResult.Error);
            return false;
        }

        return true;
    }

    public async Task<bool> PlaceSellOrderAsync(string symbol, decimal quantity)
    {
        var sellOrderResult = await _binanceRestClient.SpotApi.Trading.PlaceOrderAsync(
            symbol, Binance.Net.Enums.OrderSide.Sell, SpotOrderType.Market, quantity);

        if (sellOrderResult.Success)
        {
            Console.WriteLine($"Sell order placed successfully. Order ID: {sellOrderResult.Data.Id}");
        }
        else
        {
            Console.WriteLine($"Error placing sell order: {sellOrderResult.Error?.Message}");
        }
        return sellOrderResult.Success;
    }

    public async Task<bool> CheckSymbolBalanceAsync(string symbol)
    {
        var accountInfoResult = await _binanceRestClient.SpotApi.Account.GetAccountInfoAsync();
        if (accountInfoResult.Success)
        {
            var balance = accountInfoResult.Data.Balances.FirstOrDefault(b => b.Asset.Equals(symbol, System.StringComparison.OrdinalIgnoreCase));
            return balance?.Available > 0;
        }
        Console.WriteLine($"Error fetching account info: {accountInfoResult.Error?.Message}");
        return false;
    }

    public async Task<bool> InitializeBinanceAccountAsync()
    {
        var connectivityCheck = await _binanceRestClient.SpotApi.ExchangeData.PingAsync();

        if (!connectivityCheck.Success)
        {
            Console.WriteLine("Unable to connect to Binance. Stopping operations.");
        }
        else
        {
            Console.WriteLine("Connected to Binance successfully. Proceeding with operations.");
        }

        return connectivityCheck.Success;
    }

    public async Task<decimal?> GetMinNotionalAsync(string symbol)
    {
        var exchangeInfo = await _binanceRestClient.SpotApi.ExchangeData.GetExchangeInfoAsync();

        if (!exchangeInfo.Success)
        {
            Console.WriteLine($"Error retrieving exchange info: {exchangeInfo.Error?.Message}");
            return null;
        }

        var symbolInfo = exchangeInfo.Data.Symbols.FirstOrDefault(s => s.Name == symbol);
        if (symbolInfo == null)
        {
            Console.WriteLine("Symbol not found.");
            return null;
        }

        var minNotionalFilter = symbolInfo.Filters
            .FirstOrDefault(f => f.FilterType == Binance.Net.Enums.SymbolFilterType.Notional);

        if (minNotionalFilter == null)
        {
            Console.WriteLine("MinNotional filter not found.");
            return null;
        }

        var minNotionalValue = ((BinanceSymbolNotionalFilter)minNotionalFilter).MinNotional;
        return decimal.Parse(minNotionalValue.ToString());
    }
}