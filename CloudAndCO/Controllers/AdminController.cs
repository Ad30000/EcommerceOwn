//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using CloudAndCO.DB;
//using CloudAndCO.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace CloudAndCO.Controllers
//{

//    public class AdminController : Controller
//    {
//        private readonly CloudAndCoContext _context;

//        public AdminController(CloudAndCoContext context)
//        {
//            _context = context;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        // Admin Dashboard
//        public async Task<IActionResult> Dashboard()
//        {
//            var dashboardStats = new
//            {
//                TotalUsers = await _context.Users.CountAsync(),
//                TotalCustomers = await _context.Customers.CountAsync(),
//                TotalProducts = await _context.Products.CountAsync(),
//                TotalOrders = await _context.Orders.CountAsync(),
//                RecentOrders = await _context.Orders
//                    .Include(o => o.Customer)
//                    .OrderByDescending(o => o.OrderDate)
//                    .Take(10)
//                    .ToListAsync()
//            };

//            return View(dashboardStats);
//        }

//        // Manage Users
//        public async Task<IActionResult> ManageUsers()
//        {
//            var users = await _context.Users.ToListAsync();
//            return View(users);
//        }

//        // Manage Products
//        public async Task<IActionResult> ManageProducts()
//        {
//            var products = await _context.Products.ToListAsync();
//            return View(products);
//        }

//        // View All Orders
//        public async Task<IActionResult> ViewOrders()
//        {
//            var orders = await _context.Orders
//                .Include(o => o.Customer)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                .OrderByDescending(o => o.OrderDate)
//                .ToListAsync();

//            return View(orders);
//        }

//        // Update Order Status
//        [HttpPost]
//        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
//        {
//            var order = await _context.Orders.FindAsync(orderId);
//            if (order != null)
//            {
//                order.Status = status;
//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = $"Order #{orderId} status updated to {status}";
//            }
//            return RedirectToAction("ViewOrders");
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudAndCO.DB;
using CloudAndCO.Models;

namespace CloudAndCO.Controllers
{
    // Controller for admin-specific actions like managing users, products, and orders
    public class AdminController : Controller
    {
        private readonly CloudAndCoContext _context;

        public AdminController(CloudAndCoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        // Admin Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["ErrorMessage"] = "Access denied. Admin privileges required.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var stats = new
                {
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalCustomers = await _context.Customers.CountAsync(),
                    TotalProducts = await _context.Products.CountAsync(),
                    TotalOrders = await _context.Orders.CountAsync(),
                    PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Placed"),
                    RecentOrders = await _context.Orders
                        .Include(o => o.Customer)
                        .OrderByDescending(o => o.OrderDate)
                        .Take(5)
                        .ToListAsync()
                };

                ViewBag.TotalUsers = stats.TotalUsers;
                ViewBag.TotalCustomers = stats.TotalCustomers;
                ViewBag.TotalProducts = stats.TotalProducts;
                ViewBag.TotalOrders = stats.TotalOrders;
                ViewBag.PendingOrders = stats.PendingOrders;
                ViewBag.RecentOrders = stats.RecentOrders;

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
                return View();
            }
        }

        // Simple admin actions
        public async Task<IActionResult> ManageUsers()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> ViewOrders()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> UpdateOrderStatus(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer) // If you have customer relationship
                .Include(o => o.OrderItems) // If you have order items
                .ThenInclude(oi => oi.Product) // If order items have product relationship
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                // Map to valid status values based on your CHECK constraint
                var validStatus = status.ToUpper() switch
                {
                    "PENDING" => "PENDING",
                    "PROCESSED" => "PROCESSED",
                    "SHIPPED" => "SHIPPED",
                    "DELIVERED" => "DELIVERED",
                    "CANCELLED" => "CANCELLED",
                    _ => "PENDING"
                };

                order.Status = validStatus;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Order #{orderId} status updated to {validStatus}";
            }

            return RedirectToAction("ViewOrders");
        }

        public async Task<IActionResult> MarkAsProcessed(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = "PROCESSED";
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order #{order.OrderId} has been processed successfully!";
            return RedirectToAction(nameof(ViewOrders));
        }

        public async Task<IActionResult> MarkAsShipped(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = "SHIPPED";
            _context.Update(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Order #{order.OrderId} has been marked as shipped!";
            return RedirectToAction(nameof(ViewOrders));
        }

        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = "DELIVERED";
            _context.Update(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Order #{order.OrderId} has been marked as delivered!";
            return RedirectToAction(nameof(ViewOrders));

        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = "CANCELLED";
            _context.Update(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Order #{order.OrderId} has been cancelled!";
            return RedirectToAction(nameof(ViewOrders));
        }
    }

}