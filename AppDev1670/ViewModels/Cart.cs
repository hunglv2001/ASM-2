using System.Collections.Generic;
using AppDev1670.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppDev1670.ViewModels
{
   public class Cart
   {

      [BindProperty]
      public List<OrderDetail> orderDetails { get; set; }
      public int totalPrice { get; set; }
		
   }
}