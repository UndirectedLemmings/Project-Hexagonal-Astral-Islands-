using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Project_Hexagonal_Astral_Islands;
using System;


    public class NHibernateHelper
    {
        public static ISession OpenSession(string priority="NORMAL")
        {
        Console.WriteLine("OpenSession() started");
            ISessionFactory sessionFactory = Fluently.Configure()
     //Настройки БД. Строка подключения к БД MS Sql Server 7
     .Diagnostics(x => x.Enable(true))
     .Database(MsSqlConfiguration.MsSql7.ConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Project_Hexagonal_Astral_Islands;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=true")
            .ShowSql()
            )
            
            //Маппинг. Используя AddFromAssemblyOf NHibernate будет пытаться маппить КАЖДЫЙ класс в этой сборке (assembly). Можно выбрать любой класс. 
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Map>())
  
            //SchemeUpdate позволяет создавать/обновлять в БД таблицы и поля (2 поле ==true) 
            .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
            .BuildSessionFactory();
        Console.WriteLine("OpenSession() finished");
        ISession session = sessionFactory.OpenSession();
        
            return session.SessionWithOptions().Interceptor(new DeadLockPriorityInterceptor(priority)).OpenSession();
    }
    }

public class DeadLockPriorityInterceptor : EmptyInterceptor
{
    private ISession session;
    private string priority;

    public DeadLockPriorityInterceptor(string Priority)
    {
        priority = Priority;
    }

    public override void AfterTransactionBegin(ITransaction tx)
    {
        using (var command = session.Connection.CreateCommand())
        {
            session.Transaction.Enlist(command);
            string sql = string.Format("SET DEADLOCK_PRIORITY {0}", priority);
            // Create a SQL Command
            //System.Data.IDbCommand command = session.Connection.CreateCommand();
            // Set the query you're going to run
            command.CommandText = sql;
            // Run the query
            command.ExecuteNonQuery();

            //command.CommandText = "SET NOCOUNT ON";
            //command.ExecuteNonQuery();
        }
    }

    public override void SetSession(ISession sessionLocal)
    {
        session = sessionLocal;
    }
}

