using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest.Models
{
  public class Movie
  {
    public int Id { get; set; }
    public bool Adult { get; set; }
    public List<Genre> GenresId { get; set; }
    public int MovieId { get; set; }
    public string OriginalLanguage { get; set; }
    public string OriginalTitle { get; set; }
    public string Overview { get; set; }
    public double Popularity { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Title { get; set; }
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
    public List<Person> Persons { get; set; }
  }
}
