using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using grole.src.Logica;

namespace grole.Controllers
{
    public class SaldoCamaraController: Controller
    {
        private SaldoCamaraLogica _SaldoCamaraLogica;
        public SaldoCamaraController(SaldoCamaraLogica _SaldoCamaraLogica)
        {
            this._SaldoCamaraLogica = _SaldoCamaraLogica;
        }
        public ActionResult Index()
        {
            return View(_SaldoCamaraLogica.obtener_lista_camaras_catalogo_activas());
        }

        public JsonResult obtener_validaciones_camaras(int ACamara)
        {
            return Json(_SaldoCamaraLogica.obtener_validaciones_camara(ACamara));
        }

        public JsonResult obtener_saldo_camara_producto(int AProducto, int ACamara)
        {
            return Json(_SaldoCamaraLogica.obtener_saldo_camara_producto(AProducto, ACamara));
        }


    }
}
