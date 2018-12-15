using Telegram.Bot.Types;

namespace TelegramBotTest.Commands
{
  /// <summary>
  /// Get helpful information about bot's functions
  /// </summary>
  public class HelpCommand : BaseCommand
  {
    public override string CommandName => "/help";

    public override void Execute(Chat chatId, string message)
    {
      var text = "Hello! This bot helps to find you amazing film!\nPlease check commands below.\n" +
                 "/actor - Find a set of movies with a certain actor/ actors.You can type on English and Russian.\n" +
                 "/country - Find a set of movies from particular country. Supports only Russian.\n" +
                 "/genre - Find a set of movies with some genre/genres. Supports English and Russian.\n" +
                 "/similar - Find a set of movies which are similar as defined movie. Supports only English.\n";  
      FilmBot.filmBot.SendTextMessageAsync(chatId, text, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, FilmBot.replyKeyboardMarkup);
    }
  }
}
