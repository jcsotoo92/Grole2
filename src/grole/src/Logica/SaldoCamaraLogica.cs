using grole.src.Entidades;
using grole.src.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grole.src.Logica
{
    public class SaldoCamaraLogica
    {
        private SaldoCamaraPersistencia _SaldoCamaraPersistencia;
        public SaldoCamaraLogica(SaldoCamaraPersistencia _SaldoCamaraPersistencia)
        {
            this._SaldoCamaraPersistencia = _SaldoCamaraPersistencia;
        }

        public List<SaldoCamara> obtener_lista_camaras_catalogo_activas()
        {
            return _SaldoCamaraPersistencia.obtener_lista_camaras_catalogo_activas();
        }

        public List<SaldoCamara> obtener_validaciones_camara(int ACamara)
        {
            return _SaldoCamaraPersistencia.obtener_validaciones_camara(ACamara);
        }

        public SaldoCamara obtener_saldo_camara_producto(int AProducto, int ACamara)
        {
            return _SaldoCamaraPersistencia.obtener_saldo_camara_producto(AProducto, ACamara);
        }
    }
}
