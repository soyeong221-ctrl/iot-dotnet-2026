using AIKnowledgeApp;
using DevExpress.CodeParser.Diagnostics;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace AiKnowledgeApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow {
        // HTTP로 데이터 수전송 클라이언트 객체
        private readonly HttpClient client = new HttpClient();

        public MainWindow() {
            InitializeComponent();
        }

        // ##### 파일선택 구현 
        private void BtnSelPdf_Click(object sender, RoutedEventArgs e) {
            // OpenFileDialog 추가
            OpenFileDialog dialog = new OpenFileDialog();
            // 필터 PDF만 선택
            dialog.Filter = "PDF 파일 (*.pdf)|*.pdf";
            dialog.Multiselect = false;   // 파일 하나만

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                TxtPdfPath.Text = dialog.FileName;
            }
        }

        // ##### 서버로 데이터 전송
        private async void BtnQuestion_Click(object sender, RoutedEventArgs e) {
            string question = TxtQuestion.Text;

            if (string.IsNullOrWhiteSpace(question)) {
                DXMessageBox.Show("질문을 입력하세요.");
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

            try {
                BtnQuestion.IsEnabled = false; // 버튼 비활성화
                PrgAnswer.Visibility = Visibility.Visible;

                TxtAnswer.Text = "AI가 답변을 생성하고 있습니다..." + Environment.NewLine;
                GrdSources.ItemsSource = null;  // 초기화

                // FastAPI 서버로 전송 후 결과 전달
                HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8000/ask", content);

                if (!response.IsSuccessStatusCode) {
                    TxtAnswer.Text += $"서버 오류 : {response.StatusCode}" + Environment.NewLine;  // 403, 500 오류
                    return;
                }

                string result = await response.Content.ReadAsStringAsync();

                AskResponse askResponse = JsonSerializer.Deserialize<AskResponse>(result);

                if (askResponse == null) {
                    TxtAnswer.Text += "응답을 처리할 수 없습니다." + Environment.NewLine;
                    return;
                }

                TxtAnswer.Text = askResponse.answer;
                // 참고문서 
                var sourceList = new List<SourceInfo>();

                if (askResponse.sources != null) {
                    for (int i = 0; i < askResponse.sources.Count; i++) {
                        var source = askResponse.sources[i];

                        // 동일 소스 파악해서 중복제거
                        bool isExists = sourceList.Any(x => x.filename == source.filename &&
                                                             x.page == source.page);

                        if (!isExists) {
                            sourceList.Add(source);
                        }
                    }
                }

                GrdSources.ItemsSource = sourceList;

            } catch (HttpRequestException) {
                TxtAnswer.Text = "FastAPI 서버에 연결할 수 없습니다.";
            } catch (Exception ex) {
                TxtAnswer.Text = $"오류가 발생했습니다.\n{ex.Message}";
            } finally {
                BtnQuestion.IsEnabled = true; // 버튼 재활성화
                PrgAnswer.Visibility = Visibility.Collapsed;
            }

        }

        // ##### PDF 전송 기능
        private async void BtnUpload_Click(object sender, RoutedEventArgs e) {
            // MessageBox.Show("문서등록 준비 중");
            string filePath = TxtPdfPath.Text;

            if (string.IsNullOrWhiteSpace(filePath)) {
                DXMessageBox.Show("PDF 파일을 먼저 선택하세요.");
                return;
            }

            using var content = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            using var fileContent = new StreamContent(fileStream);

            content.Add(fileContent, "file", Path.GetFileName(filePath));

            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:8000/upload", content);

            string result = await response.Content.ReadAsStringAsync();

            DXMessageBox.Show(result);
        }

        private void TxtQuestion_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {
                BtnQuestion_Click(sender, new RoutedEventArgs());
            }
        }
    }
}