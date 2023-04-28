using System;
using System.Collections.Generic;
using System.Linq;
using AppDev1670.Data;
using AppDev1670.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppDev1670.Controllers
{
    public class CustomerController : Controller
    {
        private ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string nameOrCategoryBook)

        {
            List<BookView> listBookInHome = new List<BookView>();
            var booksToBuy = _context.Books.Include(t => t.Category).Where(b => b.QuantityBook > 0).ToList();

            if (!String.IsNullOrEmpty(nameOrCategoryBook))

            {

                booksToBuy = booksToBuy.Where(t => t.Category.NameCategory.ToLower().Contains(nameOrCategoryBook.ToLower())
                                                   || t.NameBook.ToLower().Contains(nameOrCategoryBook.ToLower())).ToList();


            }
            foreach (var item in booksToBuy)
            {
                BookView book = new BookView();
                book.Id = item.Id;
                book.NameBook = item.NameBook;
                book.QuantityBook = item.QuantityBook;
                book.PriceBook = item.Price;
                book.ImageBook = ConvertByteArrayToStringBase64(item.Image);
                listBookInHome.Add(book);
            }
            return View(listBookInHome);
        }

        [NonAction]
        private string ConvertByteArrayToStringBase64(byte[] imageArray)
        {
            string imageBase64Data = Convert.ToBase64String(imageArray);

            return string.Format("data:image/jpg;base64, {0}", imageBase64Data);
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var book = _context.Books.SingleOrDefault(x => x.Id == id);
            BookView bookView = new BookView();
            bookView.Id = book.Id;
            bookView.NameBook = book.NameBook;
            bookView.QuantityBook = book.QuantityBook;
            bookView.PriceBook = book.Price;
            bookView.DescriptionBook = book.InformationBook;
            bookView.ImageBook = ConvertByteArrayToStringBase64(book.Image);

            return View(bookView);
        }
        [HttpGet]
        public IActionResult Help()
        {
            return View();
        }
    }
}