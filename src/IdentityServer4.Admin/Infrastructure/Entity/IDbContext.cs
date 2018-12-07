using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Admin.Infrastructure.Entity
{
    public interface IDbContext
    {
        /// <summary>
        /// 提交数据上下文的变更
        /// </summary>
        /// <returns>操作影响的记录数</returns>
        int SaveChanges();

        /// <summary>
        /// 异步方式提交数据上下文的所有变更
        /// </summary>
        /// <param name="cancelToken">任务取消标识</param>
        /// <returns>操作影响的行数</returns>
        Task<int> SaveChangesAsync(CancellationToken cancelToken = default(CancellationToken));
    }
}