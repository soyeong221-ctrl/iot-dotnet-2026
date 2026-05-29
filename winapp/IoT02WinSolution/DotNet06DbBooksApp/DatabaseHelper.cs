using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DotNet06DbBooksApp
{
    public class DatabaseHelper
    {
        // MySQL 연결문자열
        private string connStr = "Server = localhost;Database=bookrentalshop;User ID=root;Password=my123456;Charset=utf8mb4;";
        
        /// <summary>
        /// SELECT 쿼리 실행 메서드
        /// MySQL, SQLServer, Oracle 등 DB종류와 상관없이 순서 동일
        /// MySQLConnection, SqlConnection, OracleConnection 등 객체명만 다름
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable Select(string sql)
        {
            // 1. Dbconnection 객체 생성: Db연결문자열 사용
            using MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();    // DB 오픈

            // 2. SqlCommand 객체 생성: 쿼리를 실행할 수 있는 준비
            using MySqlCommand cmd = new MySqlCommand(sql, conn);
            // 3. SqlDataAdapter 생성: 매우 간단하게 데이터 가져올 수 있는 방법
            //  or SqlDataReader 생성: 직접 데이터를 핸들링 
            using MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            // 4. 공통 DataTable 객체: 어댑터 객체내 데이터를 복사
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        
        
        }

        public int Execute(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    return cmd.ExecuteNonQuery();   // INSERT, UPDATE, DELETE 쿼리를 실행
                }

            }
        }
    }
}
