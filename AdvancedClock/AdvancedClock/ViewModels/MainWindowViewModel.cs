using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Timers;

namespace AdvancedClock.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Время в виде строки
        /// </summary>
        private string _timeAsString;

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

        /// <summary>
        /// Секунды секундомера
        /// </summary>
        private TimeSpan _stopwatchInterval;

        /// <summary>
        /// Публичная переменная с временем в виде строки
        /// </summary>
        public string TimeAsString
        {
            get => _timeAsString;
            set => this.RaiseAndSetIfChanged(ref _timeAsString, value);
        }

        /// <summary>
        /// Публичная переменная с секундами секундомера
        /// </summary>
        public string StopwatchAsString
        {
            get => _stopwatchAsString;
            set => this.RaiseAndSetIfChanged(ref _stopwatchAsString, value);
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

        public MainWindowViewModel()
        {
            // Настраиваем таймер часов
            _clockUpdateTimer = new Timer(500); // Срабатывать раз в 500 миллисекунд
            _clockUpdateTimer.AutoReset = true; // Бесконечно повторяться
            _clockUpdateTimer.Enabled = true; // Таймер включён

            _clockUpdateTimer.Elapsed += OnClockUpdateTimer;

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
        /// Этот метод вызывается, когда таймер секундомера дотикает до 0
        /// </summary>
        private void OnStopwatchTimer(object? sender, ElapsedEventArgs e)
        {
            _stopwatchInterval += new TimeSpan(0, 0, 1);
            FormatStopwatchTimeSpan();
        }
    }
}
