using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace AppDev1670.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { set; get; }
        List<Order> Orders { get; set; }
    }
}
