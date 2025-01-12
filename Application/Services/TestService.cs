using Application.Settings;

namespace Application.Services;

public class TestService
{
    private readonly BinanceService _binanceService;

    public TestService()
    {
        _binanceService = new BinanceService();
    }

    public async Task<bool> BuyOrder()
    {
        return await _binanceService.PlaceBuyOrderAsync(AppSettings.symbol, AppSettings.quantity);
    }

    public async Task<bool> BuyOrderTakeProfit()
    {
        const decimal tickSize = 0.10m;
        var priceResult = await _binanceService.GetCurrentPriceAsync(AppSettings.symbol);
        var price = priceResult.HasValue ? (decimal)priceResult.Value : 0;

        var profitPrice = price * (1 + (AppSettings.tpPercent / 100));

        decimal takeProfitPrice = Math.Floor(profitPrice / tickSize) * tickSize;

        return await _binanceService.PlaceBuyTakeProfitOrderAsync(
            AppSettings.symbol,
            AppSettings.quantity,
            takeProfitPrice,
            takeProfitPrice);
    }

    public async Task<bool> BuyOrderStopLoss()
    {
        var priceResult = await _binanceService.GetCurrentPriceAsync(AppSettings.symbol);
        var price = priceResult.HasValue ? (decimal)priceResult.Value : 0;

        var stopPrice = price * (1 - (AppSettings.slPercent / 100));
        decimal stopLossPrice = Math.Floor(stopPrice / AppSettings.tickSize) * AppSettings.tickSize;

        return await _binanceService.PlaceBuyStopLossOrderAsync(
            AppSettings.symbol,
            AppSettings.quantity,
            stopLossPrice,
            stopLossPrice
            );
    }

    public async Task<bool> PlaceOcoOrderAsync()
    {
        return await _binanceService.PlaceBuyOcoOrderAsync(AppSettings.symbol);
    }
}