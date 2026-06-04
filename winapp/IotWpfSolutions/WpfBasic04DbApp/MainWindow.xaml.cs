using MahApps.Metro.Controls;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfBasic04DbApp;

namespace WpfBasic03UIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        DatabaseHelper databaseHelper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            databaseHelper = new DatabaseHelper();
            LoadData();
        }

        private void LoadData()
        {
            string query = "SELECT b.book_idx, b.author, b.div_code, d.div_name, b.book_name, b.release_dt, b.isbn, b.price" +
                           " FROM books AS b JOIN division AS d " +
                           "ON b.div_code = d.div_code ORDER BY b.book_idx";
            DataTable dt = databaseHelper.Select(query);
            GrdBooks.ItemsSource = dt.DefaultView;
        }
    }
}