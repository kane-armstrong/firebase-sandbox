﻿namespace Api.Configuration
{
    public class AuthenticationOptions
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public bool RequireHttps { get; set; }
    }
}