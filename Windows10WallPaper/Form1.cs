using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Windows10WallPaper.model;
using Windows10WallPaper.service;
using Windows10WallPaper.usercontrol;

namespace Windows10WallPaper
{
    /// <summary>
    /// メインフォーム
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ロード時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.buttonReload_Click(sender, e);
        }

        private void buttonSetWallPaper_Click(object sender, EventArgs e)
        {
            try
            {
                List<ScreenModel> screenModelList = new List<ScreenModel>();
                foreach (Control conrtol in this.panel1.Controls)
                {
                    WallPaperControll wallPaperControll = (WallPaperControll)conrtol;
                    ScreenModel screenModel = new ScreenModel();
                    screenModel.Screen = wallPaperControll.Screen;
                    screenModel.bitmap = new Bitmap(wallPaperControll.FilePath);

                    screenModelList.Add(screenModel);
                }

                if (DialogResult.OK.Equals(this.openFileDialog1.ShowDialog()))
                {
                    SetWallPaper setWallPAper = new SetWallPaper();
                    setWallPAper.process(screenModelList, this.openFileDialog1.FileName);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.StackTrace, exception.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            this.panel1.Controls.Clear();
            List<PictureBox> pictureBoxes = new List<PictureBox>();
            foreach (Control control in this.splitContainer1.Panel2.Controls)
            {
                pictureBoxes.Add((PictureBox)control);
            }
            this.splitContainer1.Panel2.Controls.Clear();

            foreach (PictureBox pictureBox in pictureBoxes)
            {
                pictureBox.Image.Dispose();
            }

            this.backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// バックグラウンドワーカー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // 全ディスプレイの左上の座標を取得する
            int minLeft = 0;
            int minTop = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (minLeft > screen.Bounds.Left)
                {
                    minLeft = screen.Bounds.Left;
                }

                if (minTop > screen.Bounds.Top)
                {
                    minTop = screen.Bounds.Top;
                }
            }

            // ディスプレイのインデックス
            int displayIndex = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                // ディスプレイのスクリーンショットを取得
                Bitmap bmp = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(new Point(screen.Bounds.Left, screen.Bounds.Top), new Point(0, 0), bmp.Size);
                }

                // ピクチャーボックス作成
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = bmp;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                // 1/16サイズで表示
                pictureBox.Size = new Size(screen.Bounds.Width / 16, screen.Bounds.Height / 16);

                this.backgroundWorker1.ReportProgress(1, pictureBox);
                

                // 場所を移動
                pictureBox.Location = new Point(minLeft / 16 * -1 + screen.Bounds.Left / 16, minTop / 16 * -1 + screen.Bounds.Top / 16);


                WallPaperControll wallPaperControll = new WallPaperControll(this.openFileDialog1, pictureBox, screen);
                this.backgroundWorker1.ReportProgress(2, wallPaperControll);
                wallPaperControll.Location = new Point(0, wallPaperControll.Height * displayIndex);
                displayIndex++;
            }
        }

        /// <summary>
        /// 進捗変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                case 1:
                    // 画面右側に現在の画像を表示
                    this.splitContainer1.Panel2.Controls.Add((PictureBox)e.UserState);
                    break;

                case 2:
                    this.panel1.Controls.Add((WallPaperControll)e.UserState);
                    break;
            }
            
        }


    }
}
