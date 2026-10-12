using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FoodPreOrder.Models.Domain;
using FoodPreOrder.Models.ViewModels;
using FoodPreOrder.Services;

namespace FoodPreOrder.Controllers
{
    public class CustomerController : Controller
    {
        private readonly FoodPreOrderDataStore _dataStore;
        private const string SessionCartKey = "Customer_Cart";
        private const string SessionUserKey = "Customer_User";

        public CustomerController(FoodPreOrderDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        private List<CartItemViewModel> GetCartFromSession()
        {
            var json = HttpContext.Session.GetString(SessionCartKey);
            if (string.IsNullOrEmpty(json)) return new List<CartItemViewModel>();
            try
            {
                return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>();
            }
            catch
            {
                return new List<CartItemViewModel>();
            }
        }

        private void SaveCartToSession(List<CartItemViewModel> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(SessionCartKey, json);
        }

        private UserAccount? GetCurrentCustomer()
        {
            var json = HttpContext.Session.GetString(SessionUserKey);
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                return JsonSerializer.Deserialize<UserAccount>(json);
            }
            catch
            {
                return null;
            }
        }

        // GET: /Customer/Index
        public IActionResult Index(string? category = "ทั้งหมด", string? search = "")
        {
            var query = _dataStore.MenuItems.Where(m => m.IsAvailable).AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "ทั้งหมด")
            {
                query = query.Where(m => m.Category == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(m => m.Name.ToLower().Contains(s) || m.ShopName.ToLower().Contains(s));
            }

            ViewBag.SelectedCategory = category ?? "ทั้งหมด";
            ViewBag.SearchQuery = search ?? "";
            ViewBag.CurrentCustomer = GetCurrentCustomer();
            ViewBag.CartCount = GetCartFromSession().Sum(c => c.Quantity);

            return View(query.ToList());
        }

        // GET: /Customer/Login (Dedicated Customer Login)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new CustomerLoginViewModel());
        }

