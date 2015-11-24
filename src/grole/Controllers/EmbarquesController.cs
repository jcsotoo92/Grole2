using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using grole.src.Logica;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace grole.Controllers
{
    public class EmbarquesController : Controller
    {
        private EmbarquesLogica _EmbarquesLogica;

        public EmbarquesController(EmbarquesLogica _EmbarquesLogica)
        {
            this._EmbarquesLogica = _EmbarquesLogica;
        }
        // GET: /<controller>/
        public IActionResult DetalleSalida()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerTablaDetalleSalida(int IdSalida)
        {
            return Json(_EmbarquesLogica.ObtenerDetalleSalida(IdSalida));
        }
    }
}
