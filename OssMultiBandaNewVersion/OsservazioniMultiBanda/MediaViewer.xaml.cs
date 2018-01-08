using System;
using System.Collections;
using System.Collections.Generic;
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

        public MediaViewer(OATeSettings oATeSettings)
        {
            InitializeComponent();

            _OATeSettings = oATeSettings;
        }

        private Hashtable _MediaElements = new Hashtable();
        public void AddElement(OATeMedia item)
        {
            if (item.MediaElement != null)
            {
                LayoutRoot.Children.Add((MediaElement)item.MediaElement);
                return;
            }
            MediaElement mediaElem = new MediaElement();
            mediaElem.LoadedBehavior = MediaState.Manual;
            mediaElem.UnloadedBehavior = MediaState.Stop;
            mediaElem.MediaEnded += mediaElem_MediaEnded;
            mediaElem.Stretch = Stretch.Uniform;
            Grid.SetRow(mediaElem, 1);
            mediaElem.Opacity = 0;
            mediaElem.Source = new Uri(item.MediaUri, UriKind.RelativeOrAbsolute);
            mediaElem.Name = "MediaElement_" + LayoutRoot.Children.Count + 1;
            LayoutRoot.Children.Add(mediaElem);
            item.MediaElement = mediaElem;
        }

        private void mediaElem_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElem = sender as MediaElement;
            mediaElem.Position = new TimeSpan(0, 0, 1);
            mediaElem.Play();
        }

        private MediaElement last_media_shown;
        protected string lock_str = "lock string";
        public void UserStateVisualizer_ValueReached(object sender, ValueEventArgs e)
        {
            lock (lock_str)
            {
                Storyboard animation = new Storyboard();
                DoubleAnimation closeAnim = null;
                DoubleAnimation openAnim = null;

                if (last_media_shown != null)
                {
                    if (last_media_shown.Opacity > 0d)
                    {
                        closeAnim = new DoubleAnimation(1d, 0d, new Duration(new TimeSpan(0, 0, 1)));
                        closeAnim.Completed += animation_Completed;
                        Storyboard.SetTarget(closeAnim, last_media_shown);
                        Storyboard.SetTargetProperty(closeAnim, new PropertyPath(MediaElement.OpacityProperty));
                    }
                }

                bool found = false;
                foreach (var item in _OATeSettings.MediaFiles)
                {
                    if (e.Value >= item.FromValue && e.Value < item.ToValue)
                    {
                        MediaElement to_open_media = (item.MediaElement as MediaElement);

                        if (last_media_shown != to_open_media)
                        {
                            openAnim = new DoubleAnimation(0d, 1d, new Duration(new TimeSpan(0, 0, 1)));
                            openAnim.Completed += animation_Completed;
                            Storyboard.SetTarget(openAnim, to_open_media);
                            Storyboard.SetTargetProperty(openAnim, new PropertyPath(MediaElement.OpacityProperty));
                            last_media_shown = to_open_media;
                        }

                        (item.MediaElement as MediaElement).Play();
                        found = true;
                        break;
                    }
                }

                bool anim = false;
                if (!found && (last_media_shown != null && last_media_shown.Opacity > 0d))
                {
                    closeAnim = new DoubleAnimation(1d, 0d, new Duration(new TimeSpan(0, 0, 1)));
                    closeAnim.Completed += animation_Completed;
                    Storyboard.SetTarget(closeAnim, last_media_shown);
                    Storyboard.SetTargetProperty(closeAnim, new PropertyPath(MediaElement.OpacityProperty));
                    anim = true;
                    animation.Children.Add(closeAnim);
                    last_media_shown = null;
                }

                if (closeAnim != null && openAnim != null)
                {
                    anim = true;
                    animation.Children.Add(closeAnim);
                }
                if (openAnim != null)
                {
                    anim = true;
                    animation.Children.Add(openAnim);
                }

                if (anim)
                    animation.Begin();
            }
        }

        protected void animation_Completed(object sender, EventArgs e)
        {
            foreach (var item in _OATeSettings.MediaFiles)
            {
                MediaElement element = (item.MediaElement as MediaElement);
                if (element.Opacity == 1d)
                    element.Play();
            }
        }
    }
}
