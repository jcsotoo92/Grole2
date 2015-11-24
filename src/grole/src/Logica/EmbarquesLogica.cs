using grole.src.Entidades;
using grole.src.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grole.src.Logica
{
    public class EmbarquesLogica
    {
        private EmbarquesPersistencia _EmbarquesPersistencia;

        public EmbarquesLogica(EmbarquesPersistencia _EmbarquesPersistencia)
        {
            this._EmbarquesPersistencia = _EmbarquesPersistencia;
        }
        public List<DetalleSalida> ObtenerDetalleSalida(int AIdSalida)
        {
            return _EmbarquesPersistencia.ObtenerDetalleSalida(AIdSalida).OrderBy(x => x.Fecha).ToList();

        }
    }
}
