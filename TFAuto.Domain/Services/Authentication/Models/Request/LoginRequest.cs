﻿using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.Authentication.Models.Request;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
