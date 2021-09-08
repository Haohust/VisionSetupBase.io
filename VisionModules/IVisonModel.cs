using Microsoft.Win32;
using Emgu.CV;
using Emgu.Util;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using ST4I.Vision.Core;
using NationalInstruments.Vision;
using System;
using System.Drawing;
using Emgu.CV.Structure;
using NationalInstruments.Vision.Analysis;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Vision.Module
{
    public interface ISetup
    {

    }
    public interface IVisonModel
    {
        IResult Inspect(VisionImage InputImg);
    }
    public interface IResult
    {
        bool IsStatus { get; set; }
    }
}
