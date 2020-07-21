using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PID_Controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Init timer to upload opening status
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
            InitializeComponent();
            SerialPort.DataReceived += DataReceive; //add DataReceived event in Serial Port
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (SerialPort.IsOpen)
            {
                OpenPortJob();
            }
            else
            {
                ClosePortJob();
            }
        }

        public bool IsPortOpening = false;
        public SerialPort SerialPort = new SerialPort();

        private void BtnRefreshCom_Click(object sender, RoutedEventArgs e)
        {
            string[] ArrayComPortsNames = null;
            int index = -1;
            string ComPortName = null;
            CbComPort.Items.Clear();
            ArrayComPortsNames = SerialPort.GetPortNames();
            do
            {
                index += 1;
                CbComPort.Items.Add(ArrayComPortsNames[index]);
            } while (!((ArrayComPortsNames[index] == ComPortName) ||
                       (index == ArrayComPortsNames.GetUpperBound(0))));
        }

        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            if (IsPortOpening)
            {
                try
                {
                    int byteData = SerialPort.BytesToRead;
                    if (byteData > -1)
                    {
                        string data = SerialPort.ReadLine();
                        if (String.IsNullOrWhiteSpace(data))
                        {
                            return;
                        }

                        double value = TransferData(data);
                        if (value == -1) //Compare if error data
                        {
                            return;
                        }

                        //add value to collection
                        LineCharts.SeriesCollection[0].Values.Add(new DateModel()
                        {
                            DateTime = DateTime.Now,
                            Value = value
                        });
                        if (LineCharts.SeriesCollection[0].Values.Count > 50)
                            LineCharts.SeriesCollection[0].Values.RemoveAt(0);
                    }
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// return -1 when error
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private double TransferData(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                return -1;
            }
            else
            {
                string[] splitStrings = data.Split(',');
                if (splitStrings.Length != 2)
                    return -1;
                try
                {
                    double value = Convert.ToDouble(splitStrings[0]);
                    return value;
                }
                catch
                {
                    return -1;
                }


            }
        }

        private void ClosePortJob()
        {
            IsPortOpening = false;
            CbComPort.IsEnabled = true;
            IsPortOpening = false;
            BtnRefreshCom.IsEnabled = true;
            BtnConnect.Content = "Connect";
            BtnChangeParam.IsEnabled = false;
        }

        private void OpenPortJob()
        {
            CbComPort.IsEnabled = false;
            IsPortOpening = true;
            BtnRefreshCom.IsEnabled = false;
            BtnConnect.Content = "Disconnect";
            BtnChangeParam.IsEnabled = true;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbComPort.SelectedIndex > -1)
                {
                    if (IsPortOpening)
                    {
                        try
                        {
                            Task.Run(() => { SerialPort.Close(); }); // Use Asynchronous to prevent deadlock 
                            ClosePortJob();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Có lỗi xảy ra", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        SerialPort.PortName = CbComPort.SelectedValue.ToString();
                        SerialPort.BaudRate = 115200;
                        SerialPort.Open();
                        OpenPortJob();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn Port", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Có lỗi xảy ra", MessageBoxButton.OK, MessageBoxImage.Error);
                CbComPort.IsEnabled = true;
                IsPortOpening = false;
                BtnRefreshCom.IsEnabled = true;
                BtnConnect.Content = "Connect";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnRefreshCom.PerformClick();
        }

        private void HandleDoubleInput(object sender, TextCompositionEventArgs e)
        {
            bool approvedDecimalPoint = false;

            if (e.Text == ".")
            {
                if (!((TextBox)sender).Text.Contains("."))
                    approvedDecimalPoint = true;
            }

            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
                e.Handled = true;
        }

        private void BtnDefaultParam_Click(object sender, RoutedEventArgs e)
        {
            TbKp.Text = "0.0765";
            TbKi.Text = "0.859";
            TbKd.Text = "0.00145";
        }

        private void BtnChangeParam_Click(object sender, RoutedEventArgs e)
        {
            if (IsPortOpening)
            {
                SerialPort.WriteLine($"setup|{TbSpeed.Text}|{TbKp.Text}|{TbKi.Text}|{TbKd.Text}|");
            }
            else
            {
                MessageBox.Show("Vui lòng Open Port trước khi chỉnh thông số", "Thông báo", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Task.Run(() => { SerialPort.Close(); }); // Use Asynchronous to prevent deadlock 
            ClosePortJob();
            Thread.Sleep(500);
        }
    }
    public static class MyExt
    {
        public static void PerformClick(this Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