        // POST: /Customer/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(CustomerLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _dataStore.Users.FirstOrDefault(u => 
                u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase) && 
                u.Password == model.Password &&
                u.Role == UserRole.Customer);

            if (user != null)
            {
                HttpContext.Session.SetString(SessionUserKey, JsonSerializer.Serialize(user));
                TempData["SuccessMessage"] = $"ยินดีต้อนรับคุณ {user.FullName} เข้าสู่ระบบ!";

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "อีเมลหรือรหัสผ่านไม่ถูกต้องสำหรับบัญชีลูกค้า");
            return View(model);
        }

        // GET: /Customer/Register (Dedicated Customer Register - No operator option, no "9.")
        [HttpGet]
        public IActionResult Register()
        {
            return View(new CustomerRegisterViewModel());
        }

        // POST: /Customer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(CustomerRegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_dataStore.Users.Any(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Email", "อีเมลนี้มีผู้ใช้งานในระบบแล้ว");
                return View(model);
            }

            var newUser = new UserAccount
            {
                Id = "U-" + Guid.NewGuid().ToString("N")[..6],
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Password = model.Password,
                Role = UserRole.Customer
            };

            _dataStore.Users.Add(newUser);
            HttpContext.Session.SetString(SessionUserKey, JsonSerializer.Serialize(newUser));

            TempData["SuccessMessage"] = "ลงทะเบียนสมัครสมาชิกลูกค้าสำเร็จแล้ว!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Customer/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionUserKey);
            TempData["InfoMessage"] = "ออกจากระบบเรียบร้อยแล้ว";
            return RedirectToAction(nameof(Index));
        }

        // AJAX POST: /Customer/AddToCart
        [HttpPost]
        public IActionResult AddToCart(string foodId)
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                return Json(new { success = false, requireLogin = true, message = "กรุณาเข้าสู่ระบบก่อนทำการสั่งอาหาร" });
            }

            var item = _dataStore.MenuItems.FirstOrDefault(m => m.Id == foodId);
            if (item == null)
            {
                return Json(new { success = false, message = "ไม่พบรายการอาหารที่ระบุ" });
            }

            var cart = GetCartFromSession();
            var existing = cart.FirstOrDefault(c => c.FoodId == foodId);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    FoodId = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = 1,
                    ShopName = item.ShopName,
                    ImageUrl = item.ImageUrl
                });
            }

            SaveCartToSession(cart);
            var totalCount = cart.Sum(c => c.Quantity);
            return Json(new { success = true, cartCount = totalCount, message = $"เพิ่ม \"{item.Name}\" ลงในตะกร้าแล้ว" });
        }

        // AJAX GET: /Customer/GetCartJson
        [HttpGet]
        public IActionResult GetCartJson()
        {
            var cart = GetCartFromSession();
            var total = cart.Sum(c => c.Price * c.Quantity);
            return Json(new { items = cart, grandTotal = total });
        }

        // AJAX POST: /Customer/UpdateCartQty
        [HttpPost]
        public IActionResult UpdateCartQty(string foodId, int delta)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.FoodId == foodId);
            if (item != null)
            {
                item.Quantity += delta;
                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }
            SaveCartToSession(cart);
            var total = cart.Sum(c => c.Price * c.Quantity);
            var totalCount = cart.Sum(c => c.Quantity);
            return Json(new { success = true, items = cart, grandTotal = total, cartCount = totalCount });
        }

        // POST: /Customer/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutRequestViewModel model)
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
            {
                TempData["ErrorMessage"] = "กรุณาเข้าสู่ระบบก่อนทำการสั่งอาหาร";
                return RedirectToAction(nameof(Login));
            }

            var cart = GetCartFromSession();
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "ตะกร้าของคุณยังไม่มีรายการอาหาร";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "กรุณาระบุเวลานัดรับอาหารล่วงหน้า";
                return RedirectToAction(nameof(Index));
            }

            var firstItem = cart.First();
            var newOrder = new PreOrder
            {
                Id = "FPO-" + new Random().Next(1000, 9999),
                UserId = customer.Id,
                CustomerName = customer.FullName,
                CustomerPhone = customer.PhoneNumber,
                ShopName = firstItem.ShopName,
                ShopLocation = "อาคาร A ชั้น 1 หน้าร้าน",
                PickupTime = model.PickupTime + " น.",
                Note = string.IsNullOrWhiteSpace(model.Note) ? "ไม่มี" : model.Note,
                Status = OrderStatus.PENDING,
                TotalPrice = cart.Sum(c => c.Price * c.Quantity),
                Items = cart.Select(c => new OrderItem
                {
                    FoodId = c.FoodId,
                    Name = c.Name,
                    Price = c.Price,
                    Quantity = c.Quantity
                }).ToList()
            };

            _dataStore.AddOrder(newOrder);

            // Clear session cart
            HttpContext.Session.Remove(SessionCartKey);

            TempData["SuccessMessage"] = "ยืนยันการสั่งซื้อล่วงหน้าสำเร็จ! ชำระเงินสดเมื่อรับสินค้าหน้าร้าน";
            return RedirectToAction(nameof(Tracking), new { orderId = newOrder.Id });
        }

        // GET: /Customer/Tracking
        public IActionResult Tracking(string? orderId = null)
        {
            var customer = GetCurrentCustomer();
            ViewBag.CurrentCustomer = customer;
            ViewBag.CartCount = GetCartFromSession().Sum(c => c.Quantity);

            List<PreOrder> userOrders;
            if (customer != null)
            {
                userOrders = _dataStore.Orders.Where(o => o.UserId == customer.Id || o.CustomerPhone == customer.PhoneNumber).ToList();
                if (userOrders.Count == 0)
                {
                    // Fallback to show demo orders if newly created customer has no history yet
                    userOrders = _dataStore.Orders;
                }
            }
            else
            {
                userOrders = _dataStore.Orders;
            }

            ViewBag.HighlightedOrderId = orderId;
            return View(userOrders);
        }
    }
}
