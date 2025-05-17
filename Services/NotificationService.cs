using Amigos.Models;
using Amigos.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Google;

namespace Amigos.Services
{
    public class NotificationService
    {
        private readonly AmigoDBContext _context;
        private readonly FcmService _fcmService;

        public NotificationService(AmigoDBContext context, FcmService fcmService)
        {
            _context = context;
            _fcmService = fcmService;
        }

        // metodo para enviar notificaciones a los clientes mediante FCM
        public async Task sendNotificationAsync(Amigo amigo)
        {
            var amigosParaNotificar = await _context.Amigos
                    .Where(a => !string.IsNullOrEmpty(a.DeviceToken))
                    .ToListAsync();

            foreach (var amigoDestino in amigosParaNotificar)
            {
                try
                {
                    Console.WriteLine($"Enviando notificación a: {amigoDestino.name} con token: {amigoDestino.DeviceToken}");

                    await _fcmService.SendLocationUpdateAsync(
                        amigoDestino.DeviceToken,
                        amigo.name
                    );

                    Console.WriteLine("Notificación enviada");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar a {amigoDestino.name}: {ex.Message}");
                }
            }
        }
    }

}
