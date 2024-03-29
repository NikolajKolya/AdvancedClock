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
        /// ��������� ���������� ������ �� �������
        /// </summary>
        private const int TimerSeconds = 10;

        /// <summary>
        /// ������� ���� �������
        /// </summary>
        private readonly IBrush NormalTimerColor = new SolidColorBrush(new Color(255, 0, 0, 0));

        /// <summary>
        /// ������, ������������ ��������
        /// </summary>
        private readonly IBrush WarningTimerColor = new SolidColorBrush(new Color(255, 255, 0, 0));

        /// <summary>
        /// ������ ���������� �����
        /// </summary>
        private Timer _clockUpdateTimer;

        /// <summary>
        /// ������ �����������
        /// </summary>
        private Timer _stopwatchTimer;

        /// <summary>
        /// ������� ����������� � ���� ������
        /// </summary>
        private string _stopwatchAsString;

        private string _timerSecopndButtonName;

        /// <summary>
        /// ������� �����������
        /// </summary>
        private TimeSpan _stopwatchInterval;

        /// <summary>
        /// ������� ����� ��� ������
        /// </summary>
        private string _timeAsString;

        /// <summary>
        /// ������� ����� (��� ��� DateTime)
        /// </summary>
        private DateTime _time;

        #region Timer

        /// <summary>
        /// ������� �������� �� �������
        /// </summary>
        private TimeSpan _timerInterval;

        /// <summary>
        /// ����� ������� � ���� ������
        /// </summary>
        private string _timerAsString;

        /// <summary>
        /// ������ �������
        /// </summary>
        private Timer _stopTimer;

        /// <summary>
        /// ��������� �������
        /// </summary>
        private TimerState _timerState = TimerState.Stopped;

        private IBrush _timerColor;

        /// <summary>
        /// ���� ������ �������
        /// </summary>
        public IBrush TimerColor
        {
            get => _timerColor;
            set => this.RaiseAndSetIfChanged(ref _timerColor, value);
        }

        private string _timerIntervalValueAsString;

        /// <summary>
        /// �������� �������, ��������� �������������
        /// </summary>
        public string TimerIntervalValueAsString
        {
            get => _timerIntervalValueAsString;
            set => this.RaiseAndSetIfChanged(ref _timerIntervalValueAsString, value);
        }

        #endregion

        /// <summary>
        /// ��������� ���������� � �������� � ���� ������
        /// </summary>
        public string TimeAsString
        {
            get => _timeAsString;
            set => this.RaiseAndSetIfChanged(ref _timeAsString, value);
        }

        /// <summary>
        /// ��������� ���������� � ��������
        /// </summary>
        public DateTime Time
        {
            get => _time;
            set => this.RaiseAndSetIfChanged(ref _time, value);
        }

        /// <summary>
        /// ��������� ���������� � ��������� �����������
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
        /// ������� �� ������ ������� �����������
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
            // ����������� ������ �����
            _clockUpdateTimer = new Timer(500); // ����������� ��� � 500 �����������
            _clockUpdateTimer.AutoReset = true; // ���������� �����������
            _clockUpdateTimer.Enabled = true; // ������ �������

            _clockUpdateTimer.Elapsed += OnClockUpdateTimer;

            _stopTimer = new Timer(1000);
            _stopTimer.AutoReset = true;
            _stopTimer.Enabled = false;
            _stopTimer.Elapsed += OnTimerTick;

            // ����������� ����������
            _stopwatchInterval = new TimeSpan();
            FormatStopwatchTimeSpan();

            _stopwatchTimer = new Timer(1000);
            _stopwatchTimer.AutoReset = true;
            _stopwatchTimer.Enabled = false;
            _stopwatchTimer.Elapsed += OnStopwatchTimer;

            // ��������� ������� ����������� � ��������
            StopwatchStartCommand = ReactiveCommand.Create(StartStopwatch);
            StopwatchStopCommand = ReactiveCommand.Create(StopStopwatch);
            ResetStopWatchCommand = ReactiveCommand.Create(ResetStopwatch);

            // ����������� ������
            ResetTimer();

            SetTimerState(TimerState.Stopped);

            // ��������� ������� �������
            StartTimer = ReactiveCommand.Create(TaimerStarter);
            PauseTimer = ReactiveCommand.Create(PauseTaimer);
        }

        /// <summary>
        /// ���� ����� ���������� ����� �������� �� 0 ������ �����
        /// </summary>
        private void OnClockUpdateTimer(Object source, ElapsedEventArgs e)
        {
            Time = DateTime.Now;
            TimeAsString = DateTime.Now.ToLongTimeString();
        }
        
        /// <summary>
        /// ���� ����� ����������� �������� ����������� � ������
        /// </summary>
        private void FormatStopwatchTimeSpan()
        {
            StopwatchAsString = _stopwatchInterval.ToString();
        }

        /// <summary>
        /// ���� ����� ���������� ��� ������� �� ������ ������� �����������
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
        /// ��� �������
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
        /// ����� ������� � ��������� ���������
        /// </summary>
        private void ResetTimer()
        {
            _timerInterval = new TimeSpan(0, 0, TimerSeconds);
            TimerIntervalValueAsString = _timerInterval.TotalSeconds.ToString();
            TimerColor = NormalTimerColor;
            DisplayTimerValue();
        }

        /// <summary>
        /// ���� ����� ����������, ����� ������ ����������� �������� �� 0
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
                        .GetMessageBoxStandardWindow("Error", "�� ����� �� �����!");
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
        /// ���������� ��������� �������
        /// </summary>
        private void DisplayTimerValue()
        {
            TaimerAsString = _timerInterval.ToString();
        }

        /// <summary>
        /// ���������� ������� �� ������ ������
        /// </summary>
        private string GetTimerSecondButtonName()
        {
            switch(_timerState)
            {
                case TimerState.Stopped:
                    return "�����";

                case TimerState.Running:
                    return "�����";

                case TimerState.Paused:
                    return "�����";

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// ������������� ��������� �������
        /// </summary>
        private void SetTimerState(TimerState newState)
        {
            _timerState = newState;
            TimerSecondButtonName = GetTimerSecondButtonName();
        }
    }
}
