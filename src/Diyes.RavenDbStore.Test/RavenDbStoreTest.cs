using System;
using System.Linq;
using Diyes.AppendOnlyStore.Interfaces;
using NUnit.Framework;

namespace Diyes.RavenDbStore.Test
{
    [TestFixture]
    public class RavenDbStoreTest : RavenFixture
    {
        private RavenDbStore _sut;

        public override void DoSetup()
        {
            _sut = new RavenDbStore(DocumentStore);
        }

        [Test]
        public void CanCreateEventForNewIdentity()
        {
            var id = Guid.NewGuid().ToString();
            _sut.Append(id, string.Empty, 0);

            var ravenData = CurrentSession.Load<RavenData>(id);
            Assert.That(ravenData != null);
        }

        [Test]
        public void AppendingEventsBumpsVersion()
        {
            
        }
    }
}