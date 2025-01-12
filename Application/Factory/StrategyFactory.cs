using Application.Interfaces;
using Application.Services;
using Application.Strategies;
using Domain.Enums;

namespace Application.Factory;

public static class StrategyFactory
{
    public static IStrategy GetStrategy(StrategyType strategyType, BinanceService binanceService)
    {
        return strategyType switch
        {
            StrategyType.MultipleSuperTrendAndEma => new MultipleSuperTrendAndEmaStrategy(binanceService),
            StrategyType.TripleMovingAverageStrategy => new TripleMovingAverageStrategy(binanceService),
            _ => throw new ArgumentException("Invalid strategy type")
        };
    }
}