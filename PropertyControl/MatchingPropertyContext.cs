using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using ST4I.Vision.Core;
using ST4I.Vision.UI;
using NationalInstruments.Vision;
using NationalInstruments.Vision.Analysis;
using ST4I.Vision.Utils;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Drawing;
using Emgu.CV;
using Vision.Module;
using System.IO;
using System.Windows;

namespace SetupSolution
{
    public class MatchingPropertyContext : IPropertyContext, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private string OrangeColor = "#FFA500";
        private string BlueColor = "#0000FF";
        private string isStatus { get; set; }
        public string IsStatus
        {
            get { return isStatus; }
            set
            {
                isStatus = value;
                OnPropertyChanged();
            }
        }
        private string colorstatus { get; set; }
        public string ColorStatus
        {
            get { return colorstatus; }
            set
            {
                colorstatus = value;
                OnPropertyChanged();
            }
        }
        public int NumberOfMatching
        {
            get { return matchingHandler.NumberOfPattern; }
            set
            {
                matchingHandler.NumberOfPattern = value;
                OnPropertyChanged();
            }
        }
        
        private TemplateMatching matchingHandler = new TemplateMatching();
        public string NameSolution { get; set; }
        public RectangleRoi SearchROI { get; set; }
        public RectangleRoi TemplateROI { get; set; }
        public VisionImage VSImage { get; set; }
        private BitmapSource imageSoure { get; set; }
        public ImageBoxContext ImageBox { get; set; }
        public RelayCommand AddSelectDraw { get; set; }
        public RelayCommand EnableDrawROI { get; set; }
        public RelayCommand EnableDrawTemp { get; set; }
        
        public RelayCommand Test { get; set; }
        
        public MatchingSetUpItem matchingItem { get; set; }
        private bool isDrawSearching = false;
        private bool isDrawTemplate = true;
        public DataSetupBase DataSetup { get; set; }
        private string nameModule { get; set; }
        public string NameModule
        {
            get { return nameModule; }
            set
            {
                nameModule = value;
                OnPropertyChanged();
            }
        }

        public bool IsDrawSearching
        {
            get
            {
                return isDrawSearching;
            }
            set
            {
                isDrawSearching = value;
                ImageBox.MaxNumRoi = 1;
                if (value)
                {
                    ImageBox.AddViewLayerItems(DisplayOverLayResult.ViewRegionSetup(TemplateROI, BlueColor, "Teamplate").ToArray());
                    ImageBox.SetListROI(new IRoi[] { SearchROI });
                }
                OnPropertyChanged();
            }
        }

        public bool IsDrawTemplate
        {
            get
            {
                return isDrawTemplate;
            }
            set
            {
                isDrawTemplate = value;
                ImageBox.MaxNumRoi = 1;
                if (value)
                {
                    ImageBox.AddViewLayerItems(DisplayOverLayResult.ViewRegionSetup(SearchROI, OrangeColor, "Searching Area").ToArray());
                    ImageBox.SetListROI(new IRoi[] { TemplateROI });
                }
                OnPropertyChanged();
            }
        }
        public TemplateMatching MatchingHandler
        {
            get { return matchingHandler; }
            set
            {
                matchingHandler = value;
            }
        }
        private int txtsize { get; set; }
        public int TextSizeView
        {
            get { return txtsize; }
            set
            {
                txtsize = value;
                OnPropertyChanged();
            }
        }

        public int MinScore
        {
            get
            {
                return (int)matchingHandler.Score;
            }
            set
            {
                matchingHandler.Score = value;
                OnPropertyChanged();
            }
        }

        public int MinAngle
        {
            get
            {
                return (int)matchingHandler.MinAngle;
            }
            set
            {
                matchingHandler.MinAngle = value;
                OnPropertyChanged();
            }
        }
        public int MaxAngle
        {
            get
            {
                return (int)matchingHandler.MaxAngle;
            }
            set
            {
                matchingHandler.MaxAngle = value;
                OnPropertyChanged();
            }
        }
        public BitmapSource ImageSource
        {
            get
            {
                return imageSoure;
            }
            set
            {
                imageSoure = value;
                OnPropertyChanged();
            }
        }
        public MatchingPropertyContext()
        {
           
            Test = new RelayCommand(OnTest);
            TextSizeView = 16;
            DataSetup = new DataSetupBase();
            DataSetup = JsonConvert.DeserializeObject<DataSetupBase>(File.ReadAllText("DataBaseSetUp.json"));
           
        }

        private void OnTest(object obj)
        {
            VisionImage inputGray = new VisionImage(ImageType.U8);
            CommonAlgorithms.Cast(VSImage, inputGray, ImageType.U8);
            IResult result = matchingHandler.Inspect(inputGray);
            if (result is TemplateResult)
            {
                var item1 = result as TemplateResult;
                ImageBox.RoiLayer.RegionType = RoiType.Undefined;
                ImageBox.ClearRoiItems();
                ImageBox.ClearOverlayItems();
                ImageBox.ClearViewLayerItems();
                ImageBox.AddViewLayerItems(DisplayOverLayResult.ViewRegionSetup(SearchROI, OrangeColor, "Searching Area").ToArray());
                ImageBox.AddViewLayerItems(DisplayOverLayResult.ListOverLayResult(item1, 20).ToArray());     
                if(result.IsStatus == true)
                {
                    IsStatus = "OK";
                    ColorStatus = "#00ff00";
                }
                else
                {
                    IsStatus = "NG";
                    ColorStatus = "#ff0000";
                }
            }
            inputGray.Dispose();
        }

        public Rectangle SearchingROI = new Rectangle();
        public void OnRoiAdded(object source, IRoi roi)
        {
            if (roi != null)
            {
                var rectROI = roi.MinRectangle;
                if (IsDrawSearching)
                {
                    SearchingROI = rectROI;
                    matchingHandler.SearchingROI = new NationalInstruments.Vision.Roi(new RectangleContour(rectROI.X, rectROI.Y, rectROI.Width, rectROI.Height));
                    SearchROI = new RectangleRoi(rectROI.X, rectROI.Y, rectROI.Width, rectROI.Height);
                    ImageBox.AddViewLayerItems(DisplayOverLayResult.ViewRegionSetup(TemplateROI, BlueColor, "Teamplate").ToArray());
                }
                else if (IsDrawTemplate)
                {
                    TemplateROI = roi as RectangleRoi;
                                  
                    using (VisionImage subImage = new VisionImage(VSImage.Type))
                    using (VisionImage subImageGray = new VisionImage(ImageType.U8))
                    {                       
                        matchingHandler.RoiTemplate = new NationalInstruments.Vision.Roi(new RectangleContour(TemplateROI.X, TemplateROI.Y, TemplateROI.Width, TemplateROI.Height));
                        Algorithms.Extract(VSImage, subImage, matchingHandler.RoiTemplate);
                        CommonAlgorithms.Cast(subImage, subImageGray, ImageType.U8);
                        ImageSource = CovertVisionImage.ConvertVisionImageToBitmapSource(subImageGray);
                        matchingHandler.LearnMatchingObject(subImageGray);
                    }
                    ImageBox.AddViewLayerItems(DisplayOverLayResult.ViewRegionSetup(SearchROI, OrangeColor, "Searching Area").ToArray());
                }
            }
        }

        public void OnRoiChanged(object source, IRoi roi)
        {
            OnRoiAdded(source, roi);
        }

        public void OnRoiDeleted(object source, IRoi roi)
        {
            OnRoiAdded(source, roi);
        }

       

        public void ViewtStartUp()
        {

        }
    }

    
}
