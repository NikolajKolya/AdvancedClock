using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AdvancedClock.Enums;
using Avalonia.Media;

namespace AdvancedClock.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        /// <summary>
        /// Начальное количество секунд на таймере
        /// </summary>
        private const int TimerSeconds = 10;

        /// <summary>
        /// Обычный цвет таймера
        /// </summary>
        private readonly IBrush NormalTimerColor = new SolidColorBrush(new Color(255, 0, 0, 0));

        /// <summary>
        /// Таймер, привлекающий внимание
        /// </summary>
        private readonly IBrush WarningTimerColor = new SolidColorBrush(new Color(255, 255, 0, 0));

        /// <summary>
        /// Таймер обновления часов
        /// </summary>
        private Timer _clockUpdateTimer;

        /// <summary>
        /// Таймер секундомера
        /// </summary>
        private Timer _stopwatchTimer;

        /// <summary>
        /// Секунды секундомера в виде строки
        /// </summary>
        private string _stopwatchAsString;

        private string _timerSecopndButtonName;

        /// <summary>
        /// Секунды секундомера
        /// </summary>
        private TimeSpan _stopwatchInterval;

        private string _TimeAsString;

        #region Timer

        /// <summary>
        /// Сколько осталось на таймере
        /// </summary>
        private TimeSpan _timerInterval;

        /// <summary>
        /// Время таймера в виде строки
        /// </summary>
        private string _timerAsString;

        /// <summary>
        /// Таймер таймера
        /// </summary>
        private Timer _stopTimer;

        /// <summary>
        /// Состояние таймера
        /// </summary>
        private TimerState _timerState = TimerState.Stopped;

        private IBrush _timerColor;

        /// <summary>
        /// Цвет текста таймера
        /// </summary>
        public IBrush TimerColor
        {
            get => _timerColor;
            set => this.RaiseAndSetIfChanged(ref _timerColor, value);
        }

        private string _timerIntervalValueAsString;

        /// <summary>
        /// Интервал таймера, указанный пользователем
        /// </summary>
        public string TimerIntervalValueAsString
        {
            get => _timerIntervalValueAsString;
            set => this.RaiseAndSetIfChanged(ref _timerIntervalValueAsString, value);
        }

        #endregion

        /// <summary>
        /// Публичная переменная с временем в виде строки
        /// </summary>
        public string TimeAsString
        {
            get => _TimeAsString;
            set => this.RaiseAndSetIfChanged(ref _TimeAsString, value);
        }

        /// <summary>
        /// Публичная переменная с секундами секундомера
        /// </summary>
        public string StopwatchAsString
        {
            get => _stopwatchAsString;
            set => this.RaiseAndSetIfChanged(ref _stopwatchAsString, value);
        }

        public string TaimerAsString
        {
            get => _timerAsString;
            set => this.RaiseAndSetIfChanged(ref _timerAsString, value);
        }

        public string TimerSecondButtonName
        {
            get => _timerSecopndButtonName;
            set => this.RaiseAndSetIfChanged(ref _timerSecopndButtonName, value);
        }

        /// <summary>
        /// Нажатие на кнопку запуска секундомера
        /// </summary>
        public ReactiveCommand<Unit, Unit> StopwatchStartCommand { get; }

        /// <summary>
        /// press on button stop
        /// </summary>
        public ReactiveCommand<Unit, Unit> StopwatchStopCommand { get; }

        /// <summary>
        /// sbros
        /// </summary>
        public ReactiveCommand<Unit, Unit> ResetStopWatchCommand { get; }

        public ReactiveCommand<Unit, Unit> StartTimer { get; }

        public ReactiveCommand<Unit, Unit> PauseTimer { get; }

        public MainWindowViewModel()
        {
            // Настраиваем таймер часов
            _clockUpdateTimer = new Timer(500); // Срабатывать раз в 500 миллисекунд
            _clockUpdateTimer.AutoReset = true; // Бесконечно повторяться
            _clockUpdateTimer.Enabled = true; // Таймер включён

            _clockUpdateTimer.Elapsed += OnClockUpdateTimer;

            _stopTimer = new Timer(1000);
            _stopTimer.AutoReset = true;
            _stopTimer.Enabled = false;
            _stopTimer.Elapsed += OnTimerTick;

            // Настраиваем секундомер
            _stopwatchInterval = new TimeSpan();
            FormatStopwatchTimeSpan();

            _stopwatchTimer = new Timer(1000);
            _stopwatchTimer.AutoReset = true;
            _stopwatchTimer.Enabled = false;
            _stopwatchTimer.Elapsed += OnStopwatchTimer;

            // Связываем команды секундомера с методами
            StopwatchStartCommand = ReactiveCommand.Create(StartStopwatch);
            StopwatchStopCommand = ReactiveCommand.Create(StopStopwatch);
            ResetStopWatchCommand = ReactiveCommand.Create(ResetStopwatch);

            // Настраиваем таймер
            ResetTimer();

            SetTimerState(TimerState.Stopped);

            // Связываем команды таймера
            StartTimer = ReactiveCommand.Create(TaimerStarter);
            PauseTimer = ReactiveCommand.Create(PauseTaimer);
        }

        /// <summary>
        /// Этот метод вызывается когда дотикает до 0 таймер часов
        /// </summary>
        private void OnClockUpdateTimer(Object source, ElapsedEventArgs e)
        {
            TimeAsString = DateTime.Now.ToLongTimeString();
        }
        
        /// <summary>
        /// Этот метод преобразует интервал секундомера в строку
        /// </summary>
        private void FormatStopwatchTimeSpan()
        {
            StopwatchAsString = _stopwatchInterval.ToString();
        }

        /// <summary>
        /// Этот метод вызывается при нажатии на кнопку запуска секундомера
        /// </summary>
        private void StartStopwatch()
        {
            _stopwatchTimer.Start();
        }

        /// <summary>
        /// Stopping StopWatch
        /// </summary>
        private void StopStopwatch()
        {
            _stopwatchTimer.Stop();
        }

        /// <summary>
        /// Reset stopwatch
        /// </summary>
        private void ResetStopwatch()
        {
            _stopwatchInterval = new TimeSpan();
            FormatStopwatchTimeSpan();
        }

        /// <summary>
        /// Тик таймера
        /// </summary>
        private void OnTimerTick(object? sender, ElapsedEventArgs e)
        {
            _timerInterval -= new TimeSpan(0, 0, 1);
            DisplayTimerValue();

            if (_timerInterval.TotalSeconds <= 5)
            {
                TimerColor = WarningTimerColor;
            }

            if (_timerInterval.TotalSeconds == 0)
            {
                _stopTimer.Stop();
                SetTimerState(TimerState.Stopped);

                ResetTimer();
            }
        }

        /// <summary>
        /// Сброс таймера в начальное положение
        /// </summary>
        private void ResetTimer()
        {
            _timerInterval = new TimeSpan(0, 0, TimerSeconds);
            TimerIntervalValueAsString = _timerInterval.TotalSeconds.ToString();
            TimerColor = NormalTimerColor;
            DisplayTimerValue();
        }

        /// <summary>
        /// Этот метод вызывается, когда таймер секундомера дотикает до 0
        /// </summary>
        private void OnStopwatchTimer(object? sender, ElapsedEventArgs e)
        {
            _stopwatchInterval += new TimeSpan(0, 0, 1);
            FormatStopwatchTimeSpan();
        }

        private void TaimerStarter()
        {
            if (_timerState == TimerState.Stopped)
            {
                int numberOfSeconds = 0;
                try
                {
                    numberOfSeconds = int.Parse(TimerIntervalValueAsString);
                }
                catch(Exception)
                {
                    var errorMessageBox = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("Error", "Вы ввели не число!");
                    errorMessageBox.Show();

                    return;
                }
                
                _timerInterval = new TimeSpan(0, 0, numberOfSeconds);

                _stopTimer.Start();

                SetTimerState(TimerState.Running);

                return;
            }

            if (_timerState == TimerState.Paused)
            {
                _stopTimer.Start();

                SetTimerState(TimerState.Running);

                return;
            }
        }

        private void PauseTaimer()
        {
            if (_timerState == TimerState.Stopped)
            {
                return;
            }

            if (_timerState == TimerState.Running)
            {
                _stopTimer.Stop();

                SetTimerState(TimerState.Paused);
                return;
            }

            if (_timerState == TimerState.Paused)
            {
                SetTimerState(TimerState.Stopped);
                ResetTimer();
                return;
            }
        }

        /// <summary>
        /// Отобразить показания таймера
        /// </summary>
        private void DisplayTimerValue()
        {
            TaimerAsString = _timerInterval.ToString();
        }

        /// <summary>
        /// Возвращает надпись на правой кнопке
        /// </summary>
        private string GetTimerSecondButtonName()
        {
            switch(_timerState)
            {
                case TimerState.Stopped:
                    return "Пауза";

                case TimerState.Running:
                    return "Пауза";

                case TimerState.Paused:
                    return "Сброс";

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Устанавливает состояние таймера
        /// </summary>
        private void SetTimerState(TimerState newState)
        {
            _timerState = newState;
            TimerSecondButtonName = GetTimerSecondButtonName();
        }
    }
}
