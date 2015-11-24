﻿using grole.src.Entidades;
using grole.src.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grole.src.Logica
{
    public class EmpaquesLogica
    {
        private EmpaquesPersistencia _EmpaquesPersistencia;
        public EmpaquesLogica(EmpaquesPersistencia _EmpaquesPersistencia)
        {
            this._EmpaquesPersistencia = _EmpaquesPersistencia;
        }
        public List<Empaque> ObtenerEmpaques(string AOrderBy)
        {
            return _EmpaquesPersistencia.ObtenerEmpaques(AOrderBy);
        }

        public List<TipoEmpaque> ObtenerListaTiposEmpaques()
        {
            return _EmpaquesPersistencia.ObtenerListaTiposEmpaques();
        }

        public Empaque Insertar(Empaque AEmpaque)
        {
            if(!_EmpaquesPersistencia.ExisteEmpaque(AEmpaque))
                return _EmpaquesPersistencia.InsertarEmpaque(AEmpaque);
            return
                null;
        }

        public Empaque Modificar(Empaque AEmpaque)
        {
            if (!_EmpaquesPersistencia.ExisteEmpaque(AEmpaque))
                return _EmpaquesPersistencia.ModificarEmpaque(AEmpaque);
            return
                null;
        }

        public bool Eliminar(int AClave, out string AMensajeError)
        {
            return _EmpaquesPersistencia.EliminarEmpaque(AClave, out AMensajeError);
        }

        public List<EmpaqueProducto> ObtenerEmpaquesProducto(string AClave)
        {
            return _EmpaquesPersistencia.ObtenerEmpaquesProducto(AClave);
        }

        public bool InsertarEmpaquesProducto(string AIdProducto, int[] Achk, decimal[] Ainp)
        {
            return _EmpaquesPersistencia.InsertarEmpaquesProducto(AIdProducto, Achk, Ainp);
        }
        
        public TipoEmpaque AgregarTipoEmpaque(TipoEmpaque ATipoEmpaque){
            if(!_EmpaquesPersistencia.ExisteTipoEmpaque(ATipoEmpaque.Nombre)){
                return _EmpaquesPersistencia.AgregarTipoEmpaque(ATipoEmpaque);
            }else{
                return null;
            }
        }
        
        public TipoEmpaque ModificarTipoEmpaque(TipoEmpaque ATipoEmpaque){
            if(!_EmpaquesPersistencia.ExisteTipoEmpaque(ATipoEmpaque.Nombre)){
                return _EmpaquesPersistencia.ModificarTipoEmpaque(ATipoEmpaque);
            }else{
                return null;
            }
        }
        
        public bool EliminarTipoEmpaque(int AClave, out string AMensajeError)
        {
            return _EmpaquesPersistencia.EliminarTipoEmpaque(AClave, out AMensajeError);
        }

        public TablaProyeccionProduccion ObtenerTablaProyeccionProduccion(string AFechaIni, string AFechaFin, string ATipo)
        {
            TablaProyeccionProduccion pTabla       = new TablaProyeccionProduccion();
            List<ProyeccionProduccion> pLista      = null, pListaDetalle = null;
            
            if (ATipo.Equals("0"))
            {
                pLista        = _EmpaquesPersistencia.ObtenerProyeccionProduccion(AFechaIni, AFechaFin);
                pListaDetalle = _EmpaquesPersistencia.ObtenerProyeccionProduccionDetalle(AFechaIni, AFechaFin);
            }else if (ATipo.Equals("Nacional"))
            {
                pLista        = _EmpaquesPersistencia.ObtenerProyeccionProduccionNacional(AFechaIni, AFechaFin);
                pListaDetalle = _EmpaquesPersistencia.ObtenerProyeccionProduccionDetalleNacional(AFechaIni, AFechaFin);
            }else if (ATipo.Equals("Exportacion"))
            {
                pLista        = _EmpaquesPersistencia.ObtenerProyeccionProduccionExportacion(AFechaIni, AFechaFin);
                pListaDetalle = _EmpaquesPersistencia.ObtenerProyeccionProduccionDetalleExportacion(AFechaIni, AFechaFin);
            }
            List<FechasProyeccionEmpaques> pFechas = new List<FechasProyeccionEmpaques>();
            DateTime? pFechaTemp = null;
            
            foreach(var item in pListaDetalle)
            {
                if(pFechaTemp != item.Fecha_Embarque)
                {
                    FechasProyeccionEmpaques fecha = new FechasProyeccionEmpaques();
                    fecha.Fecha = item.Fecha_Embarque;
                    fecha.Count = 0;
                    fecha.PIds = null;

                    pFechas.Add(fecha);

                    pFechaTemp = item.Fecha_Embarque;
                }
            }

            foreach(var item in pFechas)
            {
                List<PIds> pIds = ObtenerConteoFecha(item.Fecha, pListaDetalle);
                item.Count = pIds.Count;
                item.PIds = pIds;
            }

            pTabla.Fechas = pFechas;
            pTabla.Lista  = pLista;
            pTabla.ListaDetalle = pListaDetalle;

            return pTabla;
        }

        public List<PIds> ObtenerConteoFecha(DateTime? AFecha, List<ProyeccionProduccion> ALista)
        {
            int pContador = 0;
            int pIdTmp    = 0;

            List<PIds> pResult = new List<PIds>();

            foreach(var item in ALista)
            {
                if(item.Fecha_Embarque == AFecha)
                {
                    if(pIdTmp != item.Id)
                    {
                        pContador++;

                        PIds pR        = new PIds();
                        pR.Id          = item.Id;
                        pR.Descripcion = item.Descripcion2;
                        pResult.Add(pR);

                        pIdTmp = item.Id;
                    }
                }
            }

            return pResult;
        }
        
    }
}
