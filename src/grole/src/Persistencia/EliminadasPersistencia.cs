using FirebirdSql.Data.FirebirdClient;
using grole.src.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grole.src.Persistencia
{
    public class EliminadasPersistencia
    {
        Conexiones _Conexion;
        public EliminadasPersistencia(Conexiones _Conexion)
        {
            this._Conexion = _Conexion;
        }
        
        public List<AuxiliarEliminadaProductoFecha> ObtenerAuxiliarEliminadas(List<Producto> AListaProductos, string AFechaIni, string AFechaFin)
        {
            List<AuxiliarEliminadaProductoFecha> result = new List<AuxiliarEliminadaProductoFecha>();
            DateTime pFechaIni = DateTime.Parse(AFechaIni);
            DateTime pFechaFin = DateTime.Parse(AFechaFin);
            
            DateTime pFechaTmp = DateTime.Parse(AFechaIni);
            bool todos = false;

            if(AListaProductos.Count == 0)
            {
                todos = true;
            }

            while(pFechaTmp <= pFechaFin)
            {
                if(todos == true)
                {
                    AListaProductos = listaProductosFechaEliminados(pFechaTmp.ToShortDateString());
                }

                for(int i = 0; i < AListaProductos.Count; i++)
                {
                    AuxiliarEliminadaProductoFecha tuplaAuxiliar = ObtenerAuxiliarEliminadasProductoFecha(pFechaTmp.ToShortDateString(), AListaProductos[i].Clave, AListaProductos[i].Descripcion);

                   if(tuplaAuxiliar != null)
                        result.Add(tuplaAuxiliar);
                }
                pFechaTmp.AddDays(1);
            }
            
            return result;
        }

        public List<Producto> listaProductosFechaEliminados(string AFecha)
        {
            List<Producto> pResult = new List<Producto>();
            Producto pProducto = null;

            string pSentencia = "SELECT DISTINCT(PRODUCTO) AS CLAVE, (SELECT DESCRIPCION FROM DRASPROD WHERE CLAVE = PRODUCTO) FROM DRASELIM WHERE FECHA = @FECHA AND CODIGOALTA IS NOT NULL";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;

            try
            {
                con.Open();
                FbDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    pProducto = new Producto();
                    pProducto.Clave = reader["DESCRIPCION"] != DBNull.Value ? (string)reader["DESCRIPCION"] : "";
                    pResult.Add(pProducto);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return pResult;
        }

        public AuxiliarEliminadaProductoFecha ObtenerAuxiliarEliminadasProductoFecha(string AFecha, string AProducto, string ADescripcion)
        {
            AuxiliarEliminadaProductoFecha pResult = null;

            EliminadaFecha eliminadas = ObtenerEliminadasFecha(AFecha, AProducto);
            EliminadaFecha reetiquetadas = ObtenerReetiquetadasFecha(AFecha, AProducto);

            if(eliminadas.Cajas > 0 || reetiquetadas.Cajas > 0)
            {
                AuxiliarEliminadaProductoFecha aux = new AuxiliarEliminadaProductoFecha();
                DateTime dt = new DateTime(int.Parse(AFecha.Split('/')[2]), int.Parse(AFecha.Split('/')[1]), int.Parse(AFecha.Split('/')[0]));

                aux.Fecha               = dt;
                aux.Producto            = AProducto;
                aux.Descripcion         = ADescripcion;
                aux.Reetiquetadas_Cajas = reetiquetadas.Cajas;
                aux.Reetiquetadas_Kilos = reetiquetadas.Kilos;
                aux.Eliminadas_Cajas    = eliminadas.Cajas;
                aux.Eliminadas_Kilos    = eliminadas.Kilos;
                pResult = aux;
            }
            return pResult;
        }

        public EliminadaFecha ObtenerEliminadasFecha(string AFecha, string AProducto)
        {
            EliminadaFecha pResult = null;

            string pSentencia = "SELECT COUNT(*) AS CAJAS, COALESCE(SUM(PESO), 0)  AS KILOS FROM DRASELIM WHERE FECHA = @FECHA AND PRODUCTO = @PRODUCTO  AND (CODIGOALTA IS NOT NULL OR CODIGOALTA <> '')";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;
            com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;

            try
            {
                con.Open();
                FbDataReader reader = com.ExecuteReader();
                if (reader.Read())
                {
                    pResult = new EliminadaFecha();
                    pResult.Cajas = reader["CAJAS"] != DBNull.Value ? (int)reader["CAJAS"] : 0;
                    pResult.Kilos = reader["KILOS"] != DBNull.Value ? (decimal)reader["KILOS"] : 0;
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return pResult;
        }

        public EliminadaFecha ObtenerReetiquetadasFecha(string AFecha, string AProducto)
        {
            EliminadaFecha pResult = null;

            string pSentencia = "SELECT COUNT(*) AS CAJAS, COALESCE(SUM(PESO), 0) AS KILOS FROM DRASCORT WHERE FECHA = @FECHA AND PRODUCTO = @PRODUCTO AND LOTE IN(127, 527)";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;
            com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;

            try
            {
                con.Open();
                FbDataReader reader = com.ExecuteReader();
                if (reader.Read())
                {
                    pResult = new EliminadaFecha();
                    pResult.Cajas = reader["CAJAS"] != DBNull.Value ? (int)reader["CAJAS"] : 0;
                    pResult.Kilos = reader["KILOS"] != DBNull.Value ? (decimal)reader["KILOS"] : 0;
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return pResult;
        }
    }
}
