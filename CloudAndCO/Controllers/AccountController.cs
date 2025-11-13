//using CloudAndCO.DB;
//using Microsoft.AspNetCore.Mvc;

//namespace CloudAndCO.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly CloudAndCoContext _context;
//        public AccountController(CloudAndCoContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login(string username, string password)
//        {
//            // Placeholder for authentication logic
//            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
//            if (user != null)
//            {
//                // Logic for successful login (e.g., setting cookies, session, etc.)
//                return RedirectToAction("Index", "Home");
//            }
//            ModelState.AddModelError("", "Invalid username or password");
//            return View();
//        }

//        public IActionResult Logout()
//        {
//            // Placeholder for logout logic (e.g., clearing cookies, session, etc.)
//            return RedirectToAction("Login");
//        }


//    }
//}

using System.Data;
using System.Security.Cryptography;
using System.Text;
using CloudAndCO.DB;
using CloudAndCO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudAndCO.Controllers
{
    public class AccountController : Controller
    {
        private readonly CloudAndCoContext _context;

        public AccountController(CloudAndCoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If user is already logged in, redirect to home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(string username, string password)
        //{
        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        ModelState.AddModelError("", "Please enter both username and password");
        //        return View();
        //    }

        //    // Hash the password before comparing
        //    var hashedPassword = HashPassword(password);

        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPassword);

        //    if (user != null)
        //    {
        //        // Set session variables
        //        HttpContext.Session.SetString("UserId", user.UserId.ToString());
        //        HttpContext.Session.SetString("Username", user.Username);
        //        HttpContext.Session.SetString("UserRole", user.Role);

        //        // Set temporary success message
        //        TempData["SuccessMessage"] = $"Welcome back, {user.Username}!";

        //        // Redirect based on role
        //        if (user.Role == "Admin")
        //        {
        //            return RedirectToAction("Index", "Admin");
        //        }

        //        return RedirectToAction("Index", "Home");
        //    }

        //    ModelState.AddModelError("", "Invalid username or password");
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "Please enter both username and password");
                    return View();
                }

                // Hash the password before comparing
                var hashedPassword = HashPassword(password);
                Console.WriteLine($"Login attempt - Username: {username}, Hash: {hashedPassword}");

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPassword);

                if (user != null)
                {
                    Console.WriteLine($"User found - ID: {user.UserId}, Role: {user.Role}");

                    // Set session variables
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Debug: Check session values
                    Console.WriteLine($"Session set - UserId: {user.UserId}, Role: {user.Role}");

                    // Set temporary success message
                    TempData["SuccessMessage"] = $"Welcome back, {user.Username}!";

                    // Redirect based on role
                    if (user.Role == "Admin")
                    {
                        Console.WriteLine("Redirecting to Admin Dashboard");
                        return RedirectToAction("Dashboard", "Admin");
                    }

                    Console.WriteLine("Redirecting to Home (Customer)");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("User not found or password incorrect");
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred during login");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(string username, string password, string confirmPassword, string email)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View();
            }

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists");
                return View();
            }

            // Create admin user (bypasses the normal customer creation)
            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = "Admin", // Explicitly set as Admin
                DateCreated = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin account created successfully! Please login.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword, string email, string role = "Customer")
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View();
            }

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists");
                return View();
            }

            // Determine role - default to Customer
            string userRole = "Customer";
            if (role !=
                "Customer" && role != "Admin")
                {
                    ModelState.AddModelError("", "Invalid role specified");
                    return View();
            }
            // Removed the check for 'role' since it does not exist in the method parameters or context

            // Create new user
            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Role = userRole, // Use the determined role
                DateCreated = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create customer profile
            var customer = new Customer
            {
                UserId = user.UserId,
                Name = username,
                Email = email,
                Phone = "",
                Address = ""
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }



        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
