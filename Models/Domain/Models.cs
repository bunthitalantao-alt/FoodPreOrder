namespace FoodPreOrder.Models.Domain
{
    public enum UserRole
    {
        Customer,
        Operator,
        Admin
    }

    public class UserAccount
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;
        public string? ShopName { get; set; }
        public string? ShopId { get; set; }
    }

    public class FoodItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ShopId { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "อาหารจานเดียว";
        public decimal Price { get; set; }
        public int PrepTimeMinutes { get; set; } = 15;
        public bool IsAvailable { get; set; } = true;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class OrderItem
    {
        public string FoodId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public enum OrderStatus
    {
        PENDING,   // 1. รอร้านค้ารับออเดอร์
        COOKING,   // 2. กำลังปรุงอาหาร
        READY,     // 3. พร้อมรับอาหารหน้าร้าน
        COMPLETED  // 4. รับสินค้าเรียบร้อย
    }

    public class PreOrder
    {
        public string Id { get; set; } = "FPO-" + new Random().Next(1000, 9999);
        public string UserId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShopId { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string ShopLocation { get; set; } = "จุดนัดรับอาหารหน้าร้าน";
        public List<OrderItem> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public string PickupTime { get; set; } = string.Empty;
        public string? Note { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PENDING;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum MerchantApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class MerchantApplication
    {
        public string Id { get; set; } = "S-" + Guid.NewGuid().ToString("N")[..4];
        public string OwnerName { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string Category { get; set; } = "อาหารจานเดียว";
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string OperatingHours { get; set; } = "08:00 - 20:00 น.";
        public string TaxId { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public MerchantApplicationStatus Status { get; set; } = MerchantApplicationStatus.Pending;
        public DateTime AppliedAt { get; set; } = DateTime.Now;
    }
}
