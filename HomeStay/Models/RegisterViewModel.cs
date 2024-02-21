using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HomeStay.Models
{
    public class RegisterViewModel
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "Email không được để trống!")]
        [DataType(DataType.EmailAddress) ]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        [Remote(action: "ValidateEmail", controller: "Auth")]
        public string Email { get; set; } = null!;


        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu phải là 6 ký tự")]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;


        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu phải là 6 ký tự")]
        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password" , ErrorMessage ="Mật khẩu không trùng!, nhập lại")]
        public string ConfirmPassword { get; set; } = null!;


        [MaxLength(100)]
        [Required(ErrorMessage = "Họ và tên không được để trống!")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;


        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }


        [MaxLength(100)]
        [Required(ErrorMessage = "Địa chỉ không được để trống!")]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }


        [MaxLength(12)]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        [Remote(action: "ValidatePhone", controller: "Auth")]
        public string PhoneNumber { get; set; } = null!;
       /* public bool Active { get; set; }
        public string Avatar { get; set; } = null!;*/

    }
}
