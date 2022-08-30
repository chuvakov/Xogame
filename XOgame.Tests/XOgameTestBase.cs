
using System;
using System.Configuration;
using System.Dynamic;
using System.Threading.Tasks;
using Castle.Core;
using Castle.Core.Logging;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using XOgame.Core;
using XOgame.Services;
using XOgame.Services.Account;
using XOgame.Services.Emoji;
using XOgame.Services.Game;
using XOgame.Services.Player;
using XOgame.Services.Room;
using XOgame.Services.Setting;
using XOgame.SignalR;
using XOgame.Tests.Common;
using ILogger = Castle.Core.Logging.ILogger;

namespace XOgame.Tests;

public abstract class XOgameTestBase
{
    protected readonly XOgameContext _context;
    private static IWindsorContainer _iocContainer; //di

    protected static IWindsorContainer IocContainer
    {
        get
        {
            if (_iocContainer == null)
            {
                _iocContainer = new WindsorContainer();
                RegisterServices();
            }

            return _iocContainer;
        }
    }

    public XOgameTestBase()
    {
        _context = XOgameContextFactory.Context;
    }

    private static void RegisterServices()
    {
        // Регистрация зависимостей с помощью патерна Фабричный Метод
        _iocContainer.Register(Component.For<XOgameContext>().UsingFactoryMethod(() => XOgameContextFactory.Context));
        _iocContainer.Register(Component.For(typeof(ILogger<>)).ImplementedBy(typeof(A<>)));
        // _iocContainer.Register(Component.For<IHubContext<GameHub>>().UsingFactoryMethod(() =>
        // {
        //     var gameHub = new Mock<IHubContext<GameHub>>(); //Замокали хаб
        //     var mockClients = new Mock<IHubClients>();
        //     gameHub.Setup(x => x.Clients).Returns(() => mockClients.Object);
        //
        //     return gameHub.Object;
        // }));
        _iocContainer.Register(Component.For<IHubContext<GameHub>, NullHubGameContext>());
        _iocContainer.Register(Component.For<IRoomService, RoomService>());
        _iocContainer.Register(Component.For<IAccountService, AccountService>());
        _iocContainer.Register(Component.For<IEmojiService, EmojiService>()); 
        //_iocContainer.Register(Component.For<IEmojiService, EmojiService>()
        //.DependsOn(Dependency.OnValue("context", XOgameContextFactory.Context)))); //вариант решения из гугл
        _iocContainer.Register(Component.For<IGameService, GameService>());
        _iocContainer.Register(Component.For<IPlayerService, PlayerService>());
        _iocContainer.Register(Component.For<ISettingService, SettingService>());
    }
    
    // Resolve - Решить  (отдает реализацию(объект) типа данных)
    protected TService Resolve<TService>()
    {
        return IocContainer.Resolve<TService>();
    }
}

public class A<T> : ILogger<T>
{
    public IDisposable BeginScope<TState>(TState state)
    {
        return Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        
    }
}