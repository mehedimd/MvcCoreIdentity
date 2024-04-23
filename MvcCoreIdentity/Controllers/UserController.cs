using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcCoreIdentity.Areas.Identity.Data;
using MvcCoreIdentity.Data;
using MvcCoreIdentity.View_Models;

namespace MvcCoreIdentity.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IHostEnvironment environment;
        public UserController(UserManager<ApplicationUser> _userManager, IHostEnvironment env)
        {
            this.userManager = _userManager;
            this.environment = env;
        }
        
        public IActionResult Index()
        {
            AllUserVM allUser = new AllUserVM();
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var user = userManager.Users.Where(u => u.UserName.Equals(userName)).FirstOrDefault();
                allUser.UserName = user.UserName;
                allUser.FullName = user.FullName;
                allUser.Email = user.Email;
                allUser.PicPath = user.PicPath;
            }
                return View(allUser);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserVM userVM)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var user = userManager.Users.Where(u => u.UserName.Equals(userName)).FirstOrDefault();
                if(user != null)
                {
                    user.FullName = userVM.FullName;
                    if (userVM.Picture != null)
                    {
                        var rootPath = this.environment.ContentRootPath;
                        var fileToSave = Path.Combine(rootPath, "wwwroot/Pictures", userVM.Picture.FileName);
                        using(var fileStream = new FileStream(fileToSave, FileMode.Create))
                        {
                            userVM.Picture.CopyToAsync(fileStream);
                        }
                        user.PicPath = "~/Pictures/" + userVM.Picture.FileName;
                        var result = await userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        if (result.Errors.Count() > 0)
                        {
                            var err = "";
                            foreach(var e in result.Errors)
                            {
                                err += e;
                            }
                            ModelState.AddModelError("", err);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please Provide Image");
                        return View(userVM);
                    }
                }


            }
            else
            {
                ModelState.AddModelError("", "Please Login First");
                return View(userVM);
            }
            return View(userVM);
        }
    }
}
