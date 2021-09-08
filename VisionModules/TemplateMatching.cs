using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST4I.Vision.UI;
using System.Drawing;
using NationalInstruments.Vision;
using NationalInstruments.Vision.Analysis;
using System.Threading;

using Emgu.CV;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ST4I.Vision.Core;

namespace Vision.Module
{
    public class TemplateResultItem  
    {
        public bool IsStatus { get; set; }
        public Point[] Vertices { get; set; }
        public double Rotation { get; set; }
        public double Score { get; set; }
        public TemplateResultItem()
        {
            Vertices = new Point[4];
            IsStatus = false;
        }
    }
    public class TemplateResult : IResult
    {
        public bool IsStatus { get; set; }
        public List<TemplateResultItem> MatchingResult { get; set; }
        public TemplateResult()
        {
            MatchingResult = new List<TemplateResultItem>();
            IsStatus = true;
        }
    }
    public class TemplateMatching : IVisonModel
    {
       
        public NationalInstruments.Vision.Roi SearchingROI { get; set; }
        public NationalInstruments.Vision.Roi RoiTemplate { get; set; }
        public int NumberOfPattern { get; set; }
        public double Score { get; set; }
        public double MinAngle { get; set; }
        public double MaxAngle { get; set; }
        private VisionImage Template { get; set; }
        //MatchPatternOptions matchPattern { get; set; }
        public TemplateMatching()
        {
            SearchingROI = new NationalInstruments.Vision.Roi();
            RoiTemplate = new NationalInstruments.Vision.Roi();
            Template = new VisionImage(ImageType.U8);
            Score = 700;
            MinAngle = 0;
            MaxAngle = 360;
            NumberOfPattern = 1;
        }

        public void LearnMatchingObject(VisionImage template)
        {
            CommonAlgorithms.Copy(template, Template);
            Algorithms.LearnPattern(Template);
        }

        public IResult Inspect(VisionImage img)
        {
            TemplateResult result = new TemplateResult();
            // Match
            MatchPatternOptions options = new MatchPatternOptions(MatchMode.RotationInvariant, NumberOfPattern);
            options.MinimumMatchScore = Score;
            options.MinimumContrast = 3;
            options.SubpixelAccuracy = true;
            // Chú ý rằng hàm MatchPattern phải sử dụng ảnh Template trùng với ảnh Template được sử dụng trong Algorithms.LearnPattern(Template)
            Collection<PatternMatch> matches = Algorithms.MatchPattern(img, Template, options, SearchingROI);
            //Collection<PatternMatchReport> matchesTemp = Algorithms.MatchPattern3(img, obj);
            foreach (PatternMatch match in matches)
            {
                TemplateResultItem templateResult = new TemplateResultItem();
                var res = new PolygonContour(match.Corners);
                List<Point> points = new List<Point>();
                foreach (var item in res.Points)
                {
                    points.Add(new Point((int)(item.X + 0.5), (int)(item.Y + 0.5)));
                }
                templateResult.Vertices = points.ToArray();
                templateResult.Rotation = match.Rotation;
                templateResult.Score = match.Score;
                if ((templateResult.Rotation>= MinAngle)&& (templateResult.Rotation <= MaxAngle))
                {
                    templateResult.IsStatus = true;
                }
                else
                {
                    templateResult.IsStatus = false;
                }
                result.MatchingResult.Add(templateResult);
            }
            if (NumberOfPattern <= result.MatchingResult.Count)
            {
                foreach( var item in result.MatchingResult)
                {
                    if(item.IsStatus)
                    {
                        result.IsStatus = true;
                    }
                    else
                    {
                        result.IsStatus = false;
                        
                    }
                }                
            }
            else
            {
                result.IsStatus = false;
            }
            return result;
        }
    }
}
