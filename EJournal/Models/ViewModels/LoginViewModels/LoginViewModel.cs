using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EJournal.Models.ViewModels.LoginViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Укажите логин, номер телефона или почту")]
        [MaxLength(80, ErrorMessage = "Логин не должен превышать длину больше 80 символов")]
        [DisplayName("Логин, электронная почта или пароль")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль")]
        [DisplayName("Пароль")]
        public string Password { get; set; }


    }
}
