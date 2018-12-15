using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TelegramBotTest.Commands
{
  /// <summary>
  /// Get movie by user defined country name
  /// </summary>
  public class CountryCommand : BaseCommand
  {
    public override string CommandName => "/country";

    public override void Execute(Chat chatId, string message)
    {
      //waiting for message
      if (message == null)
      {
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Now write some countries, separated by column.");
        return;
      }
      List<string> countries = FilmBot.SplitRowByColumn(new List<string>(), FilmBot.RemoveWhitespace(message), 0);
      countries = FilmBot.MakeProperStringWithUpperCase(countries);
      string query2 = "select distinct movie_name from big_kino_data where movie_country = '";
      bool flag = false;
      foreach (var country in countries)
      {
        if (flag)
        {
          query2 += " and movie_country = '";
        }
        query2 += country.Trim() + "'";
        flag = true;
      }
      //add random
      query2 += " order by RANDOM() limit 10";
      var filmsList1 = FilmBot.databaseWorker.LoadData(query2);
      if (filmsList1 != null)
      {
        string text = $"List of films: {Environment.NewLine}";
        foreach (var s in filmsList1)
        {
          text += s + Environment.NewLine;
        }
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: text);
      }
      else
      {
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Some error happened during the request process :c");
      }
    }
  }
}
