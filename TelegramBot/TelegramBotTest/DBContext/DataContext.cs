using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotTest.Models;

namespace TelegramBotTest.DBContext
{
  // Класс конфигурации
  public class MyDbConfig : DbConfiguration
  {
    public MyDbConfig()
    {
      SqlConnectionFactory defaultFactory =
          new SqlConnectionFactory("Server=DESKTOP-C2EU5QI;User=sa;Password=1;");

      this.SetDefaultConnectionFactory(defaultFactory);
    }
  }

  [DbConfigurationType(typeof(MyDbConfig))]
  public class DataContext : DbContext
  {    

    public DataContext() : base("Movies")
    { }

    // Отражение таблиц базы данных на свойства с типом DbSet
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Person> Persons { get; set; }

  }
}
