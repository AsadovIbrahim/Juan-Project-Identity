using System.ComponentModel.DataAnnotations;

namespace Juan_Project.ViewModels.Account
{
    public class RegisterVM
    {
        public string FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password))]

        public string ConfirmPassword { get; set; }
    }
}
