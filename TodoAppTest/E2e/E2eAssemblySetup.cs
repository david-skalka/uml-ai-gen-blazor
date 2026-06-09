using TodoAppTest.E2e.Infrastructure;

namespace TodoAppTest.E2e;

[SetUpFixture]
public class E2EAssemblySetup
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await E2eTestHost.EnsureStartedAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await E2eTestHost.DisposeAsync();
    }
}
