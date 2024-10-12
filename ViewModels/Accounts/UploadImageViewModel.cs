using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts
{
    public class UploadImageViewModel
    {
        [Required(ErrorMessage = "Invalid Image")]
        public string Base64Image { get; set; }
    }
}
