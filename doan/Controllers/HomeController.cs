using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using doan.Models;
using doan.Models.ViewModel;
using PagedList;

namespace doan.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index( string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
            //tim kiem san pham dua tren tu khoa
            if(!string.IsNullOrEmpty(searchTerm) )
            { 
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                p.ProductDecription.Contains(searchTerm) ||
                p.Category.CategoryName.Contains(searchTerm));
            }
            //doan code lien quan den phan trang
            int pageNumber = page ?? 1;
            int pageSize = 6;

            //lay top 10 sp ban chay nhat
            model.FeaturedProduct =products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();

            //lay 20 sp ban e nhat va phan trang
            model.NewProducts =products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(pageNumber, pageSize);


            return View(model);
        }

        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
         if(id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
         Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            //Lay tat ca san pham cung danh muc
            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID!= pro.ProductID).AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();

            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;
            model.product = pro;
            model.RelatedProducts = products.OrderBy (p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }
            return View(model);
        }

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}