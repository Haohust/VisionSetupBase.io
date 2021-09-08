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
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Vision.Module;

namespace SetupSolution
{
    public static class DisplayOverLayResult
    {
        public static List<OverlayItem>  ViewRegionSetup(RectangleRoi roi, string strokeColor, string text)
        {
            List<OverlayItem> result = new List<OverlayItem>();
            if (roi != null)
            {
                var _panel = new PanelTextOverlay(roi)
                {
                    Stroke = strokeColor,
                    StrokeThickness = 2
                };
                var _text = new TextOverlay()
                {
                    Text = text,
                    BackgroundText = "#ffffff",
                    Foreground = strokeColor,
                    X = roi.X,
                    Y = roi.Y - 25,
                    TextSize = 30
                };
                result.Add(_panel);
                result.Add(_text);                
            }
            return result;
        }
        public static string[] Color = new string[]
        {
            "#ff0000","#800080","#ff00ff","#008000","#00ff00","#ffff00","#008080","#0000ff","#ffa500"
        };
        public static List<OverlayItem> ListOverLayResult (IResult items, int TextSizeView)
        {
            List<OverlayItem> result = new List<OverlayItem>();
            int _Y = 1;
            
            if( items is TemplateResult)
            {
                var item = items as TemplateResult;
                foreach (var item2 in item.MatchingResult)
                {
                    result.Add(new PanelTextOverlay(new PolygonRoi()
                    { Vertices = item2.Vertices })
                    {
                        StrokeThickness = 2,
                        Stroke = "#ff0000"
                    }); ;
                    result.Add(new TextOverlay()
                    {
                        Text = item2.IsStatus.ToString()+", Angle=" + Math.Round(item2.Rotation, 1).ToString() + ", Score: " + Math.Round(item2.Score).ToString(),
                        TextSize = TextSizeView,
                        Foreground = "#ff0000",
                        X = 1,
                        Y = _Y
                    });                    
                    _Y += 30;
                }
            }
            else if( items is QRCodeResult)
            {
                var item = items as QRCodeResult;
                  result.Add(new PanelTextOverlay(new PolygonRoi()
                    { Vertices = item.Vertice })
                    {
                        StrokeThickness = 2,
                        Stroke = "#ff0000"
                    }); 
                    result.Add(new TextOverlay()
                    {
                        Text = item.Content,
                        TextSize = TextSizeView,
                        Foreground = "#ff0000",
                        X = item.Vertice[1].X,
                        Y = item.Vertice[1].Y
                    });
              
            }
            else if( items is ListBarCode)
            {
                var item = items as ListBarCode;
                foreach (var item2 in item.LstBarCode)
                {
                    result.Add(new PanelTextOverlay(new PolygonRoi()
                    { Vertices = item2.Vertice })
                    {
                        StrokeThickness = 2,
                        Stroke = "#ff0000"
                    });
                    result.Add(new TextOverlay()
                    {
                        Text = item2.Content,
                        TextSize = TextSizeView,
                        Foreground = "#ff0000",
                        X = item2.Vertice[1].X,
                        Y = item2.Vertice[1].Y
                    });

                }
            }
            return result;
        }
    }

    public static class CovertVisionImage
    {
        /// <summary>
        /// Chuyển đổi vision image sang Bitmap. Hỗ trợ 2 định dạng của vision image là ImageType.U8 và ImageType.Rgb32. Lưu ý đảm bảo ảnh đầu
        /// vào ở 2 định dạng này
        /// </summary>
        /// <param name="visionImage"></param>
        /// <returns></returns>
        public static Bitmap ConvertVisionImageToBitmap(VisionImage visionImage)
        {
            Bitmap bitmap = null;
            if (visionImage.Type == ImageType.U8)
            {
                bitmap = new Bitmap(visionImage.Width, visionImage.Height, (Int32)visionImage.LineWidthInBytes, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale, visionImage.StartPtr);
            }
            else if (visionImage.Type == ImageType.Rgb32)
            {
                bitmap = new Bitmap(visionImage.Width, visionImage.Height, (Int32)visionImage.LineWidthInBytes, System.Drawing.Imaging.PixelFormat.Format32bppRgb, visionImage.StartPtr);
            }
            if (bitmap != null)
            {
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                bitmap.UnlockBits(bitmapData);
                return bitmap;
            }
            return null;
        }

        /// <summary>
        /// Chuyển đổi vision image sang bitmapsource. Hỗ trợ 2 định dạng của vision image là ImageType.U8 và ImageType.Rgb32. Lưu ý đảm bảo ảnh đầu
        /// vào ở 2 định dạng này
        /// </summary>
        /// <param name="visionImage"></param>
        /// <returns></returns>
        public static BitmapSource ConvertVisionImageToBitmapSource(VisionImage visionImage)
        {
            if (visionImage.Type == ImageType.U8)
            {
                var bitmapSource = BitmapSource.Create(
                visionImage.Width, visionImage.Height,
                96, 96,
                PixelFormats.Gray8, null,
                visionImage.StartPtr, (Int32)visionImage.LineWidthInBytes * visionImage.Height, (Int32)visionImage.LineWidthInBytes);
                return bitmapSource;
            }
            else if(visionImage.Type == ImageType.Rgb32)
            {
                var bitmapSource = BitmapSource.Create(
                visionImage.Width, visionImage.Height,
                96, 96,
                PixelFormats.Bgr32, null,
                visionImage.StartPtr, (Int32)visionImage.LineWidthInBytes * visionImage.Height, (Int32)visionImage.LineWidthInBytes);
                return bitmapSource;
            }
            return null;
        }
    }
}

