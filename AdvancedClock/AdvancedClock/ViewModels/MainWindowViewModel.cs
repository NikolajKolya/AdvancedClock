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
        /// ����� � ���� ������
        /// </summary>
        private string _timeAsString;

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

        /// <summary>
        /// ������� �����������
        /// </summary>
        private TimeSpan _stopwatchInterval;

        /// <summary>
        /// ��������� ���������� � �������� � ���� ������
        /// </summary>
        public string TimeAsString
        {
            get => _timeAsString;
            set => this.RaiseAndSetIfChanged(ref _timeAsString, value);
        }

        /// <summary>
        /// ��������� ���������� � ��������� �����������
        /// </summary>
        public string StopwatchAsString
        {
            get => _stopwatchAsString;
            set => this.RaiseAndSetIfChanged(ref _stopwatchAsString, value);
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

        public MainWindowViewModel()
        {
            // ����������� ������ �����
            _clockUpdateTimer = new Timer(500); // ����������� ��� � 500 �����������
            _clockUpdateTimer.AutoReset = true; // ���������� �����������
            _clockUpdateTimer.Enabled = true; // ������ �������

            _clockUpdateTimer.Elapsed += OnClockUpdateTimer;

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
        /// ���� ����� ����������, ����� ������ ����������� �������� �� 0
        /// </summary>
        private void OnStopwatchTimer(object? sender, ElapsedEventArgs e)
        {
            _stopwatchInterval += new TimeSpan(0, 0, 1);
            FormatStopwatchTimeSpan();
        }
    }
}
