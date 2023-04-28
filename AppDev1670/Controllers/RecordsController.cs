using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDev1670.Data;
using AppDev1670.Models;
using AppDev1670.Utils;
using AppDev1670.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDev1670.Controllers
{
    [Authorize(Roles = Role.STORE_OWNER)]
    public class RecordsController : Controller
    {
        // 1 - Declare ApplicationDbContext
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public RecordsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            IEnumerable<Order> orders  = _context.Orders
                .Include(t=>t.User)
                .ToList();
            return View(orders);
        }

        public IActionResult Details(string id)
        {
           
            IEnumerable<Order> getOrderUser = (from order in _context.Orders.Where(o => o.UserId == id)  select order)
                .Include(u => u.OrderDetails)
                .Include(u => u.User); 
      
            return View(getOrderUser);
        }

        public IActionResult ShowBookInOrder(int id)
        {
            IEnumerable<OrderDetail> getOrderUser =
                (from orderDetails in _context.OrderDetails.Where(o => o.OrderId == id) select orderDetails)
                .Include(b => b.Book);
            
            return View(getOrderUser);
        }

        public IActionResult GetAllCustomers()
        {
            // var listCustomer = from user in _userManager
            var listCustomers = (List<ApplicationUser>)_userManager.GetUsersInRoleAsync(Role.CUSTOMER).Result;
            return View(listCustomers);
        }

        public async Task<IActionResult> SearchCustomer(string searchString)
        {
            var customer = await _userManager.GetUsersInRoleAsync(Role.CUSTOMER);
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(GetAllCustomers));
            }
            else
            {
                var result = customer.Where(t => t.Email.Contains(searchString)).ToList();

                return View(nameof(GetAllCustomers), result);
            }
            
            return View(nameof(GetAllCustomers));
        }
    }
}