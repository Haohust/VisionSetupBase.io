using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Module;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Emgu.CV;
using NationalInstruments.Vision;
using ST4I.Vision.Core;
using ST4I.Vision.UI;
using System.Drawing;
using ST4I.Vision.Utils;
using NationalInstruments.Vision.Analysis;

namespace SetupSolution
{
    public class BarcodePropertyContext : IPropertyContext, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool IsAutoSetup
        {
            get { return barcode1Dhandle.IsAuto; }
            set
            {
                barcode1Dhandle.IsAuto = value;
                OnPropertyChanged();
            }
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

        public BarcodeTypes BarcodeTypeItem
        {
            get
            {
                return barcode1Dhandle.barcodeTypes;
            }
            set
            {
                barcode1Dhandle.barcodeTypes = value;
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
        public RectangleRoi SearchROI { get; set; }
        public string NameSolution { get; set; }
        public VisionImage VSImage { get; set; }
        public Mat ImgMat { get; set; }
        public ImageBoxContext ImageBox { get; set; }        
        public RelayCommand Test { get; set; }
       
        public ReadBarcode1D barcode1Dhandle = new ReadBarcode1D();
        List<ReadBarcode1D> listRead1D = new List<ReadBarcode1D>();
        public BarcodePropertyContext()
        {
            IndexArea = 0;
            IsAutoSetup = true;
           
            Test = new RelayCommand(OnTest);
            
        }
        
        private string dataCompare { get; set; }
        public string DataCompare
        {
            get { return dataCompare; }
            set
            {
                dataCompare = value;                
                OnPropertyChanged();
            }
        }
        private int numberCode { get; set; }
        public int NumberCode
        {
            get { return numberCode; }
            set
            {
                numberCode = value;
                OnPropertyChanged();
            }
        }
        private void OnTest(object obj)
        {
            barcode1Dhandle.ListDataCompare = DataCompare.Split("_").ToList();
            VisionImage inputGray = new VisionImage(ImageType.U8);
            CommonAlgorithms.Cast(VSImage, inputGray, ImageType.U8);
            IResult result = barcode1Dhandle.Inspect(inputGray);
            if (result is ListBarCode)
            {
                var item1 = result as ListBarCode;
                ImageBox.RoiLayer.RegionType = RoiType.Undefined;
                ImageBox.ClearRoiItems();
                ImageBox.ClearOverlayItems();
                ImageBox.ClearViewLayerItems();
                ImageBox.AddViewLayerItems(DisplayOverLayResult.ViewRegionSetup(SearchROI, "#00ff00", "Searching Area Barcode").ToArray());
                ImageBox.AddViewLayerItems(DisplayOverLayResult.ListOverLayResult(item1, 30).ToArray());
            }
            inputGray.Dispose();
        }
       
        public void OnRoiAdded(object source, IRoi roi)
        {
            Rectangle rect = roi.MinRectangle;
            barcode1Dhandle.ROI = new NationalInstruments.Vision.Roi(new RectangleContour(rect.X, rect.Y, rect.Width, rect.Height));
            listRead1D.Clear();
            SearchROI = new RectangleRoi(rect.X, rect.Y, rect.Width, rect.Height);
            listRead1D.Add(barcode1Dhandle);
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
