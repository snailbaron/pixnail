using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace upixnail {
    class ColorState : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged = delegate {};

        class RGBComponent {
            public byte R;
            public byte G;
            public byte B;

            public RGBComponent(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }
        }

        class HSVComponent {
            public ushort H;
            public byte S;
            public byte V;

            public HSVComponent(ushort h, byte s, byte v)
            {
                H = h;
                S = s;
                V = v;
            }
        }

        RGBComponent _rgb = new RGBComponent(0, 0, 0);
        HSVComponent _hsv = new HSVComponent(0, 0, 0);

        // RGB properties
        public byte RGB_R {
            get { return _rgb.R; }
            set {
                if (_rgb.R != value) {
                    _rgb.R = value;
                    UpdateFromRGB();
                }
            }
        }
        public byte RGB_G {
            get { return _rgb.G; }
            set {
                if (_rgb.G != value) {
                    _rgb.G = value;
                    UpdateFromRGB();
                }
            }
        }
        public byte RGB_B {
            get { return _rgb.B; }
            set {
                if (_rgb.B != value) {
                    _rgb.B = value;
                    UpdateFromRGB();
                }
            }
        }

        // HSV properties
        public ushort HSV_H {
            get { return _hsv.H; }
            set {
                if (_hsv.H != value) {
                    _hsv.H = value;
                    UpdateFromHSV();
                }
            }
        }
        public byte HSV_S {
            get { return _hsv.S; }
            set {
                if (_hsv.S != value) {
                    _hsv.S = value;
                    UpdateFromHSV();
                }
            }
        }
        public byte HSV_V {
            get { return _hsv.V; }
            set {
                if (_hsv.V != value) {
                    _hsv.V = value;
                    UpdateFromHSV();
                }
            }
        }

        public SolidColorBrush ColorBrush {
            get { return new SolidColorBrush(Color.FromArgb(255, _rgb.R, _rgb.G, _rgb.B)); }
        }

        void AllPropsChanged()
        {
        }

        void UpdateFromRGB()
        {
            Convert(_rgb, _hsv);
            PropertyChanged(this, new PropertyChangedEventArgs("HSV_H"));
            PropertyChanged(this, new PropertyChangedEventArgs("HSV_S"));
            PropertyChanged(this, new PropertyChangedEventArgs("HSV_V"));
            PropertyChanged(this, new PropertyChangedEventArgs("ColorBrush"));
            //AllPropsChanged();
        }

        void UpdateFromHSV()
        {
            Convert(_hsv, _rgb);
            PropertyChanged(this, new PropertyChangedEventArgs("RGB_R"));
            PropertyChanged(this, new PropertyChangedEventArgs("RGB_G"));
            PropertyChanged(this, new PropertyChangedEventArgs("RGB_B"));
            PropertyChanged(this, new PropertyChangedEventArgs("ColorBrush"));
            //AllPropsChanged();
        }

        void Convert(HSVComponent hsv, RGBComponent rgb)
        {
            double rs = hsv.S / 100.0;
            double rv = hsv.V / 100.0;

            double c = rv * rs;


            double x = c * (1 - Math.Abs(((hsv.H / 60.0) % 2) - 1));
            double m = rv - c;

            Tuple<double, double, double> rrgb;
            if (hsv.H < 60) {
                rrgb = new Tuple<double, double, double>(c, x, 0);
            } else if (hsv.H < 120) {
                rrgb = new Tuple<double, double, double>(x, c, 0);
            } else if (hsv.H < 180) {
                rrgb = new Tuple<double, double, double>(0, c, x);
            } else if (hsv.H < 240) {
                rrgb = new Tuple<double, double, double>(0, x, c);
            } else if (hsv.H < 300) {
                rrgb = new Tuple<double, double, double>(x, 0, c);
            } else {
                rrgb = new Tuple<double, double, double>(c, 0, x);
            }

            rgb.R = (byte)((rrgb.Item1 + m) * 255.0);
            rgb.G = (byte)((rrgb.Item2 + m) * 255.0);
            rgb.B = (byte)((rrgb.Item3 + m) * 255.0);
        }

        void Convert(RGBComponent rgb, HSVComponent hsv)
        {
            double rr = rgb.R / 255.0;
            double rg = rgb.G / 255.0;
            double rb = rgb.B / 255.0;

            double cmax = Math.Max(rr, Math.Max(rg, rb));
            double cmin = Math.Min(rr, Math.Min(rg, rb));
            double delta = cmax - cmin;

            if (delta == 0.0) {
                hsv.H = 0;
            } else if (cmax == rr) {
                hsv.H = (ushort)(60 * ((int)((rg - rb) / delta) % 6));
            } else if (cmax == rg) {
                hsv.H = (ushort)(60 * ((rb - rr) / delta + 2));
            } else {
                hsv.H = (ushort)(60 * ((rr - rg) / delta + 4));
            }

            if (cmax == 0.0) {
                hsv.S = 0;
            } else {
                hsv.S = (byte)((delta / cmax) * 100.0);
            }

            hsv.V = (byte)(cmax * 100.0);
        }
    }
}
