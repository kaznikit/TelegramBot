using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TheMovieDb;
using TMDbLib;
using TMDbLib.Objects.Search;

namespace TelegramBotTest
{
    public class FilmBot
    {
        
        List<SearchMovie> searchMovies = new List<SearchMovie>();
        List<string> commands;
        TelegramBotClient filmBot;
    DatabaseWorker databaseWorker;
        string switcher = "";
//        TmdbApi tmdbApi = new TmdbApi("4d613fb15c4282217c3c380f8c9aadb2");
        TMDbLib.Client.TMDbClient client = new TMDbLib.Client.TMDbClient("4d613fb15c4282217c3c380f8c9aadb2");
    public FilmBot()
    {
      //var y = client();
      commands = new List<string> { "/genre", "/actor", "/setting", "/country", "/help" };

      //    string conn = "Data Source = C:\\Users\\nkazachenko\\movie db\\kino_data.db; Version=3;New=False;Compress=True;";
      string conn = "Data Source = D:\\Repository\\TelegramBot\\TelegramBot\\TelegramBotTest\\Database\\kino_data.db; Version=3;New=False;Compress=True;";

      databaseWorker = new DatabaseWorker(conn);

 //     databaseWorker.LoadData("select * from dist_big_kino_data");
      //           LoadFilms();
      StartBot();
      Console.ReadLine();
    }

    private async void FilmBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
    {
      switch (switcher)
      {
        case "genre":
          if (e.Message.Text.Contains(",") || e.Message.Text.Contains(", "))
          {
            List<string> genres = SplitRowByColumn(new List<string>(), RemoveWhitespace(e.Message.Text), 0);


            //  tmdbApi.GetGenresCompleted += TmdbApi_GetGenresCompleted;
            //  tmdbApi.GGetGenresAsync();
            var genres1 = client.GetMovieGenresAsync().Result;

            //если написали несколько жанров, ищем айди этих жанров
            List<int> genresId = new List<int>();

            for (int i = 0; i < genres.Count; i++)
            {
              foreach (var y in genres1)
              {
                if (genres[i].ToLower() == y.Name.ToLower())
                {
                  genresId.Add(y.Id);
                }
              }
            }

            //добавляем в лист фильмы по каждому жанру
            List<string> filmsList = new List<string>();


            int k = 0;
            foreach (var id in genresId)
            {
              var res = await client.GetGenreMoviesAsync(id, new Random(5).Next(1100), true);
              var y = await client.GetPersonListAsync(TMDbLib.Objects.People.PersonListType.Popular);
              var oo = client.SearchKeywordAsync("mo").Result;

              //var ff = client.searc("mo").Result;
              foreach (var film in res.Results)
              {
                filmsList.Add(film.Title);
              }
            }

            var uniqueFilms = filmsList.GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

            string text1 = $"List of films: {Environment.NewLine}";
            foreach (var s in uniqueFilms)
            {
              text1 += s + Environment.NewLine;
            }
            await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: text1);
          }
          await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "I'll find something to you in a second.");
          switcher = "";
          break;
        case "actor":
          if (e.Message.Text.Contains(",") || e.Message.Text.Contains(", "))
          {
            List<string> actors = SplitRowByColumn(new List<string>(), RemoveWhitespace(e.Message.Text), 0);
            var actorsImdb = client.GetPersonListAsync(TMDbLib.Objects.People.PersonListType.Popular).Result;
            List<int> actorsId = new List<int>();
            var t = client.GetMovieCreditsAsync(34).Result;
            for (int i = 0; i < actors.Count; i++)
            {
              foreach (var y in actorsImdb.Results)
              {
                if (actors[i].ToLower() == y.Name.ToLower())
                {
                  actorsId.Add(y.Id);
                }
              }
            }
          }
          break;
        case "country":
          if (e.Message.Text != "/country")
          {
            List<string> countries = SplitRowByColumn(new List<string>(), RemoveWhitespace(e.Message.Text), 0);
            string query = "select movie_name from big_kino_data where movie_country = '";
            bool flag = false;
            foreach (var country in countries)
            {
              if (flag)
              {
                query += " and movie_country = '";
              }
              query += country + "'";
              flag = true;
            }

            var filmsList1 = databaseWorker.LoadData(query);

            var uniqueFilms1 = filmsList1.GroupBy(x => x)
              .Where(group => group.Count() > 1)
              .Select(group => group.Key);

            string text = $"List of films: {Environment.NewLine}";
            foreach (var s in uniqueFilms1)
            {
              text += s + Environment.NewLine;
            }
            await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: text);
            await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "I'll find something to you in a second.");
          }
          switcher = "";
          break;
      }
      switch (e.Message.Text)
      {
        case "/genre":
          await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Now write some prefered genres, separated by column.");
          switcher = "genre";
          break;
        case "/actor":
          await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Now write some prefered genres, separated by column.");
          switcher = "actor";
          break;
        case "/setting":
          break;
        case "/country":
          switcher = "country";
          break;
        case "/help":
          var text = "Hello! This bot helps to find amazing film!\nYou can check commands below.\n";
          foreach (var s in commands)
          {
            text += s + "\n";
          }
          await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: text);
          break;
      }
    }

        private void TmdbApi_GetGenresCompleted(object sender, TmdbGenresCompletedEventArgs e)
        {
            try
            {
                foreach(var t in e.Genres)
                {
                    var y = t;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void StartBot()
        {
            //WebProxy wp = new WebProxy("ojtbp.tgproxy.me");
            //wp.Credentials = new NetworkCredential("telegram", "telegram");
            filmBot = new TelegramBotClient("404062526:AAHM35vggvveFFyvcM_JZhXslZAAGTFGuFg");//, wp);
            var me = filmBot.GetMeAsync().Result;
           // me.Wait();
            filmBot.OnMessage += FilmBot_OnMessage;
            filmBot.StartReceiving();
        }

        private List<string> SplitRowByColumn(List<string> emptyList, string row, int startIndex)
        {
            while (row.Contains(","))
            {
                emptyList.Add(row.Substring(startIndex, row.IndexOf(",")));
                row = row.Substring(row.IndexOf(",") + 1, row.Length - 1 - row.IndexOf(","));
            }
            if(emptyList.All(x =>x != row))
            {
                emptyList.Add(row);
            }
            return emptyList;
        }

        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public IEnumerable<T> Distinct<T>(IEnumerable<T> source)
        {
            List<T> uniques = new List<T>();
            foreach (T item in source)
            {
                if (!uniques.Contains(item)) uniques.Add(item);
            }
            return uniques;
        }

        private void LoadFilms()
        {
            var res = client.GetMoviePopularListAsync().Result;
            int totalPages = res.TotalPages;
            foreach (var y in res.Results)
            {
                searchMovies.Add(y);
            }
            for (int i = 1; i < totalPages; i++)
            {
                var t = client.GetMoviePopularListAsync(null, i).Result.Results;
                foreach(var y in t)
                {
                    searchMovies.Add(y);
                }
            }     
        }
    }
}
