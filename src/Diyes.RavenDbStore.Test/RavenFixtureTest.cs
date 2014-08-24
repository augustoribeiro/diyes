using NUnit.Framework;

namespace Diyes.RavenDbStore.Test
{
    [TestFixture]
    public class RavenFixtureTest : RavenFixture
    {
        [Test]
        public void CurrentSessionIsInitialized()
        {
            Assert.That(CurrentSession != null);
        }
    }
}
