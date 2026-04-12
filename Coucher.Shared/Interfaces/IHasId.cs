namespace Coucher.Shared.Interfaces;

public interface IHasId<TId>
{
    TId Id { get; set; }
}
