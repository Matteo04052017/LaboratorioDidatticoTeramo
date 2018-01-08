using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Touchless.Vision.Camera;

namespace OssMultiBanda
{
    /// <summary>
    /// Logica di interazione per MediaViewer.xaml
    /// </summary>
    public partial class CameraViewer : Window
    {
        private Camera _Camera;
        private CameraFrameSource _frameSource;
        private static Bitmap _latestFrame;


        public CameraViewer(string camera)
        {
            InitializeComponent();

            foreach (Camera item in CameraService.AvailableCameras)
            {
                if (item.Name == camera)
                    _Camera = item;
            }

            try
            {
                _frameSource = new CameraFrameSource(_Camera);
                _frameSource.Camera.CaptureWidth = 640;
                _frameSource.Camera.CaptureHeight = 480;
                _frameSource.Camera.Fps = 50;
                _frameSource.NewFrame += OnImageCapturedCamera1;
                _frameSource.StartFrameCapture();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void OnImageCapturedCamera1(Touchless.Vision.Contracts.IFrameSource frameSource, Touchless.Vision.Contracts.Frame frame, double fps)
        {
            _latestFrame = frame.Image;

            Action action = delegate()
            {
                camera1.Source = BitmapToImageSource(_latestFrame);
            };

            Dispatcher.Invoke(action);
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }

    //public class ScaleConverter : IMultiValueConverter
    //{


    //    #region IMultiValueConverter Members

    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        try
    //        {
    //            float v = (float)values[0];
    //            double m = (double)values[1];
    //            return v * m / 50;
    //        }
    //        catch (Exception ex) { }
    //        return 0;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}
}
