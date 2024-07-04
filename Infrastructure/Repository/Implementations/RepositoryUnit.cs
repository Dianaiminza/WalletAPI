
using Infrastructure.Context;
using Infrastructure.Repository.Abstractions;
using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.CustomExceptions;
using Infrastructure.Shared.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace CoreApi.Infrastructure.Repository.Implementations;

public class RepositoryUnit : IRepositoryUnit
{
  private readonly DatabaseContext _dbContext;

  public RepositoryUnit(DatabaseContext dbContext)
  {
    _dbContext = dbContext;
  }

  public IQueryable<TEntity> Entity<TEntity>() where TEntity : class, IEntity
  {
    return _dbContext.Set<TEntity>();
  }

  public void Create<TEntity>(TEntity entity) where TEntity : class, IEntity
  {
    _dbContext.Set<TEntity>().Add(entity);
  }

  public async Task<IDbContextTransaction> BeginTransactionAsync()
  {
    return await _dbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
  }

  public DatabaseContext DbContext()
  {
    return _dbContext;
  }

  public async Task SaveAsync(string errorMessage = "Failed to perform save operation",
      CancellationToken cancellationToken = default)
  {
    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (Exception ex)
    {
      throw new ApiException($"{errorMessage}: {ex.GetFullMessage()}");
    }
  }
}
