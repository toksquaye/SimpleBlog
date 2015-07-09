using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleBlog
{
    public class Database
    {
        //unique identifier for current session
        private const string SessionKey = "SimpleBlog.Database.SessionKey";

        private static ISessionFactory _sessionFactory;
        
        //this allows session object to be exposed to code for controller to use
        public static ISession Session
        {
            get { return (ISession)HttpContext.Current.Items[SessionKey]; }
        }
        public static void Configure()
        {
            var config = new Configuration();

            //configure the connection string
            //picks up settings from webconfig as default behavior
            config.Configure();

            //add our mappings - for NHibernate purposes
            var mapper = new ModelMapper();
            mapper.AddMapping<UserMap>();
            mapper.AddMapping<RoleMap>();
            mapper.AddMapping<TagMap>();
            mapper.AddMapping<PostMap>();

            config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            //create session factory
            _sessionFactory = config.BuildSessionFactory();
        }

        public static void OpenSession()
        {
            //open session to database
            HttpContext.Current.Items[SessionKey] = _sessionFactory.OpenSession();
        }
        //Note - every request has a session
        //A session is an individual connection to the database
        public static void CloseSession()
        {
            //ISession used by NHibernate to represent a session
            var session = HttpContext.Current.Items[SessionKey] as ISession;
            if (session != null)
                session.Close(); //close session to database

            HttpContext.Current.Items.Remove(SessionKey);//remove current session key from item array
        }
    }
}