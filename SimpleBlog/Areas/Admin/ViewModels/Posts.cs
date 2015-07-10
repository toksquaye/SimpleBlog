﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleBlog.Infracstructure;
using SimpleBlog.Models;

namespace SimpleBlog.Areas.Admin.ViewModels
{
    public class PostsIndex
    {
        public PagedData<Post> Posts { get; set; }
    }
}