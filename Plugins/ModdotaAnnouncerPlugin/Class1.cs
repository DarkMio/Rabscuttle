using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ModdotaAnnouncerPlugin
{
    public class Class1 {

        public void Connect() {
            string connectionString = "server=localhost; database=test; uid=root; pwd=root";
            MySqlConnection cnn = new MySqlConnection(connectionString);

        }
    }
}
