﻿namespace CCA.Models.Requests
{
    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}
