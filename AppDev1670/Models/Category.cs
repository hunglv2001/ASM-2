using AppDev1670.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppDev1670.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Category can not be null")]
        [StringLength(255)]
        public string NameCategory { get; set; }
        public CategoryStatus Status { get; set; } = CategoryStatus.InProgess;
        public List<Book> Books { get; set; }

    }
}
