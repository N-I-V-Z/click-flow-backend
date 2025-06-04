using ClickFlow.DAL.Repositories;

namespace ClickFlow.DAL.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		IRepoBase<T> GetRepo<T>() where T : class;
		Task SaveChangesAsync();
		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollBackAsync();
		Task<bool> SaveAsync();
	}
}
