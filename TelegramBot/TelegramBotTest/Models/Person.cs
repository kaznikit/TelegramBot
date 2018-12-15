using System;
using System.Collections.Generic; 

namespace TelegramBotTest.Models
{
  /// <summary>
  /// Model for database's table Person.
  /// </summary>
  public class Person
  {              
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Is actor adult.
    /// </summary>
    public bool Adult { get; set; }

    /// <summary>
    /// Person id.
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Reference for the Movie table.
    /// </summary>
    public List<Movie> KnownFor { get; set; }

    /// <summary>
    /// Actor's name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// How popular is an actor.
    /// </summary>
    public double Popularity { get; set; }
  }
}
