using Application.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMemberService _memberService;

        public AccountController(IUserService userService, IRoleService roleService, IMemberService memberService)
        {
            _userService = userService;
            _roleService = roleService;
            _memberService = memberService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userService.ValidateUserAsync(email, password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("UserId", user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (user.Role.RoleName == "Admin")
                return RedirectToAction("Index", "Dashboard");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string fullName, string phone, string address)
        {
            var existingUser = await _userService.GetByEmailAsync(email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered";
                return View();
            }

            var studentRole = (await _roleService.GetAllAsync())
                .FirstOrDefault(r => r.RoleName == "Student");

            if (studentRole == null)
            {
                ViewBag.Error = "Student role not configured";
                return View();
            }

            var user = new User
            {
                Name = fullName,
                Email = email,
                RoleId = studentRole.RoleId,
                IsActive = true
            };

            await _userService.RegisterAsync(user, password);

            var member = new Member
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Address = address,
                MembershipDate = DateTime.Now,
                IsActive = true
            };

            await _memberService.AddAsync(member);

            return RedirectToAction("Login");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
