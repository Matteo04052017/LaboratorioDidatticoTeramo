//------------------------------------------------------------------------------
// <copyright file="AdaptiveZoneVisualizer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace OssMultiBanda
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using System.Diagnostics;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Displays the adaptive zones, user positions, and sensor and display
    /// layout.
    /// </summary>
    public class AdaptiveZoneVisualizer : Control
    {
        public event EventHandler<ValueEventArgs> ValueReached;


        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "OverrideMetadata cannot be done inline.")]
        static AdaptiveZoneVisualizer()
        {
        }


        private bool _draw = false;

        public bool Draw
        {
            get { return _draw; }
            set { _draw = value; }
        }

        private float bar2Draw = 0;
        public float Step
        {
            get { return _OATeSettings.Step; }
            set { _OATeSettings.Step = value; }
        }

        public float Bar2Draw
        {
            get { return bar2Draw; }
            set { bar2Draw = value; }
        }

        public void Increase()
        {
            if (Bar2Draw < this.ActualWidth)
                Bar2Draw = Bar2Draw + Step;
            if (Bar2Draw > this.ActualWidth)
                Bar2Draw = (float)this.ActualWidth;
            Draw = true;
            this.InvalidateVisual();
        }

        private bool _StartPoint = false;
        public void StartPoint()
        {
            Bar2Draw = (float)ActualWidth / 2;
            _StartPoint = Draw = true;
            this.InvalidateVisual();
        }

        public void Decrease()
        {
            if (Bar2Draw > 0)
                Bar2Draw = Bar2Draw - Step;
            if (Bar2Draw < 0)
                Bar2Draw = 0;
            Draw = true;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Does all the drawing of the visualizer.
        /// </summary>
        /// <param name="drawingContext">DrawingContext to draw to</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }

            base.OnRender(drawingContext);

            //if (Bar2Draw == 0)
            //    Bar2Draw = (float)ActualWidth / 2;

            if (Draw)
            {
                Draw = false;

                this.PlotSkeleton(drawingContext, bar2Draw);
            }
        }

        private OATeSettings _OATeSett;
        public OATeSettings _OATeSettings
        {
            get { return _OATeSett; }
            set
            {
                _OATeSett = value;
                StartPoint();
            }
        }

        public double LastDrawX = -1;

        private void PlotSkeleton(DrawingContext drawingContext, float x)
        {
            ValueEventArgs arg = GetValueEventArs();

            double pointX = x;
            double width = 0;

            string Puntatore = _OATeSettings.Puntatore;
            foreach (var item in _OATeSettings.MediaFiles)
                if (arg.Value >= item.FromValue && arg.Value < item.ToValue)
                    if (!string.IsNullOrEmpty(item.PuntatoreSpecifico))
                    {
                        Puntatore = item.PuntatoreSpecifico;
                        if (_OATeSettings.PuntatoreFisso)
                        {
                            pointX = (item.FromValue + ((item.ToValue - item.FromValue) / 2)) * this.ActualWidth;
                            width = ((item.ToValue - item.FromValue) / 2) * this.ActualWidth;
                        }
                    }


            ImageSource image = new BitmapImage(new Uri(Puntatore, UriKind.RelativeOrAbsolute));
            if (width == 0)
                width = image.Width;
            Size size = new Size(image.Width, image.Height);
            //if (size.Height > this.ActualHeight)
            //{
            size.Height = Height;
            //size.Width = image.Width * size.Height / image.Height;
            //}
            Rect rect = new Rect(size);
            rect.X = x;
            rect.Y = 0; // this.ActualHeight / 2;
            var diff = rect.X - LastDrawX;
            diff = (diff > 0) ? diff : diff * -1;
            if (diff > 2)
            {
                LastDrawX = rect.X;
                OnValueReached(arg);
            }
            else
                rect.X = LastDrawX;

            if (rect.X > this.ActualWidth)
                rect.X = this.ActualWidth - size.Width;

            if (_StartPoint)
            {
                _StartPoint = false;
                OnValueReached(arg);
            }

            drawingContext.DrawImage(image, rect);
        }

        private ValueEventArgs GetValueEventArs()
        {
            ValueEventArgs arg = new ValueEventArgs();
            // this.ActualWidth : bar2Draw = 100 : x
            arg.Value = bar2Draw / this.ActualWidth;
            //if (arg.Value < 0)
            //    arg.Value = 0;
            //if (arg.Value > 1)
            //    arg.Value = 1;
            return arg;
        }

        protected virtual void OnValueReached(ValueEventArgs e)
        {
            EventHandler<ValueEventArgs> handler = ValueReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class ValueEventArgs : EventArgs
    {
        public double Value { get; set; }
    }
}
