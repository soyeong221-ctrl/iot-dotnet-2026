using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Text.Json;
using System.Net.Http;
using System.IO;

namespace AIKnowledgeApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // HTTP로 데이터 수전송 클라이언트 객체
        private readonly HttpClient client = new HttpClient();

        public MainWindow() {
            InitializeComponent();
        }

        // ##### 파일 선택 구현
        private void BtnSelPdf_Click(object sender, RoutedEventArgs e) {
            // OpenFileDialog 추가
            OpenFileDialog dialog = new OpenFileDialog();
            // 필터 PDF만 선택
            dialog.Filter = "PDF 파일 (*.pdf)|*.pdf";
            dialog.Multiselect = false; // 파일 하나만

            if (dialog.ShowDialog() == true) {
                TxtPdfPath.Text = dialog.FileName;
            }
        }

        // ##### 서버로 데이터 전송
        private async void BtnQuestion_Click(object sender, RoutedEventArgs e) {
            string question = TxtQuestion.Text;

            if (string.IsNullOrWhiteSpace(question)) {
                MessageBox.Show("질문을 입력하세요.");
                return;
            }

            var data = new {
                question = question
            };

            string json = JsonSerializer.Serialize(data);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
                );

            // FastAPI 서버로 전송 후 결과 전달
            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8000/ask", content);

            string result = await response.Content.ReadAsStringAsync();

            TxtAnser.Text += result + Environment.NewLine;
        }

        // ##### PDF 전송 기능
        private async void BtnUpload_Click(object sender, RoutedEventArgs e) {
            // MessageBox.Show("문서 등록 준비 중");
            string filePath = TxtPdfPath.Text;

            if (string.IsNullOrWhiteSpace(filePath)) {
                MessageBox.Show("PDF 파일을 먼저 선택하세요.");
                return;
            }

            using var content = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            using var fileContent = new StreamContent(fileStream);

            content.Add(fileContent, "file", Path.GetFileName(filePath));

            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8000/upload", content);

            string result = await response.Content.ReadAsStringAsync();

            MessageBox.Show(result);
        }
    }
}