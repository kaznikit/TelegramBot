using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest.Models
{
  public class Person
  {
    public int Id { get; set; }
    public bool Adult { get; set; }
    public int PersonId { get; set; }
    public List<Movie> KnownFor { get; set; }
    public string Name { get; set; }
    public double Popularity { get; set; }
  }
}
