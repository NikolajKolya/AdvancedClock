using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Threading;
//using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
//using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Timers;
//using Avalonia;
using Timer = System.Threading.Timer;
using System.Security.Cryptography;

namespace AdvancedClock.Controls
{
    public partial class ClockControl : UserControl
    {
        #region Constants

        /// <summary>
        /// ����� �� ����� ������� ��� ���� �� ������� �����
        /// </summary>
        private const double HandLength = 0.75;

        /// <summary>
        /// ������� ������� (� ��������)
        /// </summary>
        private const double HandThikness = 1;

        /// <summary>
        /// ����������� ������� ����������� ��������� ����
        /// </summary>
        private const double CoefficientOfNumbers = 0.95;

        /// <summary>
        /// ��������� ������� ����� (������� * �� ��������� = ������ �����)
        /// </summary>
        private const double RadiusMultiplier = 0.45;

        /// <summary>
        /// ��� �����
        /// </summary>
        private readonly Color ClockBackgroundColor = new Color(255, 230, 230, 255);

        /// <summary>
        /// ������ ���� ���� ������
        /// </summary>
        private readonly Color ClockForegroundColor = new Color(255, 0, 0, 0);

        /// <summary>
        /// ����� ��������� ������� ��� ���� �� ������� �����
        /// </summary>
        private const double SecondsHandLength = 0.9;

        /// <summary>
        /// ������� ��������� ������� (� ��������)
        /// </summary>
        private const double SecondsHandThikness = 1.0;

        /// <summary>
        /// ����� �������� ������� ��� ���� �� ������� �����
        /// </summary>
        private const double MinutesHandLength = 0.7;

        /// <summary>
        /// ������� �������� ������� (� ��������)
        /// </summary>
        private const double MinutesHandThikness = 2.0;

        /// <summary>
        /// ����� ������� ������� ��� ���� �� ������� �����
        /// </summary>
        private const double HoursHandLength = 0.5;

        /// <summary>
        /// ������� ������� ������� (� ��������)
        /// </summary>
        private const double HoursHandThikness = 3.0;

        #endregion

        int minSide;

        private double _scaling;

        private int _width;
        private int _height;

        private Point _centerPoint;

        private double _clockRadius;

        public ClockControl()
        {
            InitializeComponent();

            // ������������� �� ��������� ������� ����
            PropertyChanged += OnPropertyChangedListener;
        }

        /// <summary>
        /// ��������� ��������� ������� ����
        /// </summary>
        private void OnPropertyChangedListener(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name.Equals("Bounds")) // ���� �������� ������ ����
            {
                // ���������� ��������� �������
                OnResize((Rect)e.NewValue);
            }
        }

        /// <summary>
        /// ���������� ��� ���������� �������� ����
        /// </summary>
        private void OnResize(Rect bounds)
        {
            _scaling = VisualRoot.RenderScaling;

            _width = (int)(bounds.Width * _scaling);
            _height = (int)(bounds.Height * _scaling);

            _centerPoint = new Point(_width / 2.0, _height / 2.0);

            minSide = Math.Min(_width, _height);
            _clockRadius = RadiusMultiplier * minSide;
        }

        /// <summary>
        /// ��� ����� ��������� ����������� ��������
        /// </summary>
        /// <param name="context"></param>
        public override void Render(DrawingContext context)
        {
            base.Render(context); // ���������� � ������ � ���� ������ ���������� ���� �����

            // ���������
            context.DrawEllipse(new SolidColorBrush(ClockBackgroundColor),
                new Pen(new SolidColorBrush(ClockForegroundColor)),
                _centerPoint,
                _clockRadius,
                _clockRadius);

            // �������
            DrawHoursHand(context, 9);
            DrawMinutesHand(context, 37);
            DrawSecondsHand(context, 15);
            for (double i = 1; i <= 12; i++)
            {
                DrawNumbers(context, i);
            }
        }

        /// <summary>
        /// ���������� ������� �� ������ �������� �������
        /// </summary>
        private void DrawHandRaw(DrawingContext context, Point end, double thikness)
        {
            context.DrawLine(new Pen(new SolidColorBrush(ClockForegroundColor), thikness), _centerPoint, end);
        }

