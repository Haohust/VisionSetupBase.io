using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Module;

namespace SetupSolution
{
    
    public class MatchingSetUpItem: ISetup
    {
        public string Name { get; set; }
        public string PathFolder { get; set; }
        public string PathImage { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Score { get; set; }
        public int MinAngle { get; set; }
        public int MaxAngle { get; set; }
        public MatchingSetUpItem(string name, string pathfolder, string pathImage, int _X, int _Y , int _Witdh, int height, int score, int minAngle, int maxAngle)
        {
            Name = name;
            PathFolder = pathfolder;
            PathImage = pathImage;
            X = _X;
            Y = _Y;
            Width = _Witdh;
            Height = height;
            Score = score;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }

    }

    public class DataSetupBase
    {
        public List<MatchingSetUpItem> MatchingSetUp = new List<MatchingSetUpItem>();
    }
}
