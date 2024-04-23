using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreIdentity.View_Models
{
    public class UserVM
    {
        public string FullName { get; set; }
        [NotMapped]
        public IFormFile Picture { get; set; }
    }
}
