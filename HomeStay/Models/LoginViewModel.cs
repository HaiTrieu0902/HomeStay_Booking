using System.ComponentModel.DataAnnotations;

namespace HomeStay.Models
{
    public class LoginViewModel
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "Email không được để trống!")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;


        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu phải là 5 ký tự")]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

    }
}
