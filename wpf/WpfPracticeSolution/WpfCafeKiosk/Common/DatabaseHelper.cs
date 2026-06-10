using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace WpfCafeKiosk.Common
{
    class DatabaseHelper
    {
        // MySQL 연결문자열 key=value;
        private string connStr = "Server=localhost;"+   // 운영 아이피로 변경
                                 "Port=3306;"+          // 운영 포트로 변경
                                 "Database=cafekiosk;"+
                                 "User ID=root;"+       // 운영DB 사용자로 변경
                                 "Password=my123456;"+  // 패스워드 변경
                                 "Charset=utf8mb4;";


        public DataTable Select(string sql)
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open(); 

            using MySqlCommand cmd = new MySqlCommand(sql, conn);
            using MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }
    }
}
