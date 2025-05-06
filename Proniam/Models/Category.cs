using System.ComponentModel.DataAnnotations;

namespace Proniam.Models
{
    public class Category:BaseEntity
    {
        [MinLength(5,ErrorMessage ="en az 5 simvol olmalidir")]
        [MaxLength(25)]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
