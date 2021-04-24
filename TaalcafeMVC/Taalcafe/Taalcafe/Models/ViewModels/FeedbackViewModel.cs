using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taalcafe.Models.Shared;

namespace Taalcafe.Models.ViewModels
{
    public class FeedbackViewModel
    {
        [Required]
        public Mark Mark { get; set; }

        [Required]
        public string MarkReason { get; set; }
    }
}
