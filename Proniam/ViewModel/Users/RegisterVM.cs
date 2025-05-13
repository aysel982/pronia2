using System.ComponentModel.DataAnnotations;

namespace Proniam.ViewModel.Users
{
    public class RegisterVM
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        [MaxLength(128)]
        public string UserName { get; set; }
        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ComfirmPassword { get; set; }
    }
}
