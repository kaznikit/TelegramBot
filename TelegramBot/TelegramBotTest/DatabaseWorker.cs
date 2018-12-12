using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
  public class DatabaseWorker
  {
    SQLiteConnection connection;
    public DatabaseWorker(string connectionStr)
    {
      try
      {
        connection = new SQLiteConnection(connectionStr);
        connection.Open();
      }
      catch(Exception ex)
      {
        Console.Write(ex.Message);
      }
    }

    public List<string> LoadData(string query)
    {
      try
      {
        SQLiteCommand sqlCommand = connection.CreateCommand();
        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, connection);
        DataSet dataSet = new DataSet();
        dataAdapter.Fill(dataSet);
        List<string> movies = new List<string>();
        foreach (DataRow s in dataSet.Tables[0].Rows)
        {
          //          string name = s["name_eng"].ToString();
          // string name2 = s["movie_name"].ToString();
          //          string gg = s["list_name"].ToString();
          //          string yy = s["movie_time"].ToString();
          movies.Add(s["movie_name"].ToString());
          movies.Add(s["name_eng"].ToString());
          movies.Add(s["movie_i_rate"].ToString());

          //        movies.Add(s["movie_country"].ToString());
          //          string tt = s["movie_actor1"].ToString();
          //         movies.Add(s["movie_name"].ToString());
          //movies.Add(s["cou"].ToString());

        }
        return movies;
      }
      catch(Exception ex)
      {
        Console.Write(ex.Message);
        return null;
      }
    }
  }
}
