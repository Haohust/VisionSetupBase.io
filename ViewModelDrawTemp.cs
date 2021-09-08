using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ST4I.Vision.UI;
using ST4I.Vision.Utils;
using System.Drawing;
using ST4I.Vision.Core;
using System.Windows.Media.Imaging;

namespace SetupSolution
{
    public class ViewModelDrawTemp : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public Rectangle rectTemp { get; set; }
       
        public ImageBoxContext ImageDrawTemp { get; set; }        
        public RelayCommand SelectTemp { get; set; }
        public RelayCommand Load { get; set; }
        public ViewModelDrawTemp()
        {
            ImageDrawTemp = new ImageBoxContext()
            {
                ImageMode = ImageSizeMode.Uniform,
                RectRegionEnabled = true,
                RotatedRectRegionEnabled = true,
                CircleRegionEnabled = true,
                PolygonRegionEnabled = true,
                ZoomAndPanEnabled = true                              
            };            
            ImageDrawTemp.RoiAdded += OnRoiAdd;
            ImageDrawTemp.RoiChanged += OnRoiAdd;
            ImageDrawTemp.RoiDeleted += OnRoiAdd;
            SelectTemp = new RelayCommand(OnSelectTemp);
            Load = new RelayCommand(OnLoad);
        }

        private void OnLoad(object obj)
        {

        }

        private void OnRoiAdd(object sender, IRoi ROI)
        {
            var reg = ROI.MinRectangle;
            if(reg is Rectangle)
            {
                rectTemp = reg;
            }
        }

        private void OnSelectTemp(object obj)
        {
            if(ImageDrawTemp.ImageBitmap!= null)
            {
                
            }
        }
    }
}
