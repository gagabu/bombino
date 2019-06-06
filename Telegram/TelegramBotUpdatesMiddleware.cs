using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BombinoBomberBot
{
    internal class TelegramBotUpdatesMiddleware
    {
        private readonly RequestDelegate _next;

        public TelegramBotUpdatesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
    }
}