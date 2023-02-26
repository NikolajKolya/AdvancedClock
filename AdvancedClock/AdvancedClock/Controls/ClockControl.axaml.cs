using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace AdvancedClock.Controls
{
    public partial class ClockControl : UserControl
    {
        #region Constants

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

            var minSide = Math.Min(_width, _height);
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
            DrawHoursHand(context, 1);
            DrawMinutesHand(context, 37);
            DrawSecondsHand(context, 13);

        }

        /// <summary>
        /// ���������� ������� �� ������ �������� �������
        /// </summary>
        private void DrawHandRaw(DrawingContext context, Point end, double thikness)
        {
            context.DrawLine(new Pen(new SolidColorBrush(ClockForegroundColor), thikness), _centerPoint, end);
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
