using System.Data.Entity;
using System.Data.Entity.Infrastructure;     
using TelegramBotTest.Models;

namespace TelegramBotTest.DBContext
{
  /// <summary>
  /// Configuration class for cache database
  /// </summary>
  public class MyDbConfig : DbConfiguration
  {
    public MyDbConfig()
    {
      SqlConnectionFactory defaultFactory =
          new SqlConnectionFactory("Server=DESKTOP-C2EU5QI;User=sa;Password=1;");

      this.SetDefaultConnectionFactory(defaultFactory);
    }
  }

  /// <summary>
  /// Data context for code-first method of creation cache database.
  /// </summary>
  [DbConfigurationType(typeof(MyDbConfig))]
  public class DataContext : DbContext
  {    

    public DataContext() : base("Movies")
    { }

    /// <summary>
    /// Table with genres.
    /// </summary>
    public DbSet<Genre> Genres { get; set; }

    /// <summary>
    /// Table with movies.
    /// </summary>
    public DbSet<Movie> Movies { get; set; }

    /// <summary>
    /// Table with actors.
    /// </summary>
    public DbSet<Person> Persons { get; set; }

  }
}
