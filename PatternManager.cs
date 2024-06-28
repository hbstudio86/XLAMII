using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace XLAMII
{
    public class PatternManager
    {
        private List<Mat> patterns;

        public PatternManager()
        {
            patterns = new List<Mat>();
        }

        public void AddPattern(Mat pattern) 
        { 
            patterns.Add(pattern);
        }

        public OpenCvSharp.Point? FindPattern(Mat frame)
        {
            foreach (var pattern in patterns)
            {
                using (var result = new Mat())
                {
                    Cv2.MatchTemplate(frame, pattern, result, TemplateMatchModes.CCoeffNormed);
                    Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

                    if (maxVal >= 0.8) // 80% 일치하면 패턴이 있다고 판단
                    {
                        return maxLoc;
                    }
                }
            }

            return null;
        }
    }

}
