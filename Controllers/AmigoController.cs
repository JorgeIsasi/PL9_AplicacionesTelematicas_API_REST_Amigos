using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Amigos.DataAccessLayer;
using Amigos.Models;
using System.Runtime.InteropServices;
using Amigos.Services;

namespace Amigos.Controllers
{
    public class AmigoController : Controller
    {
        private readonly AmigoDBContext _context;

        private readonly FcmService _fcmService;

        private readonly NotificationService _notificationService;

        public AmigoController(AmigoDBContext context, FcmService fcmService, NotificationService notificationService)
        {
            _context = context;
            _fcmService = fcmService;
            _notificationService = notificationService;
        }

        // GET: Amigo
        public async Task<IActionResult> Index(string filterType, string? searchString, double? longi, double? lati, double? distance)
        {
            if (_context.Amigos == null)
            {
                return Problem("Entity set 'AmigoDBContext.Amigos'  is null.");
            }

            var amigos = from m in _context.Amigos
                         select m;

            if (filterType == "name" && !String.IsNullOrEmpty(searchString))
            {
                amigos = amigos.Where(s => s.name!.ToUpper().Contains(searchString.ToUpper()));
            }
            else if (filterType == "coordinates" && longi.HasValue && lati.HasValue && distance.HasValue)
            {
                
                var amigosLista = await amigos.ToListAsync(); // Si no no dejaba daba fallo al convertir

                var amigosFiltrados = amigosLista.Where(a => GetDistance(Convert.ToDouble(a.lati), Convert.ToDouble(a.longi), lati.Value, longi.Value) <= distance.Value);

                return View(amigosFiltrados.ToList());
            }
                return View(await amigos.ToListAsync());           
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the Earth in km

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;
            return distance;
        }

        private double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        // GET: Amigo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Amigos == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (amigo == null)
            {
                return NotFound();
            }

            return View(amigo);
        }

        // GET: Amigo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amigo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,name,longi,lati")] Amigo amigo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(amigo);
                await _context.SaveChangesAsync();
                await _notificationService.sendNotificationAsync(amigo);
                return RedirectToAction(nameof(Index));
            }
            return View(amigo);
        }

        // GET: Amigo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Amigos == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos.FindAsync(id);
            if (amigo == null)
            {
                return NotFound();
            }
            return View(amigo);
        }

        // POST: Amigo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,name,longi,lati,DeviceToken")] Amigo amigo)
        {
            if (id != amigo.ID)
            {
                return NotFound();
            }

            var amigoExistente = await _context.Amigos.FindAsync(id);
            if (amigoExistente == null)
                return NotFound();


            if (ModelState.IsValid)
            {
                try
                {
                    // Solo actualiza los campos que se han enviado y tienen valor
                    if (!string.IsNullOrWhiteSpace(amigo.name))
                        amigoExistente.name = amigo.name;
                    if (!string.IsNullOrWhiteSpace(amigo.lati))
                        amigoExistente.lati = amigo.lati;
                    if (!string.IsNullOrWhiteSpace(amigo.longi))
                        amigoExistente.longi = amigo.longi;
                    if (!string.IsNullOrWhiteSpace(amigo.DeviceToken))
                        amigoExistente.DeviceToken = amigo.DeviceToken;

                    _context.Update(amigoExistente);
                    await _context.SaveChangesAsync();

                    await _notificationService.sendNotificationAsync(amigoExistente);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmigoExists(id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(amigo);
        }

        // GET: Amigo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Amigos == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (amigo == null)
            {
                return NotFound();
            }

            return View(amigo);
        }

        // POST: Amigo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Amigos == null)
            {
                return Problem("Entity set 'AmigoDBContext.Amigos'  is null.");
            }
            var amigo = await _context.Amigos.FindAsync(id);
            if (amigo != null)
            {
                _context.Amigos.Remove(amigo);
            }
            
            await _context.SaveChangesAsync();
            await _notificationService.sendNotificationAsync(amigo);
            return RedirectToAction(nameof(Index));
        }

        private bool AmigoExists(int id)
        {
          return (_context.Amigos?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
