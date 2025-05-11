using Microsoft.Build.Execution;
using Proniam.Models;
using System.ComponentModel.DataAnnotations;

namespace Proniam.ViewModel
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public IFormFile MainPhoto { get; set; }
        public List<Category> Categories { get; set; }
    }
}
