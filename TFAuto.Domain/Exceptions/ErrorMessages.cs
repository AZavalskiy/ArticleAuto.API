using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.DAL;
public class ErrorMessages
{
    public const string USER_VALID_NAME = "Only alphabetic letters are allowed for a name!";
    public const string USER_VALID_SURNAME = "Only alphabetic letters are allowed for a surname!";
    public const string USER_VALID_PASSWORD = "Password must contain at least 8 characters with at least one lowercase letter, one uppercase letter, one digit, and one special character (!, @, #, $, %, ^, &, +, =, -)!";
    public const string USER_VALID_REPEAT_PASSWORD = "Passwords do not match!";
    public const string USER_EXISTS_BY_EMAIL = "An user with the same email already exists!";
}

