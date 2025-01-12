using Application.Interfaces;
using Application.Services;
using Application.Settings;
using Skender.Stock.Indicators;

namespace Application.Strategies;

internal class TripleMovingAverageStrategy : IStrategy
{
    private readonly BinanceService _binanceService;

    public TripleMovingAverageStrategy(BinanceService binanceService)
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

        if (quotes.Count < 100)
        {
            Console.WriteLine("Error fetching historical data.");
            return;
        }

        var currentPrice = await _binanceService.GetCurrentPriceAsync(AppSettings.symbol);
        var ma20 = quotes.GetSma(20).ToList();
        var ma60 = quotes.GetSma(60).ToList();

        if (currentPrice.HasValue)
        {
            var lastMa20 = ma20[^1].Sma;
            var lastMa60 = ma60[^1].Sma;

            bool crossOccurred = false;
            for (int i = 5; i >= 1; i--)
            {
                var ma20Prev = ma20[^i].Sma;
                var ma60Prev = ma60[^i].Sma;
                if (ma20Prev <= ma60Prev && lastMa20 > lastMa60)
                {
                    crossOccurred = true;
                    break;
                }
            }

            if (crossOccurred)
            {
                Console.WriteLine("Buy signal: Fast MA has crossed above the slower MA.");
                await _binanceService.PlaceBuyOrderAsync(AppSettings.symbol, AppSettings.quantity);
                if (await _binanceService.CheckSymbolBalanceAsync(AppSettings.symbol))
                {
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
            }
            else
            {
                Console.WriteLine("No signal: No valid crossover detected within the last 5 bars.");
            }
        }
    }
}