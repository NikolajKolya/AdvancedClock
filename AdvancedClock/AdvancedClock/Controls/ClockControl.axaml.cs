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
        /// Длина до конца засечки как доля от радиуса часов
        /// </summary>
        private const double HandLength = 0.75;

        /// <summary>
        /// Толщина засечки (в пикселях)
        /// </summary>
        private const double HandThikness = 1;

        /// <summary>
        /// Коэффициент который коректирует положение цифр
        /// </summary>
        private const double CoefficientOfNumbers = 0.95;

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

        int minSide;

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

            minSide = Math.Min(_width, _height);
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
            DrawHoursHand(context, 9);
            DrawMinutesHand(context, 37);
            DrawSecondsHand(context, 15);
            for (double i = 1; i <= 12; i++)
            {
                DrawNumbers(context, i);
            }
        }

        /// <summary>
        /// Нарисовать стрелку из центра заданной толщины
        /// </summary>
        private void DrawHandRaw(DrawingContext context, Point end, double thikness)
        {
            context.DrawLine(new Pen(new SolidColorBrush(ClockForegroundColor), thikness), _centerPoint, end);
        }

        /// <summary>
        /// Метод рисует засечки
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
        /// Метод пишет номера
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
        /// Прорисовывает цифры циферблата и линии
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hours"></param>
        private void DrawNumbers(DrawingContext context, double hours)
        {
            ///<summary>
            /// Растояние до конца засечки
            /// </summary>
            var length1 = _clockRadius * HandLength;

            ///<summary>
            /// Растояние до начала засечки
            /// </summary>
            var length2 = _clockRadius * (HandLength - 0.1);

            ///<summary>
            /// Растояние до номеров
            /// </summary>
            var length3 = _clockRadius * (HandLength + 0.14);

            var degrees = (hours / 12) * 2.0 * Math.PI;

            ///<summary>
            /// Точки начала и конца засечки
            /// </summary>
            var x = Math.Sin(degrees) * length1 + _centerPoint.X;
            var y = -1 * Math.Cos(degrees) * length1 + _centerPoint.Y;
            var a = Math.Sin(degrees) * length2 + _centerPoint.X;
            var b = -1 * Math.Cos(degrees) * length2 + _centerPoint.Y;

            ///<summary>
            /// Точка на которой находится номер
            /// </summary>
            var xNumber = Math.Sin(degrees) * length3 + _centerPoint.X * CoefficientOfNumbers;
            var yNumber = -1 * Math.Cos(degrees) * length3 + _centerPoint.Y * CoefficientOfNumbers;

            ///<summary>
            /// Вызов метода который рисует засечки
            /// </summary>
            DrawLines(context, new Point(x, y), HandThikness , new Point(a, b));

            ///<summary>
            /// Вызов метода который рисует Номера
            /// </summary>
            DrawNumber(context, hours.ToString(), new Point(xNumber, yNumber));
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
