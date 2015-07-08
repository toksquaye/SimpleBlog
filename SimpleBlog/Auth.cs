﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleBlog.Models;
using NHibernate.Linq;

namespace SimpleBlog
{
    public class Auth
    {
        private const string UserKey = "SimpleBlog.Auth.UserKey";

        //used to extract a fully hydrated entity that represents the currently logged
        // in user
        public static User User
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                    return null;

                var user = HttpContext.Current.Items[UserKey] as User;
                if (user == null)
                {
                    user = Database.Session.Query<User>().FirstOrDefault(u => u.Username == HttpContext.Current.User.Identity.Name);
                    if (user == null)
                        return null;
                    HttpContext.Current.Items[UserKey] = user;
                }

                return user;
            }

        }

    }
}