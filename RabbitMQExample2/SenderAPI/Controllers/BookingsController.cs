using Microsoft.AspNetCore.Mvc;
using SenderAPI.Models;
using SenderAPI.Services;

namespace SenderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingsController(ILogger<BookingsController> logger, IMessageProducer messageProducer) : ControllerBase
    {
        private readonly ILogger<BookingsController> _logger = logger;
        private readonly IMessageProducer _messageProducer = messageProducer;

        [HttpPost]
        public IActionResult CreateBooking(Booking booking)
        {   
            if(!ModelState.IsValid) return BadRequest();

            _messageProducer.SendMessage(booking);

            return Ok();
        }
    }
}
