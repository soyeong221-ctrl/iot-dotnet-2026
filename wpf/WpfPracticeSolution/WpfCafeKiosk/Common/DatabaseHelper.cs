using MySqlConnector;
using System.Data;

namespace WpfCafeKiosk.Common
{
    public class DatabaseHelper
    {
        // MySQL 연결문자열 key=value; 
        private string connStr = "Server=localhost;" +   // 운영아이피로 바꾸세요
                                 "Port=3306;" +   // 운영포트로 변경할 것
                                 "Database=cafekiosk;" +
                                 "User ID=root;" +  // 운영DB 사용자로 변경
                                 "Password=my123456;" +  // 패스워드 변경할 것
                                 "Charset=utf8mb4;";

        // DB조회 메서드
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

        // DB실행 메서드(실행결과 리턴)
        // INSERT, UPDATE, DELETE
        public int ExecuteScalar(string sql)
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            using MySqlCommand cmd = new MySqlCommand(sql, conn);

            // 1건 INSERT하면 1리턴, 2건 삭제하면 2리턴, count(*) 카운트갯수 리턴 등 결과건수를 확인
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // DB실행 메서드(실행결과 리턴X)
        // INSERT, UPDATE, DELETE
        public void ExecuteNonQuery(string sql)
        {
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            using MySqlCommand cmd = new MySqlCommand(sql, conn);

            cmd.ExecuteNonQuery(); // 결과 안보고 실행가능, 건수리턴도 가능
        }


    }
}