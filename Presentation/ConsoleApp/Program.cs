using Application.Services;
using Application.Settings;

var binanceService = new BinanceService();

Console.WriteLine("Welcome to Binance Trader");

var initializeAccount = await binanceService.InitializeBinanceAccountAsync();
if (!initializeAccount)
    return;

var accountInfo = await binanceService.GetAccountInfo();
Console.WriteLine($"Account No:{accountInfo.UserId} is ready for trading");

if (AppSettings.isTest)
{
    var testService = new TestService();

    var buyResult = await testService.BuyOrder();
    var ocoResult = await testService.PlaceOcoOrderAsync();

    Console.WriteLine($"Buy Order Result: {buyResult}");
    Console.WriteLine($"Oco Order Result: {ocoResult}");
}
else
{
    var strategy = new StrategyService(AppSettings.activeStrategy, binanceService);
    Console.WriteLine($"Starting trades with the {AppSettings.activeStrategy}. Waiting for buy signal...");

    while (true)
    {
        await strategy.Run();
        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}