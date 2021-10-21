using Xunit;

namespace Api.Tests.Setup.XUnit
{
    [CollectionDefinition(TestCollections.SharedFirebaseContextTests)]
    public class SharedFirebaseContextTestCollection : ICollectionFixture<TestFixture>
    {
    }
}
