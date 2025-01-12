using Binance.Net.Enums;
using Domain.Enums;

namespace Application.Settings;

public static class AppSettings
{
    public static readonly bool isTest = true;
    public static readonly string apiKey = "your-Api-Key";
    public static readonly string apiSecret = "your-Api-Secret";
    public static readonly KlineInterval timeFrame = KlineInterval.OneHour;
    public static readonly StrategyType activeStrategy = StrategyType.TripleMovingAverageStrategy;
    public static readonly string symbol = "BTCUSDT";
    public static readonly decimal tickSize = 0.10m;
    public static readonly decimal quantity = 0.00007M;
    public static readonly decimal slPercent = 1.0M;
    public static readonly decimal tpPercent = 2.0M;
    public static readonly decimal commissionRate = 0.1M;
}