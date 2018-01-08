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
    public partial class CameraViewer2 : Window
    {

        public CameraViewer2(string name)
        {
            InitializeComponent();

            //foreach (Camera item in CameraService.AvailableCameras)
            //{
            //    if (item.Name == name)
            //        camera2.DeviceName = item.Name;
            //}

            //camera2.Start();
        }

    }

    public class ScaleConverter : IMultiValueConverter
    {


        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                float v = (float)values[0];
                double m = (double)values[1];
                return v * m / 50;
            }
            catch (Exception ex) { }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
