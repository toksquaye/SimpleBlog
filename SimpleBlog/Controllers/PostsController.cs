using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Linq;
using SimpleBlog.Models;
using System.Web.Mvc;
using SimpleBlog.ViewModel;
using SimpleBlog.Infracstructure;
using System.Text.RegularExpressions;

namespace SimpleBlog.Controllers
{
    public class PostsController : Controller
    {
        private const int PostsPerPage = 10;
        public ActionResult Index(int page = 1)
        {
            //return Content("Hello World");
            var baseQuery = Database.Session.Query<Post>() //get posts that dont have a deletedat value
                .Where(t => t.DeletedAt == null)            //i.e. undeleted posts
                .OrderByDescending(t => t.CreatedAt);

            var totalPostCount = baseQuery.Count();
            var postIds = baseQuery.Skip((page - 1) * PostsPerPage) //skip posts that dont belong on this page
                                    .Take(PostsPerPage)             //take # posts per page
                                    .Select(t => t.Id)              //select the ID column
                                    .ToArray();                     //flatten into an array
            var posts = baseQuery.Where(t => postIds.Contains(t.Id)) //from the undeleted posts list, grab posts that belong to this page
                                                    .FetchMany(t => t.Tags)
                                                    .Fetch(t => t.User)
                                                    .ToList();
            return View(new PostsIndex
                {
                    Posts = new PagedData<Post>(posts, totalPostCount, page,PostsPerPage)
                });
        }

        public ActionResult Tag(string idAndSlug, int page=1)
        {
            var parts = SeperateIdAndSlug(idAndSlug);
            if (parts == null)
                return HttpNotFound();

            var tag = Database.Session.Load<Tag>(parts.Item1); //load data using id of slug
            if (tag == null)
                return HttpNotFound();

            //if id matched, but slug doesn't match, display the post of the id and use the correct slug from database
            if (!tag.Slug.Equals(parts.Item2, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoutePermanent("tag", new { id = parts.Item1, slug = tag.Slug }); //this ensures that theres only 1 way to get to a post - for seo purposes

            var totalPostCount = tag.Posts.Count();
            var postIds = tag.Posts
                                   .OrderByDescending(t => t.CreatedAt)
                                   .Skip((page - 1) * PostsPerPage)
                                   .Take(PostsPerPage)
                                   .Where(t => t.DeletedAt == null)
                                   .Select(t => t.Id)
                                   .ToArray();

            var posts = Database.Session.Query<Post>()
                .OrderByDescending(b => b.CreatedAt)
                .Where(t => postIds.Contains(t.Id))
                .FetchMany(f => f.Tags)
                .Fetch(f => f.User)
                .ToList();

            return View(new PostsTag
            {
                Tag = tag,
                Posts = new PagedData<Post>(posts, totalPostCount, page, PostsPerPage )
            });
        }

        public ActionResult Show(string idAndSlug)
        {
            var parts = SeperateIdAndSlug(idAndSlug);
            if (parts == null)
                return HttpNotFound();

            var post = Database.Session.Load<Post>(parts.Item1); //load data using id of slug
            if (post == null || post.IsDeleted)
                return HttpNotFound();

            //if id matched, but slug doesn't match, display the post of the id and use the correct slug from database
            if (!post.Slug.Equals(parts.Item2, StringComparison.CurrentCultureIgnoreCase))
                return RedirectToRoutePermanent("Post", new { id = parts.Item1, slug = post.Slug }); //this ensures that theres only 1 way to get to a post - for seo purposes

            return View(new PostsShow
            {
                Post = post
            });
        }

        //Tuple represents a pair
        private System.Tuple<int, string> SeperateIdAndSlug(string idAndSlug)
        {
            var matches = Regex.Match(idAndSlug, @"^(\d+)\-(.*)?$");
            if (!matches.Success)
                return null;

            var id = int.Parse(matches.Result("$1")); //extracts the first group - (/d+)
                                                        //since regex succeeded, we can assume it's a valid in & int.parse wont fail
            var slug = matches.Result("$2"); //extract 2nd group
            return System.Tuple.Create(id, slug);
        }
    }
}