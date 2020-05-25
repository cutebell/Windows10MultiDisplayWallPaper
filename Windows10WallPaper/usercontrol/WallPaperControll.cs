using System;
using System.Windows.Forms;

namespace Windows10WallPaper.usercontrol
{
    public partial class WallPaperControll : UserControl
    {
        private OpenFileDialog OpenFileDialog { get;  set; }
        private PictureBox PictureBox { get; set; }
        public Screen Screen { get; set; }
        public String FilePath
        {
            get
            {
                return this.textBox1.Text;
            }
        }

        public WallPaperControll(OpenFileDialog openFileDialog, PictureBox pictureBox, Screen screen)
        {
            InitializeComponent();
            this.OpenFileDialog = openFileDialog;
            this.PictureBox = pictureBox;
            this.PictureBox.Click += button1_Click;
            this.Screen = screen;
            this.richTextBox1.Text = this.Screen.ToString();
        }

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
