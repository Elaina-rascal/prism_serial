using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using prism_serial.Common.Events;
using prism_simpletemplate.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace prism_simpletemplate.ViewModels
{
    /// <summary>
    /// Represents the ViewModel for View1.
    /// </summary>
    public class View1ViewModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="View1ViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="serialPortin">The serial port.</param>
        public View1ViewModel(IEventAggregator eventAggregator, SerialPort serialPortin)
        {
            this.serialPort = serialPortin;
            this.eventAggregator = eventAggregator;

            serialPort.Encoding = Encoding.UTF8;
            serialPort.DataReceived += SerialDataReceived;
            ButtonCommand = new DelegateCommand<object>(obj => SearchAvailableCom());
            ClearCommand = new DelegateCommand<object>(obj => Received_text = "");
            OpenCloseCommand = new DelegateCommand<object>(OnOpenCloseCommand);
            Trans_ClearCommand = new DelegateCommand<object>(obj => Trans_text = "");
            TransButtonClickCommand = new DelegateCommand<object>(obj => TransData(DataTransThread));

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (s, e) => { SearchAvailableCom(); };
            timer.IsEnabled = true;
        }

        // Resources
        private SerialPort serialPort;
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer timer2 = new DispatcherTimer();
        private readonly IEventAggregator eventAggregator;
        private View1Model obj = new View1Model();
        private Thread DataTransThread;

        // Bindable properties
        /// <summary>
        /// Gets or sets the received text.
        /// </summary>
        public string Received_text
        {
            get { return (string)obj.Received_text; }
            set
            {
                obj.Received_text = value;
                show_text = Received_text;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text to be transmitted.
        /// </summary>
        public string Trans_text
        {
            get { return obj.Trans_text; }
            set { obj.Trans_text = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the text to be displayed.
        /// </summary>
        public string show_text
        {
            get { return obj.show_text; }
            set
            {
                if (obj.show_text != value)
                {
                    obj.show_text = value.Replace(@"\n", Environment.NewLine);
                }
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the baud rates.
        /// </summary>
        public List<int> baudrate
        {
            get { return (List<int>)obj.baudrate; }
            set { obj.baudrate = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the available COM ports.
        /// </summary>
        public ObservableCollection<string> com
        {
            get { return obj.com; }
            set { obj.com = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the selected baud rate.
        /// </summary>
        public int baudrate_select
        {
            get { return (int)obj.baudrate_select; }
            set { obj.baudrate_select = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the selected COM port.
        /// </summary>
        public string com_select
        {
            get { return obj.com_select; }
            set { obj.com_select = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the COM port and baud rate selection is enabled.
        /// </summary>
        public bool IsComBaudEnable
        {
            get { return obj.IsComBaudEnable; }
            set { obj.IsComBaudEnable = value; RaisePropertyChanged(); }
        }

        // Bindable commands
        /// <summary>
        /// Gets or sets the command for the button.
        /// </summary>
        public DelegateCommand<object> ButtonCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for clearing the received text.
        /// </summary>
        public DelegateCommand<object> ClearCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for clearing the transmitted text.
        /// </summary>
        public DelegateCommand<object> Trans_ClearCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for opening or closing the serial port.
        /// </summary>
        public DelegateCommand<object> OpenCloseCommand { get; set; }

        /// <summary>
        /// Gets or sets the command for transmitting the data.
        /// </summary>
        public DelegateCommand<object> TransButtonClickCommand { get; set; }

        // Event handlers
        private void SerialDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string received_text = serialPort.ReadExisting();
                Received_text = Received_text + received_text;
            }
            catch
            {

            }
        }

        [Obsolete("This method is inefficient.", true)]
        private void SearchAvailableCom(SerialPort mySerial)
        {
            try
            {
                string temp_string;
                com.Clear();
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        temp_string = "COM" + i.ToString();
                        mySerial.PortName = temp_string;
                        mySerial.Open();
                        com.Add(temp_string);
                        mySerial.Close();
                    }
                    catch { };
                }
            }
            catch
            { }
        }

        private void SearchAvailableCom()
        {
            string selectedPort = com_select;
            string[] ports = SerialPort.GetPortNames();
            bool shouldUpdate = !com.SequenceEqual(ports);

            if (shouldUpdate)
            {
                com.Clear();
                foreach (string port in ports)
                {
                    com.Add(port);
                }

                if (ports.Contains(selectedPort))
                {
                    com_select = selectedPort;
                }
            }
        }

        //[Obsolete("Use dependency injection to share a Serial instance.", true)]
        //private void PubSerialData()
        //{
        //    if (IsComBaudEnable == false)
        //    {
        //        eventAggregator.GetEvent<SerialMessage>().Publish(pubDate);
        //        pubDate = string.Empty;
        //    }
        //}

        private void OnOpenCloseCommand(object? obj)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)obj;
            if (button.Content.ToString() == "打开串口")
            {
                try
                {
                    serialPort.PortName = com_select;
                    serialPort.BaudRate = baudrate_select;
                    serialPort.Open();
                    button.Content = "关闭串口";
                    IsComBaudEnable = false;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
            else
            {
                try
                {
                    serialPort.Close();
                    button.Content = "打开串口";
                    IsComBaudEnable = true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
        }

        [Obsolete("Use s, e pattern to handle keyboard input.", true)]
        private void OnTxBox_KeyDownCommand(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    serialPort.Write(Trans_text);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + ex.Message);
                }
            }
            else if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {

            }
        }

        private void TransData(Thread? thread)
        {
            thread = new Thread(() =>
            {
                try
                {
                    serialPort.WriteTimeout = 2000;
                    serialPort.Write(Trans_text);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + ex.Message);
                }
            });
            thread.Start();
            thread.Join();
        }

        public void OnTxBox_KeyDownCommand(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;

            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                int caretIndex = textBox.CaretIndex;
                Trans_text = textBox.Text;
                int insertionPoint = caretIndex;
                textBox.Text = textBox.Text.Insert(insertionPoint, "\r\n");
                textBox.CaretIndex = insertionPoint + 2;
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                Trans_text = textBox.Text;
                TransData(DataTransThread);
            }
        }

        public void OnOpenCloseCommand(object? obj, EventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)obj;
            if (button.Content.ToString() == "打开串口")
            {
                try
                {
                    serialPort.PortName = com_select;
                    serialPort.BaudRate = baudrate_select;
                    serialPort.Open();
                    button.Content = "关闭串口";
                    IsComBaudEnable = false;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
            else
            {
                try
                {
                    serialPort.Close();
                    button.Content = "打开串口";
                    IsComBaudEnable = true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: " + "串口异常");
                }
            }
        }
    }
}
