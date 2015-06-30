using SimpleBlog.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace SimpleBlog.Controllers
{
    public class AuthController : Controller
    {
        
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute("home");
        }
        // GET: /Auth/
        /*default - get action*/
        public ActionResult Login()
        {
            //return Content("Login");
            return View( new AuthLogin {});
        }

        [HttpPost]
        public ActionResult Login(AuthLogin form, string returnUrl)
        {
            if (!ModelState.IsValid) //if validation check fails
                return View(form);  //give user form back

            /*if(form.Username != "toks")
            {
                ModelState.AddModelError("Username", "Username or password is wrong");
                return View(form);
            }*/

            //tell asp.net a person is who he says he is. letting asp know what user is logged in
            FormsAuthentication.SetAuthCookie(form.Username, true);

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToRoute("home");
            

        }

    }
}
