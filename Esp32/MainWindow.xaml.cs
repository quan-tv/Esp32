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

namespace Esp32
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public PlotModel MyModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();

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

            DataContext = this;

            InitOxyPlot();

            //DrawData("COM1"); // mặc định
        }

        //Khởi tạo đồ thị
        private void InitOxyPlot()
        {
            var model = new PlotModel { Title = "Đồ thị: " };

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            MyModel = model;
            EspPlotView.Model = model;
        }

        private void DrawData(string COMName)
        {
            var model = new PlotModel { Title = $"Đồ thị: {COMName}" };

            var series1 = new LineSeries { StrokeThickness = 2 };
            var series2 = new LineSeries { StrokeThickness = 2 };

            for (int x = 0; x < 10; x++)
            {
                int y = x;

                int z = x * 2;

                series1.Points.Add(new DataPoint(x, y));
                series2.Points.Add(new DataPoint(x, z));
            }

            model.Series.Add(series1);
            model.Series.Add(series2);
            MyModel = model;

            // cập nhật binding cho PlotView
            EspPlotView.Model = model;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BtnStart.Content.ToString() == "Start")
            {
                BtnStart.Content = "Stop";
                
                //Lay cong COM hien tai
                var selectedItem = COMComboBox.SelectedItem as string;
                
                //Bat dau doc du lieu va ve
                if (selectedItem != null)
                {
                    string comPortName = selectedItem;
                    DrawData(selectedItem);
                }
            }
            else
            {
                BtnStart.Content = "Start";
                //Dung viec doc du lieu va xoa do thi

                // 1. Lưu lại trạng thái zoom hiện tại của tất cả axes
                var axisStates = MyModel.Axes
                    .Select(a => new
                    {
                        Axis = a,
                        Min = a.ActualMinimum,
                        Max = a.ActualMaximum
                    })
                    .ToList();

                // 2. Bắt đầu update để tránh refresh giữa chừng
                MyModel.Series.Clear();

                // 3. Áp lại zoom cũ cho từng axis
                foreach (var s in axisStates)
                {
                    // Chỉ zoom nếu range hợp lệ
                    if (!double.IsNaN(s.Min) && !double.IsNaN(s.Max) && s.Min < s.Max)
                    {
                        s.Axis.Zoom(s.Min, s.Max);
                    }
                }

                // 4. Cập nhật lại plot
                EspPlotView.InvalidatePlot(true);
            }
        }

        private void MniFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}