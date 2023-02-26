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
        /// Множитель радуиса часов (сторона * на множитель = радиус часов)
        /// </summary>
        private const double RadiusMultiplier = 0.45;

        /// <summary>
        /// Фон часов
        /// </summary>
        private readonly Color ClockBackgroundColor = new Color(255, 230, 230, 255);

        /// <summary>
        /// Рисуем часы этим цветом
        /// </summary>
        private readonly Color ClockForegroundColor = new Color(255, 0, 0, 0);

        /// <summary>
        /// Длина секундной стрелки как доля от радиуса часов
        /// </summary>
        private const double SecondsHandLength = 0.9;

        /// <summary>
        /// Толщина секундной стрелки (в пикселях)
        /// </summary>
        private const double SecondsHandThikness = 1.0;

        /// <summary>
        /// Длина минутной стрелки как доля от радиуса часов
        /// </summary>
        private const double MinutesHandLength = 0.7;

        /// <summary>
        /// Толщина минутной стрелки (в пикселях)
        /// </summary>
        private const double MinutesHandThikness = 2.0;

        /// <summary>
        /// Длина часовой стрелки как доля от радиуса часов
        /// </summary>
        private const double HoursHandLength = 0.5;

        /// <summary>
        /// Толщина часовой стрелки (в пикселях)
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

            // Подписываемся на изменение свойств окна
            PropertyChanged += OnPropertyChangedListener;
        }

        /// <summary>
        /// Слушатель изменения свойств окна
        /// </summary>
        private void OnPropertyChangedListener(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name.Equals("Bounds")) // Если меняется размер окна
            {
                // Обработать изменение размера
                OnResize((Rect)e.NewValue);
            }
        }

        /// <summary>
        /// Вызывается при измененеии размеров окна
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
        /// Это метод рисования содержимого контрола
        /// </summary>
        /// <param name="context"></param>
        public override void Render(DrawingContext context)
        {
            base.Render(context); // Обратиться к предку и дать предку нарисовать свои части

            // Циферблат
            context.DrawEllipse(new SolidColorBrush(ClockBackgroundColor),
                new Pen(new SolidColorBrush(ClockForegroundColor)),
                _centerPoint,
                _clockRadius,
                _clockRadius);

            // Стрелки
            DrawHoursHand(context, 1);
            DrawMinutesHand(context, 37);
            DrawSecondsHand(context, 13);

        }

        /// <summary>
        /// Нарисовать стрелку из центра заданной толщины
        /// </summary>
        private void DrawHandRaw(DrawingContext context, Point end, double thikness)
        {
            context.DrawLine(new Pen(new SolidColorBrush(ClockForegroundColor), thikness), _centerPoint, end);
        }

        /// <summary>
        /// Принимает в себя количество секунд и рисует секундную стрелку
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
