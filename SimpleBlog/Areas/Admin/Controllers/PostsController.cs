using SimpleBlog.Infracstructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Models;
using NHibernate.Linq;
using SimpleBlog.Areas.Admin.ViewModels;
namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles="admin")] //only a logged in admin can use controller
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        private const int PostsPerPage = 5;

        public ActionResult Index(int page = 1) //default parameter value if none is provided
        {
            var totalPostCount = Database.Session.Query<Post>().Count(); //count of items in post table of database

            var currentPostPage = Database.Session.Query<Post>()
                .OrderByDescending(c => c.CreatedAt) //order in descending order based on createat date
                .Skip((page - 1) * PostsPerPage) //if page 2, skip (2-1)*5 items
                .Take(PostsPerPage)     //take 5 items from after skipping aboove
                .ToList();  //flatten them into a list
            return View(new PostsIndex
                {
                    Posts = new PagedData<Post>(currentPostPage, totalPostCount, page, PostsPerPage)
                });
        }
    }
}