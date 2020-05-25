using System;
using System.Windows.Forms;

namespace Windows10WallPaper.usercontrol
{
    /// <summary>
    /// 壁紙設定コントロール
    /// </summary>
    public partial class WallPaperControll : UserControl
    {
        /// <summary>
        /// オープンファイルダイアログ
        /// </summary>
        private OpenFileDialog OpenFileDialog { get;  set; }

        /// <summary>
        /// ピクチャーボックス
        /// </summary>
        private PictureBox PictureBox { get; set; }

        /// <summary>
        /// Screen
        /// </summary>
        public Screen Screen { get; set; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        public String FilePath
        {
            get
            {
                return this.textBox1.Text;
            }
        }

        /// <summary>
        /// 壁紙設定用コントローラ
        /// </summary>
        /// <param name="openFileDialog"></param>
        /// <param name="pictureBox"></param>
        /// <param name="screen"></param>
        public WallPaperControll(OpenFileDialog openFileDialog, PictureBox pictureBox, Screen screen)
        {
            InitializeComponent();
            this.OpenFileDialog = openFileDialog;
            this.PictureBox = pictureBox;
            this.PictureBox.Click += button1_Click;
            this.Screen = screen;
            this.richTextBox1.Text = this.Screen.ToString();
        }

        /// <summary>
        /// 壁紙選択ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if(DialogResult.OK.Equals(this.OpenFileDialog.ShowDialog()))
            {
                this.PictureBox.Load(this.OpenFileDialog.FileName);
                this.textBox1.Text = this.OpenFileDialog.FileName;
            }
        }
    }
}
