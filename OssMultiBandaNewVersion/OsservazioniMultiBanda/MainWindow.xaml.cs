//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "UI-WPF", Justification = "Sample is correctly named")]

namespace OssMultiBanda
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Data.Linq;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.IO;
    using System.Collections.Generic;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow
    {
        private OATeSettings _OATeSettings;
        public OATeSettings OATeSettings
        {
            get { return _OATeSettings; }
            set { _OATeSettings = value; }
        }

        private List<OATeSettings> _ListOATeSettings;
        public List<OATeSettings> ListOATeSettings
        {
            get { return _ListOATeSettings; }
            set { _ListOATeSettings = value; }
        }

        private Console _Console { get; set; }
        private MediaViewer _MediaViewer { get; set; }

        //private CameraViewer _CameraViewer { get; set; }
        private TwoCamerasTest.MainForm CameraViewer { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class. 
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.LayoutRoot.DataContext = this;

            CreateOATeSettings();

            this.Left = 0;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height;
            this.Width = SystemParameters.PrimaryScreenWidth;
        }

        private void CreateOATeSettings()
        {
            _Console = new Console();
            _Console.Show();

            _ListOATeSettings = new List<OATeSettings>();
            foreach (var item in Directory.EnumerateDirectories(Directory.GetCurrentDirectory()))
            {
                foreach (var settings in Directory.GetFiles(item, "OATeSettings.xml"))
                {
                    OATeSettings newsett = new OATeSettings();
                    if (OATeSettings.TryLoadSettingsNoUi(settings, out newsett))
                        _ListOATeSettings.Add(newsett);
                }
            }

            if (!OATeSettings.TryLoadSettingsNoUi("OATeSettings.xml", out _OATeSettings))
            {
                this._OATeSettings = new OATeSettings();
                _OATeSettings.Barra = "Sole/spettro.png";
                _OATeSettings.Puntatore = "Sole/Puntatore.png";
                this._OATeSettings.MediaFiles = new System.Collections.Generic.List<OATeMedia>();
                OATeMedia media1 = new OATeMedia();
                media1.FromValue = 0;
                media1.ToValue = 0.1;
                media1.MediaUri = "Sole/Video/Ic_flat_2d.mpg";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.1;
                media1.ToValue = 0.2;
                media1.MediaUri = "Sole/Video/latest_1024_0094.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.2;
                media1.ToValue = 0.3;
                media1.MediaUri = "Sole/Video/latest_1024_0131.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.3;
                media1.ToValue = 0.4;
                media1.MediaUri = "Sole/Video/latest_1024_0193.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.4;
                media1.ToValue = 0.5;
                media1.MediaUri = "Sole/Video/latest_1024_0211.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.5;
                media1.ToValue = 0.6;
                media1.MediaUri = "Sole/Video/latest_1024_0304.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.6;
                media1.ToValue = 0.7;
                media1.MediaUri = "Sole/Video/latest_1024_0335.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.8;
                media1.ToValue = 0.9;
                media1.MediaUri = "Sole/Video/latest_1024_1600.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                media1 = new OATeMedia();
                media1.FromValue = 0.9;
                media1.ToValue = 1;
                media1.MediaUri = "Sole/Video/latest_1024_1700.mp4";
                _OATeSettings.MediaFiles.Add(media1);
                OATeSettings.SaveSettings("OATeSettings.xml", _OATeSettings);
            }
            _ListOATeSettings.Add(_OATeSettings);

            CameraViewer = new TwoCamerasTest.MainForm();
            CameraViewer.Show();
            CameraViewer.Hide();

            _MediaViewer = new MediaViewer(_OATeSettings);
            _MediaViewer.Width = SystemParameters.PrimaryScreenWidth;
            _MediaViewer.Height = SystemParameters.PrimaryScreenHeight - this.Height;
            _MediaViewer.Left = 0;
            _MediaViewer.Top = 0;
            _MediaViewer.Show();

            ImageSource source = new BitmapImage(new Uri(_OATeSettings.Barra, UriKind.RelativeOrAbsolute));
            border_barra.Background = new ImageBrush(source);
            foreach (var item in _OATeSettings.MediaFiles)
                _MediaViewer.AddElement(item);

            UserStateVisualizer._OATeSettings = this._OATeSettings;
            UserStateVisualizer.ValueReached += UserStateVisualizer_ValueReached;
            UserStateVisualizer.ValueReached += _MediaViewer.UserStateVisualizer_ValueReached;
        }

        protected void UserStateVisualizer_ValueReached(object sender, ValueEventArgs e)
        {
            string txt = ("Value Reached = " + e.Value);
            foreach (var item in _OATeSettings.MediaFiles)
                if (e.Value >= item.FromValue && e.Value < item.ToValue)
                    txt += ("Playing item = " + item.MediaUri);
            _Console.WriteLine(txt);
        }
        
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                CameraViewer.TopMost = true;
                CameraViewer.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                CameraViewer.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                CameraViewer.Show();
            }

            if (e.Key == Key.F)
            {
                int index = ListOATeSettings.IndexOf(_OATeSettings);
                if ((index + 1) < ListOATeSettings.Count)
                    _OATeSettings = ListOATeSettings[index + 1];
                else
                    _OATeSettings = ListOATeSettings[0];

                //ImageSource source = new BitmapImage(new Uri(_OATeSettings.Barra, UriKind.RelativeOrAbsolute));
                //border_barra.Background = new ImageBrush(source);
                _MediaViewer._OATeSettings = _OATeSettings;
                foreach (var item in _OATeSettings.MediaFiles)
                    _MediaViewer.AddElement(item);

                UserStateVisualizer._OATeSettings = this._OATeSettings;
                UserStateVisualizer.StartPoint();
            }

            //if ((e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt)) && e.SystemKey == Key.Escape)
            //    Application.Current.Shutdown(0);

            if ((e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt)) && e.SystemKey == Key.L)
            {
                OATeSettings oATeSettings;
                if (OATeSettings.TryLoadSettingsWithOpenFileDialog("OATeSettings.xml", out oATeSettings))
                {
                    UserStateVisualizer.ValueReached -= _MediaViewer.UserStateVisualizer_ValueReached;
                    _OATeSettings = oATeSettings;

                    _MediaViewer.Close();

                    _MediaViewer = new MediaViewer(_OATeSettings);
                    _MediaViewer.Width = SystemParameters.PrimaryScreenWidth;
                    _MediaViewer.Height = SystemParameters.PrimaryScreenHeight - this.Height;
                    _MediaViewer.Left = 0;
                    _MediaViewer.Top = 0;
                    _MediaViewer.Show();
                    UserStateVisualizer.ValueReached += _MediaViewer.UserStateVisualizer_ValueReached;

                    ImageSource source = new BitmapImage(new Uri(_OATeSettings.Barra, UriKind.RelativeOrAbsolute));
                    border_barra.Background = new ImageBrush(source);
                    UserStateVisualizer._OATeSettings = _OATeSettings;
                    foreach (var item in _OATeSettings.MediaFiles)
                        _MediaViewer.AddElement(item);
                }
            }
        }

        void cameraViewer_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _Console.WriteLine(e.Data);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if ((e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt)) && e.SystemKey == Key.Escape)
            //    Application.Current.Shutdown(0);
            
            if (e.Key == Key.Right)
                UserStateVisualizer.Increase();
            if (e.Key == Key.Left)
                UserStateVisualizer.Decrease();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserStateVisualizer.StartPoint();
        }
    }
}
