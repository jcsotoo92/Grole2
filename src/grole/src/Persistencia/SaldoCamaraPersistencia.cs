using FirebirdSql.Data.FirebirdClient;
using grole.src.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grole.src.Persistencia
{
    public class SaldoCamaraPersistencia
    {
        private Conexiones _Conexiones;

        public SaldoCamaraPersistencia(Conexiones _Conexiones)
        {
            this._Conexiones = _Conexiones;
        }

        public List <SaldoCamara> obtener_lista_camaras_catalogo_activas()
        {
            List<SaldoCamara> pResult = new List<SaldoCamara>();
            SaldoCamara pSaldoCamara = null;
            DateTime thisDay = DateTime.Today;
            string pSentencia = "SELECT ID, DESCRIPCION FROM DRASCAM WHERE EMBARQUE = 'Si' AND FECHA_EMBARQUE >='"+thisDay.ToString("dd'.'MM'.'yyyy")+ ", 00:00:00.000'";
            FbConnection con = _Conexiones.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            try
            {
                con.Open();
                FbDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    pSaldoCamara             = new SaldoCamara();
                    pSaldoCamara.Id          = (reader["ID"] != DBNull.Value) ? (int)reader["ID"] : -1;
                    pSaldoCamara.Descripcion = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.Add(pSaldoCamara);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            var cam = 123;
            this.obtener_validaciones_camara(cam);
            return pResult;
        }

        public List<SaldoCamara> obtener_validaciones_camara(int ACamara)
        {
            List<SaldoCamara> pResult = new List<SaldoCamara>();
            SaldoCamara pSaldoCamara = null;
            string pSentencia = "SELECT a.ID, a.ID_CAMARA, a.PRODUCTO, a.FECHA_MIN_PRODUCCION, a.FECHA_MAX_PRODUCCION, b.DESCRIPCION, a.CANTIDAD_MAXIM, a.KILOS_MAXIM FROM DRASVALIDAPTOSCAMARA a JOIN DRASPROD b ON b.CLAVE = a.PRODUCTO WHERE a.ID_CAMARA ="+ACamara;
            FbConnection con = _Conexiones.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            try
            {
                con.Open();
                FbDataReader reader = com.ExecuteReader();
                Console.WriteLine("Validaciones Camara");
                while (reader.Read())
                {
                    pSaldoCamara                      = new SaldoCamara();
                    pSaldoCamara.Id                   = (reader["ID"] != DBNull.Value) ? (int)reader["ID"] : -1;
                    pSaldoCamara.Id_Camara            = (reader["ID_CAMARA"] != DBNull.Value) ? (int)reader["ID_CAMARA"] : -1;
                    pSaldoCamara.Producto             = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pSaldoCamara.Fecha_Min_Produccion = reader["FECHA_MIN_PRODUCCION"] != DBNull.Value ? Utilerias.dateTimeToString((DateTime)reader["FECHA_MIN_PRODUCCION"]) : "";
                    pSaldoCamara.Fecha_Max_Produccion = reader["FECHA_MAX_PRODUCCION"] != DBNull.Value ? Utilerias.dateTimeToString((DateTime)reader["FECHA_MAX_PRODUCCION"]) : "";
                    pSaldoCamara.Descripcion          = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pSaldoCamara.Cantidad_Maxim       = (reader["CANTIDAD_MAXIM"] != DBNull.Value) ? (int)reader["CANTIDAD_MAXIM"] : -1;
                    pSaldoCamara.Kilos_Maxim          = reader["KILOS_MAXIM"] != DBNull.Value ? (decimal)reader["KILOS_MAXIM"] : -1;
                    pResult.Add(pSaldoCamara);

                    Console.WriteLine("ID: "+pSaldoCamara.Id + " ID_CAMARA: "+pSaldoCamara.Id_Camara+" PRODUCTO: "+pSaldoCamara.Producto+" FECHA_MIN_PRODUCCION: "+pSaldoCamara.Fecha_Min_Produccion+" FECHA_MAX_PRODUCCION: "+pSaldoCamara.Fecha_Max_Produccion+" DESCRIPCION: "+pSaldoCamara.Descripcion+" CANTIDAD_MAXIM: "+pSaldoCamara.Cantidad_Maxim + " KILOS_MAXIM: "+pSaldoCamara.Kilos_Maxim);
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

        public SaldoCamara obtener_saldo_camara_producto(int AProducto, int ACamara)
        {
            SaldoCamara pResult = new SaldoCamara();
            string pSentencia = "SELECT COUNT(*) AS CAJAS, COALESCE(SUM(PESO), 0) AS KILOS, MIN(FECHA) AS FECHA_MIN, MAX(FECHA) AS FECHA_MAX FROM DRASCORT WHERE PRODUCTO = @PRODUCTO AND CAMARA = @CAMARA";
            FbConnection con = _Conexiones.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;
            com.Parameters.Add("@CAMARA", FbDbType.VarChar).Value = ACamara;
            try
            {
                con.Open();
                FbDataReader reader = com.ExecuteReader();
                Console.WriteLine("Saldo Camara");
                if (reader.Read())
                {
                    pResult           = new SaldoCamara();
                    pResult.Cajas     = (reader["CAJAS"] != DBNull.Value) ? (int)reader["CAJAS"] : -1;
                    pResult.Kilos     = (reader["KILOS"] != DBNull.Value) ? (decimal)reader["KILOS"] : -1;
                    pResult.Fecha_Min = reader["FECHA_MIN"] != DBNull.Value ? Utilerias.dateTimeToString((DateTime)reader["FECHA_MIN"]) : "";
                    pResult.Fecha_Max = reader["FECHA_MAX"] != DBNull.Value ? Utilerias.dateTimeToString((DateTime)reader["FECHA_MAX"]) : "";
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
