﻿namespace JWTServiceCore.Models.DTOs
{
    public class LoginDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
