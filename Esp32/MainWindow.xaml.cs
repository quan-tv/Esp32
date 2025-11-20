using OxyPlot;
using OxyPlot.Axes;
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

        private LineSeries _series1;
        private LineSeries _series2;
        private DispatcherTimer _timer;
        private double _x = 0;

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
            var model = new PlotModel { Title = "Đồ thị Esp32 " };

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Thời gian"
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Khối lượng"
            });

            // Tạo 2 series, giữ lại ở field để timer thêm điểm
            _series1 = new LineSeries { Title = "Line 1", StrokeThickness = 2 };
            _series2 = new LineSeries { Title = "Line 2", StrokeThickness = 2 };

            model.Series.Add(_series1);
            model.Series.Add(_series2);

            MyModel = model;

            // Vì đã Binding Model="{Binding MyModel}" trong XAML,
            // về lý thuyết chỉ cần MyModel = model là đủ.
            // Nhưng để chắc, vẫn set trực tiếp:
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

            // DEMO: dữ liệu giả, sau này bạn thay bằng dữ liệu đọc từ COM
            double y1 = Math.Sin(_x);
            double y2 = Math.Cos(_x * 0.5);

            _series1.Points.Add(new DataPoint(_x, y1));
            _series2.Points.Add(new DataPoint(_x, y2));

            // Giữ tối đa 200 điểm để không bị nặng
            if (_series1.Points.Count > 200) _series1.Points.RemoveAt(0);
            if (_series2.Points.Count > 200) _series2.Points.RemoveAt(0);

            // Auto scroll trục X
            var xAxis = MyModel.Axes[0];
            xAxis.Minimum = _x - 20;
            xAxis.Maximum = _x;

            EspPlotView.InvalidatePlot(true);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BtnStart.Content.ToString() == "Start")
            {
                BtnStart.Content = "Stop";

                // Lấy cổng COM đang chọn (sau này bạn dùng để mở SerialPort)
                var selectedItem = COMComboBox.SelectedItem as string;

                // Reset dữ liệu nếu muốn mỗi lần Start là vẽ từ đầu
                _x = 0;
                _series1.Points.Clear();
                _series2.Points.Clear();

                // TODO: mở cổng COM ở đây nếu bạn đọc dữ liệu thật
                // OpenSerialPort(selectedItem);

                _timer.Start();
            }
            else
            {
                BtnStart.Content = "Start";

                _timer.Stop();

                // TODO: đóng cổng COM nếu có dùng
                // CloseSerialPort();

                // Nếu muốn giữ đường vừa vẽ thì KHÔNG cần clear
                // Nếu muốn xóa luôn:
                // _series1.Points.Clear();
                // _series2.Points.Clear();
                // EspPlotView.InvalidatePlot(true);
            }
        }

        private void MniFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}