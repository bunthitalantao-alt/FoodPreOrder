using System.Collections.Concurrent;
using FoodPreOrder.Models.Domain;

namespace FoodPreOrder.Services
{
    public class FoodPreOrderDataStore
    {
        public ConcurrentBag<UserAccount> Users { get; } = new();
        public List<FoodItem> MenuItems { get; } = new();
        public List<PreOrder> Orders { get; } = new();
        public List<MerchantApplication> MerchantApplications { get; } = new();

        private readonly object _lock = new();

        public FoodPreOrderDataStore()
        {
            SeedInitialData();
        }

        private void SeedInitialData()
        {
            // Seed Users
            Users.Add(new UserAccount
            {
                Id = "U-CUST-1",
                FullName = "สมชาย ใจดี",
                PhoneNumber = "081-234-5678",
                Email = "customer@demo.com",
                Password = "password123",
                Role = UserRole.Customer
            });

            Users.Add(new UserAccount
            {
                Id = "S1",
                FullName = "นายวิชัย อร่อยดี",
                PhoneNumber = "02-999-8888",
                Email = "operator@demo.com",
                Password = "password123",
                Role = UserRole.Operator,
                ShopId = "S1",
                ShopName = "Ocean Bites (โอเชี่ยน ไบท์ส)"
            });

            Users.Add(new UserAccount
            {
                Id = "ADM-1",
                FullName = "ผู้ดูแลระบบสูงสุด",
                PhoneNumber = "080-000-0000",
                Email = "admin@demo.com",
                Password = "password123",
                Role = UserRole.Admin
            });

            // Seed Menu Items
            MenuItems.AddRange(new[]
            {
                new FoodItem
                {
                    Id = "M1",
                    ShopId = "S1",
                    ShopName = "Ocean Bites (โอเชี่ยน ไบท์ส)",
                    Name = "ข้าวผัดซีฟู้ดต้มยำแห้ง",
                    Category = "อาหารจานเดียว",
                    Price = 95m,
                    PrepTimeMinutes = 15,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1559847844-5315695dadae?w=500&auto=format&fit=crop&q=60"
                },
                new FoodItem
                {
                    Id = "M2",
                    ShopId = "S1",
                    ShopName = "Ocean Bites (โอเชี่ยน ไบท์ส)",
                    Name = "สเต๊กปลากะพงซอสซัลซ่า",
                    Category = "อาหารจานเดียว",
                    Price = 189m,
                    PrepTimeMinutes = 20,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=500&auto=format&fit=crop&q=60"
                },
                new FoodItem
                {
                    Id = "M3",
                    ShopId = "S1",
                    ShopName = "Ocean Bites (โอเชี่ยน ไบท์ส)",
                    Name = "บลูฮาวายโซดาเย็น",
                    Category = "เครื่องดื่ม",
                    Price = 55m,
                    PrepTimeMinutes = 5,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=500&auto=format&fit=crop&q=60"
                },
                new FoodItem
                {
                    Id = "M4",
                    ShopId = "S2",
                    ShopName = "Blue Wave Cafe",
                    Name = "บลูเบอร์รี่ชีสพาย",
                    Category = "เบเกอรี่",
                    Price = 85m,
                    PrepTimeMinutes = 5,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?w=500&auto=format&fit=crop&q=60"
                },
                new FoodItem
                {
                    Id = "M5",
                    ShopId = "S2",
                    ShopName = "Blue Wave Cafe",
                    Name = "เฟรนช์ฟรายส์ซอสชีส",
                    Category = "ทานเล่น",
                    Price = 69m,
                    PrepTimeMinutes = 10,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=500&auto=format&fit=crop&q=60"
                }
            });

            // Seed Pre-Orders
            Orders.AddRange(new[]
            {
                new PreOrder
                {
                    Id = "FPO-2410",
                    UserId = "U-CUST-1",
                    CustomerName = "สมชาย ใจดี",
                    CustomerPhone = "081-234-5678",
                    ShopId = "S1",
                    ShopName = "Ocean Bites (โอเชี่ยน ไบท์ส)",
                    ShopLocation = "อาคาร A ชั้น 1 หน้าร้าน",
                    PickupTime = "21:30 น.",
                    Note = "ขอเผ็ดน้อย ใส่กล่อง",
                    Status = OrderStatus.PENDING,
                    TotalPrice = 95m,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { FoodId = "M1", Name = "ข้าวผัดซีฟู้ดต้มยำแห้ง", Price = 95m, Quantity = 1 }
                    }
                },
                new PreOrder
                {
                    Id = "FPO-1092",
                    UserId = "U-CUST-1",
                    CustomerName = "สมชาย ใจดี",
                    CustomerPhone = "081-234-5678",
                    ShopId = "S1",
                    ShopName = "Ocean Bites (โอเชี่ยน ไบท์ส)",
                    ShopLocation = "อาคาร A ชั้น 1 หน้าร้าน",
                    PickupTime = "12:30 น.",
                    Note = "แยกน้ำซอสครับ",
                    Status = OrderStatus.COOKING,
                    TotalPrice = 190m,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { FoodId = "M1", Name = "ข้าวผัดซีฟู้ดต้มยำแห้ง", Price = 95m, Quantity = 2 }
                    }
                }
            });

            // Seed Merchant Applications
            MerchantApplications.AddRange(new[]
            {
                new MerchantApplication
                {
                    Id = "S1",
                    OwnerName = "นายวิชัย อร่อยดี",
                    ShopName = "Ocean Bites (โอเชี่ยน ไบท์ส)",
                    Category = "อาหารจานเดียว",
                    Phone = "02-999-8888",
                    Email = "operator@demo.com",
                    Location = "อาคาร A ชั้น 1 หน้าร้าน",
                    OperatingHours = "08:00 - 20:00 น.",
                    TaxId = "1234567890123",
                    BankAccount = "กสิกรไทย 012-3-45678-9",
                    Status = MerchantApplicationStatus.Approved
                },
                new MerchantApplication
                {
                    Id = "S2",
                    OwnerName = "นางสาวสุดา ฟ้าใส",
                    ShopName = "Blue Wave Cafe",
                    Category = "เครื่องดื่ม",
                    Phone = "089-111-2222",
                    Email = "bluewave@demo.com",
                    Location = "โซน B ติดสวนน้ำ",
                    OperatingHours = "09:00 - 18:00 น.",
                    TaxId = "9876543210987",
                    BankAccount = "ไทยพาณิชย์ 987-6-54321-0",
                    Status = MerchantApplicationStatus.Pending
                }
            });
        }

        public void AddOrder(PreOrder order)
        {
            lock (_lock)
            {
                Orders.Insert(0, order);
            }
        }

        public void UpdateOrderStatus(string orderId, OrderStatus status)
        {
            lock (_lock)
            {
                var ord = Orders.FirstOrDefault(o => o.Id == orderId);
                if (ord != null)
                {
                    ord.Status = status;
                }
            }
        }

        public void AddMenuItem(FoodItem item)
        {
            lock (_lock)
            {
                MenuItems.Add(item);
            }
        }

        public void DeleteMenuItem(string id)
        {
            lock (_lock)
            {
                var item = MenuItems.FirstOrDefault(m => m.Id == id);
                if (item != null)
                {
                    MenuItems.Remove(item);
                }
            }
        }

        public void ApproveMerchant(string merchantId)
        {
            lock (_lock)
            {
                var m = MerchantApplications.FirstOrDefault(a => a.Id == merchantId);
                if (m != null)
                {
                    m.Status = MerchantApplicationStatus.Approved;
                }
            }
        }
    }
}
