using System.Threading.Tasks;

namespace IdentityServer4.Admin.Infrastructure.Entity
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync();
    }
}