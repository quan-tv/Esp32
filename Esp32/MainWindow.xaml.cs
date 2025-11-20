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
using Esp32.View;
using Esp32;

namespace Esp32
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public PlotModel MyModel { get; set; }

        private readonly List<LineSeries> loadcellsList = new();   // List chứa các cell
        private DispatcherTimer _timer;
        private double _x = 0;

        private const int LoadCellsCount = 6;   // số cell
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

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

            // Chỉ cho phép tải dữ liệu nếu có ít nhất 1 cồng COM
            if (ports.Length > 0)
            {
                COMComboBox.SelectedIndex = 0;
                BtnStart.IsEnabled = true;
            } else
            {
                BtnStart.IsEnabled = false;
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

            // Tạo 6 line và add vào list + model
            for (int i = 0; i < LoadCellsCount; i++)
            {
                var line = new LineSeries
                {
                    Title = $"Loadcell {i + 1}",
                    StrokeThickness = 2
                };

                loadcellsList.Add(line);
                model.Series.Add(line);
            }

            MyModel = model;
            EspPlotView.Model = model;
        }

        private void InitTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(0.0125); // 80 lần/giây
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _x += 0.0125;

            // DEMO: 6 giá trị khác nhau cho 6 cell -> cần thay bằng dữ liệu từ Hệ thống
            // values sẽ là mảng chứa 6 giá trị

            HeThong heThong = new HeThong();

            double[] values = heThong.ReadData(_x);
            
            // Đảm bảo mảng values đủ số series
            int count = Math.Min(loadcellsList.Count, values.Length);

            for (int i = 0; i < count; i++)
            {
                loadcellsList[i].Points.Add(new DataPoint(_x, values[i]));
            }

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
                foreach (var cell in loadcellsList)
                {
                    cell.Points.Clear();
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

        private void MniAbout_Click(object sender, RoutedEventArgs e)
        {
            Opacity = 0.9;
            //MessageBox.Show("Thong tin phan mem va tac gia", "About");
            AboutWindow aboutWindow = new AboutWindow(this);
            aboutWindow.ShowDialog();
            Opacity = 1;
        }
    }
}