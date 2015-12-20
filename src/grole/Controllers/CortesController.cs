using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using grole.src.Logica;
using grole.src.Entidades;
using Microsoft.AspNet.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace grole.Controllers
{
    public class CortesController : Controller
    {
        private CortesLogica _CortesLogica;
        private CajasLogica _CajasLogica;

        public CortesController(CortesLogica _CortesLogica, CajasLogica _CajasLogica)
        {
            this._CortesLogica = _CortesLogica;
            this._CajasLogica  = _CajasLogica;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConsultaPedimentos()
        {
            return View();
        }

        public JsonResult ObtenerTablaPedimento(int Pedimento)
        {
            return Json(_CortesLogica.ObtenerSaldosPedimento(Pedimento));
        }

        public ActionResult CorteFecha()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerTablaCorteFecha(string Fecha)
        {
            return Json(_CortesLogica.ObtenerCorteFecha(Fecha));
        }

        [HttpGet]
        public JsonResult ObtenerPendientesCorte(string Fecha)
        {
            return Json(_CortesLogica.ObtenerPendientesCorte(Fecha));
        }

        public ActionResult PendientesFecha(string Fecha)
        {
            return View(_CortesLogica.ObtenerDetallePendientesCorte(Fecha));
        }

        [HttpGet]
        public ActionResult ResumenFecha(string Fecha, int Camara, string Embarcado)
        {
            return View(_CortesLogica.ObtenerDetalleCorteFecha(Fecha, Camara, Embarcado));
        }

        [HttpGet]
        public ActionResult ConsultaSalidas()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerTablaSalidas(string FechaIni, string FechaFin)
        {
            return Json(_CortesLogica.ObtenerSalidaDelDia(FechaIni, FechaFin));
        }

        [HttpGet]
        public ActionResult ResumenCortesArea()
        {
            int id_usuario = (int)HttpContext.Session.GetInt32("IdUsuario");
            Console.WriteLine(id_usuario);
            var areas_usuario = _CortesLogica.ObtenerAreasUsuario(id_usuario);
            return View(areas_usuario);
        }

        public ActionResult ProduccionPorBascula()
        {
            return View(_CortesLogica.ObtenerBasculasActivas());
        }

        public JsonResult ObtenerTablaProduccionPorBascula(int Bascula, string Fecha)
        {
            return Json(_CortesLogica.ObtenerProduccionPorBascula(Fecha,Bascula));
        }
        
        public ActionResult DesgloseProductoPorCamara(string Producto)
        {
            return View(_CajasLogica.ObtenerDesgloseProductoPorCamara(Producto));
        }

        public ActionResult RecepcionCajasEmbarques()
        {
            return View();
        }

        public JsonResult ObtenerTablaPendientesRecepcionEmbarques(string Fecha)
        {
            return Json(_CajasLogica.ObtenerCajasPendientesRecepcionEmbarques(Fecha));
        }

        public ActionResult DetalleRecepcionCajasEmbarque(string Fecha, string Producto)
        {
            return View(_CajasLogica.ObtenerDetalleCajasPendientesRecepcionEmbarques(Fecha, Producto));
        }
        public ActionResult RegresarCajas()
        {
            return View();
        }

        [HttpGet]
        public JsonResult TablaInfoCaja(string ACodigo)
        {
            return Json(_CajasLogica.ObtenerCaja(ACodigo));
        }

        public JsonResult RegresarCaja(string ACodigo)
        {
            int id_usuario = (int)HttpContext.Session.GetInt32("IdUsuario");
            string IP = HttpContext.Connection.RemoteIpAddress.ToString();
            bool rest = _CajasLogica.RegresaCaja(IP, id_usuario, ACodigo);
            if(rest)
                return Json("La caja se regreso exitosamente");
            else
                return Json("La caja no tiene salida");
        }

        
    }
}
