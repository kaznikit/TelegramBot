using System;
using System.Linq;           
using Telegram.Bot.Types;

namespace TelegramBotTest.Commands
{
  /// <summary>
  /// Get movie similar to defined by user
  /// </summary>
  public class SimilarCommand : BaseCommand
  {
    public override string CommandName => "/similar";

    public override void Execute(Chat chatId, string message)
    {
      //waiting for message
      if (message == null)
      {
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Now write some film to find fimilar.");
        return;
      }
      var sourceMovie = FilmBot.dataContext.Movies.Where(x => x.Title == message).FirstOrDefault();
      if (sourceMovie != null)
      {
        var sourceMovieId = sourceMovie.Id;
        var similarMovieList = FilmBot.client.GetMovieSimilarAsync(sourceMovieId).Result;
        string text = $"List of similar films: {Environment.NewLine}";
        for (int i = 0; i < 10 && i < similarMovieList.Results.Count; i++)
        {
          text += similarMovieList.Results[i].Title + Environment.NewLine;
        }
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: text);
      }
      else
      {
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Can't find a movie with such title.");
      }
    }
  }
}
