using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDev1670.Data;
using AppDev1670.Enums;
using AppDev1670.Models;
using AppDev1670.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDev1670.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            IEnumerable<Order> orders = context
                .Orders.Where(t => t.UserId == userManager.GetUserId(User)).ToList();
            return View(orders);
        }
        
        [HttpGet]
        public IActionResult Create(int id)
        {

            var orderInDb = context.Orders.SingleOrDefault(t => t.StatusOrder == OrderStatus.Unconfirmed &&
                                                                t.UserId == userManager.GetUserId(User));

            if (orderInDb == null)
            {
                var order = new Order
                {
                    UserId = userManager.GetUserId(User),
                    StatusOrder = OrderStatus.Unconfirmed,
                };

                context.Add(order);
                context.SaveChanges();

                var orderDetail = CreateOrderDetails(id, order.Id);

                if (orderDetail is null) { return NotFound(); }

                order.PriceOrder = GetPriceOfOrder(order.Id);

            }

            else
            {
                var orderDetail = CreateOrderDetails(id, orderInDb.Id);
                if (orderDetail is null) return NotFound();
                orderInDb.PriceOrder = GetPriceOfOrder(orderInDb.Id);

            }
            context.SaveChanges();
            return RedirectToAction("Index", "OrderDetails");
        }
        
        [NonAction]
        internal int GetPriceOfOrder(int idOrder)
        {
            var orderDetails = context.OrderDetails.Include(t => t.Book).Where(t => t.OrderId == idOrder);
            int totalPrice = 0;
            foreach (var item in orderDetails)
            {
                item.Price = item.Quantity * item.Book.Price;

                totalPrice += item.Price;
            }
            return totalPrice;
        }

        [NonAction]
        internal OrderDetail CreateOrderDetails(int idBook, int idOrder)
        {
            var bookInDb = context.Books.SingleOrDefault(t => t.Id == idBook);
            if (bookInDb.QuantityBook > 0)
            {
                var orderDetailsInDb = context.OrderDetails.SingleOrDefault(t => t.OrderId == idOrder && t.BookId == idBook);
                if (orderDetailsInDb is null)
                {
                    OrderDetail newOrderDetail = new OrderDetail();

                    newOrderDetail.OrderId = idOrder;
                    newOrderDetail.BookId = bookInDb.Id;
                    newOrderDetail.Quantity = 1;
                    newOrderDetail.Price = bookInDb.Price;
                    context.OrderDetails.Add(newOrderDetail);
                    context.SaveChanges();
                    return newOrderDetail;
                }
                else
                {
                    orderDetailsInDb.Quantity += 1;
                    orderDetailsInDb.Price = orderDetailsInDb.Quantity * bookInDb.Price;
                    context.SaveChanges();

                    return orderDetailsInDb;
                }
            }
            else return null;

        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var currentUser = await userManager.GetUserAsync(User);
            CheckOut checkOut = new CheckOut();
            checkOut.Name = currentUser.FullName;
            checkOut.Address = currentUser.Address;
            checkOut.Phone = currentUser.PhoneNumber;

            var orderToBuy = context.Orders.Include(t => t.OrderDetails)
                .SingleOrDefault(t => t.UserId == userManager.GetUserId(User) && t.StatusOrder == OrderStatus.Unconfirmed);

            foreach (var item in orderToBuy.OrderDetails)
            {
                var productToBuy = context.Books.SingleOrDefault(t => t.Id == item.BookId);

                if (productToBuy.QuantityBook >= item.Quantity)
                {
                    productToBuy.QuantityBook -= item.Quantity;

                }
                else
                {
                     TempData["BookState"] = item.Book.NameBook + " is out of stock! Please reduce quantity! ";
                    return RedirectToAction("Index", "OrderDetails");

                }
            }
			
            orderToBuy.PriceOrder = GetPriceOfOrder(orderToBuy.Id);
            checkOut.orderDetails = orderToBuy.OrderDetails;
            checkOut.TotalPrice = orderToBuy.PriceOrder;
            context.SaveChanges();



            return View(checkOut);

        }
        
        [HttpGet]
        public IActionResult Purchase()
        {
            var orderToBuy = context.Orders.Include(t => t.OrderDetails).SingleOrDefault(t => t.UserId == userManager.GetUserId(User) && t.StatusOrder == OrderStatus.Unconfirmed);
            orderToBuy.StatusOrder = OrderStatus.InProgress;
            context.SaveChanges();
            return RedirectToAction("Index", "Orders");
        }

    }
}