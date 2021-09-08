using ST4I.Vision.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SetupSolution
{
    /// <summary>
    /// Interaction logic for DrawTemp.xaml
    /// </summary>
    public partial class DrawTemp : Window
    {
        private ViewModelDrawTemp _viewModelDrawTemp { get; set; }
        public ViewModelDrawTemp HandModelDrawTemp
        {
            get { return _viewModelDrawTemp; }
        }
        public DrawTemp()
        {
            InitializeComponent();
            //_viewModelDrawTemp = (ViewModelDrawTemp)this.DataContext;
            
        }
    }
}
