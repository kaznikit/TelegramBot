using System.Collections.Generic;     

namespace TelegramBotTest.Models
{
  /// <summary>
  /// Model for database's table Genre.
  /// </summary>
  public class Genre
  {    
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Genre Id.
    /// </summary>
    public int GenreId { get; set; }

    /// <summary>
    /// Name of genre.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Reference to movies table.
    /// </summary>
    public List<Movie> Movies { get; set; }
  }
}
