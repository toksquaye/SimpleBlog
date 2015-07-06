﻿using SimpleBlog.Areas.Admin.ViewModels;
using SimpleBlog.Infracstructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using SimpleBlog.Models;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")] //only a logged in admin can use controller
    [SelectedTab("users")]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            return View(new UsersIndex 
            {
                Users = Database.Session.Query<User>().ToList()
            });
        }

        public ActionResult New()
        {
            return View(new UsersNew
                {

                });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(UsersNew form)
        {
            if (Database.Session.Query<User>().Any(u => u.Username == form.Username))
                ModelState.AddModelError("Username","Username must be unique");

            if (!ModelState.IsValid)
                return View(form);
            
            
            //continue processing if no error
            var user = new User
            {
              Email = form.Email,
              Username = form.Username
            };
            user.SetPassword(form.Password);

            Database.Session.Save(user);
            return RedirectToAction("index");
            
        }

        public ActionResult Edit(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            return View(new UsersEdit
            {
                Username = user.Username,
                Email = user.Email
            });
        }

        //create a post equivalent method for when the form is submitted
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UsersEdit form)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            if (Database.Session.Query<User>().Any(u => u.Username == form.Username && u.Id != id))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            user.Username = form.Username;
            user.Email = form.Email;
            Database.Session.Update(user);

            return RedirectToAction("index");
        }

        public ActionResult ResetPassword(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            return View(new UsersResetPassword
            {
                Username = user.Username
            });

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetPassword(int id, UsersResetPassword form)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            form.Username = user.Username;

            if (!ModelState.IsValid)
                return View(form);

            user.SetPassword(form.Password);
            Database.Session.Update(user);

            return RedirectToAction("index");

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            Database.Session.Delete(user);
            return RedirectToAction("index");
        }
    }
}