using System.Threading.Tasks;
using NUnit.Framework;
using TodoAppTest.E2e.Infrastructure;

namespace TodoAppTest.E2e;

[SetUpFixture]
public class E2EAssemblySetup
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await E2ETestHost.EnsureStartedAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await E2ETestHost.DisposeAsync();
    }
}
