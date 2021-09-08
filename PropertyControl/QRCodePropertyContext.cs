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
using ST4I.Vision.Utils;
using System.Windows.Media;
using System.Drawing;
using Emgu.CV;
using Vision.Module;

namespace SetupSolution
{
    class QRCodePropertyContext: INotifyPropertyChanged, IPropertyContext
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }   
        private bool isDraw { get; set; }
        public bool IsDraw
        {
            get { return isDraw; }
            set
            {
                isDraw = value;
                OnPropertyChanged();
            }
        }

       
        private int indexArea { get; set; }
        public int IndexArea
        {
            get { return indexArea; }
            set
            {
                indexArea = value;
                OnPropertyChanged();
            }
        }
        public string NameSolution { get; set; }
        public VisionImage VSImage { get; set; }
        public Mat ImgMat { get; set; }
        public ImageBoxContext ImageBox { get; set; }
        public RelayCommand AddBarCode { get; set; }
        public RelayCommand Test { get; set; }
        public RelayCommand EnableDraw { get; set; }
        public ReadQRCode QRCodeHandle { get; set; }
        public RectangleRoi SearchROI { get; set; }
        public QRCodePropertyContext()
        {
            IndexArea = 0;
            QRCodeHandle = new ReadQRCode();
            if (ImageBox != null)
            {
                ImageBox.MaxNumRoi = 0;
            }
            AddBarCode = new RelayCommand(OnAddBarCode);
            Test = new RelayCommand(OnTest);
            EnableDraw = new RelayCommand(OnEnableDraw);
        }

        private void OnEnableDraw(object obj)
        {
            ImageBox.FunctionalMode = ImageBoxFunctionalModeOption.SetRoi;
        }

        private void OnTest(object obj)
        {
            VisionImage inputGray = new VisionImage(ImageType.U8);
            CommonAlgorithms.Cast(VSImage, inputGray, ImageType.U8);
            IResult result = QRCodeHandle.Inspect(inputGray);
            if (result is QRCodeResult)
            {
                var item1 = result as QRCodeResult;
                ImageBox.RoiLayer.RegionType = RoiType.Undefined;
                ImageBox.ClearRoiItems();
                ImageBox.ClearOverlayItems();
                ImageBox.ClearViewLayerItems();
                ImageBox.AddViewLayerItems(DisplayOverLayResult.ViewRegionSetup(SearchROI, "#7fffd4", "Searching Area QR").ToArray());
                ImageBox.AddViewLayerItems(DisplayOverLayResult.ListOverLayResult(item1, 30).ToArray());
            }
            inputGray.Dispose();
        }
        private void OnAddBarCode(object obj)
        {

        }
        public void OnRoiAdded(object source, IRoi roi)
        {
            Rectangle rect = roi.MinRectangle;
            var itemROI = new NationalInstruments.Vision.Roi(new RectangleContour(rect.X, rect.Y, rect.Width, rect.Height));
            SearchROI = new RectangleRoi(rect.X, rect.Y, rect.Width, rect.Height);
            QRCodeHandle.ROI= itemROI;

        }
        public void OnRoiChanged(object source, IRoi roi)
        {
            OnRoiAdded(source, roi);
        }
        public void OnRoiDeleted(object source, IRoi roi)
        {
            OnRoiAdded(source, roi);
        }

    }
}
