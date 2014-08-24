using NUnit.Framework;

namespace Diyes.RavenDbStore.Test
{
    [TestFixture]
    public class RavenDbStoreTest : RavenFixture
    {
        public override void DoSetup()
        {
            CreateIndex<RavenDbIndex>();
        }

        [Test]
        public void CanAppend()
        {
            var store = new RavenDbStore(DocumentStore);
        }
    }
}