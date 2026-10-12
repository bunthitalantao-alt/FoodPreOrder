using System.ComponentModel.DataAnnotations;
using FoodPreOrder.Models.Domain;

namespace FoodPreOrder.Models.ViewModels
{
    // ================= CUSTOMER VIEW MODELS =================
    public class CustomerLoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        [Display(Name = "อีเมลผู้ใช้งาน")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Display(Name = "รหัสผ่าน")]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public class CustomerRegisterViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อ - นามสกุล")]
        [Display(Name = "ชื่อ - นามสกุล *")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท์ติดต่อ")]
        [Phone(ErrorMessage = "รูปแบบเบอร์โทรศัพท์ไม่ถูกต้อง")]
        [Display(Name = "เบอร์โทรศัพท์ติดต่อ *")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกอีเมลสำหรับเข้าสู่ระบบ")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        [Display(Name = "อีเมลสำหรับเข้าสู่ระบบ *")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [StringLength(100, ErrorMessage = "รหัสผ่านต้องมีความยาวอย่างน้อย {2} ตัวอักษร", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "รหัสผ่าน *")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "ยืนยันรหัสผ่าน *")]
        [Compare("Password", ErrorMessage = "รหัสผ่านและการยืนยันรหัสผ่านไม่ตรงกัน")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class CartItemViewModel
    {
        public string FoodId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string ShopName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class CheckoutRequestViewModel
    {
        [Required(ErrorMessage = "กรุณาระบุเวลานัดรับอาหารล่วงหน้า")]
        [Display(Name = "เวลานัดรับอาหารล่วงหน้า *")]
        public string PickupTime { get; set; } = string.Empty;

        [Display(Name = "หมายเหตุเพิ่มเติมถึงร้านค้า")]
        public string? Note { get; set; }
    }

    // ================= OPERATOR VIEW MODELS =================
    public class OperatorLoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกอีเมลหรือรหัสร้านค้า")]
        [Display(Name = "อีเมลร้านค้า")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Display(Name = "รหัสผ่าน")]
        public string Password { get; set; } = string.Empty;
    }

    public class OperatorRegisterViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อ-นามสกุล เจ้าของร้าน")]
        [Display(Name = "ชื่อ-นามสกุล เจ้าของร้าน *")]
        public string OwnerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกชื่อร้านค้า")]
        [Display(Name = "ชื่อร้านค้า (Shop Name) *")]
        public string ShopName { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่อาหารหลัก")]
        [Display(Name = "หมวดหมู่อาหารหลัก *")]
        public string Category { get; set; } = "อาหารจานเดียว";

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท์ติดต่อร้านค้า")]
        [Display(Name = "เบอร์โทรศัพท์ติดต่อร้านค้า *")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกที่ตั้งร้านค้าหรือจุดนัดรับอาหาร")]
        [Display(Name = "ที่ตั้งร้านค้า / จุดนัดรับอาหารหน้าร้าน *")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาระบุเวลาเปิด - ปิด ให้บริการ")]
        [Display(Name = "เวลาเปิด - ปิด ให้บริการ *")]
        public string OperatingHours { get; set; } = "08:00 - 20:00 น.";

        [Display(Name = "เลขประจำตัวผู้เสียภาษี / พาณิชย์")]
        public string? TaxId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกบัญชีธนาคารสำหรับรับยอดเงิน")]
        [Display(Name = "บัญชีธนาคารสำหรับรับยอดสรุปรายเดือน *")]
        public string BankAccount { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกอีเมลสำหรับเข้าสู่ระบบ")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        [Display(Name = "อีเมลสำหรับเข้าสู่ระบบ *")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากำหนดรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Display(Name = "รหัสผ่าน *")]
        public string Password { get; set; } = string.Empty;
    }

    public class AddFoodItemViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อเมนูอาหาร")]
        [Display(Name = "ชื่อเมนูอาหาร *")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกราคา")]
        [Range(1, 10000, ErrorMessage = "ราคาต้องมากกว่า 0 บาท")]
        [Display(Name = "ราคา (บาท) *")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "กรุณาระบุเวลาเตรียม")]
        [Range(1, 120, ErrorMessage = "เวลาเตรียมต้องอยู่ระหว่าง 1-120 นาที")]
        [Display(Name = "เวลาเตรียม (นาที) *")]
        public int PrepTimeMinutes { get; set; } = 15;

        [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่อาหาร")]
        [Display(Name = "หมวดหมู่อาหาร *")]
        public string Category { get; set; } = "อาหารจานเดียว";

        [Display(Name = "รูปภาพ URL")]
        public string? ImageUrl { get; set; }
    }

    public class OperatorDashboardViewModel
    {
        public string ShopName { get; set; } = string.Empty;
        public decimal TodaySales { get; set; }
        public int PendingOrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public int TotalMenusCount { get; set; }
        public List<PreOrder> Orders { get; set; } = new();
        public List<FoodItem> MenuItems { get; set; } = new();
    }

    // ================= ADMIN VIEW MODELS =================
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกอีเมลผู้ดูแลระบบ")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        [Display(Name = "อีเมลผู้ดูแลระบบ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Display(Name = "รหัสผ่าน")]
        public string Password { get; set; } = string.Empty;
    }

    public class AdminDashboardViewModel
    {
        public decimal TotalGMV { get; set; } = 148900m;
        public int RegisteredMerchantsCount { get; set; } = 2;
        public int CustomerUsersCount { get; set; } = 248;
        public List<MerchantApplication> Applications { get; set; } = new();
    }
}
