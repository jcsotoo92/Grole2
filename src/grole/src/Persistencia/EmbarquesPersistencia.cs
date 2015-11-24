using FirebirdSql.Data.FirebirdClient;
using grole.src.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grole.src.Persistencia
{
    public class EmbarquesPersistencia
    {
        private Conexiones _Conexiones;

        public EmbarquesPersistencia(Conexiones _Conexiones)
        {
            this._Conexiones = _Conexiones;
        }

        public List<DetalleSalida> ObtenerDetalleSalida(int AIdSalida)
        {
            List<DetalleSalida> pResult = new List<DetalleSalida>();
            DetalleSalida pDetalleSalida = null;

            string pSentencia = "SELECT T0.FECHA, T0.FOLIO, T0.PRODUCTO, T0.CODIGOBARRAS, T0.TARIMA, T0.PESO, T1.DESCRIPCION FROM DRASCORT T0 JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO WHERE T0.ID_SALIDA = @IDSALIDA";
            FbConnection con = _Conexiones.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@IDSALIDA", FbDbType.Integer).Value = AIdSalida;
            try
            {
                con.Open();
                FbDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    pDetalleSalida              = new DetalleSalida();
                    pDetalleSalida.Fecha        = reader["FECHA"] != DBNull.Value ? (DateTime?)reader["FECHA"] : null;
                    pDetalleSalida.Folio        = (reader["FOLIO"] != DBNull.Value) ? (int)reader["FOLIO"] : -1;
                    pDetalleSalida.Producto     = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pDetalleSalida.CodigoBarras = (reader["CODIGOBARRAS"] != DBNull.Value) ? (string)reader["CODIGOBARRAS"] : "";
                    pDetalleSalida.Tarima       = (reader["TARIMA"] != DBNull.Value) ? (int)reader["TARIMA"] : -1;
                    pDetalleSalida.Peso         = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;
                    pDetalleSalida.Descripcion  = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.Add(pDetalleSalida);
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
