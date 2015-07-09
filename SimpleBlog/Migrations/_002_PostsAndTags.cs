﻿using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SimpleBlog.Migrations
{
    [Migration(2)]
    public class _002_PostsAndTags : Migration
    {
        public override void Down()
        {
            Delete.Table("posts_tags"); // delete this first because it hold relationships to other tables
            Delete.Table("posts");
            Delete.Table("tags");
        }

        public override void Up()
        {
            Create.Table("posts")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().ForeignKey("users", "id") //foreign key - connects to users table via id column
                .WithColumn("title").AsString(128)
                .WithColumn("slug").AsString(128)
                .WithColumn("created_at").AsDateTime()
                .WithColumn("updated_at").AsDateTime().Nullable()
                .WithColumn("deleted_at").AsDateTime().Nullable();

            Create.Table("tags")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("slug").AsString(128)
                .WithColumn("name").AsString(128);

            Create.Table("post_tags")
                .WithColumn("tag_id").AsInt32().ForeignKey("tags", "id").OnDelete(Rule.Cascade)
                .WithColumn("post_id").AsInt32().ForeignKey("posts", "id").OnDelete(Rule.Cascade);
        }
    }
}