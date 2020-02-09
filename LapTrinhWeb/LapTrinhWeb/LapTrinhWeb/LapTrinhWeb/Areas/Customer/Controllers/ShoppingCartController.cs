/*Nội dung: Sửa lỗi load vào trang index của ShoppingCart khi chưa có sản phẩm nào được add vào 
 Sinh viên thực hiện: Đỗ Tường Khải
 MSSV: 17110314*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LapTrinhWeb.Data;
using LapTrinhWeb.Extensions;
using LapTrinhWeb.Models;
using LapTrinhWeb.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LapTrinhWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        //Get Index Shopping Cart
        public async Task<IActionResult> Index()
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
        //Dòng if số 40 là để tránh trường hợp khi mới load website lên chưa có thêm món hàng nào vào giỏ hàng bị văng lỗi 
        //NullReferenceException: Object reference not set to an instance of an object.
            if (HttpContext.Session.Get<List<int>>("ssShoppingCart") != null)
            {
                if (lstShoppingCart.Count > 0)
                {
                    foreach (int cartItem in lstShoppingCart)
                    {
                        Products prod = _db.Products.Include(p => p.SpecialTags).Include(p => p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                        ShoppingCartVM.Products.Add(prod);
                    }
                }
            }
            //string ad = HttpContext.Session.GetString("id");
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate
                                                            .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                                                            .AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);

            Appointments appointments = ShoppingCartVM.Appointments;
            _db.Appointments.Add(appointments);
            _db.SaveChanges();

            int appointmentId = appointments.Id;

            foreach (int productId in lstCartItems)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };
                _db.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);

            }
            _db.SaveChanges();
            lstCartItems = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });

        }

        public IActionResult Remove(int id)
        {
            List<int> lstCartItem = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(lstCartItem.Count>0)
            {
                if(lstCartItem.Contains(id))
                {
                    lstCartItem.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", lstCartItem);
            return RedirectToAction("Index"); //tương tự return RedirectToAction(nameof(Index)); 
        }

        //Phương thức trả về hành động xác nhận các đơn hàng
        public IActionResult AppointmentConfirmation(int id)
        {
            //id là mã đơn hàng
            //lấy dữ liệu từ bảng Appointments dưới db để show lên ShoppingCartVM (VM là View Model dùng để đổ dữ liệu từ 2 bảng trong db lên cùng một View)
            ShoppingCartVM.Appointments = _db.Appointments.Where(d => d.Id == id).FirstOrDefault();
            //trả về danh sách các món hàng đã được chọn thuộc một đơn hàng
            List<ProductsSelectedForAppointment> objProList = _db.ProductsSelectedForAppointment.Where(d => d.AppointmentId == id).ToList();
            foreach(ProductsSelectedForAppointment ProdAptObj in objProList)
            {
                //với sản phẩm được chọn có trong một đơn hàng trả về thông tin của sản phẩm đó lên ModelView
                ShoppingCartVM.Products.Add(_db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(p=>p.Id==ProdAptObj.ProductId).FirstOrDefault());
            }
            return View(ShoppingCartVM);
        }
    }
}