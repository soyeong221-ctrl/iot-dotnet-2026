using MaterialSkin.Controls;
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
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            // SQL 쿼리문 작성
            string query = "SELECT book_idx, author, div_code, book_name, release_dt, isbn, price FROM books";

            // DataGridView 컨트롤 내 DataSource: DataTable 객체를 할당
            DgvBooks.DataSource = dbHelper.Select(query);
        }
    }
}
