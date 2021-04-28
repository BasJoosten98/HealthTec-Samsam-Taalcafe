using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.Models.ViewModels
{
    public class ManageGroupsUserModel 
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        public string GroupName { get; set; }
        [Required]
        public string Role { get; set; }
    }

    public class ManageGroupsViewModel
    {
        public IEnumerable<ManageGroupsUserModel> Users { get; set; }
    }
}
