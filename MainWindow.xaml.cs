using Microsoft.Win32;
using NationalInstruments.Vision;
using NationalInstruments.Vision.Acquisition.Imaqdx;
using ST4I.Vision.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;

namespace SetupSolution
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public VisionImage SetupVsImage { get; set; }
        public VisionImage InputVsImage { get; set; }
        private ImaqdxSession _session = null;
        public bool IsConnected = false;
        public ImaqdxCameraInformation[] cameraList { get; set; }
        
        private ViewModel handleModel { get; set; }
        public ViewModel HandleModel
        {
            get { return handleModel; }
        }
        public DataSetupBase DataSetup { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            handleModel = (ViewModel)this.DataContext;
            SetupVsImage = new VisionImage();
            cameraList = ImaqdxSystem.GetCameraInformation(true);
            foreach (ImaqdxCameraInformation camInfo in cameraList)
            {
                cbxCamera.Items.Add(camInfo.Name+ ":"+ camInfo.Model);       
            }
            cbxCamera.SelectedIndex = cameraList.Length > 0 ? 0 : -1;
            DataSetup = new DataSetupBase();
            DataSetup = JsonConvert.DeserializeObject<DataSetupBase>(File.ReadAllText("DataBaseSetUp.json"));
            
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_session != null)
            {
                _session.Dispose();
            }    
            
        }
    }
}
