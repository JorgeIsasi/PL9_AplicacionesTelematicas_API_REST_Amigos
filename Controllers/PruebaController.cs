using Microsoft.AspNetCore.Mvc;

namespace Amigos.Controllers
{
    public class PruebaController : Controller
    {
        private readonly IInc _inc;

        // Inyeccion de dependencias
        public PruebaController(IInc inc)
        {
            _inc = inc;
        }

        // Probar Inyeccion de dependencias -> Probar en: http://localhost:PUERTO/prueba/contador
        public string Contador()
        {
            return $"Valor del contador: {_inc.Inc()}";
        }

        // Funcion Index se dejo como venía originalmente
        public IActionResult Index()
        {
            return View();
        }

        // Metodo Adios que pasas Viewbag a la vista Adios -> Probar en: http://localhost:PUERTO/prueba/Adios?valor=Hasta%20luego&veces=10
        public IActionResult Adios(string valor, int veces)
        {
            ViewBag.valor = valor;
            ViewBag.veces = veces;

            return View();
        }

        // Probar en: http://localhost:PUERTO/prueba/hola/HelloWorld/100 o http://localhost:PUERTO/prueba/hola?valor=Rubén&veces=10
        public string Hola(string valor, int veces) 
        {

            // Inspeccionamos el HttpContext y obtenemos el método de la petición
            string metodo = this.HttpContext.Request.Method;

            //Devolvemos el metodo de peticion junto al valor repetido
            string devolver = $"Metodo de petición: {metodo}\n";

            for (int i=0 ; i<veces ; i++)
            {
                devolver += valor + " ";
            }

            return devolver;
        }
    }
}
