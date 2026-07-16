using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Data;
using WpfMvvm02.Helpers;
using WpfMvvm02.Models;

namespace WpfMvvm02.ViewModels {
    public partial class DivisionViewModel : ObservableObject {

        private readonly DatabaseHelper _helper;

        private readonly IDialogCoordinator _coordinator;

        public ObservableCollection<Division> Divisions { get; set; }

        // 데이터그리드에서 선택된 값
        [ObservableProperty]
        private Division selectedDivision;

        public DivisionViewModel(IDialogCoordinator coordinator) {
            _coordinator = coordinator;
            _helper = new DatabaseHelper();

            LoadDataFromDb();
        }

        private void LoadDataFromDb() {
            try {
                Divisions = new ObservableCollection<Division>();
                string query = "SELECT div_code, div_name FROM division";
                var result = _helper.Select(query).AsEnumerable().ToList();

                foreach (DataRow row in result) {
                    Division division = new Division {
                        DivCode = row["div_code"].ToString(),
                        DivName = row["div_name"].ToString()
                    };

                    Divisions.Add(division);
                }

            } catch (Exception ex) {
                _coordinator.ShowMessageAsync(this, "조회오류", "DB조회 오류 발생: {ex.Message}");
            }
        }
    }
}