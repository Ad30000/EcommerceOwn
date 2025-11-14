using System;
using System.Linq;
using System.Threading.Tasks;
using CloudAndCO.DB;
using CloudAndCO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CloudAndCO.Controllers
{
    // Controller for managing cart items
    [Route("[controller]")]
    public class CartItemsController : Controller
    {
        private readonly CloudAndCoContext _context;

        public CartItemsController(CloudAndCoContext context)
        {
            _context = context;
        }

        // GET: CartItems
        [HttpGet]
        [Route("")]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == int.Parse(userId));

            if (customer == null)
            {
                return RedirectToAction("Create", "Customers");
            }

            // Load cart items with products for this customer
            var cartItems = await _context.CartItems
                .Where(c => c.CustomerId == customer.CustomerId)
                .Include(c => c.Product)
                .Include(c => c.Customer)
                .ToListAsync();

            return View(cartItems);
        }

        // POST: CartItems/AddToCart
        [HttpPost("AddToCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get customer ID from user
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == int.Parse(userId));

            if (customer == null)
            {
                // Handle error - customer not found
                return RedirectToAction("Create", "Customers");
            }

            // Check if product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Check if item already in cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId && c.ProductId == productId);

            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Create new cart item
                var cartItem = new CartItem
                {
                    CustomerId = customer.CustomerId,
                    ProductId = productId,
                    Quantity = quantity,
                    Date = DateTime.Now
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product added to cart!";
            return RedirectToAction("Index");
        }

        // POST: CartItems/ProcessOrder
        [HttpPost("ProcessOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == int.Parse(userId));

            if (customer == null)
            {
                return RedirectToAction("Create", "Customers");
            }

            // Get all cart items for this customer
            var cartItems = await _context.CartItems
                .Where(c => c.CustomerId == customer.CustomerId)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            // Create new order - Use a valid status based on your CHECK constraint
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                OrderDate = DateTime.Now,
                Status = "Placed", // Changed from "Processing" to "Placed"
                TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Save to get OrderId

            // Create order items from cart items
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };
                _context.OrderItems.Add(orderItem);

                // Update product stock
                cartItem.Product.StockqUantity -= cartItem.Quantity;
            }

            // Clear the cart
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order #{order.OrderId} placed successfully!";
            return RedirectToAction("Details", "Orders", new { id = order.OrderId });
        }

        // Additional CRUD operations for CartItems (Edit, Delete, etc.)
        // GET: CartItems/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: CartItems/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartItemId,CustomerId,ProductId,Quantity,Date")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", cartItem.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", cartItem.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", cartItem.ProductId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartItemId,CustomerId,ProductId,Quantity,Date")] CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.CartItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", cartItem.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Customer)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }
    }
}
