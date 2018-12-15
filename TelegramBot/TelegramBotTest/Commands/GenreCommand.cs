using System;
using System.Collections.Generic;
using System.Linq;                   
using Telegram.Bot.Types;

namespace TelegramBotTest.Commands
{
  /// <summary>
  /// Get movie by user defined genre
  /// </summary>
  public class GenreCommand : BaseCommand
  {
    public override string CommandName => "/genre";

    public override void Execute(Chat chatId, string message)
    {
      //waiting for message
      if (message == null)
      {
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Now write some prefered genre or genres, separated by column.");
        return;
      }
      List<string> genres = FilmBot.SplitRowByColumn(new List<string>(), FilmBot.RemoveWhitespace(message), 0);

      List<int> genresId = new List<int>();

      for (int i = 0; i < genres.Count; i++)
      {
        foreach (var y in FilmBot.dataContext.Genres)
        {
          if (genres[i].ToLower() == y.Name.ToLower())
          {
            genresId.Add(y.Id);
          }
        }
      }
      if (genresId.Count != 0)
      {
        List<string> filmsList = new List<string>();
        string query = "with t0 as ";
        string query1 = "select Title from Movies where Id in (select m0 from t0 ";
        string column = ",";
        bool isColumn = false;
        for (int i = 0; i < genresId.Count; i++)
        {
          if (isColumn)
          {
            query += column + $"t{i} as ";
            query1 += $"inner join t{i} on m{i - 1} = m{i} ";
          }
          query += $"(select Movie_Id 'm{i}' from MovieGenres where Genre_Id = {genresId[i]})";
          isColumn = true;
        }
        query += query1 + ")";
        List<string> movies = FilmBot.dataContext.Database.SqlQuery<string>(query).ToList();

        string text1 = $"List of films: {Environment.NewLine}";
        for (int j = 0; j < 10 && j < movies.ToList().Count(); j++)
        {
          text1 += movies.ToList()[j] + Environment.NewLine;
        }
        FilmBot.filmBot.SendTextMessageAsync(chatId, text1);
      }
      else
      {
        string query = "select distinct movie_name from kino_data where ";
        bool isAnd = false;
        for (int i = 0; i < genres.Count; i++)
        {
          if (isAnd)
          {
            query += " and ";
          }
          query += $"movie_genres like '%{genres[i].ToLower()}%'";
          isAnd = true;
        }
        query += " order by RANDOM() limit 10";
        var filmsList = FilmBot.databaseWorker.LoadData(query);
        if (filmsList != null)
        {
          string text = $"List of films: {Environment.NewLine}";
          for (int i = 0; i < filmsList.Count; i++)
          {
            text += filmsList[i] + Environment.NewLine;
          }
          if (filmsList.Count == 0)
          {
            FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Oops, can't find any films :c");
          }
          else
          {
            FilmBot.filmBot.SendTextMessageAsync(chatId, text: text);
          }
        }
        else
        {
          FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Some error happened during the request process :c");
        }
      }
    }
  }
}
