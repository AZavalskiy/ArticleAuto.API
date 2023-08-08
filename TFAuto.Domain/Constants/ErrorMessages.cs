using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.DAL;
public class ErrorMessages
{
    public const string UserValidName = "Only alphabetic letters are allowed for a name!";
    public const string UserValidSurname = "Only alphabetic letters are allowed for a surname!";
    public const string UserValidPassword = "Password must contain at least 8 characters with at least one lowercase letter, one uppercase letter, one digit, and one special character (!, @, #, $, %, ^, &, +, =, -)!";
    public const string UserPasswordsDontMatch = "Passwords do not match!";
    public const string UserExistsByEmail = "An user with the same email already exists!";
}

