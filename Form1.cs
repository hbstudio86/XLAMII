using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;



namespace XLAMII
{
    public partial class Form1 : Form
    {

        private VideoCapture capture;
        private Mat frame;
        private PatternManager patternManager;
        //private bool camera_flag = false;

        public Form1()
        {
            InitializeComponent();
            patternManager = new PatternManager();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            capture = new VideoCapture(0);
            if (!capture.IsOpened())
            {
                MessageBox.Show("Failed to open camera.");
                return;
            }

            //camera_flag = true;
            frame = new Mat();
            Timer timer = new Timer();
            timer.Interval = 33; // 약 30 FPS
            timer.Tick += new EventHandler(ProcessFrame);
            timer.Start();
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            capture.Read(frame);
            if (!frame.Empty())
            {
                pictureBox1.Image = BitmapConverter.ToBitmap(frame);

            }
        }

        private void addPatternButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var pattern = new Mat(openFileDialog.FileName, ImreadModes.Grayscale);
                patternManager.AddPattern(pattern);
                MessageBox.Show("Pattern added.");
            }
        }

        private void findPatternButton_Click(object sender, EventArgs e)
        {
            if (frame == null || frame.Empty())
            {
                MessageBox.Show("No frame captured.");
                return;
            }

            var location = patternManager.FindPattern(frame);
            if (location != null)
            {
                MessageBox.Show($"Pattern found at: {location}");
            }
            else
            {
                MessageBox.Show("Pattern not found.");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            capture.Release();
        }

    }
}
