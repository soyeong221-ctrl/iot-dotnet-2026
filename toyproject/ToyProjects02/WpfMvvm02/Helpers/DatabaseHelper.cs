using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace WpfMvvm02.Helpers {
    public class DatabaseHelper {
        // MySQL 연결문자열 key=value; 
        public string connStr = "Server=localhost;" +   // 운영아이피로 바꾸세요
                                 "Port=3306;" +   // 운영포트로 변경할 것
                                 "Database=bookrentalshop;" +
                                 "User ID=root;" +  // 운영DB 사용자로 변경
                                 "Password=my123456;" +  // 패스워드 변경할 것
                                 "Charset=utf8mb4;";

        // DB조회 메서드
        public DataTable Select(string sql) {
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
        public int ExecuteScalar(string sql) {
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            using MySqlCommand cmd = new MySqlCommand(sql, conn);

            // 1건 INSERT하면 1리턴, 2건 삭제하면 2리턴, count(*) 카운트갯수 리턴 등 결과건수를 확인
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // DB실행 메서드(실행결과 리턴X)
        // INSERT, UPDATE, DELETE
        public void ExecuteNonQuery(string sql) {
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            using MySqlCommand cmd = new MySqlCommand(sql, conn);

            cmd.ExecuteNonQuery(); // 결과 안보고 실행가능, 건수리턴도 가능
        }

        public int Execute(string sql) {
            using (MySqlConnection conn = new MySqlConnection(connStr)) {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(sql, conn)) {
                    return cmd.ExecuteNonQuery();  // INSERT, UPDATE, DELETE 쿼리를 실행
                }
            }
        }

        // Parameter 사용 Execute, 오버로딩
        // params 키워드 사용 MySqlParameter 갯수 제한이 없음
        public int Execute(string sql, params MySqlParameter[] parameters) {
            using (MySqlConnection conn = new MySqlConnection(connStr)) {
                conn.Open();

                using MySqlCommand cmd = new MySqlCommand(sql, conn);

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();  // INSERT, UPDATE, DELETE 쿼리를 실행
            }
        }

    }
}