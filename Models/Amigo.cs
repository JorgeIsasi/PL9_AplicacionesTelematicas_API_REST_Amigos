using System.ComponentModel.DataAnnotations;

namespace Amigos.Models
{
    public class Amigo
    {
        public int ID { get; set; }

        [Display(Name = "Nombre")]
        public string? name { get; set; }

        [Display(Name = "Longitud")]
        public string? longi { get; set; }

        [Display(Name = "Latitud")]
        public string? lati { get; set; }

        public string? DeviceToken { get; set; } // <- Aquí el token FCM
    }
}
