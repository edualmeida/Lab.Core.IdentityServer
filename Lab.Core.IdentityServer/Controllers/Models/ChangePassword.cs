﻿namespace Lab.Core.IdentityServer.Controllers.Models
{
    public class ChangePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}