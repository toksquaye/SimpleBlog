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
            //picks up settings from webconfig
            config.Configure();

            //add our mappings
            var mapper = new ModelMapper();
            mapper.AddMapping<UserMap>();

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
            var session = HttpContext.Current.Items[SessionKey] as ISession;
            if (session != null)
                session.Close(); //close session to database

            HttpContext.Current.Items.Remove(SessionKey);//remove current session key from item array
        }
    }
}