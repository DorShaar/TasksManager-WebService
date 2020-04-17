﻿using Microsoft.AspNetCore.Identity;

namespace Tasker.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}