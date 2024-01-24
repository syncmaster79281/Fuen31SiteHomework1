using System.ComponentModel.DataAnnotations;

namespace Ajax.Models.ViewModels
{
    public class MemberVm
    {
        [Display(Name = "姓名:")]
        public string? Name { get; set; }

        [Display(Name = "電子郵件:")]
        public string? Email { get; set; }
        [Display(Name = "密碼:")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Display(Name = "確認密碼:")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "年紀:")]
        public int Age { get; set; }
    }
}
