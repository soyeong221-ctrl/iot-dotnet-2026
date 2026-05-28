namespace DotNet05AsyncApp
{
    public partial class Frmmain : Form
    {
        public Frmmain()
        {
            InitializeComponent();
        }

        private void BtnSource_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "ALL Files(*.*)|*.*";

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                TxtSource.Text = dlg.FileName;
            }
        }

        private void BtnTarget_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "ALL Files(*.*)|*.*";

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                TxtTarget.Text = dlg.FileName;
            }
        }

        private void BtnSyncCopy_Click(object sender, EventArgs e)
        {
            // 동기화 복사
            long result = CopySync(TxtSource.Text, TxtTarget.Text);
        }


        // 이벤트핸들러 메서드 자체가 비동기메서드화 되어야 함(async)
        private async void BtnASyncCopy_Click(object sender, EventArgs e)
        {
            // 비동기화 복사
            long result = await CopyAsync(TxtSource.Text, TxtTarget.Text);
        }

        private long CopySync(string srcFile, string destFile)
        {
            // 버튼 비활성화
            BtnSource.Enabled = BtnTarget.Enabled = BtnSyncCopy.Enabled = BtnASyncCopy.Enabled = false;
            long totalCopied = 0;

            // 읽어오는쪽
            using (FileStream fromStream = new FileStream(srcFile, FileMode.Open))
            {
                // 새로 쓰는(만드는)쪽
                using (FileStream toStream = new FileStream(destFile, FileMode.Create)) { 
                
                    // 파일 복사할 때 항상 버퍼. byte[]배열로 버퍼 생성
                    byte[] buffer = new byte[1024];  // 1Kbyte로 버퍼 (1MYB = 1024KB = 1024*1024byte)
                    int nRead = 0;  // 1K씩 읽어오는 횟수

                    while ((nRead = fromStream.Read(buffer, 0, buffer.Length)) != 0) {
                        toStream.Write(buffer, 0, nRead);   // 계속 쓴다
                        totalCopied += nRead;   // 전체 복사횟수

                        // 진행사항 상태바 표시
                        PrgProcess.Value = (int)((totalCopied / fromStream.Length) * 100);
                    }
                }
            }

            BtnSource.Enabled = BtnTarget.Enabled = BtnSyncCopy.Enabled = BtnASyncCopy.Enabled = true;
            return totalCopied;
        }

        // Task: 비동기작업, 백그라운드작업 작업객체
        // <long>: 작업 후에 리턴할 값
        private async Task<long> CopyAsync(string srcFile, string destFile)
        {
            // 버튼 비활성화
            BtnSource.Enabled = BtnTarget.Enabled = BtnSyncCopy.Enabled = BtnASyncCopy.Enabled = false;
            long totalCopied = 0;

            // 읽어오는쪽
            using (FileStream fromStream = new FileStream(srcFile, FileMode.Open))
            {
                // 새로 쓰는(만드는)쪽
                using (FileStream toStream = new FileStream(destFile, FileMode.Create))
                {

                    // 파일 복사할 때 항상 버퍼. byte[]배열로 버퍼 생성
                    byte[] buffer = new byte[1024 * 1024];  // 1Kbyte로 버퍼 (1MYB = 1024KB = 1024*1024byte)
                    int nRead = 0;  // 1K씩 읽어오는 횟수

                    while ((nRead = await fromStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        await toStream.WriteAsync(buffer, 0, nRead);   // 계속 쓴다
                        totalCopied += nRead;   // 전체 복사횟수

                        // 진행사항 상태바 표시
                        PrgProcess.Value = (int)((totalCopied / fromStream.Length) * 100);
                    }
                }
            }

            BtnSource.Enabled = BtnTarget.Enabled = BtnSyncCopy.Enabled = BtnASyncCopy.Enabled = true;
            return totalCopied;
        }
    }
}
