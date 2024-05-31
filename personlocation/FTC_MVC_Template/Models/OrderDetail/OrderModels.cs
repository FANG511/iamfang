using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.OrderDetail.ViewModels
{

    public class Order_Detail
    {
        public string OrderDate { get; set; }
        public string OrderID { get; set; }
        public int Total_quantity { get; set; }
        public string Total_price { get; set; }
   
    }

 
    
}