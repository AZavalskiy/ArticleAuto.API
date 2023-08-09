using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain;

public class UserRegistrationRequestModel
{
    private string _name;
    private string _surname;
    [Required]
    [DefaultValue("Vasia")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = ErrorMessages.USER_VALID_NAME)]
    public string Name 
    {
        get { return _name; }
        set {
            if (value.Length > 0)
                {
                    _name = char.ToUpper(value[0]) + value.Substring(1);
                }
            else
                {
                    _name = value;
                }
        }
    }

    [Required]
    [DefaultValue("Vasiliev")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = ErrorMessages.USER_VALID_SURNAME)]
    public string Surname 
    {
        get { return _surname; }
        set
        {
            if (value.Length > 0)
            {
                _surname = char.ToUpper(value[0]) + value.Substring(1);
            }
            else
            {
                _surname = value;
            }
        }
    }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DefaultValue("Qwerty123!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&+=-]).{8,}$", ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    [MinLength(8, ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    public string Password { get; set; }

    [Required]
    [DefaultValue("Qwerty123!")]
    [Compare(nameof(Password), ErrorMessage = ErrorMessages.USER_VALID_REPEAT_PASSWORD)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&+=-]).{8,}$", ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    [MinLength(8, ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    public string RepeatPassword { get; set; }

}
