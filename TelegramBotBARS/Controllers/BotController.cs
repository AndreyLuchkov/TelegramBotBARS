using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace TelegramBotBARS.Controllers
{
    public class BotController : Controller
    {
        [HttpPost]
        public IActionResult GetUpdate([FromBody]Update update)
        {
            return Ok();
        }
    }
}
