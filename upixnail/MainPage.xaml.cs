using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace upixnail {
    public sealed partial class MainPage : Page {
        const float DefaultZoomFactor = 1.5f;

        CanvasVirtualBitmap _bitmap = null;
        ScaleEffect _scaleEffect = new ScaleEffect();

        public MainPage()
        {
            InitializeComponent();
            _scaleEffect.InterpolationMode = CanvasImageInterpolation.NearestNeighbor;
        }

        private async void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                using (var stream = await file.OpenReadAsync()) {
                    _bitmap = await CanvasVirtualBitmap.LoadAsync(
                        ImageCanvas,
                        stream,
                        CanvasVirtualBitmapOptions.CacheOnDemand);
                }
                _scaleEffect.Source = _bitmap;
                _scaleEffect.Scale = new Vector2(1, 1);
                ImageCanvas.Width = _bitmap.Size.Width;
                ImageCanvas.Height = _bitmap.Size.Height;
                ImageCanvas.Invalidate();
            }
        }

        private void ImageCanvas_Draw(
            CanvasControl sender,
            CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;
            if (_bitmap != null) {
                session.DrawImage(_scaleEffect);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ImageCanvas.RemoveFromVisualTree();
            ImageCanvas = null;
        }

        void ZoomUpdated()
        {
            ImageCanvas.Width = _bitmap.Size.Width * _scaleEffect.Scale.X;
            ImageCanvas.Height = _bitmap.Size.Height * _scaleEffect.Scale.Y;
            ZoomText.Text = String.Format("{0} %", (uint)(_scaleEffect.Scale.X * 100));
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _scaleEffect.Scale = Vector2.Multiply(_scaleEffect.Scale, DefaultZoomFactor);
            ZoomUpdated();
            ImageCanvas.Invalidate();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _scaleEffect.Scale = Vector2.Divide(_scaleEffect.Scale, DefaultZoomFactor);
            ZoomUpdated();
            ImageCanvas.Invalidate();
        }

        private async void ColorPicker_Click(object sender, RoutedEventArgs e)
        {
            var colorPicker = new ColorPickerDialog();
            await colorPicker.ShowAsync();
        }
    }
}
