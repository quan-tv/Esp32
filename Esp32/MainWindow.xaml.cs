using OxyPlot;
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

            //Them mot dummy comment
            var ports = System.IO.Ports.SerialPort.GetPortNames();

            foreach (var port in ports)
            {
                ComComboBox.Items.Add((string)port);
            }

            if (ports.Length > 0)
            {
                ComComboBox.SelectedIndex = 0;
            }

            MyModel = new PlotModel { Title = "Đồ thị hàm số" };
            DataContext = this;

            DrawData("COM1"); // mặc định
        }

        private void DrawData(string funcName)
        {
            var model = new PlotModel { Title = $"Đồ thị: {funcName}" };

            var series1 = new LineSeries { StrokeThickness = 2 };
            var series2 = new LineSeries { StrokeThickness = 2 };

            // Vẽ trong khoảng -10 -> 10
            //for (double x = -10; x <= 10; x += 0.1)
            //{
            //    double y = 0;

            //    switch (funcName)
            //    {
            //        case "COM1":
            //            y = x;
            //            break;
            //        case "y = x²":
            //            y = x * x;
            //            break;
            //        case "y = sin(x)":
            //            y = Math.Sin(x);
            //            break;
            //    }

            //    series.Points.Add(new DataPoint(x, y));
            //}

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
            PlotView1.Model = MyModel;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (BtnStart.Content == "Start")
            {
                BtnStart.Content = "Stop";
                //Bat dau doc du lieu va ve
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
                PlotView1.InvalidatePlot(true);
            }

            var selectedItem = ComComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string funcName = selectedItem.Content.ToString();
                DrawData(funcName);
            }
        }

        //private void FunctionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // Nếu muốn chọn là vẽ luôn, có thể gọi lại DrawFunction ở đây
        //    var selectedItem = FunctionComboBox.SelectedItem as ComboBoxItem;
        //    if (selectedItem != null)
        //    {
        //        string funcName = selectedItem.Content.ToString();
        //        DrawFunction(funcName);
        //    }
        //}

        private void MniFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}