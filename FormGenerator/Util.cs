using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGenerator
{
    public class Util
    {
        public static String connectionString = @"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + @"..\..\Database\Formularios.db";

        public static DataView LoadFormularios()
        {
            DataTable dt = new DataTable();

            String SQL = "SELECT * FROM tb_formulario";

            SQLiteConnection conn = new SQLiteConnection(Util.connectionString);

            SQLiteDataAdapter da = new SQLiteDataAdapter(SQL, Util.connectionString);
            da.Fill(dt);

            conn.Close();

            return dt.AsDataView();
        }
    }
}
