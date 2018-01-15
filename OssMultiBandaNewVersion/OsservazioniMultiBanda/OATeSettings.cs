using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xaml;
using System.Xml;

namespace OssMultiBanda
{
    public class OATeSettings
    {
        private string _Camera1 = "Integrated Webcam";
        public string Camera1
        {
            get { return _Camera1; }
            set { _Camera1 = value; }
        }

        private string _Camera2 = "";
        public string Camera2
        {
            get { return _Camera2; }
            set { _Camera2 = value; }
        }

        private float _Step = 10;
        public float Step
        {
            get { return _Step; }
            set { _Step = value; }
        }

        private int _AnimationMilliseconds = 1000;
        public int AnimationMilliseconds
        {
            get { return _AnimationMilliseconds; }
            set { _AnimationMilliseconds = value; }
        }

        private string _Barra = "Images/spettro.png";
        public string Barra
        {
            get { return _Barra; }
            set { _Barra = value; }
        }

        private string _Puntatore = "Sole/Puntatore.png";
        public string Puntatore
        {
            get { return _Puntatore; }
            set { _Puntatore = value; }
        }

        private bool _PuntatoreFisso = false;
        public bool PuntatoreFisso
        {
            get { return _PuntatoreFisso; }
            set { _PuntatoreFisso = value; }
        }

        private List<OATeMedia> _MediaFiles = new List<OATeMedia>();
        public List<OATeMedia> MediaFiles
        {
            get { return _MediaFiles; }
            set { _MediaFiles = value; }
        }

        /// <summary>
        /// Use the common dialog to save the settings to a file.
        /// </summary>
        /// <param name="fileName">settings file name to start with</param>
        /// <param name="settings">the settings to save</param>
        public static void SaveSettings(string fileName, OATeSettings settings)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = Path.GetFileName(fileName),
                InitialDirectory = Path.GetDirectoryName(fileName),
                DefaultExt = "xml",
                Filter = Properties.Resources.FileDialogFilter
            };

            if (saveFileDialog.ShowDialog() == false)
            {
                // User canceled the dialog.  Quietly return.
                return;
            }

            fileName = saveFileDialog.FileName;

            var settingsXmlString = XamlServices.Save(settings);

            try
            {
                using (var streamWriter = new StreamWriter(fileName))
                {
                    streamWriter.Write(settingsXmlString);
                }
            }
            catch (Exception e)
            {
                if (e is IOException || e is UnauthorizedAccessException)
                {
                    // Couldn't save for a reason we expect.  Show an error message.
                    var errorMessageText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.SaveErrorMessage, fileName);
                    MessageBox.Show(
                        errorMessageText, Properties.Resources.ErrorMessageCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Use the common dialog to load settings from a file.
        /// </summary>
        /// <param name="fileName">File to start with.</param>
        /// <param name="loadedSettings">The settings that were loaded</param>
        /// <returns>true if we were able to load the settings, false otherwise</returns>
        public static bool TryLoadSettingsWithOpenFileDialog(string fileName, out OATeSettings loadedSettings)
        {
            loadedSettings = null;

            var openFileDialog = new OpenFileDialog
            {
                FileName = Path.GetFileName(fileName),
                InitialDirectory = Path.GetDirectoryName(fileName),
                DefaultExt = "xml",
                Filter = Properties.Resources.FileDialogFilter
            };

            if (openFileDialog.ShowDialog() == false)
            {
                // User canceled the dialog.  Quietly return.
                return false;
            }

            fileName = openFileDialog.FileName;

            if (!TryLoadSettingsNoUi(fileName, out loadedSettings))
            {
                // Couldn't load the file.  Show an error message.
                var errorMessageText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.OpenErrorMessage, fileName);
                MessageBox.Show(
                    errorMessageText, Properties.Resources.ErrorMessageCaption, MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Load the settings from a file with no UI.
        /// </summary>
        /// <param name="fileName">filename to use</param>
        /// <param name="loadedSettings">settings that were loaded</param>
        /// <returns>true if settings were loaded, false otherwise</returns>
        public static bool TryLoadSettingsNoUi(string fileName, out OATeSettings loadedSettings)
        {
            loadedSettings = null;
            if (!File.Exists(fileName))
            {
                return false;
            }

            using (var streamReader = new StreamReader(fileName))
            {
                try
                {
                    var loadedObjects = XamlServices.Load(streamReader.BaseStream);
                    loadedSettings = loadedObjects as OATeSettings;
                    if (loadedSettings == null)
                    {
                        return false;
                    }
                }
                catch (XmlException)
                {
                    return false;
                }
                catch (XamlParseException)
                {
                    return false;
                }
                catch (XamlObjectWriterException)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class OATeMedia
    {
        private double _FromValue;

        public double FromValue
        {
            get { return _FromValue; }
            set { _FromValue = value; }
        }

        private double _ToValue;

        public double ToValue
        {
            get { return _ToValue; }
            set { _ToValue = value; }
        }

        private string _MediaUri;

        public string MediaUri
        {
            get { return _MediaUri; }
            set { _MediaUri = value; }
        }

        private string _PuntatoreSpecifico;

        public string PuntatoreSpecifico
        {
            get { return _PuntatoreSpecifico; }
            set { _PuntatoreSpecifico = value; }
        }

        public object MediaElement { get; set; }
    }
}
