using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace SimpleBlog.Models
{
    public class Tag
    {
        public virtual int Id { get; set; }
        public virtual string Slug { get; set; }
        public virtual string Name { get; set; }

        public virtual IList<Post> Posts { get; set; } //Posts and Tags have a manytomany relationship
 
        public Tag()
        {
            Posts = new List<Post>();
        }
    }

    public class TagMap : ClassMapping<Tag>
    {
        public TagMap()
        {
            Table("tags");

            Id(x => x.Id, x => x.Generator(Generators.Identity));

            Property(x => x.Name, x => x.NotNullable(true));
            Property(x => x.Slug, x => x.NotNullable(true));

            Bag(x => x.Posts, x =>
                {
                    x.Key(y => y.Column("tag_id")); //key - entity we're mapping from, column - entity we're mapping to
                    x.Table("post_tags"); //pivot table
                }, x => x.ManyToMany(y => y.Column("post_id")));
        }
    }
}