using System.Collections.Generic;
using AppDev1670.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppDev1670.ViewModels
{
     public class AdminViewModel
        {
            // Lưu trữ thông tin mà Admin cần để sổ thông tin ra View() 
            // List<ApplicationUser> = list các phần tử có kiểu dữ liệu là ApplicationUser
            public List<ApplicationUser> Users { set; get; } = new List<ApplicationUser>();
            public List<Category> Categories { set; get; } = new List<Category>();
            public List<Category> CategoriesHidden { set; get; } = new List<Category>();
    
            [BindProperty]
            public InputModel Input { get; set; }
            public SelectList RoleSelectList { get; set; }
            
    
            public class InputModel
            {
                
                public string Role { get; set; }
    
    
            }
    
        }
}