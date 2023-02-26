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

namespace AdvancedClock.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        /// <summary>
        /// ��������� ���������� ������ �� �������
        /// </summary>
        private const int TimerSeconds = 60;

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

        private string _TimeAsString;

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

        #endregion

        /// <summary>
        /// ��������� ���������� � �������� � ���� ������
        /// </summary>
        public string TimeAsString
        {
            get => _TimeAsString;
            set => this.RaiseAndSetIfChanged(ref _TimeAsString, value);
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
            _timerInterval = new TimeSpan(0, 0, TimerSeconds);
            DisplayTimerValue();

            TimerSecondButtonName = "�����";


            // ��������� ������� �������
            StartTimer = ReactiveCommand.Create(TaimerStarter);
            PauseTimer = ReactiveCommand.Create(PauseTaimer);
        }

        /// <summary>
        /// ���� ����� ���������� ����� �������� �� 0 ������ �����
        /// </summary>
        private void OnClockUpdateTimer(Object source, ElapsedEventArgs e)
        {
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

            if (_timerInterval.TotalSeconds == 0)
            {

            }
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
            if ((_timerState == TimerState.Stopped) || (_timerState == TimerState.Paused))
            {
                _stopTimer.Start();
                _timerState = TimerState.Running;
                TimerSecondButtonName = "�����";

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
                TimerSecondButtonName = "�����";
                _timerState = TimerState.Paused;
                return;
            }

            if (_timerState == TimerState.Paused)
            {
                _timerInterval = new TimeSpan(0, 0, TimerSeconds);
                DisplayTimerValue();

                TimerSecondButtonName = "�����";

                _timerState = TimerState.Stopped;
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
    }
}
