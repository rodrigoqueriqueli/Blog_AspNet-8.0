using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "The Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email is required")]
        [EmailAddress(ErrorMessage = "The Email is invalid")] //Validate if value received is an email
        public string Email { get; set; }
    }
}
