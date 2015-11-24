using System.Collections.Generic;
using grole.src.Persistencia;
using grole.src.Entidades;
using System.Linq;

namespace grole.src.Logica
{
	public class CajasLogica{
		CajasPersistencia _CajasPersistencia;
		public CajasLogica (CajasPersistencia _CajasPersistencia){
			this._CajasPersistencia=_CajasPersistencia;
		}
		
		public InventarioProducto ObtenerInventarioProducto(string AFechaIni, string AFechaFin, string AProducto)
		{
			return _CajasPersistencia.ObtenerInventarioProducto(AFechaIni, AFechaFin, AProducto);
		}
		
		public List<InventarioProductoDesglosado> ObtenerInventarioProductoDesglosado(string AFechaIni, string AFechaFin, string AProducto)
		{
			return _CajasPersistencia.ObtenerInventarioProductoDesglosado(AFechaIni, AFechaFin, AProducto);
		}

        public List<ProduccionNoInventariada> ObtenerProduccionNoInventariadasPorProducto(string AProducto, string AFechaIni, string AFechaFin)
        {
            return _CajasPersistencia.ObtenerProduccionNoInventariadasPorProducto(AProducto, AFechaIni, AFechaFin);
        }

        public List<DesgloseProductoCamara> ObtenerDesgloseProductoPorCamara(string AProducto)
        {
            return _CajasPersistencia.ObtenerDesgloseProductoPorCamara(AProducto);
        }

        public List<ResumenProductoPorCamara> ObtenerResumenPtosCamara(int[] ACamara)
        {
            List<ResumenProductoPorCamara> pLista = new List<ResumenProductoPorCamara>();

            foreach(var item in ACamara)
            {
                pLista.AddRange(_CajasPersistencia.ObtenerResumenPtosCamara(item));
            }
            pLista = pLista.OrderBy(x => x.Producto).ToList();
            return pLista;
        }

        public List<Corte> ObtenerDetalleProductosCamara(int ACamara, string AProducto)
        {
            return _CajasPersistencia.ObtenerDetalleProductosCamara(ACamara, AProducto);
        }

        public List<CajaPendienteRecepcionEmbarque> ObtenerCajasPendientesRecepcionEmbarques(string AFecha)
        {
            return _CajasPersistencia.ObtenerCajasPendientesRecepcionEmbarques(AFecha);
        }

        public List<Corte> ObtenerDetalleCajasPendientesRecepcionEmbarques(string AFecha, string AProducto)
        {
            return _CajasPersistencia.ObtenerDetalleCajasPendientesRecepcionEmbarques(AFecha, AProducto);
        }
    }
}