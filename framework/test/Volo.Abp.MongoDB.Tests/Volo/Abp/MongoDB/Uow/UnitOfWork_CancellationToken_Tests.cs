using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.TestApp.Domain;
using Volo.Abp.TestApp.Testing;
using Volo.Abp.Uow;
using Xunit;

namespace Volo.Abp.MongoDB.Uow;

[Collection(MongoTestCollection.Name)]
public class UnitOfWork_CancellationToken_Tests : TestAppTestBase<AbpMongoDbTestModule>
{
    [Fact]
    public async Task Should_Cancel_Test()
    {
        using (var uow = GetRequiredService<IUnitOfWorkManager>().Begin(isTransactional: true))
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                var cst = new CancellationTokenSource();
                cst.Cancel();

                await GetRequiredService<IBasicRepository<Person, Guid>>().InsertAsync(new Person(Guid.NewGuid(), "Adam", 42));

                await uow.CompleteAsync(cst.Token);
            });
        }
    }
}
