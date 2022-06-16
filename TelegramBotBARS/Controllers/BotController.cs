using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramBotBARS.Services;

namespace TelegramBotBARS.Controllers
{
    public class BotController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> GetUpdate([FromBody]Update update, [FromServices]UpdateHandleService updateHandleService)
        {
            await updateHandleService.HandleUpdateAsync(update);

            return Ok();
        }
    }
}
