using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using System;
using grole.src.Entidades;

namespace grole.src.Persistencia
{
	
	public class CajasPersistencia
	{
		private Conexiones _Conexion;
		
		public CajasPersistencia(Conexiones _Conexion){
			this._Conexion=_Conexion;
		}
		
		public InventarioProducto ObtenerInventarioProducto(string AFechaIni, string AFechaFin, string AProducto){
			
			InventarioProducto pResult = null;
			
			string pSentencia = "SELECT COUNT(*) AS CAJAS, COALESCE(SUM(PESO), 0) AS KILOS FROM DRASCORT "+
					   "WHERE CAST((SUBSTRING(CODIGOBARRAS FROM 11 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 13 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 7 FOR 4)) AS TIMESTAMP) >= @FECHAINI AND "+
					   "CAST((SUBSTRING(CODIGOBARRAS FROM 11 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 13 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 7 FOR 4)) AS TIMESTAMP) <= @FECHAFIN  "+
					   "AND PRODUCTO = @PRODUCTO AND EMBARCADO = 'No'";
			FbConnection con  = _Conexion.ObtenerConexion();
			
			FbCommand com = new FbCommand(pSentencia, con);
			com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;
			com.Parameters.Add("@FECHAINI", FbDbType.VarChar).Value = AFechaIni;
			com.Parameters.Add("@FECHAFIN", FbDbType.VarChar).Value = AFechaFin;
			try
			{
				con.Open();
				FbDataReader reader = com.ExecuteReader();
				if(reader.Read())
				{
					pResult        = new InventarioProducto();
					pResult.Cajas  = (int)reader["CAJAS"];
					pResult.Kilos  = (decimal)reader["KILOS"];
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
		
		public List<InventarioProductoDesglosado> ObtenerInventarioProductoDesglosado(string AFechaIni, string AFechaFin, string AProducto)
		{
			List<InventarioProductoDesglosado> pInventarioProdDes=new List<InventarioProductoDesglosado>();
			InventarioProductoDesglosado pResult = null;
			string pSentencia = "SELECT T0.FOLIO, T0.FECHA, (SUBSTRING(CODIGOBARRAS FROM 11 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 13 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 7 FOR 4))"+
			                    " AS FECHA_CODIGO_BARRAS, T0.PRODUCTO, T1.DESCRIPCION, T0.CODIGOBARRAS, T0.PESO, T0.CAMARA, T0.TARIMA FROM DRASCORT T0 JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO WHERE "+ 
			                    "CAST((SUBSTRING(CODIGOBARRAS FROM 11 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 13 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 7 FOR 4)) AS TIMESTAMP) >= @FECHAINI AND "+
			                    "CAST((SUBSTRING(CODIGOBARRAS FROM 11 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 13 FOR 2) || '/' || SUBSTRING(CODIGOBARRAS FROM 7 FOR 4)) AS TIMESTAMP) <= @FECHAFIN "+ 
			                    "AND PRODUCTO = @PRODUCTO ORDER BY TARIMA";
			FbConnection con  = _Conexion.ObtenerConexion();
			
			FbCommand com = new FbCommand(pSentencia, con);
			com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;
			com.Parameters.Add("@FECHAINI", FbDbType.VarChar).Value = AFechaIni;
			com.Parameters.Add("@FECHAFIN", FbDbType.VarChar).Value = AFechaFin;
			try
			{
				
				con.Open();
				FbDataReader reader = com.ExecuteReader();
				
				while(reader.Read())
				{
					pResult                   = new InventarioProductoDesglosado();
					pResult.Folio             = (int)reader["FOLIO"];
					pResult.Fecha  		      = Utilerias.dateTimeToString((DateTime)reader["FECHA"]);
					pResult.FechaCodigoBarras = (reader["FECHA_CODIGO_BARRAS"]!=DBNull.Value) ? (string)reader["FECHA_CODIGO_BARRAS"] : "";
					pResult.Producto          = (reader["PRODUCTO"]!=DBNull.Value) ? (string)reader["PRODUCTO"] : "";
					pResult.Descripcion       = (reader["DESCRIPCION"]!=DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
					pResult.CodigoBarras      = (reader["CODIGOBARRAS"]!=DBNull.Value) ? (string)reader["CODIGOBARRAS"] : "";
					pResult.Peso              = (decimal)reader["PESO"];
					pResult.Camara  		  = (int)reader["CAMARA"];
					pResult.Tarima            = (int)reader["TARIMA"];
					pInventarioProdDes.Add(pResult);
				}
			}
			finally
			{
				if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
			}
			
			return pInventarioProdDes;
		}

        public List<Corte> ObtenerCajasPorFolio(int AFolio)
        {
            List<Corte> pCajasPorFolio = new List<Corte>();
            Corte pResult = null;
            string pSentencia = "SELECT FECHA, FOLIO, PRODUCTO, LOTE, CODIGOBARRAS, PESO, EMBARCADO, COALESCE(TARIMA, 0) AS TARIMA, EMB_FECHA FROM DRASCORT WHERE FOLIO = @FOLIO ORDER BY FECHA";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FOLIO", FbDbType.Integer).Value = AFolio;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult = new Corte();
                    if(reader["FECHA"] != DBNull.Value)
                        pResult.Fecha    = (DateTime)reader["FECHA"];
                    pResult.Folio        = reader["FOLIO"] != DBNull.Value ? (int)reader["FOLIO"] : -1;
                    pResult.Producto     = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Lote         = reader["LOTE"] != DBNull.Value ? (int)reader["LOTE"] : -1;
                    pResult.CodigoBarras = (reader["CODIGOBARRAS"] != DBNull.Value) ? (string)reader["CODIGOBARRAS"] : "";
                    pResult.Peso         = reader["PESO"] != DBNull.Value ? (decimal)reader["PESO"] : -1;
                    pResult.Embarcado    = (reader["EMBARCADO"] != DBNull.Value) ? (string)reader["EMBARCADO"] : "";
                    pResult.Tarima       = (int)reader["TARIMA"];
                    if (reader["EMB_FECHA"] != DBNull.Value)
                        pResult.Emb_Fecha    = (DateTime)reader["EMB_FECHA"];

                    pCajasPorFolio.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return pCajasPorFolio;
        }

        public List<SaldoPedimento> ObtenerSaldosPedimento(int APedimento)
        {
            List<SaldoPedimento> pSaldosPedimento = new List<SaldoPedimento>();
            SaldoPedimento pResult = null;
            string pSentencia = "SELECT T0.PRODUCTO, T1.DESCRIPCION, SUM(T0.PESO) AS KILOS, "+
                               "COALESCE((SELECT SUM(R0.PESO) FROM DRASCORT R0 "+
                               "JOIN DRASRESUMENCORTES R1 ON R1.ID = R0.ID_ACUM "+
                               "WHERE R1.PEDIMENTO = @PEDIMENTO AND R0.EMBARCADO = 'Si' AND R0.PRODUCTO = T0.PRODUCTO "+
                               "GROUP BY R0.PRODUCTO "+
                               "), 0) AS SALIDAS "+
                               "FROM DRASRESUMENCORTES T0 "+
                               "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                               "WHERE T0.PEDIMENTO = @PEDIMENTO "+
                               "GROUP BY T0.PRODUCTO, T1.DESCRIPCION";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@PEDIMENTO", FbDbType.Integer).Value = APedimento;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult             = new SaldoPedimento();
                    pResult.Producto    = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion = reader["DESCRIPCION"] != DBNull.Value ? (string)reader["DESCRIPCION"] : "";
                    pResult.Kilos       = reader["KILOS"] != DBNull.Value ? (decimal)reader["KILOS"] : -1;
                    pResult.Salidas     = reader["SALIDAS"] != DBNull.Value ? (decimal)reader["SALIDAS"] : -1;

                    pSaldosPedimento.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return pSaldosPedimento;
        }

        public List<CorteFecha> ObtenerCorteFecha(string AFecha)
        {
            List<CorteFecha> pCorteFecha = new List<CorteFecha>();
            CorteFecha pResult = null;
            string pSentencia = "SELECT T0.CAMARA, T2.DESCRIPCION AS NOMBRE_CAMARA, T0.EMBARCADO, COUNT(*) AS CAJAS, SUM(T0.PESO) AS PESO, 1 AS APLICADAS, 1 AS TRANSITORIAS, 1 AS DIFERENCIA FROM DRASCORT T0 "+
                       "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                       "JOIN DRASCAM T2 ON T2.ID = T0.CAMARA "+
                       "WHERE FECHA = @FECHA "+
                       "GROUP BY T0.CAMARA, T0.EMBARCADO, T2.DESCRIPCION "+
                       "ORDER BY T0.CAMARA, T0.EMBARCADO";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult              = new CorteFecha();
                    pResult.Camara       = (reader["CAMARA"] != DBNull.Value) ? (int)reader["CAMARA"] : -1;
                    pResult.NombreCamara = (reader["NOMBRE_CAMARA"] != DBNull.Value) ? (string)reader["NOMBRE_CAMARA"] : "";
                    pResult.Embarcado    = (reader["EMBARCADO"] != DBNull.Value) ? (string)reader["EMBARCADO"] : "";
                    pResult.Cajas        = (reader["CAJAS"] != DBNull.Value) ? (int)reader["CAJAS"] : 0;
                    pResult.Peso         = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;
                    pResult.Aplicadas    = ObtenerCajasAplicadasCorte(AFecha, (int)reader["CAMARA"], (string)reader["EMBARCADO"]);
                    pResult.Transitorias = (reader["TRANSITORIAS"] != DBNull.Value) ? (int)reader["TRANSITORIAS"] : 0;
                    pResult.Diferencia   = (reader["DIFERENCIA"] != DBNull.Value) ? (int)reader["DIFERENCIA"] : 0;

                    pCorteFecha.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return pCorteFecha;
        }

        public int ObtenerPendientesCorte(string AFecha)
        {
            int pResult = 0;
            string pSentencia = "SELECT COUNT(*) FROM DRASCORTTRAN T0 WHERE FECHA = @FECHA AND ESCANEADO = 'No' AND ESTATUS = 'E' AND PRODUCTO NOT IN (SELECT CLAVE FROM DRASPROD WHERE INVENTARIABLE = 'No')";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                if (reader.Read())
                {
                    pResult = (int)reader["COUNT"];
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

        public List<DetallePendienteCorte> ObtenerDetallePendientesCorte(string AFecha)
        {
            List<DetallePendienteCorte> pDetallePendienteCorte = new List<DetallePendienteCorte>();
            DetallePendienteCorte pResult = null;
            string pSentencia = "SELECT T0.FOLIO, T0.FECHA, T0.PRODUCTO, T1.DESCRIPCION, T0.CODIGOBARRAS, T0.LOTE, T0.PESO FROM DRASCORTTRAN T0 "+
                                "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                                "WHERE T0.FECHA = @FECHA AND T0.ESCANEADO = 'No' AND T0.ESTATUS = 'E' AND PRODUCTO NOT IN (SELECT CLAVE FROM DRASPROD WHERE INVENTARIABLE = 'No')";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult = new DetallePendienteCorte();
                    pResult.Folio        = (reader["FOLIO"] != DBNull.Value) ? (int)reader["FOLIO"] : -1;
                    pResult.Fecha        = (reader["FECHA"] != DBNull.Value) ? (DateTime?)reader["FECHA"] : null;
                    pResult.Producto     = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion  = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.CodigoBarras = (reader["CODIGOBARRAS"] != DBNull.Value) ? (string)reader["CODIGOBARRAS"] : "";
                    pResult.Lote         = (reader["LOTE"] != DBNull.Value) ? (int)reader["LOTE"] : 0;
                    pResult.Peso         = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;

                    pDetallePendienteCorte.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return pDetallePendienteCorte;
        }

        public int ObtenerCajasAplicadasCorte(string AFecha, int ACamara, string AEmbarcado)
        {
            string pSentencia = "SELECT COUNT(*) FROM DRASCORT WHERE "+ 
                               "FECHA = @FECHA "+
                               "AND CAMARA = @CAMARA "+
                               "AND EMBARCADO = @EMBARCADO "+
                               "AND ENTRADA_APLICADA = 'Si'";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value   = AFecha;
            com.Parameters.Add("@CAMARA", FbDbType.Integer).Value    = ACamara;
            com.Parameters.Add("@EMBARCADO", FbDbType.VarChar).Value = AEmbarcado;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                if (reader.Read())
                {
                    return (int)reader["COUNT"];
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return 0;
        }

        public List<DetalleCorteFecha> ObtenerDetalleCorteFecha(string AFecha, int ACamara, string AEmbarcado)
        {
            List<DetalleCorteFecha> pDetalleCorteFecha = new List<DetalleCorteFecha>();
            DetalleCorteFecha pResult = null;
            string pSentencia = "SELECT T0.FOLIO, T0.LOTE, T0.PRODUCTO, T1.DESCRIPCION, T0.CODIGOBARRAS, T0.EMBARCADO, T0.PESO, T0.TARIMA, T0.UBICACION, "+
                               "COALESCE(T0.ENTRADA_APLICADA, 'No') AS ENTRADA_APLICADA, ID_ACUM FROM DRASCORT T0 "+
                               "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                               "WHERE FECHA = @FECHA AND EMBARCADO = @EMBARCADO AND CAMARA = @CAMARA ORDER BY T0.PRODUCTO";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value   = AFecha;
            com.Parameters.Add("@CAMARA", FbDbType.Integer).Value    = ACamara;
            com.Parameters.Add("@EMBARCADO", FbDbType.VarChar).Value = AEmbarcado;

            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult                  = new DetalleCorteFecha();
                    pResult.Folio            = (reader["FOLIO"] != DBNull.Value) ? (int)reader["FOLIO"] : -1;
                    pResult.Lote             = (reader["LOTE"] != DBNull.Value) ? (int)reader["LOTE"] : -1;
                    pResult.Producto         = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion      = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.CodigoBarras     = (reader["CODIGOBARRAS"] != DBNull.Value) ? (string)reader["CODIGOBARRAS"] : "";
                    pResult.Embarcado        = (reader["EMBARCADO"] != DBNull.Value) ? (string)reader["EMBARCADO"] : "";
                    pResult.Peso             = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;
                    pResult.Tarima           = (reader["TARIMA"] != DBNull.Value) ? (int)reader["TARIMA"] : -1;
                    pResult.Ubicacion        = (reader["UBICACION"] != DBNull.Value) ? (string)reader["UBICACION"] : "";
                    pResult.Entrada_Aplicada = (reader["ENTRADA_APLICADA"] != DBNull.Value) ? (string)reader["ENTRADA_APLICADA"] : "";
                    pResult.Id_Acum          = (reader["ID_ACUM"] != DBNull.Value) ? (int)reader["ID_ACUM"] : 0;
                    pDetalleCorteFecha.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return pDetalleCorteFecha;
        }

        public List<ProduccionNoInventariada> ObtenerProduccionNoInventariadas(string AProducto, string AFechaIni, string AFechaFin)
        {
            List<ProduccionNoInventariada> pProduccionNoInventariada = new List<ProduccionNoInventariada>();
            ProduccionNoInventariada pResult = null;
            string pSentencia = "SELECT T0.FECHA, T0.LOTE, T0.PRODUCTO, T1.DESCRIPCION, COUNT(*) AS CANTIDAD, SUM(PESO) AS PESO FROM DRASCORTTRAN T0 " +
                                "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO " +
                                "WHERE T0.FECHA >= @FECHAINI AND T0.FECHA <= @FECHAFIN ";

            if (AProducto != "0")
                pSentencia = pSentencia + "AND T0.PRODUCTO = '" + AProducto + "' AND LOTE IN (SELECT LOTE FROM LOTES_NO_INVENTARIABLES) ";

            pSentencia = pSentencia + "GROUP BY T0.FECHA, T0.LOTE, T0.PRODUCTO, T1.DESCRIPCION " +
                                    "ORDER BY FECHA, PRODUCTO, LOTE ";

            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);

            com.Parameters.Add("@FECHAINI", FbDbType.TimeStamp).Value = AFechaIni;
            com.Parameters.Add("@FECHAFIN", FbDbType.TimeStamp).Value = AFechaFin;

            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult             = new ProduccionNoInventariada();
                    pResult.Fecha       = (reader["FECHA"] != DBNull.Value) ? (DateTime?)reader["FECHA"] : null;
                    pResult.Lote        = (reader["LOTE"] != DBNull.Value) ? (int)reader["LOTE"] : -1;
                    pResult.Producto    = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.Cantidad    = (reader["CANTIDAD"] != DBNull.Value) ? (int)reader["CANTIDAD"] : 0;
                    pResult.Peso        = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;

                    pProduccionNoInventariada.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return pProduccionNoInventariada;
        }

        public bool EliminarProduccionNoInventariable(string AFecha, int ALote, string AProducto, out string AMensajeError)
        {
            bool pResult = true;
            AMensajeError = "";

            string pSentencia = "DELETE FROM DRASCORTTRAN WHERE FECHA = @FECHA AND LOTE = @LOTE AND PRODUCTO = @PRODUCTO";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value  = AFecha;
            com.Parameters.Add("@LOTE", FbDbType.Integer).Value     = ALote;
            com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;

            try
            {
                con.Open();

                try
                {
                    com.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    AMensajeError = ex.Message;
                    pResult = false;
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

        public List<ProduccionPorBascula> ObtenerProduccionPorBascula(string AFecha, int ABascula)
        {
            List<ProduccionPorBascula> listaTarimas = new List<ProduccionPorBascula>();
            ProduccionPorBascula pResult = null;

            string pSentencia = "SELECT T0.FOLIO, T0.FECHA, T0.HORA, T0.LOTE, T0.PRODUCTO, T1.DESCRIPCION, T0.CODIGOBARRAS, T0.PESO, COALESCE(T0.CONSECUTIVO_BASCULA, 0) " +
                                "AS CONSECUTIVO_BASCULA, T0.EMBARCADO, T0.ORDEN_PRODUCCION, T0.HORA FROM DRASCORTTRAN T0 " +
                                "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO " +
                                "WHERE T0.FECHA = @FECHA AND T0.BASCULA = @BASCULA ORDER BY T0.CONSECUTIVO_BASCULA";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;
            com.Parameters.Add("@BASCULA", FbDbType.Integer).Value = ABascula;
            try
            {
                con.Open();

                FbDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    pResult = new ProduccionPorBascula();
                    pResult.Folio               = reader["FOLIO"] != DBNull.Value ? (int)reader["FOLIO"] : -1;
                    pResult.Fecha               = reader["FECHA"] != DBNull.Value ? (DateTime?)reader["FECHA"] : null;
                    pResult.Hora                = reader["HORA"] != DBNull.Value ? (string)reader["HORA"] : "";
                    pResult.Lote                = reader["LOTE"] != DBNull.Value ? (int)reader["LOTE"] : -1;
                    pResult.Producto            = reader["PRODUCTO"] != DBNull.Value ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion         = reader["DESCRIPCION"] != DBNull.Value ? (string)reader["DESCRIPCION"] : "";
                    pResult.CodigoBarras        = reader["CODIGOBARRAS"] != DBNull.Value ? (string)reader["CODIGOBARRAS"] : "";
                    pResult.Peso                = reader["PESO"] != DBNull.Value ? (decimal)reader["PESO"] : 0;
                    pResult.Consecutivo_Bascula = reader["CONSECUTIVO_BASCULA"] != DBNull.Value ? (int)reader["CONSECUTIVO_BASCULA"] : -1;
                    pResult.Embarcado           = reader["EMBARCADO"] != DBNull.Value ? (string)reader["EMBARCADO"] : "";
                    pResult.OrdenProduccion     = reader["ORDEN_PRODUCCION"] != DBNull.Value ? (int)reader["ORDEN_PRODUCCION"] : -1;
                    listaTarimas.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return listaTarimas;
        }

        public List<ProduccionNoInventariada> ObtenerProduccionNoInventariadasPorProducto(string AProducto, string AFechaIni, string AFechaFin)
        {
            List<ProduccionNoInventariada> pProduccionNoInventariada = new List<ProduccionNoInventariada>();
            ProduccionNoInventariada pResult = null;
            string pSentencia = "SELECT T0.FECHA, T0.LOTE, T0.PRODUCTO, T1.DESCRIPCION, COUNT(*) AS CANTIDAD, SUM(T0.PESO) AS PESO, SUM(T0.PESO) AS PESO FROM DRASCORTTRAN T0 "+
                                "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                                "WHERE T0.FECHA >= ? AND T0.FECHA <= ? "+
                                "AND T0.PRODUCTO IN(SELECT CLAVE FROM DRASPROD WHERE CLAVE NOT CONTAINING('-') AND INVENTARIABLE = 'No')";

            if (AProducto != "0")
                pSentencia = pSentencia + "AND T0.PRODUCTO = '" + AProducto + "' ";

            pSentencia = pSentencia + "GROUP BY T0.FECHA, T0.LOTE, T0.PRODUCTO, T1.DESCRIPCION ORDER BY FECHA, PRODUCTO, LOTE ";

            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);

            com.Parameters.Add("@FECHAINI", FbDbType.TimeStamp).Value = AFechaIni;
            com.Parameters.Add("@FECHAFIN", FbDbType.TimeStamp).Value = AFechaFin;

            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult             = new ProduccionNoInventariada();
                    pResult.Fecha       = (reader["FECHA"] != DBNull.Value) ? (DateTime?)reader["FECHA"] : null;
                    pResult.Lote        = (reader["LOTE"] != DBNull.Value) ? (int)reader["LOTE"] : -1;
                    pResult.Producto    = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.Cantidad    = (reader["CANTIDAD"] != DBNull.Value) ? (int)reader["CANTIDAD"] : 0;
                    pResult.Peso        = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;

                    pProduccionNoInventariada.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return pProduccionNoInventariada;
        }

        public List<DesgloseProductoCamara> ObtenerDesgloseProductoPorCamara(string AProducto)
        {
            List<DesgloseProductoCamara> pDesgloseProductoCamara = new List<DesgloseProductoCamara>();
            DesgloseProductoCamara pResult = null;
            string pSentencia = "SELECT T0.PRODUCTO, T1.DESCRIPCION, T0.CAMARA, T2.DESCRIPCION AS NOMBRE_CAMARA, COUNT(*) AS CAJAS FROM DRASCORT T0 "+
                               "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                               "JOIN DRASCAM T2 ON T2.ID = T0.CAMARA "+
                               "WHERE T0.PRODUCTO = @PRODUCTO AND T0.EMBARCADO = 'No' "+
                               "AND T2.ACTIVO = 'Si' "+
                               "GROUP BY T0.PRODUCTO, T1.DESCRIPCION, T0.CAMARA, T2.DESCRIPCION";

            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);

            com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;

            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult = new DesgloseProductoCamara();
                    
                    pResult.Producto      = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion   = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.Camara        = (reader["CAMARA"] != DBNull.Value) ? (int)reader["CAMARA"] : -1;
                    pResult.Nombre_Camara = (reader["NOMBRE_CAMARA"] != DBNull.Value) ? (string)reader["NOMBRE_CAMARA"] : "";
                    pResult.Cajas         = (reader["CAJAS"] != DBNull.Value) ? (int)reader["CAJAS"] : 0;
                    pDesgloseProductoCamara.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return pDesgloseProductoCamara;
        }

        public List<ResumenProductoPorCamara> ObtenerResumenPtosCamara(int ACamara)
        {
            List<ResumenProductoPorCamara> ResumenPtosCamara = new List<ResumenProductoPorCamara>();
            ResumenProductoPorCamara pResult = null;
            string pSentencia = "SELECT T0.PRODUCTO, T1.DESCRIPCION, T1.CODIGOSAP, COUNT(*) AS CAJAS, SUM(T0.PESO) AS KILOS FROM DRASCORT T0 "+
                               "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                               "WHERE T0.CAMARA IN (@CAMARA) AND T0.EMBARCADO = 'No' "+
                               "GROUP BY T0.PRODUCTO, T1.DESCRIPCION, T1.CODIGOSAP";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@CAMARA", FbDbType.Integer).Value = ACamara;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult             = new ResumenProductoPorCamara();
                    pResult.Producto    = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.CodigoSAP   = (reader["CODIGOSAP"] != DBNull.Value) ? (string)reader["CODIGOSAP"] : "";
                    pResult.Cajas       = (reader["CAJAS"] != DBNull.Value) ? (int)reader["CAJAS"] : 0;
                    pResult.Kilos       = (reader["KILOS"] != DBNull.Value) ? (decimal)reader["KILOS"] : 0;
                    pResult.Camara      = ACamara;
                    ResumenPtosCamara.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return ResumenPtosCamara;
        }

        public List<Corte> ObtenerDetalleProductosCamara(int ACamara, string AProducto)
        {
            List<Corte> DetallePtosCamara = new List<Corte>();
            Corte pResult = null;
            string pSentencia = "SELECT TARIMA, FECHA, FOLIO, CODIGOBARRAS, PESO, CAMARA, UBICACION FROM DRASCORT T0 "+
                                "WHERE T0.CAMARA IN (@CAMARA) AND T0.EMBARCADO = 'No' AND PRODUCTO = @PRODUCTO ";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@CAMARA", FbDbType.Integer).Value = ACamara;
            com.Parameters.Add("@PRODUCTO", FbDbType.Integer).Value = AProducto;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult              = new Corte();
                    pResult.Tarima       = (reader["TARIMA"] != DBNull.Value) ? (int)reader["TARIMA"] : -1;
                    pResult.Fecha        = (DateTime)reader["FECHA"];
                    pResult.Folio        = (reader["FOLIO"] != DBNull.Value) ? (int)reader["FOLIO"] : -1;
                    pResult.CodigoBarras = (reader["CODIGOBARRAS"] != DBNull.Value) ? (string)reader["CODIGOBARRAS"] : "";
                    pResult.Peso         = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;
                    pResult.Camara       = (reader["CAMARA"] != DBNull.Value) ? (int)reader["CAMARA"] : -1;
                    pResult.Ubicacion    = (reader["UBICACION"] != DBNull.Value) ? (string)reader["UBICACION"] : "";
                    DetallePtosCamara.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return DetallePtosCamara;
        }

        public List<CajaPendienteRecepcionEmbarque> ObtenerCajasPendientesRecepcionEmbarques(string AFecha)
        {
            List<CajaPendienteRecepcionEmbarque> CajasPendientesRecepcionEmbarques = new List<CajaPendienteRecepcionEmbarque>();
            CajaPendienteRecepcionEmbarque pResult = null;
            string pSentencia = "SELECT T0.PRODUCTO, T1.DESCRIPCION, COUNT(*) AS CAJAS FROM DRASCORT T0 "+
                               "JOIN DRASPROD T1 ON T1.CLAVE = T0.PRODUCTO "+
                               "WHERE FECHA = @FECHA AND COALESCE(ESCANEADO, 'No') = 'No' AND COALESCE(EMBARCADO, 'No') = 'No' "+
                               "GROUP BY T0.PRODUCTO, T1.DESCRIPCION";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value = AFecha;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult             = new CajaPendienteRecepcionEmbarque();
                    pResult.Producto    = (reader["PRODUCTO"] != DBNull.Value) ? (string)reader["PRODUCTO"] : "";
                    pResult.Descripcion = (reader["DESCRIPCION"] != DBNull.Value) ? (string)reader["DESCRIPCION"] : "";
                    pResult.Cajas       = (reader["CAJAS"] != DBNull.Value) ? (int)reader["CAJAS"] : 0;

                    CajasPendientesRecepcionEmbarques.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return CajasPendientesRecepcionEmbarques;
        }

        public List<Corte> ObtenerDetalleCajasPendientesRecepcionEmbarques(string AFecha, string AProducto)
        {
            List<Corte> DetalleCajasPendientesRecepcionEmbarques = new List<Corte>();
            Corte pResult = null;
            string pSentencia = "SELECT FOLIO, CODIGOBARRAS, PESO FROM DRASCORT WHERE FECHA = @FECHA AND PRODUCTO = @PRODUCTO  AND COALESCE(ESCANEADO, 'No') = 'No' AND COALESCE(EMBARCADO, 'No') = 'No' ORDER BY FOLIO";
            FbConnection con = _Conexion.ObtenerConexion();

            FbCommand com = new FbCommand(pSentencia, con);
            com.Parameters.Add("@FECHA", FbDbType.TimeStamp).Value  = AFecha;
            com.Parameters.Add("@PRODUCTO", FbDbType.VarChar).Value = AProducto;
            try
            {

                con.Open();
                FbDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    pResult              = new Corte();
                    pResult.Folio        = (reader["FOLIO"] != DBNull.Value) ? (int)reader["FOLIO"] : -1;
                    pResult.CodigoBarras = (reader["CODIGOBARRAS"] != DBNull.Value) ? (string)reader["CODIGOBARRAS"] : "";
                    pResult.Peso         = (reader["PESO"] != DBNull.Value) ? (decimal)reader["PESO"] : 0;

                    DetalleCajasPendientesRecepcionEmbarques.Add(pResult);
                }
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return DetalleCajasPendientesRecepcionEmbarques;
        }
    }
}