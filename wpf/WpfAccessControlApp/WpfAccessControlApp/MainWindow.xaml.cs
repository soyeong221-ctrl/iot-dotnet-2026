using MySqlConnector;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfAccessControlApp.Helpers;

namespace WpfAccessControlApp
{
    /// <summary>
    /// MainWindow.xaml의 뒤에서 실제 기능을 처리하는 코드입니다.
    /// 
    /// 이 프로젝트의 핵심 역할:
    /// 1. 부서 목록과 사원 목록을 DB에서 불러오기
    /// 2. 사원을 신규 등록 / 수정 / 삭제하기
    /// 3. 3초마다 출입 신호를 자동으로 발생시키는 가상 하드웨어 엔진 실행
    /// 4. 출입 결과를 access_logs 테이블과 화면 로그 그리드에 실시간 반영하기
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseHelper databaseHelper;
        private DispatcherTimer accessSimulationTimer;
        private readonly Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                databaseHelper = new DatabaseHelper();

                LoadDepartmentComboBox();
                LoadUsers();
                LoadAccessLogs();

                StartAccessSimulationTimer();

                SbiResMsg.Content = "시스템 준비 완료 - 가상 출입 통제 엔진이 시작되었습니다.";
            }
            catch (Exception ex)
            {
                SbiResMsg.Content = $"초기 로딩 오류: {ex.Message}";
            }
        }

        private void LoadDepartmentComboBox()
        {
            string query =
                "SELECT dept_code, dept_name " +
                "FROM departments " +
                "ORDER BY dept_code";

            DataTable dt = databaseHelper.Select(query);

            CboDeptCode.ItemsSource = dt.DefaultView;
            CboDeptCode.DisplayMemberPath = "dept_name";
            CboDeptCode.SelectedValuePath = "dept_code";
            CboDeptCode.SelectedIndex = -1;
        }

        private void LoadUsers()
        {
            string query =
                "SELECT u.user_idx, " +
                "       u.user_name, " +
                "       u.dept_code, " +
                "       d.dept_name, " +
                "       u.user_phone, " +
                "       u.card_uid, " +
                "       u.auth_level " +
                "FROM users AS u " +
                "LEFT JOIN departments AS d ON u.dept_code = d.dept_code " +
                "ORDER BY u.user_idx DESC";

            DataTable dt = databaseHelper.Select(query);
            GrdUsers.ItemsSource = dt.DefaultView;
        }

        private void LoadAccessLogs()
        {
            string query =
                "SELECT l.log_idx, " +
                "       l.user_idx, " +
                "       u.user_name, " +
                "       d.dept_name, " +
                "       l.access_time, " +
                "       l.is_success, " +
                "       CASE WHEN l.is_success = 1 THEN '성공' ELSE '실패' END AS access_result " +
                "FROM access_logs AS l " +
                "LEFT JOIN users AS u ON l.user_idx = u.user_idx " +
                "LEFT JOIN departments AS d ON u.dept_code = d.dept_code " +
                "ORDER BY l.access_time DESC, l.log_idx DESC " +
                "LIMIT 50";

            DataTable dt = databaseHelper.Select(query);
            GrdAccessLogs.ItemsSource = dt.DefaultView;
        }

        private void GrdUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (GrdUsers.SelectedItem == null)
                {
                    return;
                }

                DataRowView row = GrdUsers.SelectedItem as DataRowView;

                if (row == null)
                {
                    return;
                }

                TxtUserIdx.Text = Convert.ToString(row["user_idx"]);
                TxtUserName.Text = Convert.ToString(row["user_name"]);
                TxtUserPhone.Text = Convert.ToString(row["user_phone"]);
                TxtCardUid.Text = Convert.ToString(row["card_uid"]);
                TxtAuthLevel.Text = Convert.ToString(row["auth_level"]);

                CboDeptCode.SelectedValue = Convert.ToString(row["dept_code"]);

                SbiResMsg.Content = $"{TxtUserName.Text} 사원 정보를 불러왔습니다.";
            }
            catch (Exception ex)
            {
                SbiResMsg.Content = $"사원 상세 로딩 오류: {ex.Message}";
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
            TxtUserName.Focus();
            SbiResMsg.Content = "신규 사원 정보를 입력하세요.";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string userName = TxtUserName.Text.Trim();
                string deptCode = Convert.ToString(CboDeptCode.SelectedValue);
                string userPhone = TxtUserPhone.Text.Trim();
                string cardUid = TxtCardUid.Text.Trim();
                string authLevelText = TxtAuthLevel.Text.Trim();

                if (string.IsNullOrWhiteSpace(userName))
                {
                    MessageBox.Show("사원명을 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtUserName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(deptCode))
                {
                    MessageBox.Show("부서를 선택하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CboDeptCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(cardUid))
                {
                    MessageBox.Show("카드 UID를 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtCardUid.Focus();
                    return;
                }

                if (!string.IsNullOrWhiteSpace(userPhone))
                {
                    if (!Regex.IsMatch(userPhone, @"^010-\d{4}-\d{4}$"))
                    {
                        MessageBox.Show("전화번호는 010-1234-5678 형식으로 입력하세요.", "입력 오류",
                            MessageBoxButton.OK, MessageBoxImage.Warning);

                        TxtUserPhone.Focus();
                        return;
                    }
                }

                if (!Regex.IsMatch(cardUid, @"^\d{4}$"))
                {
                    MessageBox.Show("카드 UID는 숫자 4자리로 입력하세요. 예: 1001", "입력 오류",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    TxtCardUid.Focus();
                    return;
                }

                if (!int.TryParse(authLevelText, out int authLevel))
                {
                    MessageBox.Show("출입 권한 등급은 숫자로 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtAuthLevel.Focus();
                    return;
                }

                if (authLevel < 1 || authLevel > 3)
                {
                    MessageBox.Show("출입 권한 등급은 1~3 사이로 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtAuthLevel.Focus();
                    return;
                }

                bool isUpdate = int.TryParse(TxtUserIdx.Text.Trim(), out int userIdx) && userIdx > 0;

                if (isUpdate)
                {
                    UpdateUser(userIdx, userName, deptCode, userPhone, cardUid, authLevel);
                    SbiResMsg.Content = $"{userName} 사원 정보가 수정되었습니다.";
                }
                else
                {
                    InsertUser(userName, deptCode, userPhone, cardUid, authLevel);
                    SbiResMsg.Content = $"{userName} 사원이 신규 등록되었습니다.";
                }

                ClearInputs();
                LoadUsers();
            }
            catch (Exception ex)
            {
                SbiResMsg.Content = $"사원 저장 오류: {ex.Message}";
            }
        }

        private void InsertUser(string userName, string deptCode, string userPhone, string cardUid, int authLevel)
        {
            string query =
                "INSERT INTO users " +
                "    (user_name, dept_code, user_phone, card_uid, auth_level) " +
                "VALUES " +
                "    (@user_name, @dept_code, @user_phone, @card_uid, @auth_level)";

            databaseHelper.Execute(query,
                new MySqlParameter("@user_name", userName),
                new MySqlParameter("@dept_code", deptCode),
                new MySqlParameter("@user_phone", userPhone),
                new MySqlParameter("@card_uid", cardUid),
                new MySqlParameter("@auth_level", authLevel));
        }

        private void UpdateUser(int userIdx, string userName, string deptCode, string userPhone, string cardUid, int authLevel)
        {
            string query =
                "UPDATE users " +
                "SET user_name = @user_name, " +
                "    dept_code = @dept_code, " +
                "    user_phone = @user_phone, " +
                "    card_uid = @card_uid, " +
                "    auth_level = @auth_level " +
                "WHERE user_idx = @user_idx";

            databaseHelper.Execute(query,
                new MySqlParameter("@user_name", userName),
                new MySqlParameter("@dept_code", deptCode),
                new MySqlParameter("@user_phone", userPhone),
                new MySqlParameter("@card_uid", cardUid),
                new MySqlParameter("@auth_level", authLevel),
                new MySqlParameter("@user_idx", userIdx));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(TxtUserIdx.Text.Trim(), out int userIdx) || userIdx <= 0)
                {
                    MessageBox.Show("먼저 삭제할 사원을 선택하세요.", "삭제 안내", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string userName = TxtUserName.Text.Trim();

                MessageBoxResult result = MessageBox.Show(
                    $"{userName} 사원을 삭제하시겠습니까?",
                    "삭제 확인",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    SbiResMsg.Content = "삭제가 취소되었습니다.";
                    return;
                }

                string query = "DELETE FROM users WHERE user_idx = @user_idx";

                int affectedRows = databaseHelper.Execute(query,
                    new MySqlParameter("@user_idx", userIdx));

                if (affectedRows > 0)
                {
                    ClearInputs();
                    LoadUsers();

                    SbiResMsg.Content = $"{userName} 사원이 삭제되었습니다.";
                }
                else
                {
                    SbiResMsg.Content = "삭제된 사원이 없습니다.";
                }
            }
            catch (Exception ex)
            {
                SbiResMsg.Content = $"사원 삭제 오류: {ex.Message}";
            }
        }

        private void ClearInputs()
        {
            TxtUserIdx.Text = string.Empty;
            TxtUserName.Text = string.Empty;
            CboDeptCode.SelectedIndex = -1;
            TxtUserPhone.Text = string.Empty;
            TxtCardUid.Text = string.Empty;
            TxtAuthLevel.Text = "1";
        }

        private void StartAccessSimulationTimer()
        {
            accessSimulationTimer = new DispatcherTimer();
            accessSimulationTimer.Interval = TimeSpan.FromSeconds(3);
            accessSimulationTimer.Tick += AccessSimulationTimer_Tick;
            accessSimulationTimer.Start();
        }

        private void AccessSimulationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                DataRowView randomUser = GetRandomUser();

                if (randomUser == null)
                {
                    SbiResMsg.Content = "등록된 사원이 없어 출입 시뮬레이션을 대기 중입니다.";
                    return;
                }

                int userIdx = Convert.ToInt32(randomUser["user_idx"]);
                string userName = Convert.ToString(randomUser["user_name"]);
                string deptName = Convert.ToString(randomUser["dept_name"]);

                bool isSuccess = random.Next(0, 100) < 80;

                InsertAccessLog(userIdx, isSuccess);
                LoadAccessLogs();


                if (isSuccess)
                {
                    // 성공 시: 상태바 글자 초록색으로 변경
                    SbiResMsg.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 125, 50)); // #2E7D32
                    SbiResMsg.Content = $"✅ 성공 - {deptName} {userName} 사원 출입 인증 완료";
                }
                else
                {
                    // 실패 시: 상태바 글자 빨간색으로 변경
                    SbiResMsg.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 40, 40));  // #C62828
                    SbiResMsg.Content = $"🚨 경고 - {deptName} {userName} 사원 보안 구역 출입 실패 (미승인 카드)";
                }
            }
            catch (Exception ex)
            {
                SbiResMsg.Content = $"가상 하드웨어 엔진 오류: {ex.Message}";
            }
        }

        private DataRowView GetRandomUser()
        {
            string query =
                "SELECT u.user_idx, " +
                "       u.user_name, " +
                "       u.dept_code, " +
                "       d.dept_name " +
                "FROM users AS u " +
                "LEFT JOIN departments AS d ON u.dept_code = d.dept_code";

            DataTable dt = databaseHelper.Select(query);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            int randomIndex = random.Next(0, dt.Rows.Count);
            return dt.DefaultView[randomIndex];
        }

        private void InsertAccessLog(int userIdx, bool isSuccess)
        {
            string query =
                "INSERT INTO access_logs " +
                "    (user_idx, access_time, is_success) " +
                "VALUES " +
                "    (@user_idx, NOW(), @is_success)";

            databaseHelper.Execute(query,
                new MySqlParameter("@user_idx", userIdx),
                new MySqlParameter("@is_success", isSuccess));
        }
    }
}