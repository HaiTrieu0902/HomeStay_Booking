using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HomeStay.Models
{
    public class LoginViewModel
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "Email không được để trống!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;


        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu phải là 5 ký tự")]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

    }
}
