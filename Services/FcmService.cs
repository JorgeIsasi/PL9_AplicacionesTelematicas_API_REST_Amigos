using FirebaseAdmin.Messaging;
using System.Globalization;


namespace Amigos.Services
{
    public class FcmService
    {
        public async Task<string> SendLocationUpdateAsync(string deviceToken, string name)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = "Ubicación actualizada",
                    Body = $"{name} se ha movido.",

                },
                Data = new Dictionary<string, string>()
                {
                    { "action", "location_updated" },
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("FCM enviado: " + response);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR FCM: " + ex.Message);
                throw ex;
            }
            
        }
    }
}
