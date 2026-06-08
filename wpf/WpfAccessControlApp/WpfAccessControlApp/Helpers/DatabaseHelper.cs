using System;
using System.Data;
using MySqlConnector;

namespace WpfAccessControlApp.Helpers
{
    public class DatabaseHelper
    {
        // DBeaver에서 만든 access_control_db 사용.
        private readonly string connString =
            "Server=localhost;Port=3306;Database=access_control_db;Uid=root;Pwd=my123456;";

        /// <summary>
        /// 데이터 조회(SELECT)용 메서드.
        /// DataTable dt = databaseHelper.Select("SELECT * FROM users");
        /// 
        /// </summary>
        public DataTable Select(string query, params MySqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                // using -> DB 연결을 사용한 뒤 자동으로 정리.
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Select 오류: {ex.Message}");

                // 오류를 MainWindow 쪽으로 다시 전달 -> MessageBox로 사용자에게 오류를 보여줄 수 있음.
                throw;
            }

            return dt;
        }

        /// <summary>
        /// 데이터 삽입, 수정, 삭제(INSERT, UPDATE, DELETE)용 메서드.
        /// 
        /// 반환값: 실제로 영향을 받은 행(row)의 개수.
        /// INSERT 1건 성공이면 1, DELETE 대상이 없으면 0
        /// </summary>
        public int Execute(string query, params MySqlParameter[] parameters)
        {
            int resultRows = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        resultRows = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Execute 오류: {ex.Message}");

                throw;
            }

            return resultRows;
        }
    }
}