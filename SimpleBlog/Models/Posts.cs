using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace SimpleBlog.Models
{
    public class Post
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }

        public virtual string Title { get; set; }
        public virtual string Slug { get; set; }
        public virtual string Content { get; set; }

        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime? UpdatedAt { get; set; } //? denotes that is a nullable type
        public virtual DateTime? DeletedAt { get; set; } //soft delete. admin sees deleted posts, but not user

        public virtual IList<Tag> Tags { get; set; }

        public Post()
        {
        
            Tags = new List<Tag>();
        }
        public virtual bool IsDeleted { get { return DeletedAt != null; } }
    }

    public class PostMap : ClassMapping<Post>
    {
        public PostMap()
        {
            Table("posts");
            Id(x => x.Id, x => x.Generator(Generators.Identity));
            ManyToOne(x => x.User, x =>
                {
                    x.Column("user_id");
                    x.NotNullable(true);
                });

            Property(x => x.Title, x => x.NotNullable(true));
            Property(x => x.Slug, x => x.NotNullable(true));
            Property(x => x.Content, x => x.NotNullable(true));

            Property(x => x.CreatedAt, x =>
                {
                    x.Column("created_at");
                    x.NotNullable(true);
                });

            Property(x => x.UpdatedAt, x => x.Column("updated_at"));
            Property(x => x.DeletedAt, x => x.Column("deleted_at"));

            Bag(x => x.Tags, x =>
            {
                x.Key(y => y.Column("post_id")); //key - entity we're mapping from, column - entity we're mapping to
                x.Table("post_tags"); //pivot table
            }, x => x.ManyToMany(y => y.Column("tag_id")));

        }

    }
}