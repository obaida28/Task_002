namespace Core.Entites;
public class BaseEntity
{
    public Guid Id { get; set; }
    public BaseEntity() => Id = Guid.NewGuid();
}