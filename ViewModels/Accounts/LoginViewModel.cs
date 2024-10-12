using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "The Email is required")]
        [EmailAddress(ErrorMessage = "The Email is invalid")] //Validate if value received is an email
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password is required")]
        public string Password { get; set; }
    }
}
