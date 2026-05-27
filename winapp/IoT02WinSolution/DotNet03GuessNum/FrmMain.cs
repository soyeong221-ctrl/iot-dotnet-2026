namespace DotNet03GuessNum
{
    public partial class TxtNum : Form
    {
        private int findNumber = 0; // 변수만 소문자로 시작
        private int chance = 0;

        // 생성자는 되도록 손대지 않음
        public TxtNum()
        {
            InitializeComponent();
        }

        /// <summary>
        /// BtnStart 클릭 이벤트 핸들러 메서드
        /// </summary>
        /// <param name="sender"></param>   // 이벤트를 발생시킨 객체
        /// <param name="e"></param>    // 버튼 자체에 필요한 매개변수 속성
        private void BtnStart_Click(object sender, EventArgs e)
        {
            var rand = new Random();
            findNumber = rand.Next(1, 30 + 1); // 1 ~ 30 사이의 난수 
            chance = 10; // 기회는 10번
            LblDisplay.Text = "숫자를 입력하세요.";

            MessageBox.Show("게임을 시작합니다.", "게임 시작", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 텍스트박스에 입력한 숫자가 정답인지 체크하는 기능
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCheck_Click(object sender, EventArgs e)
        {
            int inputNum = int.Parse(TxtNum.Text);

            if (inputNum == findNumber){

                LblDisplay.Text = "정답입니다!";
                MessageBox.Show("축하합니다!", "게임 종료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                chance--;
                string strVal;
                if (inputNum > findNumber) {
                    strVal = "큼";
                }
                else {
                    strVal = "작음";
                }

                LblDisplay.Text = $"틀렸습니다! 남은 기회: {chance}. 찾는 수보다 {strVal}"; 
            }

            if (chance <= 0) {

                LblDisplay.Text = "실패했습니다!";
                MessageBox.Show("실패했습니다!", "게임 종료", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
