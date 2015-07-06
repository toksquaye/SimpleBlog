using SimpleBlog.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NHibernate.Linq;
using SimpleBlog.Models;


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
            var user = Database.Session.Query<User>().FirstOrDefault(u => u.Username == form.Username);

            //this is to prevent timing attacks during login
            //the time to check for a user that isn't in the db and the time
            //that checks for a user that is in the db is now similar
            if (user == null)
                SimpleBlog.Models.User.FakeHash();

            if (user == null || !user.CheckPassword(form.Password))
                ModelState.AddModelError("Username", "Username or password is incorrect");
            if (!ModelState.IsValid) //if validation check fails
                return View(form);  //give user form back

            

            //tell asp.net a person is who he says he is. letting asp know what user is logged in
            FormsAuthentication.SetAuthCookie(user.Username, true);

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToRoute("home");
            

        }

    }
}
