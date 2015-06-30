using SimpleBlog.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SimpleBlog.Controllers
{
    public class AuthController : Controller
    {
        //
        // GET: /Auth/

        /*public ActionResult Index()
        {
            return View();
        }*/

        /*default - get action*/
        public ActionResult Login()
        {
            //return Content("Login");
            return View( new AuthLogin {});
        }

        [HttpPost]
        public ActionResult Login(AuthLogin form)
        {
            if (!ModelState.IsValid) //if validation check fails
                return View(form);  //give user form back

            if(form.Username != "toks")
            {
                ModelState.AddModelError("Username", "Username or password is wrong");
                return View(form);
            }
            return Content("The form is valid!");

        }

    }
}
