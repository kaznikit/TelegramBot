using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;               
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotTest.Commands;
using TelegramBotTest.DBContext;  
using TMDbLib.Client;         

namespace TelegramBotTest
{
  public class FilmBot
  {
    private List<BaseCommand> commands;

    /// <summary>
    /// Static access to the cache database.
    /// </summary>
    public static DataContext dataContext;

    /// <summary>
    /// Static telegram bot.
    /// </summary>
    public static TelegramBotClient filmBot;

    /// <summary>
    /// Static SQLite database access.
    /// </summary>
    public static DatabaseWorker databaseWorker;

    /// <summary>
    /// Static API access.
    /// </summary>
    public static TMDbClient client = new TMDbClient(ConfigurationManager.AppSettings["tmdbApiKey"]);

    /// <summary>
    /// Dictionary with user's chat id and command to perform.
    /// </summary>
    private Dictionary<long, BaseCommand> chatDictionary;

    /// <summary>
    /// Keyboard for comfortable interaction with bot.
    /// </summary>
    public static ReplyKeyboardMarkup replyKeyboardMarkup;

    /// <summary>
    /// Main constructor of bot.
    /// </summary>
    public FilmBot()
    {
      chatDictionary = new Dictionary<long, BaseCommand>();
      commands = new List<BaseCommand> { new ActorCommand(), new CountryCommand(), new GenreCommand(), new HelpCommand(), new SimilarCommand(), new StartCommand() };
      string conn = ConfigurationManager.AppSettings["connectionString"];
      databaseWorker = new DatabaseWorker(conn);
      dataContext = new DataContext();
      StartBot();
    }

    /// <summary>
    /// Event on getting message from user.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Message event arguments from bot.</param>
    private async void FilmBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
    {              
      if (!chatDictionary.ContainsKey(e.Message.Chat.Id))
      {
        if (e.Message.Text.StartsWith("/"))
        {
          var command = commands.FirstOrDefault(_x => _x.CommandName == e.Message.Text);
          if (command != null)
          {
            command.Execute(e.Message.Chat, null);
            if (command.CommandName != "/help")
            {
              chatDictionary.Add(e.Message.Chat.Id, command);
            }
          }
          else
          {
            filmBot.SendTextMessageAsync(e.Message.Chat, "Wrong command.", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, replyKeyboardMarkup);
          }
        }
        else
        {
          filmBot.SendTextMessageAsync(e.Message.Chat, "There is no such command.", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, replyKeyboardMarkup);
        }
      }
      else
      {
        if (!e.Message.Text.StartsWith("/"))
        {
          if (!string.IsNullOrEmpty(e.Message.Text))
          {
            var command = chatDictionary.FirstOrDefault(_x => _x.Key == e.Message.Chat.Id).Value;
            command.Execute(e.Message.Chat, e.Message.Text);
            chatDictionary.Remove(e.Message.Chat.Id);
          }
        }
        else
        {
          chatDictionary.Remove(e.Message.Chat.Id);
          filmBot.SendTextMessageAsync(e.Message.Chat, "Please choose a command.", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, replyKeyboardMarkup);
        }
      }   
    }

    /// <summary>
    /// Bot initialization. Set API key, creating bot's keyboard.
    /// </summary>
    public void StartBot()
    {                
      filmBot = new TelegramBotClient(ConfigurationManager.AppSettings["telegramApiKey"]);
      var me = filmBot.GetMeAsync().Result;

      //creating keyboard
      replyKeyboardMarkup = new ReplyKeyboardMarkup();
      replyKeyboardMarkup.Keyboard = new KeyboardButton[][]
      {
        new KeyboardButton[]
        {
            new KeyboardButton("/actor"),
            new KeyboardButton("/country")
        },
          new KeyboardButton[]
        {
            new KeyboardButton("/genre"),
            new KeyboardButton("/similar")
        },
          new KeyboardButton[]
        {
            new KeyboardButton("/help"),
        }
      };

      filmBot.OnMessage += FilmBot_OnMessage;
      filmBot.StartReceiving();
    }
    
    /// <summary>
    /// Split string to list by columns.
    /// </summary>
    /// <param name="emptyList">Buffer for parts of string.</param>
    /// <param name="row">Source string with columns.</param>
    /// <param name="startIndex">Index from which string should be splitted.</param>
    /// <returns>List with parts of source string without columns.</returns>
    public static List<string> SplitRowByColumn(List<string> emptyList, string row, int startIndex)
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

    /// <summary>
    /// Remove whitespaces in string.
    /// </summary>
    /// <param name="input">Source string with whitespaces.</param>
    /// <returns>String without whitespaces.</returns>
    public static string RemoveWhitespace(string input)
    {
      return new string(input.ToCharArray()
          .Where(c => !Char.IsWhiteSpace(c))
          .ToArray());
    }

    /// <summary>
    /// Make upper case string.
    /// </summary>
    /// <param name="strList">Source list with strings which should be upper cased.</param>
    /// <returns>List with upper cased strings.</returns>
    public static List<string> MakeProperStringWithUpperCase(List<string> strList)
    {
      List<string> finalList = new List<string>();
      foreach (var str in strList)
      {
        if (str.Length > 3)
        {
          string x;
          x = str.First().ToString().ToUpper() + str.ToLower().Substring(1);
          finalList.Add(x);
        }
        else
        {
          finalList.Add(str.ToUpper());
        }
      }
      return finalList;
    }
  }
}
