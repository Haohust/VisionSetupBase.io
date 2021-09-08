using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ST4I.Vision.UI;
using System.Drawing;
using NationalInstruments.Vision;
using NationalInstruments.Vision.Analysis;
using Emgu.CV;
using System.IO;
namespace Vision.Module
{
    public class QRCodeResult : IResult
    {
        public bool IsStatus { get; set; }
        public string Content { get; set; }
        public Point[] Vertice { get; set; }
        public QRCodeResult()
        {
            Content = "";
            Vertice = new Point[4];
        }
    }
    public class ListQRCode 
    { 
        public List<QRCodeResult> LstQR { get; set; }
        
    }

    public class ReadQRCode:IVisonModel
    {      
        public VisionImage InputImg { get; set; }      
        public Roi ROI { get; set; }          
        public ReadQRCode()
        {
            ROI = new Roi();
            
        }              
        public IResult Inspect(VisionImage input)
        {
            QRCodeResult qRCodeResult = new QRCodeResult();
            
            if (ROI!= null)
            {
                QRReport qrReport = new QRReport();
                QRDescriptionOptions descOptions = new QRDescriptionOptions();
                QRSizeOptions sizeOptions = new QRSizeOptions();
                QRSearchOptions searchOptions = new QRSearchOptions();              
                QRCodeResult itemResult = new QRCodeResult();
                qrReport = Algorithms.ReadQRCode(input, ROI, descOptions, sizeOptions, searchOptions);
                if (qrReport.Found)
                {
                    itemResult.Content = System.Text.Encoding.Default.GetString(qrReport.GetData());
                    List<Point> cornes = new List<Point>();
                    foreach (var p in qrReport.Corners)
                    {
                        cornes.Add(new Point((int)p.X, (int)p.Y));
                    }
                    itemResult.Vertice = cornes.ToArray();
                }
                if (!string.IsNullOrEmpty(itemResult.Content))
                {
                    qRCodeResult= itemResult;
                }

            }                     
            return qRCodeResult;
        }
    }
}
