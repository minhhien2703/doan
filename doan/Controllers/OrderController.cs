using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using doan.Models;
using doan.Models.ViewModel;
using System.Runtime.Remoting.Messaging;


namespace doan.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        //GET:Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            //kiểm tra giỏ hàng trong session
            //nếu giỏ hàng trống hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            //Xác thực người đùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới đăng nhập
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) { return RedirectToAction("Login", "Account"); }
            //lấy thông tin khách hàng  từ CSDL nếu chưa có thì chuyển hướng tới đăng nhập
            //nếu có rồi thì lấy địa chỉ khách hàng và gán vào ShippngAddress cúa CheckoutVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null) { return RedirectToAction("Login", "Account"); }

            var model = new CheckoutVM
            {
                //tạo dữ liệu hiển thị cho checkoutVM
                CartItems = cart, //lấy danh sách các sản phẩm có trong giỏ
                TotalAmount = cart.Sum(item => item.TotalPrice), //Tổng giá trị của các măt hàng trong giỏ 
                OrderDate = DateTime.Now, //Mặc định lấy bằng thời điểm đặt hàng
                ShippingAddress = customer.CustomerAddress, //lấy địa chỉ mặc định từ bảng Customer
                CustomerID = customer.CustomerID,
                Username = user.Username,
            };
            return View(model);
        }
        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as List<CartItem>;
                //nếu giỏ hàng trống thì sẽ điều hướng tới trang HOme
                if (cart == null || !cart.Any()) { return RedirectToAction("Index", "Home"); }
                //nếu người dùng chưa đăng nhập thì sẽ điều hướng tới trang Login
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null) { return RedirectToAction("Login", "Account"); }
                //nếu khách hàng không khớp với tên đăng nhập thì sẽ điều hướng tới trang login
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null) { return RedirectToAction("Login", "Account"); }
                //nếu người dùng thanh toán bằng paypal sẽ điều hướng tới trang PaymentWithPaypal 
                if (model.PaymentMethod == "Paypal") { return RedirectToAction("PaymenWithPaypal", "Paypal", model); }
                //Thiết lập paymentstatus 
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán Paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }
                //tạo đơn hàng và chi tiết đơn hàng liên quan 
                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetail = cart.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                    }).ToList()
                };
                //lưu danh sách vào CSDL
                db.Orders.Add(order);
                db.SaveChanges();
                //xoá giỏ hàng sau khi đặt hàng thành công
                Session["Cart"] = null;
                //điều hướng tới trang xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }

    }
}