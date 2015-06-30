using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles="admin")] //only a logged in admin can use controller
    public class PostsController : Controller
    {
        public ActionResult Index()
        {
            return Content("Admin Posts");
        }
    }
}