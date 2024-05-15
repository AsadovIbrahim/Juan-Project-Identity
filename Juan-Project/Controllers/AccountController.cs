using Juan_Project.Helpers.Account;
using Juan_Project.ViewModels.Account;
using JuanDB.Entities.Concretes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Juan_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            User newUser = new User()
            {
                FullName = registerVM.FullName,
                Email = registerVM.Email,
                UserName = registerVM.Email
            };
            var result = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (!result.Succeeded)
            {

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            //await _userManager.AddToRoleAsync(newUser, UserRole.Member.ToString());

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM,string ?returnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            User user;

            if (loginVM.UsernameOrEmail.Contains("@"))
            {
                user=await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
            }
            else
            {
                user=await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Username Or Email Incorrect!");
                return View();
            }

            var result=await _signInManager.CheckPasswordSignInAsync(user,loginVM.Password,true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Try again later");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username Or Email Incorrect!");
                return View();
            }
            await _signInManager.SignInAsync(user, loginVM.RememberMe);

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }


        public async Task<IActionResult> CreateRole()
        {
            foreach (var item in Enum.GetValues(typeof(UserRole)))
            {
                await _roleManager.CreateAsync(new IdentityRole()
                {
                    Name = item.ToString()
                });
            }
            return Ok();
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
