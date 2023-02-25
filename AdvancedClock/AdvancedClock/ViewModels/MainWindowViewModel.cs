using Avalonia.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        private bool a = true;

        private bool b = false;
        /// <summary>
        /// Время в виде строки
        /// </summary>
        private string _taimerAsString;

        private int TaimerNumber =  60;
        /// <summary>
        /// Таймер обновления часов
        /// </summary>
        private Timer _clockUpdateTimer;

        /// <summary>
        /// Таймер секундомера
        /// </summary>
        private Timer _stopwatchTimer;

        private Timer _stopTimer;
        /// <summary>
        /// Секунды секундомера в виде строки
        /// </summary>
        private string _stopwatchAsString;

        private string _ButtonName;

        /// <summary>
        /// Секунды секундомера
        /// </summary>
        private TimeSpan _stopwatchInterval;

        private string _TimeAsString;

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
            get => _taimerAsString;
            set => this.RaiseAndSetIfChanged(ref _taimerAsString, value);
        }

        public string ButtonName
        {
            get => _ButtonName;
            set => this.RaiseAndSetIfChanged(ref _ButtonName, value);
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
            ButtonName = "Пауза";
            TaimerAsString = "00:00";
            // Настраиваем таймер часов
            _clockUpdateTimer = new Timer(500); // Срабатывать раз в 500 миллисекунд
            _clockUpdateTimer.AutoReset = true; // Бесконечно повторяться
            _clockUpdateTimer.Enabled = true; // Таймер включён

            _clockUpdateTimer.Elapsed += OnClockUpdateTimer;

            _stopTimer = new Timer(1000);
            _stopTimer.AutoReset = true;
            _stopTimer.Enabled = false;
            _stopTimer.Elapsed += Taimer;

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

        private void Taimer(object? sender, ElapsedEventArgs e)
        {
            TaimerNumber -= 1;
            if (TaimerNumber < 0)
            {
                _stopTimer.Stop();
            }
            else
            {
                TaimerAsString = (TaimerNumber / 60).ToString() + ":" + (TaimerNumber % 60).ToString();
            }
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
            _stopTimer.Start();
            ButtonName = "Пауза";
            a = true;
            b = true;
        }

        private void PauseTaimer()
        {
            if (a && b)
            {
                _stopTimer.Stop();
                ButtonName = "Сброс";
                a = false;
                b = true;
            }
            else if(b)
            {
                TaimerNumber = 60;
                ButtonName = "Пауза";
                _stopTimer.Stop();
                TaimerAsString = (TaimerNumber / 60).ToString() + ":" + (TaimerNumber % 60).ToString();
                a = true;
                b= false;
            }
        }

    }
}
