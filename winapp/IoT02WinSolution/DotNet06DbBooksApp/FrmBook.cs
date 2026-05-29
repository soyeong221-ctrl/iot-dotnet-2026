using MaterialSkin.Controls;
using System.Data;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace DotNet06DbBooksApp
{
    public partial class FrmBook : MaterialForm
    {
        DatabaseHelper dbHelper;

        public FrmBook()
        {
            InitializeComponent();
        }

        private void FrmBook_Load(object sender, EventArgs e)
        {
            dbHelper = new DatabaseHelper(); // 객체생성

            // 자동으로 컬럼 생성하면 개발자 의도대로 컬럼 변경 불가
            DgvBooks.AutoGenerateColumns = false;

            InitGrid(); // 데이터그리드뷰 컬럼 초기 설정
            InitData(); // division 테이블 데이터 연동

            LoadData(); // 초기화 완료 후 전체 데이터 읽어오기
        }

        private void InitData()
        {
            // 책장르 데이터 조회
            string divSql = "SELECT div_code, div_name FROM division";
            DataTable divTable = dbHelper.Select(divSql);

            // 기존 div_code 컬럼위치 인덱스 
            var colIndex = DgvBooks.Columns["div_code"].Index;  // 아마 2

            // 기존 DataGridViewTextBoxColumn으로 생성된 컬럼 제거
            DgvBooks.Columns.RemoveAt(colIndex);

            // 콤보박스컬럼 새로 생성
            DataGridViewComboBoxColumn colCboDivCode = new DataGridViewComboBoxColumn
            {
                Name = "div_code",
                HeaderText = "책장르",
                DataPropertyName = "div_code",
                // 연동되는 데이터 설정
                DataSource = divTable,
                ValueMember = "div_code",
                DisplayMember = "div_name",
            };

            // 기존 div_code 컬럼 위치에 추가
            DgvBooks.Columns.Insert(colIndex, colCboDivCode);
            DgvBooks.Columns["div_code"].ReadOnly = false;
        }

        private void InitGrid()
        {
            // 7개 컬럼 설정
            // book_idx
            DataGridViewTextBoxColumn colBookIdx = new DataGridViewTextBoxColumn
            {
                Name = "book_idx",
                HeaderText = "순번", // 화면표시 컬럼명
                DataPropertyName = "book_idx",
                Width = 60,
                ReadOnly = true // PK는 수정하면 안 됨!
            };

            DgvBooks.Columns.Add(colBookIdx);

            // author
            // book_idx
            DataGridViewTextBoxColumn colAuthor = new DataGridViewTextBoxColumn
            {
                Name = "author",
                HeaderText = "저자",
                DataPropertyName = "author",
                Width = 170
            };
            DgvBooks.Columns.Add(colAuthor);

            // div_code
            DataGridViewTextBoxColumn colDivCode = new DataGridViewTextBoxColumn
            {
                Name = "div_code",
                HeaderText = "책장르",
                DataPropertyName = "div_code",
                Width = 100
            };
            DgvBooks.Columns.Add(colDivCode);

            // book_name
            DataGridViewTextBoxColumn colBookName = new DataGridViewTextBoxColumn
            {
                Name = "book_name",
                HeaderText = "책이름",
                DataPropertyName = "book_name",
                Width = 230
            };
            DgvBooks.Columns.Add(colBookName);

            // release_dt
            DataGridViewTextBoxColumn colReleaseDt = new DataGridViewTextBoxColumn
            {
                Name = "release_dt",
                HeaderText = "출판일",
                DataPropertyName = "release_dt",
                Width = 80,
                DefaultCellStyle = {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "yyyy-MM-dd"
                }
            };
            DgvBooks.Columns.Add(colReleaseDt);

            // isbn
            DataGridViewTextBoxColumn colIsbn = new DataGridViewTextBoxColumn
            {
                Name = "isbn",
                HeaderText = "ISBN",
                DataPropertyName = "isbn",
                Width = 120,
                DefaultCellStyle = {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                }
            };
            DgvBooks.Columns.Add(colIsbn);

            // price
            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn
            {
                Name = "price",
                HeaderText = "가격",
                DataPropertyName = "price",
                Width = 80,
                DefaultCellStyle = {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "#,##0"
                }
            };
            DgvBooks.Columns.Add(colPrice);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadData();

        }

        private void LoadData()
        {
            // SQL 쿼리문 작성
            string query = "SELECT book_idx, author, div_code, book_name, release_dt, isbn, price FROM books";

            // DataGridView 컨트롤 내 DataSource: DataTable 객체를 할당
            DgvBooks.DataSource = dbHelper.Select(query);
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            try
            {
                var insRes = 0;

                foreach (DataGridViewRow row in DgvBooks.Rows)
                {
                    if (row.IsNewRow) continue;
                    string bookIdx = row.Cells["book_idx"].Value?.ToString();

                    // Debug.WriteLine(bookIdx);

                    // book_idx가 비어있으면 새로운 레코드 추가
                    if (string.IsNullOrWhiteSpace(bookIdx))
                    {
                        // 신규 추가할 셀의 값을 읽어옴
                        string author = row.Cells["author"].Value?.ToString();
                        string divCode = row.Cells["div_code"].Value?.ToString();
                        string bookName = row.Cells["book_name"].Value?.ToString();
                        string releaseDt = row.Cells["release_dt"].Value?.ToString().Substring(0, 10);
                        string isbn = row.Cells["isbn"].Value?.ToString();
                        string price = row.Cells["price"].Value?.ToString();

                        // SQL Injection 해킹 가장 취약한 쿼리
                        string insSql = $"INSERT INTO bookrentalshop.books " +
                                        " (author, div_code, book_name, release_dt, isbn, price) " +
                                        $" VALUES('{author}', '{divCode}', '{bookName}', '{releaseDt}', '{isbn}', {price})";

                        insRes += dbHelper.Execute(insSql);
                    }
                }

                if (insRes > 0)
                {
                    MessageBox.Show("데이터 추가 완료!");
                }
                else
                {
                    MessageBox.Show("데이터 추가 실패!");

                }
                LoadData();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"INSERT 오류: {ex.Message}");
            }
        }

        // 수정
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var updRes = 0;

                foreach (DataGridViewRow row in DgvBooks.SelectedRows)
                {
                    string bookIdx = row.Cells["book_idx"].Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(bookIdx))
                    {

                        // MessageBox.Show(bookIdx.ToString());
                        string author = row.Cells["author"].Value?.ToString();
                        string divCode = row.Cells["div_code"].Value?.ToString();
                        string bookName = row.Cells["book_name"].Value?.ToString();
                        string releaseDt = row.Cells["release_dt"].Value?.ToString().Substring(0, 10);
                        string isbn = row.Cells["isbn"].Value?.ToString();
                        string price = row.Cells["price"].Value?.ToString();

                        string upSql = "UPDATE books " +
                                        $"SET author='{author}', " +
                                        $"div_code='{divCode}', " +
                                        $"book_name='{bookName}', " +
                                        $"release_dt='{releaseDt}', " +
                                        $"isbn='{isbn}', " +
                                        $"price={price} " +
                                        $"WHERE book_idx={bookIdx}";

                        updRes += dbHelper.Execute(upSql);
                    }
                }

                if (updRes > 0)
                {
                    MessageBox.Show($"{updRes}건의 데이터가 수정되었습니다.");
                }
                else
                {
                    MessageBox.Show("수정 실패!");
                }
                LoadData(); // 데이터 변경로드
            }
            catch (Exception ex)
            {

                MessageBox.Show($"UPDATE 오류: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DgvBooks.SelectedRows.Count == 0)
                {
                    MessageBox.Show("삭제할 행을 선택하세요.");
                    return;
                }

                DialogResult result = MessageBox.Show($"{DgvBooks.SelectedRows.Count}건 삭제하시겠습니까?",
                                            "삭제 확인",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                var delRes = 0;

                if (result == DialogResult.Yes)
                {
                    // 삭제 진행
                    foreach (DataGridViewRow row in DgvBooks.SelectedRows)
                    {
                        string bookIdx = row.Cells["book_idx"].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(bookIdx))
                        {
                            // 책번호PK가 없으면 패스
                            continue;
                        }

                        string delSql = $"DELETE FROM books WHERE book_idx = {bookIdx}";

                        delRes += dbHelper.Execute(delSql);
                    }

                    MessageBox.Show($"{delRes}건 삭제 완료!");
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"");
            }

            
            }
        }
    }