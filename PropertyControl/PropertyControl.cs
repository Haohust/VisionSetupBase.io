using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ST4I.Vision.Core;
using ST4I.Vision.UI;
using Emgu.CV;
using NationalInstruments.Vision;

namespace SetupSolution
{
    public interface IPropertyContext
    {
        void OnRoiAdded(object source, IRoi roi);
        void OnRoiChanged(object source, IRoi roi);
        void OnRoiDeleted(object source, IRoi roi);
        ImageBoxContext ImageBox { get; set; }
        VisionImage VSImage { get; set; }
        string NameSolution { get; set; }
    }


    public enum ModuleType
    {
        Barcode,
        Qrcode,
        GreyMatching,
        MeasureItensity
    }
}
