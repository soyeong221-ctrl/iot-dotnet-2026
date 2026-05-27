namespace DotNet04ControlsApp
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        // 폼로드 이벤트핸들러
        private void FrmMain_Load(object sender, EventArgs e)
        {
            var Fonts = FontFamily.Families; // OS에 설치된 폰트 리스트 가져오기
            foreach (var font in Fonts)
            {
                CboFonts.Items.Add(font.Name); // 콤보박스에 폰트 이름 추가
            }

            TxtResult.Text = "현재 글씨체 / Fonts ";
            PrgStatus.Value = 0; // 0으로 초기화
        }

        private void ChkItalic_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFontStyle();
        }

        private void ChkBold_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFontStyle();
        }

        private void CboFonts_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeFontStyle();
        }

        // 폰트 글씨체, 굵게, 이탤릭 변경 메서드
        private void ChangeFontStyle()
        {
            if (CboFonts.SelectedIndex < 0)
            {
                return; // 콤보박스 아무것도 선택 안 됨
            }

            FontStyle style = FontStyle.Regular; // 처음에는 기본 글씨체

            if (ChkBold.Checked)
            {
                style |= FontStyle.Bold; // 볼드체 변경
            }

            if (ChkItalic.Checked)
            {
                style |= FontStyle.Italic; // 이탤릭체 변경
            }

            TxtResult.Font = new Font(CboFonts.SelectedItem as string, 10, style);
        }

        // 모달버튼 클릭이벤트핸들러
        // 모달창이 닫히기 전엔 부모창을 제어할 수 없음
        private void BtnModal_Click(object sender, EventArgs e)
        {
            FrmSub frmSub = new FrmSub();
            frmSub.Text = "모달창"; // 런타임시 속성값 변경
            frmSub.BackColor = Color.OrangeRed;
            frmSub.StartPosition = FormStartPosition.CenterParent; // 부모창 중앙에 띄우기
            frmSub.ShowDialog(); // 모달로 폼 띄우기
        }

        // 모달리스버튼 클릭이벤트핸들러
        // 모달리스창과 관계 없이 부모창을 제어할 수 있음
        private void BtnModaless_Click(object sender, EventArgs e)
        {
            FrmSub frmSub = new FrmSub();
            frmSub.Text = "모달리스창"; // 런타임시 속성값 변경"
            frmSub.BackColor = Color.GreenYellow;
            // frmSub.StartPosition = FormStartPosition.CenterParent; // 적용 안 됨
            // 모달리스 창은 위치를 직접 계산
            frmSub.StartPosition = FormStartPosition.Manual;
            frmSub.Location = new Point(
                this.Location.X + (this.Width - frmSub.Width) / 2,
                this.Location.Y + (this.Height - frmSub.Height) / 2
                );

            frmSub.Show(this);  // this -> FrmMain 
        }

        private void BtnMsgbox_Click(object sender, EventArgs e)
        {
            // 기본적인 메시지박스
            // 파라미터: 메시지, 타이틀, 버튼종류, 아이콘종류
            MessageBox.Show(TxtResult.Text, "메시지 박스", MessageBoxButtons.CancelTryContinue,
                MessageBoxIcon.Error);
        }

        private void BtnDialog_Click(object sender, EventArgs e)
        {
            // DlgOpenFIle.ShowDialog(this);    // 일반 오픈
            if (DlgOpenFIle.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show($"선택한 파일은 {DlgOpenFIle.FileName}입니다.");
            }
        }

        // 트랙바 스크롤변경 이벤트핸들러
        private void TrkStatus_Scroll(object sender, EventArgs e)
        {
            PrgStatus.Value = TrkStatus.Value; // 트랙바의 값과 프로그레스바의 값을 동일하게 변경
        }

        // 트리뷰 내용을 리스트뷰에 표현 메서드
        private void TreeToList()
        {
            Lvwdummy.Items.Clear();
            foreach (TreeNode node in TvwDummy.Nodes)
            {
                TreeToList(node);
            }
        }

        private void TreeToList(TreeNode node)
        {
            Lvwdummy.Items.Add(
                new ListViewItem(
                    new String[] { node.Text, node.FullPath.Count(f => f == '\\').ToString() }
                    )
                );
            foreach (TreeNode subNode in node.Nodes)
            {
                TreeToList(subNode); // 재귀호출
            }
        }

        private void BtnAddRoot_Click(object sender, EventArgs e)
        {
            var random = new Random();

            TvwDummy.Nodes.Add(random.Next().ToString());
            TreeToList();

        }

        private void BtnAddNode_Click(object sender, EventArgs e)
        {
            if (TvwDummy.SelectedNode == null)
            {
                MessageBox.Show("노드를 선택하세요", "경고", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // 메서드 탈출
            }

            var random = new Random();
            TreeNode childNode = new TreeNode(random.Next().ToString());
            childNode.ImageIndex = 1;
            TvwDummy.SelectedNode.Nodes.Add(childNode);
            TvwDummy.ExpandAll(); // 트리뷰 전체 노드 확장

            TreeToList();
        }

        private void BtnLoadImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "이미지 열기";
            dlg.Filter = "Image Files(*.bmp;*.jpg;*.jpg)|*.bmp;*.png;*.jpg";

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {

                PicImage.Image = Bitmap.FromFile(dlg.FileName);
                PicImage.SizeMode = PictureBoxSizeMode.Zoom; // 이미지 크기에 맞게 조절
            }
        }

        // 픽쳐박스 컨트롤 클릭 이벤트핸들러
        private void PicImage_Click(object sender, EventArgs e)
        {
            if (PicImage.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                PicImage.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else {
                PicImage.SizeMode = PictureBoxSizeMode.CenterImage;
            

            }
        }
    }
}
