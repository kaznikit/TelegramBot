using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;       

namespace TelegramBotTest
{
  /// <summary>
  /// Class for interaction with SQLite database.
  /// </summary>
  public class DatabaseWorker
  {
    SQLiteConnection connection;
    /// <summary>
    /// Main class constructor.
    /// </summary>
    /// <param name="connectionStr">Connection string for the database.</param>
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

    /// <summary>
    /// Get movies from the database.
    /// </summary>
    /// <param name="query">SQL query to get movies from the database.</param>
    /// <returns>List of movie names.</returns>
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
          movies.Add(s["movie_name"].ToString());    
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
