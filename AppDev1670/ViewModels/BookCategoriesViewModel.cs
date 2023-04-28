using AppDev1670.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AppDev1670.ViewModels
{
    public class BookCategoriesViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        
        [Required(ErrorMessage = "Please choose profile image")]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
