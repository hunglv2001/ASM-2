using System.Collections.Generic;
using AppDev1670.Models;

namespace AppDev1670.ViewModels
{
    public class CheckOut
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<OrderDetail> orderDetails { get; set; }
        public int TotalPrice { get; set; }

    }
}