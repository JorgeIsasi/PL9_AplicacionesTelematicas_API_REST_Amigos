using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Amigos.DataAccessLayer;
using Amigos.Models;
using Amigos.Services;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Amigos.Controllers
{
    [Route("api/amigo")]
    [ApiController]
    public class AmigoAPIController : ControllerBase
    {
        private readonly AmigoDBContext _context;

        private readonly NotificationService _notificationService;

        public AmigoAPIController(AmigoDBContext context, FcmService fcmService, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: api/amigo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Amigo>>> GetAmigos()
        {
          if (_context.Amigos == null)
          {
              return NotFound();
          }
            return await _context.Amigos.ToListAsync();
        }

        // GET: api/amigo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Amigo>> GetAmigo(int id)
        {
          if (_context.Amigos == null)
          {
              return NotFound();
          }
            var amigo = await _context.Amigos.FindAsync(id);

            if (amigo == null)
            {
                return NotFound();
            }

            return amigo;
        }

        // PUT: api/amigo/id/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("id/{id}")]
        public async Task<IActionResult> PutAmigo(int id, Amigo amigo)
        {
            var amigoExistente = await _context.Amigos.FirstOrDefaultAsync(a => a.ID == id);
            if (amigoExistente == null) return NotFound();

            // Guardamos valores antiguos
            var oldLati = amigoExistente.lati;
            var oldLongi = amigoExistente.longi;

            // Solo actualizamos si hay valores nuevos
            if (!string.IsNullOrWhiteSpace(amigo.lati))
                amigoExistente.lati = amigo.lati;
            if (!string.IsNullOrWhiteSpace(amigo.longi))
                amigoExistente.longi = amigo.longi;
            if (!string.IsNullOrWhiteSpace(amigo.DeviceToken))
                amigoExistente.DeviceToken = amigo.DeviceToken;

            await _context.SaveChangesAsync();

            // Comprobamos si ha cambiado la ubicación
            if (oldLati != amigoExistente.lati || oldLongi != amigoExistente.longi)
            {
            await _notificationService.sendNotificationAsync(amigoExistente);
            }
            return NoContent();
        }

        // PUT: api/amigo/name/Juan
        [HttpPut("name/{name}")]
        public async Task<IActionResult> PutAmigoByName(string name, Amigo amigo)
        {
            var amigoExistente = await _context.Amigos.FirstOrDefaultAsync(a => a.name == name);
            if (amigoExistente == null) return NotFound();

            // Guardamos valores antiguos
            var oldLati = amigoExistente.lati;
            var oldLongi = amigoExistente.longi;

            // Solo actualizamos si hay valores nuevos
            if (!string.IsNullOrWhiteSpace(amigo.lati))
                amigoExistente.lati = amigo.lati;
            if (!string.IsNullOrWhiteSpace(amigo.longi))
                amigoExistente.longi = amigo.longi;
            if (!string.IsNullOrWhiteSpace(amigo.DeviceToken))
                amigoExistente.DeviceToken = amigo.DeviceToken;

            await _context.SaveChangesAsync();

            // Comprobamos si ha cambiado la ubicación
            if (oldLati != amigoExistente.lati || oldLongi != amigoExistente.longi)
            {
                await _notificationService.sendNotificationAsync(amigoExistente);
            }
            return NoContent();
        }


        // PUT: api/amigo/register-token
        [HttpPut("register-token")]
        public async Task<IActionResult> RegisterToken([FromBody] TokenRegistration model)
        {
            var amigo = await _context.Amigos.FirstOrDefaultAsync(a => a.name == model.Nombre);
            if (amigo == null) return NotFound();

            amigo.DeviceToken = model.Token;
            await _context.SaveChangesAsync();

            return Ok();
        }
        public class TokenRegistration
        {
            public string Nombre { get; set; }
            public string Token { get; set; }
        }


        // POST: api/amigo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Amigo>> PostAmigo(Amigo amigo)
        {
          if (_context.Amigos == null)
          {
              return Problem("Entity set 'AmigoDBContext.Amigos'  is null.");
          }
            _context.Amigos.Add(amigo);
            await _context.SaveChangesAsync();

            await _notificationService.sendNotificationAsync(amigo);

            return CreatedAtAction("GetAmigo", new { id = amigo.ID }, amigo);
        }


        // DELETE: api/AmigoAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAmigo(int id)
        {
            if (_context.Amigos == null)
            {
                return NotFound();
            }
            var amigo = await _context.Amigos.FindAsync(id);
            if (amigo == null)
            {
                return NotFound();
            }

            _context.Amigos.Remove(amigo);
            await _context.SaveChangesAsync();

            await _notificationService.sendNotificationAsync(amigo);

            return NoContent();
        }

    }
}
