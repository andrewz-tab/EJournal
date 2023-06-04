using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EJournal.Models.ViewModels.LoginViewModels
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите текущий пароль")]
        [DisplayName("Текущий пароль")]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль")]
        [MinLength(8, ErrorMessage = "Пароль должен быть больше 8 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9])\S{1,16}$", ErrorMessage ="Пароль должен содержать как минимум одну цифру, одну заглавную и прописную букву, а также один из знаков: !,@,#,$,%,^,&,*,(,),.,, ")]
        [DisplayName("Пароль")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль")]
        [Compare("Password", ErrorMessage="Пароли не совпадают")]
        [DisplayName("Повторите пароль")]
        public string PasswordConfirm { get; set; }
    }
}
