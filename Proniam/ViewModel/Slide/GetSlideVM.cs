using System.ComponentModel.DataAnnotations;

namespace Proniam.ViewModel
{
    public class GetSlideVM
    {
        [MaxLength(250, ErrorMessage = "maximum 250 simvoldan istifade etmek olar")]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
    }
}
