using System.ComponentModel.DataAnnotations;

namespace Proniam.ViewModel
{
    public class UpdateSlideVM
    {
        [MaxLength(250, ErrorMessage = "maximum 250 simvoldan istifade etmek olar")]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
