namespace Tigwi.UI.Models.User
{
    using System.ComponentModel.DataAnnotations;

    public class UserLogOnViewModel
    {
        [Key]
        [Required]
        [Display(Name = "Username :")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password :")]
        public string Password { get; set; }

        [Display(Name = "Remember me ?")]
        public bool RememberMe { get; set; }
    }
}