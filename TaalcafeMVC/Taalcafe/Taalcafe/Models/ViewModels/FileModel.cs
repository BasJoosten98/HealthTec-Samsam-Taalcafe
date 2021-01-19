
using Microsoft.AspNetCore.Http;

namespace Taalcafe.Models.ViewModels
{
    public class FileModel
    {
        public string path { get; set; }
        public IFormFile file { get; set; }
    }
}