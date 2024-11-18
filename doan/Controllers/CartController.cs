using doan.Models;
using doan.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace doan.Controllers
{
    public class CartController : Controller
    {
       private MyStoreEntities db = new MyStoreEntities();

        //ham lay dich vu gio hang
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // hie thi gio hang khong gom nhom theo danh muc
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }

        //them san pham vao gio
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(product.ProductID, product.ProductImage, product.ProductName, product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
        }

        //xoa san pham khoi gio\
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        //lam trong gio hang
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int quantity, int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(quantity, id);
            return RedirectToAction("Index");
        }
    }
}