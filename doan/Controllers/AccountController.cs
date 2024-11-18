using doan.Models.ViewModel;
using doan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Remoting.Messaging;


namespace doan.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Account/Register  
        public ActionResult Register()
        {
            return View();
        }
        // POST: Account/Register  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                //kiểm tra xem tên đăng nhập đã tổn tại chưa  
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }
                //nếu chưa tồn tại thì tạo bản ghi thông tin tài khoản trong bảng User  
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password, // Lưu ý: Nên mã hóa mật khẩu trước khi lưu  
                    UserRole = "Customer"
                };
                db.Users.Add(user);
                // và tạo bản ghi thông tin khách hàng trong bảng Customer  
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };
                db.Customers.Add(customer);
                //lưu thông tin tài khoản và thông tin khách hàng vào CSDL
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        //GET: account/login
        public ActionResult Login()
        {
            return View();
        }
        //Post:account/login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Password == model.Password && u.UserRole == "Customer");
                if (user != null)
                {
                    //lưu trạng thái đăng  nhập vào session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;
                    //lưu thông tin xác thực người dùng vào cookie
                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đang nhặp hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }

    }

}