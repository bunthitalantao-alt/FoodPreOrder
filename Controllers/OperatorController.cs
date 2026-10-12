using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FoodPreOrder.Models.Domain;
using FoodPreOrder.Models.ViewModels;
using FoodPreOrder.Services;

namespace FoodPreOrder.Controllers
{
    public class OperatorController : Controller
    {
        private readonly FoodPreOrderDataStore _dataStore;
        private const string SessionOperatorKey = "Operator_User";

        public OperatorController(FoodPreOrderDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        private UserAccount? GetCurrentOperator()
        {
            var json = HttpContext.Session.GetString(SessionOperatorKey);
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

        // GET: /Operator/Login (Dedicated Operator Login)
        [HttpGet]
        public IActionResult Login()
        {
            if (GetCurrentOperator() != null)
            {
                return RedirectToAction(nameof(Dashboard));
            }
            return View(new OperatorLoginViewModel());
        }

        // POST: /Operator/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(OperatorLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var op = _dataStore.Users.FirstOrDefault(u => 
                u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase) && 
                u.Password == model.Password &&
                u.Role == UserRole.Operator);

            if (op != null)
            {
                HttpContext.Session.SetString(SessionOperatorKey, JsonSerializer.Serialize(op));
                TempData["SuccessMessage"] = $"เข้าสู่ระบบร้านค้า {op.ShopName} สำเร็จ!";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError(string.Empty, "อีเมลหรือรหัสผ่านไม่ถูกต้องสำหรับบัญชีผู้ประกอบการ");
            return View(model);
        }

        // GET: /Operator/Register (Merchant Application)
        [HttpGet]
        public IActionResult Register()
        {
            return View(new OperatorRegisterViewModel());
        }

        // POST: /Operator/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(OperatorRegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_dataStore.Users.Any(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Email", "อีเมลนี้มีอยู่ในระบบแล้ว");
                return View(model);
            }

            var newShopId = "S-" + Guid.NewGuid().ToString("N")[..4];
            var newApp = new MerchantApplication
            {
                Id = newShopId,
                OwnerName = model.OwnerName,
                ShopName = model.ShopName,
                Category = model.Category,
                Phone = model.Phone,
                Email = model.Email,
                Location = model.Location,
                OperatingHours = model.OperatingHours,
                TaxId = model.TaxId ?? "-",
                BankAccount = model.BankAccount,
                Status = MerchantApplicationStatus.Pending
            };

            _dataStore.MerchantApplications.Add(newApp);

            var opUser = new UserAccount
            {
                Id = newShopId,
                FullName = model.OwnerName,
                PhoneNumber = model.Phone,
                Email = model.Email,
                Password = model.Password,
                Role = UserRole.Operator,
                ShopId = newShopId,
                ShopName = model.ShopName
            };
            _dataStore.Users.Add(opUser);

            HttpContext.Session.SetString(SessionOperatorKey, JsonSerializer.Serialize(opUser));
            TempData["SuccessMessage"] = "ส่งคำขอลงทะเบียนเปิดร้านค้าสำเร็จ! รอผู้ดูแลระบบอนุมัติ";
            return RedirectToAction(nameof(Dashboard));
        }

        // POST: /Operator/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionOperatorKey);
            TempData["InfoMessage"] = "ออกจากระบบร้านค้าเรียบร้อยแล้ว";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Operator/Dashboard (Single continuous page "ต่อกัน" with only "จัดการร้านค้า")
        public IActionResult Dashboard()
        {
            var op = GetCurrentOperator();
            if (op == null)
            {
                // Default to first operator in store for easy viewing
                op = _dataStore.Users.FirstOrDefault(u => u.Role == UserRole.Operator);
                if (op != null)
                {
                    HttpContext.Session.SetString(SessionOperatorKey, JsonSerializer.Serialize(op));
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }
            }

            var orders = _dataStore.Orders.Where(o => o.ShopName.Contains("Ocean Bites") || o.ShopId == op.ShopId).ToList();
            if (orders.Count == 0) orders = _dataStore.Orders;

            var menus = _dataStore.MenuItems.Where(m => m.ShopName.Contains("Ocean Bites") || m.ShopId == op.ShopId).ToList();
            if (menus.Count == 0) menus = _dataStore.MenuItems;

            var todaySales = orders.Sum(o => o.TotalPrice);
            var pendingCount = orders.Count(o => o.Status == OrderStatus.PENDING || o.Status == OrderStatus.COOKING);
            var completedCount = orders.Count(o => o.Status == OrderStatus.COMPLETED);

            var vm = new OperatorDashboardViewModel
            {
                ShopName = op.ShopName ?? "Ocean Bites (โอเชี่ยน ไบท์ส)",
                TodaySales = todaySales,
                PendingOrdersCount = pendingCount,
                CompletedOrdersCount = completedCount,
                TotalMenusCount = menus.Count,
                Orders = orders,
                MenuItems = menus
            };

            ViewBag.CurrentOperator = op;
            return View(vm);
        }

        // POST: /Operator/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus(string orderId, OrderStatus status)
        {
            _dataStore.UpdateOrderStatus(orderId, status);
            TempData["SuccessMessage"] = $"อัปเดตสถานะออเดอร์ #{orderId} เรียบร้อยแล้ว";
            return RedirectToAction(nameof(Dashboard));
        }

        // POST: /Operator/AddMenu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMenu(AddFoodItemViewModel model)
        {
            var op = GetCurrentOperator();
            var shopName = op?.ShopName ?? "Ocean Bites (โอเชี่ยน ไบท์ส)";
            var shopId = op?.ShopId ?? "S1";

            if (ModelState.IsValid)
            {
                var newItem = new FoodItem
                {
                    Id = "M-" + Guid.NewGuid().ToString("N")[..4],
                    ShopId = shopId,
                    ShopName = shopName,
                    Name = model.Name,
                    Price = model.Price,
                    PrepTimeMinutes = model.PrepTimeMinutes,
                    Category = model.Category,
                    IsAvailable = true,
                    ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) 
                        ? "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=500" 
                        : model.ImageUrl
                };

                _dataStore.AddMenuItem(newItem);
                TempData["SuccessMessage"] = $"เพิ่มเมนู \"{newItem.Name}\" สำเร็จแล้ว!";
            }
            else
            {
                TempData["ErrorMessage"] = "ข้อมูลเมนูไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        // POST: /Operator/DeleteMenu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMenu(string id)
        {
            _dataStore.DeleteMenuItem(id);
            TempData["SuccessMessage"] = "ลบรายการเมนูเรียบร้อยแล้ว";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
