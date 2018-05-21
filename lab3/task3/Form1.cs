using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace task3
{
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            greyscaleToolStripMenuItem1.Enabled = false;
            histogramToolStripMenuItem.Enabled = false;
            adaptiveToolStripMenuItem.Enabled = false;

        }

        private bool done;
        private Bitmap original;
        private Bitmap bmp, bmp1;
        private int BMPheight;
        private void loadImageToolStripMenuItem1_Click(object sender, EventArgs e) // Load Image
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpg)|*.jpg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff";

                if (dlg.ShowDialog() == DialogResult.OK)
                {

                    pictureBox1.Image = new Bitmap(dlg.FileName);
                    original = pictureBox1.Image.Clone() as Bitmap;

                    this.MaximumSize = pictureBox1.Image.Size;
                    GraphicsUnit x = GraphicsUnit.Pixel;
                    var b = pictureBox1.Image.GetBounds(ref x);

                    BMPheight = (int)b.Height;
                    pictureBox1.SetBounds(pictureBox1.Bounds.X, pictureBox1.Bounds.Y, (int)b.Width, (int)b.Height);

                    this.Height = (int)b.Height + progressBar1.Height;
                    this.Width = (int)b.Width;

                }

            }

            bmp = new Bitmap(original);
            bmp1 = new Bitmap(original);
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            adaptiveToolStripMenuItem.Enabled = true;
            resetToolStripMenuItem1.Enabled = false;
            greyscaleToolStripMenuItem1.Enabled = true;
        }

        private void resetToolStripMenuItem1_Click(object sender, EventArgs e) // reset
        {
            if (original == null)
                return;

            if (backgroundWorker1.CancellationPending)
                return;

            backgroundWorker1.CancelAsync();
            progressBar1.Value = 0;

            resetToolStripMenuItem1.Enabled = false;
            pictureBox1.Image = original;
            greyscaleToolStripMenuItem1.Enabled = true;
        }

        private void greyscaleToolStripMenuItem1_Click(object sender, EventArgs e) // greyscale sync
        {

            greyscaleToolStripMenuItem1.Enabled = false;
            resetToolStripMenuItem1.Enabled = true;

            backgroundWorker1.RunWorkerAsync();

            while (this.backgroundWorker1.IsBusy)
            {
                Application.DoEvents();
            }

            if(!done)
            {
                progressBar1.Value = 0;

                resetToolStripMenuItem1.Enabled = false;
                pictureBox1.Image = original;
                greyscaleToolStripMenuItem1.Enabled = true;

                bmp = new Bitmap(original);
                bmp1 = new Bitmap(original);
            }
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;
            done = false;

            // Start the time-consuming operation.
            int x, y;

            // Loop through the images pixels to reset color.
            for (y = 0; y < bmp.Height; y++)
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                for (x = 0; x < bmp.Width; x++)
                {
                    Color pixelColor = bmp.GetPixel(x, y);
                    int red = (int)(pixelColor.R * 0.3) + (int)(pixelColor.G * 0.59) + (int)(pixelColor.B * 0.11);
                    int green = (int)(pixelColor.R * 0.3) + (int)(pixelColor.G * 0.59) + (int)(pixelColor.B * 0.11);
                    int blue = (int)(pixelColor.R * 0.3) + (int)(pixelColor.G * 0.59) + (int)(pixelColor.B * 0.11);
                    Color newColor = Color.FromArgb(red, green, blue);
                    bmp.SetPixel(x, y, newColor); // Now greyscale
                }

                bmp1 = new Bitmap(bmp);
                int progress = (100 * (y + 1) / bmp.Height);
                bw.ReportProgress(progress);
            }

            done = true;

        }

        private void backgroundWorker1_DoWorkAdaptive(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            BackgroundWorker bw = sender as BackgroundWorker;
            done = false;

            // Start the time-consuming operation.
            int x, y;

            // Loop through the images pixels to reset color.
            int pixelW = bmp.Width;
            int pixelH = bmp.Height;


            while(pixelW > 0 && pixelH > 0)
            {

                for (y = 0; y < bmp.Height; y++)
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    for (x = 0; x < bmp.Width; x++)
                    {
                        //Color pixelColor = original.GetPixel( (x/pixelW) * x%(bmp.Width/pixelW), y/pixelH * y%(bmp.Height/pixelH));
                        Color pixelColor = original.GetPixel(x / pixelW, y / pixelH );
                        int red = (int)(pixelColor.R * 0.3) + (int)(pixelColor.G * 0.59) + (int)(pixelColor.B * 0.11);
                        int green = (int)(pixelColor.R * 0.3) + (int)(pixelColor.G * 0.59) + (int)(pixelColor.B * 0.11);
                        int blue = (int)(pixelColor.R * 0.3) + (int)(pixelColor.G * 0.59) + (int)(pixelColor.B * 0.11);
                        Color newColor = Color.FromArgb(red, green, blue);
                        bmp.SetPixel(x, y, newColor); // Now greyscale
                    }
                    
                }

                pixelH /= 2;
                pixelW /= 2;

                bmp1 = new Bitmap(bmp);
                int progress = (100 * (y + 1) / bmp.Height);
                bw.ReportProgress(progress);

                if (pixelH==0 || pixelW == 0)
                {
                    pixelH = pixelW = 1;
                }
            }
            done = true;

        }
        private void adaptiveToolStripMenuItem_Click(object sender, EventArgs e) // greyscale async
        {
            this.backgroundWorker1.DoWork -= new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWorkAdaptive);

            greyscaleToolStripMenuItem1.Enabled = false;
            resetToolStripMenuItem1.Enabled = true;

            backgroundWorker1.RunWorkerAsync();

            while (this.backgroundWorker1.IsBusy)
            {
                Application.DoEvents();
            }

            if (!done)
            {
                progressBar1.Value = 0;

                resetToolStripMenuItem1.Enabled = false;
                pictureBox1.Image = original;
                greyscaleToolStripMenuItem1.Enabled = true;

                bmp = new Bitmap(original);
                bmp1 = new Bitmap(original);
            }

        }

        // funkcja wywoływana po odpaleniu metody ReportProgress
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            this.pictureBox1.Image = bmp1;
        }
    }
}
