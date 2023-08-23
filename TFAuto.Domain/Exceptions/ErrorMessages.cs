﻿namespace TFAuto.Domain;

public class ErrorMessages
{
    public const string USER_VALID_USER_NAME = "Only alphabetic letters are allowed for a username!";
    public const string USER_VALID_SURNAME = "Only alphabetic letters are allowed for a surname!";
    public const string USER_VALID_PASSWORD = "Password must contain at least 8 characters with at least one lowercase letter, one uppercase letter, one digit, and one special character (!, @, #, $, %, ^, &, +, =, -)!";
    public const string USER_VALID_REPEAT_PASSWORD = "Passwords do not match!";
    public const string USER_EXISTS_BY_EMAIL = "An user with the same email already exists!";

    public const string ROLES_NOT_FOUND = "Roles not found.";
    public const string ROLE_NOT_FOUND = "Role not found.";
    public const string ROLE_ALREADY_EXISTS = "Role already exists. Please input another name.";

    public const string INVALID_EMAIL = "That's not the right email.";
    public const string NOT_MATCH_PASS = "Passwords don't match.";    
    public const string INVALID_TOKEN = "The token is not valid. Repeat the password reset request.";

    public const string LOG_IN_INVALID_CREDENTIALS = "Invalid credentials";
    public const string LOG_IN_CREDENTIALS_AGAIN = "Please enter credentials again";
}

