using Application.Factory;
using Application.Interfaces;
using Domain.Enums;

namespace Application.Services;

public class StrategyService
{
    private readonly IStrategy _strategy;

    public StrategyService(StrategyType strategyType, BinanceService binanceService)
    {
        _strategy = StrategyFactory.GetStrategy(strategyType, binanceService);
    }

    public async Task Run()
    {
       await _strategy.Execute();
    }
}
