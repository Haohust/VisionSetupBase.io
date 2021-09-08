using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NationalInstruments.Vision;
using NationalInstruments.Vision.Acquisition.Imaqdx;
using Microsoft.Win32;
using ST4I.Vision.Core;
using ST4I.Vision.UI;
using ST4I.Vision.Utils;
using Emgu.CV;
using System.Windows;

namespace SetupSolution
{
    public class ItemTask
    {
        public int IP { get; set; }
        public string Name { get; set; }
        public ItemTask(int ip, string name)
        {
            Name = name;
            IP = ip;
        }
    }
    
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private int selectedIndexPropertyControl;
        private IPropertyContext curPropertyControl;
        private ImaqdxSession _session = null;
        private int selectedIndexModule = -1;
       
            
        private string contentConnect { get; set; }
        public bool IsConnected { get; set; }
        public string ContentConnect
        {
            get { return contentConnect; }
            set
            {
                contentConnect = value;
                OnPropertyChanged();
            }
        }
        private string cameraSelected { get; set; }
        public string CameraSelected
        {
            get { return cameraSelected; }
            set
            {
                cameraSelected = value;
                OnPropertyChanged();
            }
        }
        private string nameNewModule { get; set; }
        public string NameNewModule
        {
            get { return nameNewModule; }
            set
            {
                nameNewModule = value;
                OnPropertyChanged();
            }
        }
        public IPropertyContext CurPropertyContext
        {
            get
            {
                return curPropertyControl;
            }
            set
            {
                curPropertyControl = value;
                OnPropertyChanged();
            }
        }
        public List<IPropertyContext> PropertyContextItems { get; set; }
        public ObservableCollection<UserControl> PropertyControlItems { get; set; }
        public ObservableCollection<ItemTask> ListTask { get; set; }
        public VisionImage SetupVsImage { get; set; }
        public RelayCommand BrowserImageCommand { get; set; }
        public RelayCommand AddBarcodeCommand { get; set; }
        public RelayCommand AddMatchingCommand { get; set; }
        public RelayCommand Connect { get; set; }
        public RelayCommand LoadTask { get; set; }
        public ImageBoxContext ImageSetupView { get; set; }
        public RelayCommand AddQRcodeCommand { get; set; }

        public int SelectedIndexModule
        {

            get
            {
                return selectedIndexModule;
            }
            set
            {
                selectedIndexModule = value;
                OnPropertyChanged();
            }
        }

        public ViewModel()
        {
            SetupVsImage = new VisionImage(ImageType.Rgb32);
            PropertyControlItems = new ObservableCollection<UserControl>();
            PropertyContextItems = new List<IPropertyContext>();
            AddBarcodeCommand = new RelayCommand(OnAddBarcodeCommand);
            AddMatchingCommand = new RelayCommand(OnAddMatchingCommand);
            BrowserImageCommand = new RelayCommand(OnBrowserImageCommand);
            AddQRcodeCommand = new RelayCommand(OnAddQRCodeCommand);
            Connect = new RelayCommand(OnConnect);
            ImageSetupView = new ImageBoxContext()
            {
                ImageMode = ImageSizeMode.Uniform,
                RectRegionEnabled = true,
                RotatedRectRegionEnabled = true,
                CircleRegionEnabled = true,
                PolygonRegionEnabled = true,
                ZoomAndPanEnabled = true,
            };
            ImageSetupView.FunctionalMode = ImageBoxFunctionalModeOption.SetRoi;
            ImageSetupView.RoiAdded += OnRoiAdded;
            ImageSetupView.RoiChanged += OnRoiChanged;
            ImageSetupView.RoiDeleted += OnRoiDeleted;
            ContentConnect = "CONNECT";
            IsConnected = false;
            ListTask = new ObservableCollection<ItemTask>();
        }

        private void OnConnect(object obj)
        {
            try
            {
                if (!IsConnected)
                {
                    // Open camera
                    
                    _session = new ImaqdxSession(CameraSelected.Split(':')[0]);
                    
                    IsConnected = true;
                    ContentConnect = "DISCONNECT";
                }
                else
                {
                    _session.Close();
                    _session.Dispose();
                    IsConnected = false;
                    ContentConnect = "CONNECT";
                }

            }
            catch (ImaqdxException ex)
            {
                MessageBox.Show(ex.Message, "NI-IMAQdx Error");
            }
        }

        public double MinScore { get; set; }
        private void OnAddQRCodeCommand(object obj)
        {
            AddNewPropertyControl(ModuleType.Qrcode);
        }
        public void OnAddBarcodeCommand(object o)
        {
            AddNewPropertyControl(ModuleType.Barcode);
        }
        public void OnAddMatchingCommand(object o)
        {
            AddNewPropertyControl(ModuleType.GreyMatching);
        }
        public void AddNewPropertyControl(ModuleType type)
        {
            UserControl control = new UserControl();
            IPropertyContext context = new BarcodePropertyContext();
            switch (type)
            {
                case ModuleType.Barcode:
                    context = new BarcodePropertyContext();
                    control = new PropertiesBarCode();
                    break;
                case ModuleType.Qrcode:
                    context = new QRCodePropertyContext();
                    control = new PropertiesQRCode();
                    break;
                case ModuleType.GreyMatching:
                    context = new MatchingPropertyContext();
                    control = new PropertiesMatching();
                    break;
                case ModuleType.MeasureItensity:
                    break;
                default:
                    break;
            }
            context.ImageBox = ImageSetupView;
            context.VSImage = SetupVsImage;
            context.NameSolution = NameNewModule;
            control.DataContext = context;
            PropertyContextItems.Add(context);
            PropertyControlItems.Add(control);
            // add list view
            ListTask.Add(new ItemTask(ListTask.Count+1, type.ToString() + "_"+(ListTask.Count + 1).ToString()));
            // select property control
            SelectedIndexPropertyControl = PropertyControlItems.Count - 1;
        }

        public int SelectedIndexPropertyControl
        {
            get
            {
                return selectedIndexPropertyControl;
            }
            set
            {
                selectedIndexPropertyControl = value;
                if (value != -1)
                {
                    CurPropertyContext = PropertyContextItems[SelectedIndexPropertyControl];
                }
                else
                {
                    CurPropertyContext = null;
                }
                OnPropertyChanged();
            }
        }
        private void OnRoiAdded(object source, IRoi roi)
        {
            if (CurPropertyContext != null)
            {
                CurPropertyContext.OnRoiAdded(source, roi);
            }
        }

        private void OnRoiChanged(object source, IRoi roi)
        {
            if (CurPropertyContext != null)
            {
                CurPropertyContext.OnRoiChanged(source, roi);
            }
        }

        private void OnRoiDeleted(object source, IRoi roi)
        {
            if (CurPropertyContext != null)
            {
                CurPropertyContext.OnRoiDeleted(source, roi);
            }
        }

        private void OnBrowserImageCommand(object o)
        {
            if(!IsConnected)
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.tiff, *.png,*.bmp) | *.jpg; *.jpeg; *.jpe; *.tiff; *.png; *.bmp";
                if (opf.ShowDialog() == true)
                {
                    SetupVsImage.ReadFile(opf.FileName);
                    ImageSetupView.ImageBitmap = new BitmapImage(new Uri(opf.FileName));
                }
            }
            else
            {
                _session.Snap(SetupVsImage);   
                var img = CovertVisionImage.ConvertVisionImageToBitmapSource(SetupVsImage);
                ImageSetupView.ImageBitmap = img;
            } 
                
        }

    }
}
