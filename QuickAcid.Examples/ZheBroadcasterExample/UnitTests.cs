

using QuickAcid.Examples.ZheBroadcasterExample.SimpleModel;

namespace QuickAcid.Examples.ZheBroadcasterExample;

public class UnitTests
{
    [Fact]
    public void Broadcaster_start_stop_no_exception()
    {
        var clientProxyFactory = new TestClientProxyFactory();
        var broadcaster = new Broadcaster(clientProxyFactory);
        var needler = new Needler();
        needler.Start(() => broadcaster.Broadcast(new Notification()));
        needler.Stop();
        Assert.Null(needler.ExceptionFromThread);
    }

    [Fact]
    public void Broadcaster_start_register_stop_no_exception()
    {
        var clientProxyFactory = new TestClientProxyFactory();
        var broadcaster = new Broadcaster(clientProxyFactory);
        var needler = new Needler();
        needler.Start(() => broadcaster.Broadcast(new Notification()));
        broadcaster.Register();
        needler.Stop();
        Assert.Null(needler.ExceptionFromThread);
    }

    [Fact]
    public void Broadcaster_register_start_register_stop_no_exception()
    {
        var clientProxyFactory = new TestClientProxyFactory();
        var broadcaster = new Broadcaster(clientProxyFactory);
        var needler = new Needler();
        broadcaster.Register();
        needler.Start(() => broadcaster.Broadcast(new Notification()));
        broadcaster.Register();
        needler.Stop();
        Assert.Null(needler.ExceptionFromThread);
    }
}