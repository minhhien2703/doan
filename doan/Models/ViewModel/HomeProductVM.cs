using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace doan.Models.ViewModel
{
    public class HomeProductVM 


    {
        //tieu chi de search theo ten, mo ta
        public string SearchTerm { get; set; }

        //cac thuoc tinh ho tro phan trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;

        //danh sach san pham noi bat
        public List<Product> FeaturedProduct { get; set; }

        //danh sach moi da phan trang
        public PagedList.IPagedList<Product> NewProducts { get; set; }
    }
}