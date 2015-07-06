using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace SimpleBlog.Models
{
    //user entity
    public class User
    {
        private const int WorkFactor = 13;
        public static void FakeHash()
        {
            BCrypt.Net.BCrypt.HashPassword("", WorkFactor);
        }
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string PasswordHash { get; set; }

        public virtual void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, WorkFactor); ;
        }

        public virtual bool CheckPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }

    //maps entity to user table in database
    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Table("users"); //tells what table to map to
            Id(x => x.Id, x => x.Generator(Generators.Identity)); //primary key

            //database field name not specifiedd, so class property name "username" is
            //defaulted to. since sql isn't case sensitive, "Username" works
            Property(x => x.Username, x => x.NotNullable(true));
            Property(x => x.Email, x => x.NotNullable(true));

            //database field column name is specified here
            Property(x => x.PasswordHash, x =>
                {
                    x.Column("password_hash");
                    x.NotNullable(true);
                });
        }
    }
}