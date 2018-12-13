using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBotTest.DBContext;
using TelegramBotTest.Models;
using TheMovieDb;
using TMDbLib;
using TMDbLib.Objects.Search;

namespace TelegramBotTest
{
  public class FilmBot
  {
    //entity movie database
    DataContext dataContext;
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

      //string conn = "Data Source = C:\\Users\\nkazachenko\\Source\\Repos\\TelegramBot\\TelegramBot\\TelegramBotTest\\Database\\kino_data.db; Version=3;New=False;Compress=True;";
      string conn = "Data Source = D:\\Repository\\TelegramBot\\TelegramBot\\TelegramBotTest\\Database\\kino_data.db; Version=3;New=False;Compress=True;";

      databaseWorker = new DatabaseWorker(conn);

      //     databaseWorker.LoadData("select * from dist_big_kino_data");
      dataContext = new DataContext();

 //     LoadFilms();
      StartBot();
      Console.ReadLine();
    }

    private async void FilmBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
    {
      switch (switcher)
      {
        case "genre":
          if (e.Message.Text != "/genre")
          {
            List<string> genres = SplitRowByColumn(new List<string>(), RemoveWhitespace(e.Message.Text), 0);

            //если написали несколько жанров, ищем айди этих жанров
            List<int> genresId = new List<int>();

            for (int i = 0; i < genres.Count; i++)
            {
              foreach (var y in dataContext.Genres)
              {
                if (genres[i].ToLower() == y.Name.ToLower())
                {
                  genresId.Add(y.Id);
                }
              }
            }

            //добавляем в лист фильмы по каждому жанру
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
            List<string> movies = dataContext.Database.SqlQuery<string>(query).ToList();

            string text1 = $"List of films: {Environment.NewLine}";
            for (int j = 0; j < 10 && j < movies.ToList().Count(); j++)
            {
              text1 += movies.ToList()[j] + Environment.NewLine;
            }
            await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: text1);
            await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "I'll find something to you in a second.");
            switcher = "";
          }
          break;
        case "actor":
          if (e.Message.Text != "/actor")
          {           
            List<string> actors = SplitRowByColumn(new List<string>(), e.Message.Text, 0);

            //если написали несколько жанров, ищем айди этих жанров
            List<int> actorsId = new List<int>();

            for (int i = 0; i < actors.Count; i++)
            {
              string actor = actors[i].ToLower().Trim();
              foreach (var y in dataContext.Persons)
              {
                if (actor == y.Name.ToLower())
                {
                  actorsId.Add(y.Id);
                  break;
                }
              }
            }           
                   
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
            List<string> movies = dataContext.Database.SqlQuery<string>(query).ToList();

            string text1 = $"List of films: {Environment.NewLine}";
            for (int j = 0; j < 10 && j < movies.ToList().Count(); j++)
            {
              text1 += movies.ToList()[j] + Environment.NewLine;
            }
            await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: text1);    
          }
          break;
        case "similar":
          if (e.Message.Text != "/similar")
          {
            var sourceMovie = dataContext.Movies.Where(x => x.Title == e.Message.Text).FirstOrDefault();
            if (sourceMovie != null)
            {
              var sourceMovieId = sourceMovie.Id;
              var similarMovieList = client.GetMovieSimilarAsync(sourceMovieId).Result;
              string text = $"List of similar films: {Environment.NewLine}";
              for (int i = 0; i < 10 && i < similarMovieList.Results.Count; i++)
              {
                text += similarMovieList.Results[i].Title + Environment.NewLine;
              }
              await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: text);
            }
            else
            {
              await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Can't find a movie with such title.");
            }
          }
          break;
        case "country":
          if (e.Message.Text != "/country")
          {
            List<string> countries = SplitRowByColumn(new List<string>(), RemoveWhitespace(e.Message.Text), 0);
            countries = MakeProperStringWithUpperCase(countries);
            string query2 = "select movie_name from big_kino_data where movie_country = '";
            bool flag = false;
            foreach (var country in countries)
            {
              if (flag)
              {
                query2 += " and movie_country = '";
              }
              query2 += country + "'";
              flag = true;
            }
            //рандомим данные
            query2 += " order by RANDOM() limit 5";
            var filmsList1 = databaseWorker.LoadData(query2);

            string text = $"List of films: {Environment.NewLine}";
            foreach (var s in filmsList1)
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
          await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Now write some prefered genre or genres, separated by column.");
          switcher = "genre";
          break;
        case "/actor":
          await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Now write some prefered actor or actors, separated by column.");
          switcher = "actor";
          break;
        case "/setting":
          break;
        case "/country":
          await filmBot.SendTextMessageAsync(chatId: e.Message.Chat, text: "Now write some prefered genres, separated by column.");
          switcher = "country";
          break;
        case "/similar":
          //искать похожий фильм на заданный
          switcher = "similar";
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
        foreach (var t in e.Genres)
        {
          var y = t;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    public void StartBot()
    {
      //WebProxy wp = new WebProxy("ojtbp.tgproxy.me");
      //wp.Credentials = new NetworkCredential("telegram", "telegram");



      //var str = "великОбриТания";

      //List<string> countries = new List<string>();//SplitRowByColumn(new List<string>(), RemoveWhitespace(e.Message.Text), 0);
      //countries.Add("великОбриТания");
      //countries = MakeProperStringWithUpperCase(countries);
      //countries.Add("США");
      //string query = "select movie_actor1 from dist_kino_data where movie_country = '";
      //bool flag = false;
      //foreach (var country in countries)
      //{
      //  if (flag)
      //  {
      //    query += " and movie_country = '";
      //  }
      //  query += country + "'";
      //  flag = true;
      //}
      //рандомим данные
      //query += " order by RANDOM() limit 5";

      //query = "select movie_name, Count(movie_name) 'cou' from big_kino_data GROUP BY movie_name HAVING cou > 1 limit 10";
      //query = "select distinct name_eng, movie_name, movie_i_rate from kino_data where movie_actor1 = 'Брэд Питт'";
      //var filmsList1 = databaseWorker.LoadData(query);

      //string text = $"List of films: {Environment.NewLine}";
      //foreach (var s in filmsList1)
      //{
      //  text += s + Environment.NewLine;
      //}
















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
      if (emptyList.All(x => x != row))
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

    private List<string> MakeProperStringWithUpperCase(List<string> strList)
    {
      List<string> finalList = new List<string>();
      foreach (var str in strList)
      {
        string x;
        x = str.First().ToString().ToUpper() + str.ToLower().Substring(1);
        finalList.Add(x);
      }
      return finalList;
    }

    //write data to entity database from API
    private void LoadFilms()
    {
      //   var genresRes = client.GetMovieGenresAsync().Result;
      var personRes = client.GetPersonListAsync(TMDbLib.Objects.People.PersonListType.Popular).Result;
      //   var moviesRes = client.GetMoviePopularListAsync().Result;

      //foreach (var genre in genresRes)
      //{
      //  Genre genre1 = new Genre();
      //  genre1.GenreId = genre.Id;
      //  genre1.Name = genre.Name;
      //  dataContext.Genres.Add(genre1);
      //}
      //dataContext.SaveChanges();

      //foreach (var y in res.Results)
      //{
      //    searchMovies.Add(y);
      //}
      int u = 0;
      try
      {
        //10 загрузил
        //for (int i = 0; i < moviesRes.TotalPages; i++)
        //{
        //  u = i;
        //  var t = client.GetMoviePopularListAsync(null, i).Result.Results;
        //  foreach (var y in t)
        //  {
        //    //searchMovies.Add(y);
        //    Movie movie = new Movie();
        //    movie.GenresId = new List<Genre>();
        //    movie.Adult = y.Adult;
        //    foreach (var oo in y.GenreIds)
        //    {
        //      movie.GenresId.Add(dataContext.Genres.Where(x => x.GenreId == oo).FirstOrDefault());
        //    }
        //    movie.MovieId = y.Id;
        //    movie.OriginalLanguage = y.OriginalLanguage;
        //    movie.OriginalTitle = y.OriginalTitle;
        //    movie.Overview = y.Overview;
        //    movie.Popularity = y.Popularity;
        //    if (y.ReleaseDate != null)
        //    {
        //      movie.ReleaseDate = (DateTime)y.ReleaseDate;
        //    }
        //    movie.Title = y.Title;
        //    movie.VoteAverage = y.VoteAverage;
        //    movie.VoteCount = y.VoteCount;
        //    dataContext.Movies.Add(movie);
        //  }

        //foreach (var genre in res)
        //{
        //  Genre genre1 = new Genre();
        //  genre1.GenreId = genre.Id;
        //  genre1.Name = genre.Name;
        //  dataContext.Genres.Add(genre1);
        //}

        for (int i = 800; i < personRes.TotalPages; i++)
        {
          u = i;
          var t = client.GetPersonListAsync(TMDbLib.Objects.People.PersonListType.Popular, i).Result.Results;
          foreach (var y in t)
          {
            //searchMovies.Add(y);
            Person person = new Person();
            person.KnownFor = new List<Movie>();
            person.PersonId = y.Id;
            person.Name = y.Name;
            person.Popularity = y.Popularity;
            person.Adult = y.Adult;
            foreach (var oo in y.KnownFor)
            {
              person.KnownFor.Add(dataContext.Movies.Where(x => x.MovieId == oo.Id).FirstOrDefault());
            }
            dataContext.Persons.Add(person);
          }
        }
        dataContext.SaveChanges();
      }
      catch (Exception ex)
      {
        var eee = ex.Message;
      }
    }
  }
}
