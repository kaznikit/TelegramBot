using Telegram.Bot.Types;

namespace TelegramBotTest.Commands
{
  /// <summary>
  /// Base class for bot functions
  /// </summary>
  public abstract class BaseCommand
  {
    /// <summary>
    /// Name of command
    /// </summary>
    public abstract string CommandName { get; }

    /// <summary>
    /// Method which will be performed
    /// </summary>
    /// <param name="chatId">User's chat id</param>
    /// <param name="message">User's message to bot</param>
    public abstract void Execute(Chat chatId, string message);
  }
}
