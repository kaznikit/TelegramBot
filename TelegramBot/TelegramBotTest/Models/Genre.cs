using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest.Models
{
  public class Genre
  {
    
    public int Id { get; set; }
    public int GenreId { get; set; }
    public string Name { get; set; }
    public List<Movie> Movies { get; set; }
  }
}
