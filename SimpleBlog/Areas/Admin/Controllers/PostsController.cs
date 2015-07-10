﻿using SimpleBlog.Infracstructure;
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

        public ActionResult New()
        {
            return View("form", new PostsForm
            {
                IsNew = true
            });
        }

        public ActionResult Edit(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

            return View("Form", new PostsForm
            {
                IsNew = false,
                PostId = id,
                Content = post.Content,
                Slug = post.Slug,
                Title = post.Title
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Form(PostsForm form)
        {
            form.IsNew = form.PostId == null; //form is new if theres no postId

            if (!ModelState.IsValid)
                return View(form);

            Post post;
            if (form.IsNew)
            {
                post = new Post
                {
                    CreatedAt = DateTime.UtcNow,
                    User = Auth.User,
                };
            }
            else
            {
                post = Database.Session.Load<Post>(form.PostId);

                if (post == null)
                    return HttpNotFound();

                post.UpdatedAt = DateTime.UtcNow;
            }

            post.Title = form.Title;
            post.Slug = form.Slug;
            post.Content = form.Content;

            Database.Session.SaveOrUpdate(post);

            return RedirectToAction("Index");
        }
    }
}