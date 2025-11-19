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
                MessageBox.Show((string)port);
                FunctionComboBox.Items.Add((string)port);
            }

            MyModel = new PlotModel { Title = "Đồ thị hàm số" };
            DataContext = this;

            DrawFunction("y = x"); // mặc định
        }

        private void DrawFunction(string funcName)
        {
            var model = new PlotModel { Title = $"Đồ thị: {funcName}" };
            var series = new LineSeries { StrokeThickness = 2 };

            // Vẽ trong khoảng -10 -> 10
            for (double x = -10; x <= 10; x += 0.1)
            {
                double y = 0;

                switch (funcName)
                {
                    case "y = x":
                        y = x;
                        break;
                    case "y = x²":
                        y = x * x;
                        break;
                    case "y = sin(x)":
                        y = Math.Sin(x);
                        break;
                }

                series.Points.Add(new DataPoint(x, y));
            }

            model.Series.Add(series);
            MyModel = model;

            // cập nhật binding cho PlotView
            PlotView1.Model = MyModel;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = FunctionComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string funcName = selectedItem.Content.ToString();
                DrawFunction(funcName);
            }
        }

        private void FunctionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nếu muốn chọn là vẽ luôn, có thể gọi lại DrawFunction ở đây
            var selectedItem = FunctionComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string funcName = selectedItem.Content.ToString();
                DrawFunction(funcName);
            }
        }

        private void MniFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}