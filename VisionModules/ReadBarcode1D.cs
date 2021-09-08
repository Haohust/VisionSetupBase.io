using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using ST4I.Vision.UI;
using System.Drawing;
using NationalInstruments.Vision;
using NationalInstruments.Vision.Analysis;
using Emgu.CV;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace Vision.Module
{
    public class BarCode1DResult
    {
        public bool IsExist { get; set; }
        public string Content { get; set; }
        public System.Drawing.Point[] Vertice { get; set; }
        public BarCode1DResult()
        {
            Content = "";
            Vertice = new System.Drawing.Point[4];
        }
    }
    public class ListBarCode : IResult
    {
        public bool IsStatus { get; set; }
        public List<BarCode1DResult> LstBarCode { get; set; }
        public ListBarCode()
        {
            LstBarCode = new List<BarCode1DResult>();
            IsStatus = true;
        }
    }
    public class ReadBarcode1D: IVisonModel
    {   
        public List<string> ListDataCompare { get; set; }
        public int NumberCodeLimit { get; set; }
        public Roi ROI { get; set; }       
        public BarcodeTypes barcodeTypes { get; set; }
        public bool IsAuto { get; set; }
        public ReadBarcode1D()
        {
            ROI = new Roi();
            IsAuto = true;
            ListDataCompare = new List<string>();
        }       
        public IResult Inspect(VisionImage input)
        {
            ListBarCode LstResult = new ListBarCode();           
            if (input != null)
            {               
                if (ROI!=null)
                {                                  
                    try
                    {
                        Barcode1DSearchOptions opts = new Barcode1DSearchOptions()
                        {
                            MinBarcodeWidth = 3,
                            MinEdgeStrength = 75,
                            MinBars = 20,
                            SkipLocationSearch = 0
                        };
                        //Roi barcodeRoi = new Roi(new Shape[] { new RectangleContour(0, 0, input.Width, input.Height) });
                        input.Overlays.Default.AddRoi(ROI);
                        if(IsAuto)
                        {
                            BarcodeInfoReport res = Algorithms.ReadBarcode2(input, opts, new BarcodeTypes[] { BarcodeTypes.Code39, BarcodeTypes.Code128, BarcodeTypes.Ean13, BarcodeTypes.Code93 }, 5, ROI, false);
                            // DISPLAY 
                            foreach (var item in res.Info)
                            {
                                BarCode1DResult itemBar = new BarCode1DResult();
                                itemBar.Content = item.Text;                               
                                itemBar.IsExist = ListDataCompare.ToArray().Contains(itemBar.Content) ? true : false;                           
                                itemBar.Vertice[0] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[0].X,
                                    Y = (int)item.BoundingBox[0].Y,
                                };
                                itemBar.Vertice[1] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[1].X,
                                    Y = (int)item.BoundingBox[1].Y,
                                };
                                itemBar.Vertice[2] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[2].X,
                                    Y = (int)item.BoundingBox[2].Y,
                                };
                                itemBar.Vertice[3] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[3].X,
                                    Y = (int)item.BoundingBox[3].Y,
                                };
                                LstResult.LstBarCode.Add(itemBar);
                            }                      
                        }
                        else
                        {
                            BarcodeInfoReport res = Algorithms.ReadBarcode2(input, opts, new BarcodeTypes[] { barcodeTypes }, NumberCodeLimit, ROI, false);
                            // DISPLAY 
                            foreach (var item in res.Info)
                            {
                                BarCode1DResult itemBar = new BarCode1DResult();
                                itemBar.Content = item.Text;
                                itemBar.Content = item.Text;
                                itemBar.IsExist = ListDataCompare.ToArray().Contains(itemBar.Content) ? true : false;
                                itemBar.Vertice[0] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[0].X,
                                    Y = (int)item.BoundingBox[0].Y,
                                };
                                itemBar.Vertice[1] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[1].X,
                                    Y = (int)item.BoundingBox[1].Y,
                                };
                                itemBar.Vertice[2] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[2].X,
                                    Y = (int)item.BoundingBox[2].Y,
                                };
                                itemBar.Vertice[3] = new System.Drawing.Point()
                                {
                                    X = (int)item.BoundingBox[3].X,
                                    Y = (int)item.BoundingBox[3].Y,
                                };
                                LstResult.LstBarCode.Add(itemBar);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message+ "\r"+ex.StackTrace);
                    }                                                                                      
                }
            }
            return LstResult;
        }
    }
}
