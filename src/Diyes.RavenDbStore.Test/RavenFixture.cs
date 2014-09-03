using System;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Diyes.RavenDbStore.Test
{
    [TestFixture]
    public abstract class RavenFixture
    {
        private Lazy<IDocumentSession> _session;

        protected IDocumentSession CurrentSession
        {
            get { return _session.Value; }
        }

        protected IDocumentStore DocumentStore { get; private set; }

        [SetUp]
        public void Setup()
        {
            DocumentStore = new EmbeddableDocumentStore()
            {
                RunInMemory = true,

            };

            DocumentStore.Initialize();
            _session = new Lazy<IDocumentSession>(() => DocumentStore.OpenSession());

            DoSetup();
        }

        [TearDown]
        public void Teardown()
        {
            DocumentStore.Dispose();
            DocumentStore = null;
        }

        public virtual void DoSetup() { }

        protected void CreateIndex<TIndex>() where TIndex : AbstractIndexCreationTask
        {
            var index = Activator.CreateInstance(typeof (TIndex)) as AbstractIndexCreationTask;
            DocumentStore.ExecuteIndex(index);
        }
    }
}
