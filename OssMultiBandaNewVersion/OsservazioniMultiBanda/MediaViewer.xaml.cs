using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace OssMultiBanda
{
    /// <summary>
    /// Logica di interazione per MediaViewer.xaml
    /// </summary>
    public partial class MediaViewer : Window
    {
        private OATeSettings oATeSettings;
        public OATeSettings _OATeSettings
        {
            get { return oATeSettings; }
            set
            {
                oATeSettings = value;
                LayoutRoot.Children.Clear();
            }
        }

        public int AnimationMilliseconds
        {
            get { return _OATeSettings.AnimationMilliseconds; }
        }

        public MediaViewer(OATeSettings oATeSettings)
        {
            InitializeComponent();

            _OATeSettings = oATeSettings;
        }

        public void AddElement(OATeMedia item, string description)
        {
            if (item.UI_Element != null)
            {
                LayoutRoot.Children.Add((Border)item.UI_Element);
                return;
            }
            Border mediaElem = new Border();
            Grid.SetRow(mediaElem, 1);
            mediaElem.Opacity = 0;
            mediaElem.Visibility = System.Windows.Visibility.Hidden;
            BitmapImage source = new BitmapImage(new Uri(item.MediaUri, UriKind.RelativeOrAbsolute));
            ImageBrush brush = new ImageBrush(source);
            brush.Stretch = Stretch.Uniform;
            mediaElem.Background = brush;
            mediaElem.Name = "MediaElement_" + LayoutRoot.Children.Count + 1;
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Text = description + "       ";
            textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            textBlock.Foreground = Brushes.White;
            textBlock.FontSize = 32;
            mediaElem.Child = textBlock;
            if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "/" + item.MediaUri))
                throw new Exception("Not existing media: " + item.MediaUri.ToString());
            LayoutRoot.Children.Add(mediaElem);
            
            item.UI_Element = mediaElem;
        }

        private Border last_media_shown;
        protected string lock_str = "lock string";
        public void UserStateVisualizer_ValueReached(object sender, ValueEventArgs e)
        {
            lock (lock_str)
            {
                // find out the new media to open and the old one to close
                // check if something is open and should not be open
                Border media_to_open = null;
                List<Border> media_to_close = new List<Border>();
                foreach (var item in _OATeSettings.MediaFiles)
                {
                    Border local = (item.UI_Element as Border);
                    if (local.Opacity > 0d)
                        media_to_close.Add(local);
                }

                foreach (var item in _OATeSettings.MediaFiles)
                {
                    if (e.Value >= item.FromValue && e.Value < item.ToValue)
                    {
                        media_to_open = (item.UI_Element as Border);
                        media_to_open.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                }

                Storyboard animation = new Storyboard();
                DoubleAnimation closeAnim = null;
                DoubleAnimation openAnim = null;
                bool anim = false;
                foreach (var single_media_to_close in media_to_close)
                {
                    if (single_media_to_close != null &&
                        single_media_to_close.Opacity == 1d &&
                    (media_to_open != single_media_to_close || media_to_open == null))
                    {
                        closeAnim = new DoubleAnimation(1d, 0d, new Duration(new TimeSpan(0, 0, 0, 0, AnimationMilliseconds)));
                        closeAnim.Completed += animation_Completed;
                        Storyboard.SetTarget(closeAnim, single_media_to_close);
                        Storyboard.SetTargetProperty(closeAnim, new PropertyPath(Border.OpacityProperty));
                        animation.Children.Add(closeAnim);
                        anim = true;
                    }
                }


                if (media_to_open != null &&
                    media_to_open.Opacity < 1d &&
                    media_to_open != last_media_shown)
                {
                    openAnim = new DoubleAnimation(0d, 1d, new Duration(new TimeSpan(0, 0, 0, 0, AnimationMilliseconds)));
                    openAnim.Completed += animation_Completed;
                    Storyboard.SetTarget(openAnim, media_to_open);
                    Storyboard.SetTargetProperty(openAnim, new PropertyPath(Border.OpacityProperty));
                    animation.Children.Add(openAnim);
                    anim = true;
                }

                last_media_shown = media_to_open;

                if (anim)
                    animation.Begin();
            }
        }

        private bool IsAnyMediaOpen()
        {
            foreach (var item in _OATeSettings.MediaFiles)
            {
                Border mediaElem = (item.UI_Element as Border);
                if (mediaElem.Opacity > 0d)
                    return true;
            }
            return false;
        }

        protected void animation_Completed(object sender, EventArgs e)
        {
            foreach (var item in _OATeSettings.MediaFiles)
            {
                Border element = (item.UI_Element as Border);
                if (element != last_media_shown && element.Visibility != System.Windows.Visibility.Hidden)
                {
                    element.Opacity = 0d;
                    element.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
    }
}
