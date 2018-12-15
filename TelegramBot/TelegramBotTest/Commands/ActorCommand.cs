using System;
using System.Collections.Generic;
using System.Linq;             
using Telegram.Bot.Types;

namespace TelegramBotTest.Commands
{
  /// <summary>
  /// Get movie by user defined actor
  /// </summary>
  public class ActorCommand : BaseCommand
  {
    public override string CommandName => "/actor";

    public override void Execute(Chat chatId, string message)
    {
      //waiting for message
      if(message == null)
      {
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Now write some prefered actor or actors, separated by column. Russian is case sensitive.");
        return;
      }

      List<string> actors = FilmBot.SplitRowByColumn(new List<string>(), message, 0);

      List<int> actorsId = new List<int>();
      if (FilmBot.dataContext != null)
      {
        for (int i = 0; i < actors.Count; i++)
        {
          string actor = actors[i].ToLower().Trim();
          foreach (var y in FilmBot.dataContext.Persons)
          {
            if (actor == y.Name.ToLower())
            {
              actorsId.Add(y.Id);
              break;
            }
          }
        }
      }
      //if found actors in database
      if (actorsId.Count != 0)
      {
        List<string> filmsList = new List<string>();
        string query = "with t0 as ";
        string query1 = "select Title from Movies where Id in (select m0 from t0 ";
        string column = ",";
        bool isColumn = false;
        for (int i = 0; i < actorsId.Count; i++)
        {
          if (isColumn)
          {
            query += column + $"t{i} as ";
            query1 += $"inner join t{i} on m{i - 1} = m{i} ";
          }
          query += $"(select Movie_Id 'm{i}' from PersonMovies where Person_Id = {actorsId[i]})";
          isColumn = true;
        }
        query += query1 + ")";
        List<string> movies = FilmBot.dataContext.Database.SqlQuery<string>(query).ToList();

        string text1 = $"List of films: {Environment.NewLine}";
        for (int j = 0; j < 10 && j < movies.ToList().Count(); j++)
        {
          text1 += movies.ToList()[j] + Environment.NewLine;
        }
        FilmBot.filmBot.SendTextMessageAsync(chatId, text: text1);
      }
      //check in other database
      else
      {
        string query = "select distinct movie_name from kino_data where ";
        bool isAnd = false;
        for (int i = 0; i < 2 && i < actors.Count; i++)
        {
          if (isAnd)
          {
            query += " and ";
          }
          isAnd = true;
          query += $"movie_actor{i + 1} = '{actors[i].Trim()}' ";
        }
        query += " order by RANDOM() limit 10";
        var filmsList = FilmBot.databaseWorker.LoadData(query);
        if (filmsList != null)
        {
          string text = $"List of films: {Environment.NewLine}";
          foreach (var s in filmsList)
          {
            text += s + Environment.NewLine;
          }
          if (filmsList.Count == 0)
          {
            FilmBot.filmBot.SendTextMessageAsync(chatId, text: "Oops, can't find such actor :c");
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
