namespace Proniam.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDelated { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
