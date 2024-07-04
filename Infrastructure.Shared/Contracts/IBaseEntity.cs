
namespace Infrastructure.Shared.Contracts;

public interface IBaseEntity : IEntity
{
  DateTimeOffset CreatedOn { get; set; }


  DateTimeOffset? LastModifiedOn { get; set; }
}
