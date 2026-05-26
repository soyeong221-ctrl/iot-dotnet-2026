using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNet01Test
{
    // partial 키워드는 컴파일시 나눠진 클래스 파일을 하나로 합침
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnPress_Click(object sender, EventArgs e)
        {
            LblResult.Text = "결과: 컴퓨터 터짐!";
            MessageBox.Show("버튼클릭", "테스트", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
    } }
}
