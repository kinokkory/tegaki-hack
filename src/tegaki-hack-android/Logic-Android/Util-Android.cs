using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using NativeColor = Android.Graphics.Color;

namespace tegaki_hack
{
    public delegate void CanvasDelegate(Canvas canvas);

    public class ExtensibleView : View
    {
        public event CanvasDelegate Drawing;

        public ExtensibleView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { }

        public ExtensibleView(Context context, IAttributeSet attrs, int defStyleAttr) :
            base(context, attrs, defStyleAttr)
        { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            Drawing?.Invoke(canvas);
        }
    }

    public class ColorSetter : LinearLayout
    {
        public Button colorIndicator;
        public NumberPicker rPicker, gPicker, bPicker, aPicker;
        public SeekBar rSeekBar, gSeekBar, bSeekBar, aSeekBar, hSeekBar, sSeekBar, lSeekBar;
        public TextView hText, sText, lText;
        public event Action ColorChanged;
        Color _color;
        int prevh, prevs;
        bool setting;

        public Color color
        {
            get { return _color; }
            set { _color = value; colorChanged(); }
        }
        public byte r
        {
            get { return _color.r; }
            set { _color.r = value; colorChanged(); }
        }
        public byte g
        {
            get { return _color.g; }
            set { _color.g = value; colorChanged(); }
        }
        public byte b
        {
            get { return _color.b; }
            set { _color.b = value; colorChanged(); }
        }
        public byte a
        {
            get { return _color.a; }
            set { _color.a = value; colorChanged(); }
        }
        public float h
        {
            get { return _color.h; }
            set { prevh = (int)value; _color = Color.Hsla(value, prevs, l, a); colorChanged(); }
        }
        public float s
        {
            get { return _color.s; }
            set { prevs = (int)value; _color = Color.Hsla(prevh, value, l, a); colorChanged(); }
        }
        public float l
        {
            get { return _color.l; }
            set { _color = Color.Hsla(prevh, prevs, value, a); colorChanged(); }
        }

        public ColorSetter(Context context, IAttributeSet attrs) :
            base(context, attrs)
        { Initialize(); }

