using WpfBusanFoodApp.Helpers;
using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using WpfBusanFoodApp.Models;
using WpfBusanFoodApp.Services;

namespace WpfBusanFoodApp
{

    public partial class MainWindow : MetroWindow
    {
        // 부산맛집 API를 호출하는 서비스 객체.
        private readonly FoodApiService foodApiService;

        public MainWindow()
        {
            InitializeComponent();

            // API 서비스 객체 생성.
            foodApiService = new FoodApiService();

            // 앱이 시작되었음을 로그에 기록.
            Common.Logger.Info("부산 맛집정보 앱 시작");


        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 기본 1페이지 데이터 불러오기.
            await SearchFoodAsync();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            await SearchFoodAsync();
        }

        // 검색 기능.
        private async Task SearchFoodAsync()
        {
            try
            {
                // 버튼 비활성화: 사용자가 여러 번 누르는 것 방지.
                BtnSearch.IsEnabled = false;

                // NumericUpDown 기본값 지정.
                int pageNo = Convert.ToInt32(NumPageNo.Value ?? 1);
                int numOfRows = Convert.ToInt32(NumOfRows.Value ?? 10);

                SbiStatus.Text = "데이터를 불러오는 중...";

                // 어떤 조건으로 검색했는지 로그에 기록.
                Common.Logger.Info($"맛집정보 조회 시작 / pageNo={pageNo}, numOfRows={numOfRows}");

                // API 호출 - 맛집 목록
                var foods = await foodApiService.GetFoodsAsync(pageNo, numOfRows);

                // 화면 표시용 순번 계산
                for (int i = 0; i < foods.Count; i++)
                {
                    foods[i].DisplayNo = ((pageNo - 1) * numOfRows) + i + 1;
                }

                // DataGrid에 맛집 목록 연결. -> DgrFood에 데이터가 표시.
                DgrFood.ItemsSource = foods;

                // 상태표시줄 - 결과
                SbiStatus.Text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} / {foods.Count}건 로드 완료";

                // 조회 성공 로그
                Common.Logger.Info($"맛집정보 조회 완료 / {foods.Count}건");
            }
            catch (Exception ex)
            {
                SbiStatus.Text = "데이터 로드 실패";

                // 오류 내용 로그에 기록.
                Common.Logger.Error(ex, "맛집정보 조회 중 오류 발생");

                MessageBox.Show(
                    ex.Message,
                    "오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                // 성공하든 실패하든 검색 버튼은 다시 활성화.
                BtnSearch.IsEnabled = true;
            }
        }

        private void DgrFood_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 선택된 데이터가 없으면 아무 작업도 하지 않습니다.
            if (DgrFood.SelectedItem == null)
            {
                return;
            }

            FoodItem selectedFood = DgrFood.SelectedItem as FoodItem;

            if (selectedFood == null)
            {
                return;
            }

            FoodDetailWindow detailWindow = new FoodDetailWindow(selectedFood);

            detailWindow.Owner = this;

            // 상세창을 닫기 전까지 메인창 조작을 막습니다.
            detailWindow.ShowDialog();
        }

        // 현재 DataGrid에 표시된 맛집 중 하나를 랜덤으로 선택해서 상세창을 엽니다.
        private void BtnRandom_Click(object sender, RoutedEventArgs e)
        {
            if (DgrFood.Items.Count == 0)
            {
                MessageBox.Show(
                    "추천할 맛집 목록이 없습니다.\n먼저 검색을 실행해주세요.",
                    "알림",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            Random random = new Random();
            int index = random.Next(DgrFood.Items.Count);

            FoodItem? selectedFood = DgrFood.Items[index] as FoodItem;

            if (selectedFood == null)
            {
                return;
            }

            // 추천된 맛집을 표에서도 선택 표시.
            DgrFood.SelectedItem = selectedFood;
            DgrFood.ScrollIntoView(selectedFood);

            Common.Logger.Info($"랜덤 추천 맛집 선택 / {selectedFood.MainTitle}");

            FoodDetailWindow detailWindow = new FoodDetailWindow(selectedFood);
            detailWindow.Owner = this;
            detailWindow.ShowDialog();
        }
    }
}

        