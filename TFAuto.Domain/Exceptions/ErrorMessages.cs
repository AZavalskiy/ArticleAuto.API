namespace TFAuto.Domain;

public class ErrorMessages
{
    public const string USER_VALID_USER_NAME = "Only alphabetic letters are allowed for a username!";
    public const string USER_VALID_USER_NAME_LENGTH = "Username must contain no more than 50 alphabetic characters!";
    public const string USER_VALID_PASSWORD = "Password must contain at least 8 characters with at least one lowercase letter, one uppercase letter, one digit, and one special character (!, @, #, $, %, ^, &, +, =, -)!";
    public const string USER_VALID_REPEAT_PASSWORD = "Passwords do not match!";
    public const string USER_EXISTS_BY_EMAIL = "An user with the same email already exists!";
    public const string USER_INVALID_INFO = "Personal information isn't valid, please fill the application one more time";

    public const string ROLES_NOT_FOUND = "Roles not found.";
    public const string ROLE_NOT_FOUND = "Role not found.";
    public const string ROLE_ALREADY_EXISTS = "Role already exists. Please input another name.";

    public const string INVALID_EMAIL = "That's not the right email.";
    public const string NOT_MATCH_PASS = "Passwords don't match.";
    public const string INVALID_TOKEN = "The token is not valid. Repeat the password reset request.";
    public const string INVALID_CONFIRM_TOKEN = "The confirmation code is not valid.";

    public const string LOG_IN_INVALID_CREDENTIALS = "Invalid credentials";
    public const string LOG_IN_CREDENTIALS_AGAIN = "Please enter credentials again";

    public const string FILE_NOT_FOUND = "File not found.";
    public const string FILE_ALREADY_EXISTS = "File already exists.";
    public const string FILE_IS_EMPTY = "File is empty.";
    public const string FILE_EXCEEDS_ALLOWED_SIZE = "The size of the uploaded File exceeds the allowed size.";
    public const string FILE_INVALID_FORMAT = "The format of the File to be uploaded is invalid.";
    public const string FILE_OR_REQUEST_INVALID = "Invalid request or file";
    public const string FILE_ALLOWED_EXTENSIONS = "Only next extensions are allowed: ";

    public const string ARTICLE_MAX_TAGS_QUANTITY = "You can chose up to 5 tags";
    public const string ARTICLE_NOT_FOUND = "Article not found";
    public const string ARTICLE_USER_NOT_FOUND = "There is no information about the author of the article";
    public const string ARTICLE_USER_WHO_UPDATED_NOT_FOUND = "There is no information about the author who made adjustments to the article";

    public const string PAGE_NOT_EXISTS = "Page doesn't exist";
}