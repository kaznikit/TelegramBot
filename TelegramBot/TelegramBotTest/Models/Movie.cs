using System;
using System.Collections.Generic;   

namespace TelegramBotTest.Models
{
  /// <summary>
  /// Model for database's table Movie.
  /// </summary>
  public class Movie
  {
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Is movie adult.
    /// </summary>
    public bool Adult { get; set; }
    
    /// <summary>
    /// Reference for Genres table.
    /// </summary>
    public List<Genre> GenresId { get; set; }

    /// <summary>
    /// Movie Id.
    /// </summary>
    public int MovieId { get; set; }

    /// <summary>
    /// Original language of movie.
    /// </summary>
    public string OriginalLanguage { get; set; }

    /// <summary>
    /// Original title of movie.
    /// </summary>
    public string OriginalTitle { get; set; }

    /// <summary>
    /// Overview for the movie.
    /// </summary>
    public string Overview { get; set; }

    /// <summary>
    /// How popular is the movie.
    /// </summary>
    public double Popularity { get; set; }

    /// <summary>
    /// When movie was released.
    /// </summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// Movie title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Average vote.
    /// </summary>
    public double VoteAverage { get; set; }

    /// <summary>
    /// Votes count.
    /// </summary>
    public int VoteCount { get; set; }

    /// <summary>
    /// Reference for the Person table.
    /// </summary>
    public List<Person> Persons { get; set; }
  }
}
