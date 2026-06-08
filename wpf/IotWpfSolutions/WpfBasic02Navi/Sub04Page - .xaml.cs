using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Sub04Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Sub04Page : Page
    {
        public List<Employee> employees;  // employee 컬렉션 선언

        public Employee SelectedEmployee { get; set; }

        public Sub04Page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 초기화
            Car car = new Car
            {
                Speed = 10.0,
                Color = Colors.Black
            };

            
            TxtSpeed.Text = car.Speed.ToString();
            ScbColor.Color = car.Color;
            TxtColor.Text = car.Color.ToString();
        }

        private void TxtColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { 
            
                Color color = (Color)ColorConverter.ConvertFromString(TxtColor.Text);

                ScbColor.Color = color;
            }
            catch (Exception)
            {
                Debug.WriteLine("색상 변환 실패");
            }
        }

        private void TxtSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}