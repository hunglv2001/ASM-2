using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDev1670.Data;
using AppDev1670.Models;
using AppDev1670.Utils;
using AppDev1670.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppDev1670.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        // 
        public SelectList RoleSelectList { get; set; } = new SelectList(new List<string>
          {
            "All User",
            Role.STORE_OWNER,
            Role.CUSTOMER
          }
        ); 
        public AdminController(ApplicationDbContext context, 
                                UserManager<ApplicationUser> userManager, 
                                RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<Category> CategoriesHidden { set; get; } = new List<Category>();



        [HttpGet]
        public async Task<IActionResult> Index()
        {

            // Khai báo AdminViewModel 
            var model = new AdminViewModel();
            
            // Duyệt từng user trong Database 
            foreach (var user in _userManager.Users)
            {
                // Chỉ lấy User có role là Store Owner || Customer
                if (!await _userManager.IsInRoleAsync(user, Role.ADMIN))
                {
                    // Add từng user thỏa mãn điều kiện vào Users của AdminViewModel
                    model.Users.Add(user);
                }
            }
            
            // Gán roleSelectList cho AdminViewModel
            model.RoleSelectList = RoleSelectList; 


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Index(AdminViewModel adminViewModel)
        {
            // Tạo AdminViewModel
            var adminUser = new AdminViewModel();
         
            // Gán role mà admin select từ View() 
            var roleSelectedInView = adminViewModel.Input.Role; 
            
            // Check role mà admin chọn là gì 
            if(roleSelectedInView == Role.STORE_OWNER)
            {

                adminUser = getUserByRole(Role.STORE_OWNER); 
            }
            else if(roleSelectedInView == Role.CUSTOMER)
            {
                adminUser = getUserByRole(Role.CUSTOMER);
            }
            else
            {
                adminUser = new AdminViewModel();

                foreach (var user in _userManager.Users)
                {
                    if (!await _userManager.IsInRoleAsync(user, Role.ADMIN))
                    {
                        adminUser.Users.Add(user);
                    }
                }
            }
            
            // Gán RoleSelectList cho AdminViewModel để xổ ra View() 
            adminUser.RoleSelectList = RoleSelectList;
            return View(adminUser);
        }
        
        [HttpGet]
        public IActionResult ChangePassword(string id)
        {
            //
            var getUser = _context.Users.SingleOrDefault(t => t.Id == id);
            if (getUser == null || getUser.EmailConfirmed == false)
            {
                TempData["Message"] = "Can not update Because Email not confirmed";
                
                return View(getUser);
            }

            return View(getUser);
        }

        [HttpPost]
        public IActionResult ChangePassword(string id, [Bind("PasswordHash")] ApplicationUser user)
        {
            // Lấy User 
            var getUser = _context.Users.SingleOrDefault(t => t.Id == id);
            // Lấy Password
            var newPassword = user.PasswordHash; 
            // Check 
            if (getUser == null && getUser.EmailConfirmed == false)
            {
                return BadRequest();
            }
            if(newPassword == null)
            {
                ModelState.AddModelError("NoInput", "You have not input new password.");
                return View(getUser);
            }
            
            // Đổi pass cho User và trả ra View thông báo đổi thành công 
            getUser.PasswordHash = _userManager.PasswordHasher.HashPassword(getUser, newPassword);
            TempData["Message"] = "Update Successfully";
            


            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult ShowCategoriesInProgress()
        {
            // Lấy list Category có Category Status == Inprogess
            var categories = _context.Categories
                .Where(t => t.Status == Enums.CategoryStatus.InProgess)
                .ToList();

            return View("VerifyCategoryRequest", categories);
        }

        [HttpGet]
        public IActionResult VerifyCategoryRequest(string name, int id)
        {
            // Lấy List category trong Database  
            var listCategory = from Category in _context.Categories select Category;   
            
            // Lấy Category mà Admin chọn để Update lại Status
            var categoryAfterUpdate = _context.Categories.SingleOrDefault(c => c.Id == id);
            
            // Check 
            if (name == "accept")
            {
                AcceptCategoryRequest(id);

            }
            if (name == "reject")

            {
                RejectCategoryRequest(id);
            }

            return RedirectToAction(nameof(ShowCategoriesInProgress));
        }


        // thay đổi Category status thành accept 
        [HttpGet]
        public IActionResult AcceptCategoryRequest(int id)
        {
            // Lấy category ra 
            var categoryVerify = _context.Categories.SingleOrDefault(c => c.Id == id);
            
            // check 
            if (categoryVerify == null)
            {
                return BadRequest();
            }
            
            // Update lại status thành Accept 
            categoryVerify.Status = Enums.CategoryStatus.Accepted;
            _context.SaveChanges();
               
            return RedirectToAction("VerifyCategoryRequest");
        } 
        
        // thay đổi Category status thành reject 
        [HttpGet]
        public IActionResult RejectCategoryRequest(int id)
        {
            var categoryVerify = _context.Categories.SingleOrDefault(c => c.Id == id);

            if (categoryVerify == null)
            {
                return BadRequest();
            }

            categoryVerify.Status = Enums.CategoryStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("VerifyCategoryRequest");
        }

        
        [NonAction]
        private AdminViewModel getUserByRole(string role)
        {
            // Tạo ra adminviewmodel 
            var adminUser = new AdminViewModel()
            {
                // Gán Users trong adminViewModel == Role mà admin muốn chọn 
                Users = (List<ApplicationUser>)_userManager.GetUsersInRoleAsync(role).Result
            };
            return adminUser;
        }
    }
}