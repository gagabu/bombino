using Telegram.Bot.Types;

namespace BombinoBomberBot.Helpers
{
    public static class UserHelpers
    {
        public static string Mention(this User user)
        {
            return $"[{user.Username ?? (user.FirstName + " " + user.LastName)}](tg://user?id={user.Id})";
        }

        public static string Mention(this Model.User user)
        {
            return $"[{user.Username ?? (user.FirstName + " " + user.LastName)}](tg://user?id={user.TelegramUserId})";
        }
    }
}
