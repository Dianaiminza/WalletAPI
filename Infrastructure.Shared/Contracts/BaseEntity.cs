namespace Infrastructure.Shared.Contracts;

public abstract class BaseEntity : IBaseEntity
{
  public BaseEntity()
  {
    CreatedOn = DateTime.UtcNow;
    LastModifiedOn = DateTime.UtcNow;
  }

  public DateTimeOffset CreatedOn { get; set; }
  public DateTimeOffset? LastModifiedOn { get; set; }
}
