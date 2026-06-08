using System;
using System.Collections.Generic;
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

namespace WpfBasic02Navi
{
    /// <summary>
    /// Sub02Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Sub02Page : Page
    {
        public List<Employee> Employees { get; set; }  // employee 컬렉션 속성

        public Employee SelectedEmployee { get; set; }

        public List<string> Departments { get; set; }

        public string SelectedDepartment { get; set; }

        public Sub02Page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Departments = new List<string>
            {
                "개발팀",
                "인사팀",
                "영업팀",
                "디자인팀",
                "경영팀"
            };
            // 초기화. DB에서 불러오는 것과 유사
            Employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "김철수",
                    Department = "개발팀",
                    Salary = 4200,
                    HireDate = new DateTime(2021, 3, 15),
                    IsActive = true
                },

                new Employee
                {
                    Id = 2,
                    Name = "이영희",
                    Department = "인사팀",
                    Salary = 5100,
                    HireDate = new DateTime(2019, 7, 1),
                    IsActive = true
                },

                new Employee
                {
                    Id = 3,
                    Name = "박민수",
                    Department = "영업팀",
                    Salary = 3300,
                    HireDate = new DateTime(2023, 1, 10),
                    IsActive = false
                },

                new Employee
                {
                    Id = 4,
                    Name = "최지은",
                    Department = "디자인팀",
                    Salary = 6200,
                    HireDate = new DateTime(2018, 11, 20),
                    IsActive = true
                },

                new Employee
                {
                    Id = 5,
                    Name = "홍길동",
                    Department = "경영팀",
                    Salary = 9000,
                    HireDate = new DateTime(2010, 1, 1),
                    IsActive = true
                }
            };

            // 데이터그리드 할당
            this.DataContext = this;  // 코드비하인드의 모든 바인딩 객체를 화면상에서 사용하겠다(바인딩 컨텍스트 설정)
            
        }
    }
}