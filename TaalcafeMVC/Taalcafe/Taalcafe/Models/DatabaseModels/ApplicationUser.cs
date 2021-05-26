﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Models.Shared;

namespace Taalcafe.Models.DatabaseModels
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<UserEntry> UserEntries { get; set; }
    }
}