        /// <summary>
        /// ����� ������ �������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="end"></param>
        /// <param name="thikness"></param>
        /// <param name="begin"></param>
        private void DrawLines(DrawingContext context, Point end, double thikness, Point begin)
        {
            context.DrawLine(new Pen(new SolidColorBrush(ClockForegroundColor), thikness), begin, end);
        }
        
        /// <summary>
        /// ����� ����� ������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hours"></param>
        /// <param name="Number"></param>
        private void DrawNumber(DrawingContext context, string hours, Point Number)
        {
            context.DrawText(new SolidColorBrush(ClockForegroundColor),
            Number,
            new FormattedText(hours, Typeface.Default, minSide / 18, 0, TextWrapping.NoWrap, new Size(double.MinValue, double.MaxValue)));
        }

        /// <summary>
        /// ������������� ����� ���������� � �����
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hours"></param>
        private void DrawNumbers(DrawingContext context, double hours)
        {
            ///<summary>
            /// ��������� �� ����� �������
            /// </summary>
            var length1 = _clockRadius * HandLength;

            ///<summary>
            /// ��������� �� ������ �������
            /// </summary>
            var length2 = _clockRadius * (HandLength - 0.1);

            ///<summary>
            /// ��������� �� �������
            /// </summary>
            var length3 = _clockRadius * (HandLength + 0.14);

            var degrees = (hours / 12) * 2.0 * Math.PI;

            ///<summary>
            /// ����� ������ � ����� �������
            /// </summary>
            var x = Math.Sin(degrees) * length1 + _centerPoint.X;
            var y = -1 * Math.Cos(degrees) * length1 + _centerPoint.Y;
            var a = Math.Sin(degrees) * length2 + _centerPoint.X;
            var b = -1 * Math.Cos(degrees) * length2 + _centerPoint.Y;

            ///<summary>
            /// ����� �� ������� ��������� �����
            /// </summary>
            var xNumber = Math.Sin(degrees) * length3 + _centerPoint.X * CoefficientOfNumbers;
            var yNumber = -1 * Math.Cos(degrees) * length3 + _centerPoint.Y * CoefficientOfNumbers;

            ///<summary>
            /// ����� ������ ������� ������ �������
            /// </summary>
            DrawLines(context, new Point(x, y), HandThikness , new Point(a, b));

            ///<summary>
            /// ����� ������ ������� ������ ������
            /// </summary>
            DrawNumber(context, hours.ToString(), new Point(xNumber, yNumber));
        }

        /// <summary>
        /// ��������� � ���� ���������� ������ � ������ ��������� �������
        /// </summary>
        private void DrawSecondsHand(DrawingContext context, double seconds)
        {
            var degrees = (seconds / 60.0) * 2.0 * Math.PI;

            var length = _clockRadius * SecondsHandLength;

            var x = Math.Sin(degrees) * length + _centerPoint.X;
            var y = -1 * Math.Cos(degrees) * length + _centerPoint.Y;

            DrawHandRaw(context, new Point(x, y), SecondsHandThikness);
        }

        private void DrawMinutesHand(DrawingContext context, double minutes)
        {
            var degrees = (minutes / 60.0) * 2.0 * Math.PI;

            var length = _clockRadius * MinutesHandLength;

            var x = Math.Sin(degrees) * length + _centerPoint.X;
            var y = -1 * Math.Cos(degrees) * length + _centerPoint.Y;

            DrawHandRaw(context, new Point(x, y), MinutesHandThikness);
        }

        private void DrawHoursHand(DrawingContext context, double hours)
        {
            var degrees = (hours / 12.0) * 2.0 * Math.PI;

            var length = _clockRadius * HoursHandLength;

            var x = Math.Sin(degrees) * length + _centerPoint.X;
            var y = -1 * Math.Cos(degrees) * length + _centerPoint.Y;

            DrawHandRaw(context, new Point(x, y), HoursHandThikness);
        }
    }
}