        public ColorSetter(Context context, IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
        { Initialize(); }


        void Initialize()
        {
            prevh = prevs = 0;
            setting = false;

            Inflate(Context, Resource.Layout.ColorSetter, this);
            colorIndicator = FindViewById<Button>(Resource.Id.ColorIndicator);
            rPicker = FindViewById<NumberPicker>(Resource.Id.RPicker);
            gPicker = FindViewById<NumberPicker>(Resource.Id.GPicker);
            bPicker = FindViewById<NumberPicker>(Resource.Id.BPicker);
            aPicker = FindViewById<NumberPicker>(Resource.Id.APicker);
            rSeekBar = FindViewById<SeekBar>(Resource.Id.RSeekBar);
            gSeekBar = FindViewById<SeekBar>(Resource.Id.GSeekBar);
            bSeekBar = FindViewById<SeekBar>(Resource.Id.BSeekBar);
            aSeekBar = FindViewById<SeekBar>(Resource.Id.ASeekBar);
            hSeekBar = FindViewById<SeekBar>(Resource.Id.HSeekBar);
            sSeekBar = FindViewById<SeekBar>(Resource.Id.SSeekBar);
            lSeekBar = FindViewById<SeekBar>(Resource.Id.LSeekBar);
            hText = FindViewById<TextView>(Resource.Id.HText);
            sText = FindViewById<TextView>(Resource.Id.SText);
            lText = FindViewById<TextView>(Resource.Id.LText);

            rPicker.MinValue = gPicker.MinValue = bPicker.MinValue = aPicker.MinValue = 0;
            rPicker.MaxValue = gPicker.MaxValue = bPicker.MaxValue = aPicker.MaxValue = 255;
            var ffstrings = new List<string>();
            for (int i = 0; i < 256; i++)
            {
                ffstrings.Add(string.Format("{0:X2}", i));
            }
            var affstrings = ffstrings.ToArray();
            rPicker.SetDisplayedValues(affstrings);
            gPicker.SetDisplayedValues(affstrings);
            bPicker.SetDisplayedValues(affstrings);
            aPicker.SetDisplayedValues(affstrings);

            colorIndicator.Click += (o, e) =>
            {
                Util.ShowToast(Context, "color");
            };

            rPicker.ValueChanged += (o, e) => { if (!setting) r = (byte)rPicker.Value; };
            gPicker.ValueChanged += (o, e) => { if (!setting) g = (byte)gPicker.Value; };
            bPicker.ValueChanged += (o, e) => { if (!setting) b = (byte)bPicker.Value; };
            aPicker.ValueChanged += (o, e) => { if (!setting) a = (byte)aPicker.Value; };
            rSeekBar.ProgressChanged += (o, e) => { if (!setting) r = (byte)rSeekBar.Progress; };
            gSeekBar.ProgressChanged += (o, e) => { if (!setting) g = (byte)gSeekBar.Progress; };
            bSeekBar.ProgressChanged += (o, e) => { if (!setting) b = (byte)bSeekBar.Progress; };
            aSeekBar.ProgressChanged += (o, e) => { if (!setting) a = (byte)aSeekBar.Progress; };
            hSeekBar.ProgressChanged += (o, e) => { if (!setting) h = hSeekBar.Progress; };
            sSeekBar.ProgressChanged += (o, e) => { if (!setting) s = sSeekBar.Progress; };
            lSeekBar.ProgressChanged += (o, e) => { if (!setting) l = lSeekBar.Progress; };
        }

        void SetControls()
        {
            setting = true;

            colorIndicator.BackgroundTintList = ColorStateList.ValueOf(_color.native);
            rPicker.Value = r;
            gPicker.Value = g;
            bPicker.Value = b;
            aPicker.Value = a;
            rSeekBar.Progress = r;
            gSeekBar.Progress = g;
            bSeekBar.Progress = b;
            aSeekBar.Progress = a;
            hSeekBar.Progress = float.IsNaN(h) ? prevh : (prevh = (int)Math.Round(h) % 360);
            sSeekBar.Progress = s == 0 ? prevs : (prevs = (int)Math.Round(s));
            lSeekBar.Progress = (int)Math.Round(l);

            rSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.Hsla(0, r * 100.0f / 255.0f, 50, 255).native);
            gSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.Hsla(120, g * 100.0f / 255.0f, 50, 255).native);
            bSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.Hsla(240, b * 100.0f / 255.0f, 50, 255).native);
            aSeekBar.ThumbTintList = ColorStateList.ValueOf(Color.Rgba(r, g, b, (byte)((a + 64.0f) * 255.0f / 319.0f)).native);
            var hColors = ColorStateList.ValueOf(Color.Hsla(prevh, 100, 50, 255).native);
            hText.SetTextColor(hColors);
            hSeekBar.ThumbTintList = hColors;
            var sColors = ColorStateList.ValueOf(Color.Hsla(prevh, prevs, 50, 255).native);
            sText.SetTextColor(sColors);
            sSeekBar.ThumbTintList = sColors;
            var lColors = ColorStateList.ValueOf(Color.Hsla(prevh, 100, l, 255).native);
            lText.SetTextColor(lColors);
            lSeekBar.ThumbTintList = lColors;

            setting = false;
        }

        void colorChanged()
        {
            ColorChanged?.Invoke();
            SetControls();
        }
    }

    public static partial class Util
    {
        public static void Activate(this View view, bool activate)
        {
            view.BackgroundTintList = !activate ? null : ColorStateList.ValueOf(NativeColor.Orange);
        }

        public static void ShowToast(Context context, string text, ToastLength duration = ToastLength.Short)
        {
            Toast.MakeText(context, text, duration).Show();
        }
    }
}