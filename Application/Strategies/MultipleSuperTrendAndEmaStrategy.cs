using Application.Interfaces;
using Application.Services;
using Application.Settings;
using Skender.Stock.Indicators;

namespace Application.Strategies;

internal class MultipleSuperTrendAndEmaStrategy : IStrategy
{
    private readonly BinanceService _binanceService;

    public MultipleSuperTrendAndEmaStrategy(BinanceService binanceService)
    {
        _binanceService = binanceService;
    }

    public async Task Execute()
    {
        var historicalData = await _binanceService.GetHistoricalData(AppSettings.symbol, AppSettings.timeFrame);
        List<Quote> quotes = historicalData.Select(kline => new Quote
        {
            Date = kline.CloseTime,
            Open = kline.OpenPrice,
            High = kline.HighPrice,
            Low = kline.LowPrice,
            Volume = kline.QuoteVolume,
            Close = kline.ClosePrice
        }).ToList();

        if (quotes.Count <= 200)
        {
            Console.WriteLine("Error fetching historical data.");
            return;
        }

        var currentPrice = await _binanceService.GetCurrentPriceAsync(AppSettings.symbol);
        var ema = quotes.GetEma(200).ToList();
        var st1 = quotes.GetSuperTrend(10, 1);
        var st2 = quotes.GetSuperTrend(11, 2);
        var st3 = quotes.GetSuperTrend(12, 3);

        if (currentPrice.HasValue &&
              st1.Last().LowerBand is not null &&
              st2.Last().LowerBand is not null &&
              st3.Last().LowerBand is not null)
        {
            var lastEma = ema[^1].Ema;
            var previousEma = ema[^2].Ema;
            var previousClosePrice = (double)quotes[^2].Close;

            bool wasBelowEmaFor10Bars = true;
            for (int i = 1; i <= 10; i++)
            {
                if ((double?)quotes[^i].Close > ema[^i].Ema)
                {
                    wasBelowEmaFor10Bars = false;
                    break;
                }
            }

            if (wasBelowEmaFor10Bars && previousClosePrice <= previousEma && currentPrice > lastEma)
            {
                Console.WriteLine("Buy signal: Current price has crossed above the EMA after being below for 10 bars.");
                await _binanceService.PlaceBuyOrderAsync(AppSettings.symbol, AppSettings.quantity);
                if (await _binanceService.CheckSymbolBalanceAsync(AppSettings.symbol))
                {
                    var buyOrderResult = await _binanceService.PlaceBuyOrderAsync(AppSettings.symbol, AppSettings.quantity);
                    if (buyOrderResult)
                    {
                        await _binanceService.PlaceBuyOcoOrderAsync(AppSettings.symbol);
                    }
                }
            }
            else if (previousClosePrice >= previousEma && currentPrice < lastEma)
            {
                Console.WriteLine("Sell signal: Current price has crossed below the EMA.");
            }
            else
            {
                Console.WriteLine("No signal: Current price has not crossed the EMA.");
            }
        }
    }
}