using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Esp32
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public PlotModel MyModel { get; set; }

        private readonly List<LineSeries> _seriesList = new();   // quản lý tất cả series
        private DispatcherTimer _timer;
        private double _x = 0;

        private const int SeriesCount = 6;   // số series
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;      // để XAML Binding MyModel dùng được

            InitOxyPlot();
            InitCOMPort();
            InitTimer();
        }

        private void InitCOMPort()
        {
            //Danh sách các cổng COM

            var ports = System.IO.Ports.SerialPort.GetPortNames();

            foreach (var port in ports)
            {
                COMComboBox.Items.Add((string)port);
            }

            if (ports.Length > 0)
            {
                COMComboBox.SelectedIndex = 0;
            }
        }

        //Khởi tạo đồ thị
        private void InitOxyPlot()
        {
            var model = new PlotModel { };

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Thời gian (s)"
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Khối lượng (Kg)"
            });

            model.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.TopRight,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBorderThickness = 0,
                LegendMargin = 4
            });

            // Tạo 6 series và add vào list + model
            for (int i = 0; i < SeriesCount; i++)
            {
                var series = new LineSeries
                {
                    Title = $"Line {i + 1}",
                    StrokeThickness = 2
                };

                _seriesList.Add(series);
                model.Series.Add(series);
            }

            MyModel = model;
            EspPlotView.Model = model;
        }

        private void InitTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(12.5); // 80 lần/giây
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _x += 0.1;

            // DEMO: 6 giá trị khác nhau, sau này bạn thay bằng dữ liệu đọc từ COM
            double[] values =
            {
                Math.Sin(_x),
                Math.Cos(_x),
                Math.Sin(_x * 0.5),
                Math.Cos(_x * 0.5),
                Math.Sin(_x * 0.2),
                Math.Cos(_x * 0.2)
            };

            // Đảm bảo mảng values đủ số series
            int count = Math.Min(_seriesList.Count, values.Length);

            for (int i = 0; i < count; i++)
            {
                _seriesList[i].Points.Add(new DataPoint(_x, values[i]));
            }

            // KHÔNG xóa dữ liệu cũ nếu muốn vẽ nối tiếp
            // (nếu muốn giới hạn số điểm thì thêm RemoveAt ở đây)

            // Auto scroll trục X nếu muốn
            var xAxis = MyModel.Axes[0];
            xAxis.Minimum = _x - 20;
            xAxis.Maximum = _x;

            EspPlotView.InvalidatePlot(true);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BtnStart.Content?.ToString() == "Start")
            {
                BtnStart.Content = "Stop";

                var selectedItem = COMComboBox.SelectedItem as string;

                // Nếu muốn reset mỗi lần Start:
                _x = 0;
                foreach (var s in _seriesList)
                {
                    s.Points.Clear();
                }

                _timer.Start();
            }
            else
            {
                BtnStart.Content = "Start";
                _timer.Stop();
            }
        }

        private void MniFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}