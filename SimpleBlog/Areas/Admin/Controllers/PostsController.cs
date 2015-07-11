using SimpleBlog.Infracstructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Models;
using NHibernate.Linq;
using SimpleBlog.Areas.Admin.ViewModels;
using SimpleBlog.Infracstructure.Extensions;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles="admin")] //only a logged in admin can use controller
    [SelectedTab("posts")]
    public class PostsController : Controller
    {
        private const int PostsPerPage = 10;

        public ActionResult Index(int page = 1) //default parameter value if none is provided
        {
            var totalPostCount = Database.Session.Query<Post>().Count(); //count of items in post table of database

            var baseQuery = Database.Session.Query<Post>().OrderByDescending(c => c.CreatedAt);
            
            //pagination and filtering if necessary
            var postIds = baseQuery//order in descending order based on createat date
                .Skip((page - 1) * PostsPerPage)//if page 2, skip (2-1)*5 items
                .Take(PostsPerPage)//take PostsPerPage items from after skipping aboove
                .Select(p => p.Id)//select their ids
                .ToArray(); //make into an arra

            //data retrieval
            var currentPostPage = baseQuery
                .Where(p => postIds.Contains(p.Id))
                .OrderByDescending(c => c.CreatedAt) //order in descending order based on createat date
                .FetchMany(f => f.Tags)
                .Fetch(f => f.User)
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
                IsNew = true,
                Tags = Database.Session.Query<Tag>().Select(tag => new TagCheckbox 
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    IsChecked = false
                }).ToList()
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
                Title = post.Title,
                Tags = Database.Session.Query<Tag>().Select(tag => new TagCheckbox 
                { 
                    Id = tag.Id,
                    Name = tag.Name,
                    IsChecked = post.Tags.Contains(tag)
                }).ToList()
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Form(PostsForm form)
        {
            form.IsNew = form.PostId == null; //form is new if theres no postId

            if (!ModelState.IsValid)
                return View(form);

            var selectedTags = ReconsileTags(form.Tags).ToList(); //create list of selected tags and match them with values in database
            Post post;
            if (form.IsNew)
            {
                post = new Post
                {
                    CreatedAt = DateTime.UtcNow,
                    User = Auth.User,
                };

                foreach (var tag in selectedTags)
                    post.Tags.Add(tag);
            }
            else
            {
                post = Database.Session.Load<Post>(form.PostId); //extract data with using formid

                if (post == null)
                    return HttpNotFound();

                post.UpdatedAt = DateTime.UtcNow;

                foreach (var toAdd in selectedTags.Where(t => !post.Tags.Contains(t)))
                    post.Tags.Add(toAdd);

                foreach (var toRemove in post.Tags.Where(t => !selectedTags.Contains(t)).ToList())
                    post.Tags.Remove(toRemove);
            }

            post.Title = form.Title;
            post.Slug = form.Slug;
            post.Content = form.Content;

            Database.Session.SaveOrUpdate(post);

            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Trash(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

            post.DeletedAt = DateTime.UtcNow;
            Database.Session.Update(post);
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

            Database.Session.Delete(post);
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Restore(int id)
        {
            var post = Database.Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

            post.DeletedAt = DateTime.UtcNow;
            Database.Session.Update(post);
            return RedirectToAction("Index");
        }

        private IEnumerable<Tag> ReconsileTags(IEnumerable<TagCheckbox> tags)
        {
            foreach (var tag in tags.Where(t => t.IsChecked)) //loop only through checked tags
            {
                if (tag.Id != null)
                {
                    yield return Database.Session.Load<Tag>(tag.Id); //if not new tag, continue
                    continue;
                }

                var existingTag = Database.Session.Query<Tag>().FirstOrDefault(t => t.Name == tag.Name);
                if(existingTag != null) //if new tag matchs one that already exists, return that one
                {
                    yield return existingTag;
                    continue;
                }

                var newTag = new Tag{
                    Name = tag.Name,
                    Slug = tag.Name.Slugify()
                };

                Database.Session.Save(newTag);
                yield return newTag;
            }
        }


    }
}