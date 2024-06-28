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
        private Mat pattern;
        private Mat result;
        private PatternManager patternManager;
        private Mat loadedImage;
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

        private OpenCvSharp.Point? FindPattern(Mat frame)
        {
            // [추가된 부분] 이미지가 다채널인 경우 그레이스케일로 변환
            if (frame.Channels() > 1)
            {
                Cv2.CvtColor(frame, frame, ColorConversionCodes.BGR2GRAY);
            }

            if (pattern.Channels() > 1)
            {
                Cv2.CvtColor(pattern, pattern, ColorConversionCodes.BGR2GRAY);
            }

            // [수정된 부분] 이미지 형식 일치시키기
            frame.ConvertTo(frame, MatType.CV_8U);
            pattern.ConvertTo(pattern, MatType.CV_8U);

            result = new Mat();
            Cv2.MatchTemplate(frame, pattern, result, TemplateMatchModes.CCoeffNormed);

            double minVal, maxVal;
            OpenCvSharp.Point minLoc, maxLoc;
            Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);

            if (maxVal > 0.8) // Threshold
            {
                return maxLoc;
            }

            return null;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            Mat frame = new Mat();
            capture.Read(frame);

            if (frame.Empty())
            {
                MessageBox.Show("No frame captured.");
                return;
            }

            pictureBox1.Image = BitmapConverter.ToBitmap(frame);

            if (pattern != null && !pattern.Empty())
            {
                var location = FindPattern(frame);
                if (location.HasValue)
                {
                    Cv2.Rectangle(frame, new Rect(location.Value, pattern.Size()), Scalar.Red, 2);
                }
            }

            pictureBox1.Image = BitmapConverter.ToBitmap(frame);
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
            capture?.Release();
        }

        private void loadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                loadedImage = new Mat(openFileDialog.FileName, ImreadModes.Color);
                if (loadedImage != null && !loadedImage.Empty())
                {
                    pictureBox2.Image = BitmapConverter.ToBitmap(loadedImage);
                    MessageBox.Show("Image loaded.");
                }
                else
                {
                    MessageBox.Show("Failed to load image.");
                }
            }
        }

        private void findPatternInImageButton_Click(object sender, EventArgs e)
        {
            if (loadedImage == null || loadedImage.Empty())
            {
                MessageBox.Show("No image loaded.");
                return;
            }

            var location = patternManager.FindPattern(loadedImage);
            if (location != null)
            {
                MessageBox.Show($"Pattern found in image at: {location}");
            }
            else
            {
                MessageBox.Show("Pattern not found in image.");
            }
        }

    }
}
