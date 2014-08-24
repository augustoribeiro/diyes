using Raven.Client;
using Raven.Client.Listeners;

namespace Diyes.RavenDbStore.Test
{
    public class NonStaleQueryListener : IDocumentQueryListener
    {
        public void BeforeQueryExecuted(IDocumentQueryCustomization customization)
        {
            customization.WaitForNonStaleResults();
        }
    }
}
