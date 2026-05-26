namespace DotNet03GuessNum
{
    public partial class FrmMain : Form
    {
        private int findNumber = 0; // 맞힐 숫자
        private int chance = 0; // 남은 기회

        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            findNumber = rand.Next(1, 31); // 1~30 사이의 난수 생성
            chance = 10; // 10번 기회
            LblDisplay.Text = "숫자가 생성되었습니다. 1~30 사이의 숫자를 맞혀보세요!";
        }
    }
}
