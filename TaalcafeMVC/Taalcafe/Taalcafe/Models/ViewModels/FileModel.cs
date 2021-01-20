
using Microsoft.AspNetCore.Http;

namespace Taalcafe.Models.ViewModels
{
    public class FileModel
    {
        public string path { get; set; }
        public IFormFile file { get; set; }
        public string status {get; set; }
        // status should be one of the following:
        // EMPTY, UNCHANGED, EDIT, DELETE
        // Isn't enum since there were some problems with submitting the enum from a form.
    }
}