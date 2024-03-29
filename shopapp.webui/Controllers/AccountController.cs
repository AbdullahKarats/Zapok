using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController:Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _userManager=userManager;
            _signInManager=signInManager;
        }
        public IActionResult Login(string ReturnUrl=null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {   
                return View(model);
            }

            // var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user==null)
            {
                ModelState.AddModelError("","Bu kullanıcı adı ile daha önce hesap oluşturulmamış");
                return View(model);
            } 

            var result = await _signInManager.PasswordSignInAsync(user,model.Password,true,false);

            if(result.Succeeded) 
            {
                return Redirect(model.ReturnUrl??"~/");
            }

            ModelState.AddModelError("","Girilen kullanıcı adı veya parola yanlış");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName  = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email    
            };           

            var result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded)
            {
                // generate token
                // email
                return RedirectToAction("Login","Account");
            }

            ModelState.AddModelError("","Bilinmeyen hata oldu lütfen tekrar deneyiniz.");
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

    }
}