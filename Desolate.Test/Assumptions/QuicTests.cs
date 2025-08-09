using System.Net.Quic;

namespace Desolate.Test.Assumptions;

[TestClass]
public sealed class QuicTests
{
    [TestMethod]
    public void ListenerSupported()
    {
        Assert.IsTrue(QuicListener.IsSupported);
    }

    [TestMethod]
    public void ConnectionSupported()
    {
        Assert.IsTrue(QuicConnection.IsSupported);
    }
}