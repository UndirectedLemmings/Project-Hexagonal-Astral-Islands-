using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Project_Hexagonal_Astral_Islands;


    public class NHibernateHelper
    {
        public static ISession OpenSession()
        {
            ISessionFactory sessionFactory = Fluently.Configure()
     //Настройки БД. Строка подключения к БД MS Sql Server 7
     .Database(MsSqlConfiguration.MsSql7.ConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Project_Hexagonal_Astral_Islands;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=true")
            .ShowSql()
            )
            //Маппинг. Используя AddFromAssemblyOf NHibernate будет пытаться маппить КАЖДЫЙ класс в этой сборке (assembly). Можно выбрать любой класс. 
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Map>())
            //SchemeUpdate позволяет создавать/обновлять в БД таблицы и поля (2 поле ==true) 
            .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
            .BuildSessionFactory();
            return sessionFactory.OpenSession();
        }
    }

