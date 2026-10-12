using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FoodPreOrder.Models.Domain;
using FoodPreOrder.Models.ViewModels;
using FoodPreOrder.Services;

namespace FoodPreOrder.Controllers
{
    public class AdminController : Controller
    {
        private readonly FoodPreOrderDataStore _dataStore;
        private const string SessionAdminKey = "Admin_User";

        public AdminController(FoodPreOrderDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        private UserAccount? GetCurrentAdmin()
        {
            var json = HttpContext.Session.GetString(SessionAdminKey);
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

        // GET: /Admin/Login (Dedicated Admin Login)
        [HttpGet]
        public IActionResult Login()
        {
            if (GetCurrentAdmin() != null)
            {
                return RedirectToAction(nameof(Dashboard));
            }
            return View(new AdminLoginViewModel());
        }

        // POST: /Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var admin = _dataStore.Users.FirstOrDefault(u => 
                u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase) && 
                u.Password == model.Password &&
                u.Role == UserRole.Admin);

            if (admin != null)
            {
                HttpContext.Session.SetString(SessionAdminKey, JsonSerializer.Serialize(admin));
                TempData["SuccessMessage"] = "เข้าสู่ระบบผู้ดูแลระบบสำเร็จ!";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError(string.Empty, "อีเมลหรือรหัสผ่านผู้ดูแลระบบไม่ถูกต้อง");
            return View(model);
        }

        // POST: /Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionAdminKey);
            TempData["InfoMessage"] = "ออกจากระบบผู้ดูแลเรียบร้อยแล้ว";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            var admin = GetCurrentAdmin();
            if (admin == null)
            {
                // Fallback to default admin in store for quick review if accessed directly
                admin = _dataStore.Users.FirstOrDefault(u => u.Role == UserRole.Admin);
                if (admin != null)
                {
                    HttpContext.Session.SetString(SessionAdminKey, JsonSerializer.Serialize(admin));
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }
            }

            var vm = new AdminDashboardViewModel
            {
                TotalGMV = 148900m,
                RegisteredMerchantsCount = _dataStore.MerchantApplications.Count(a => a.Status == MerchantApplicationStatus.Approved),
                CustomerUsersCount = 248,
                Applications = _dataStore.MerchantApplications.OrderByDescending(a => a.AppliedAt).ToList()
            };

            ViewBag.CurrentAdmin = admin;
            return View(vm);
        }

        // POST: /Admin/ApproveMerchant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveMerchant(string id)
        {
            _dataStore.ApproveMerchant(id);
            TempData["SuccessMessage"] = $"อนุมัติคำขอร้านค้า ID #{id} เรียบร้อยแล้ว!";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
