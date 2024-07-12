using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using prism_simpletemplate.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using prism_serial.Model;
using System.Collections.ObjectModel;

namespace prism_simpletemplate.ViewModels
{
    public class View2ViewModel : BindableBase
    {
        public View2ViewModel(SerialPort serialPort_in)
        {
            this.serialPort = serialPort_in;
            TestCommand = new DelegateCommand(OnTest);
            CarCommand = new DelegateCommand(OnCar);
            CarStopCommand = new DelegateCommand(()=>
            {
                if(serialPort.IsOpen)
                {
                    byte[] dataToSend = new byte[] { 0xFF,0x00,
                        0x00, 0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFE };

                    // 发送数据
                    serialPort.Write(dataToSend, 0, dataToSend.Length);
                }
            });
            ClearCommand = new DelegateCommand(() => { control_first = (float)0.0;control_second =(float) 0.0;control_third = (float)0.0; });
        }
        private View2Model obj = new View2Model();
        public string text_list_selected
        {
            get { return obj.text_list_selected; }
            set
            {
                obj.text_list_selected = value;
                if (value == "车身速度")
                {
                    control_mode = View2Model.Car_Control_Mode_t.speed_control_self;
                }
                else if (value == "大地速度")
                {
                    control_mode = View2Model.Car_Control_Mode_t.speed_control_ground;
                }
                else if (value == "位置闭环")
                {
                    control_mode = View2Model.Car_Control_Mode_t.location_control;
                }
            }
        }
        public List<string> text_list_control
        {
            get { return obj.text_list_control; }
            set { obj.text_list_control = value; }
        }


        public View2Model.Car_Control_Mode_t control_mode
        {
            get { return obj.control_mode; }
            set { obj.control_mode = value; }
        }
        //第一第二第三个控制量
        public string control_first_text
        {
            get { return obj.control_first_text; }
            set { obj.control_first_text = value; control_first = float.Parse(value); RaisePropertyChanged(); }
        }
        public string control_second_text
        {
            get { return obj.control_second_text; }
            set { obj.control_second_text = value; control_second = float.Parse(value); RaisePropertyChanged(); }
        }
        public string control_third_text
        {
            get { return obj.control_third_text; }
            set { obj.control_third_text = value; control_third = float.Parse(value); RaisePropertyChanged(); }
        }
        public float control_first
        {
            get { return obj.control_first; }
            set
            {
                if (obj.control_first != value)
                {
                    obj.control_first = value;
                    control_first_text = string.Format("{0:f2}", value.ToString());
                    RaisePropertyChanged();
                }
            }
        }
        public float control_second
        {
            get { return obj.control_second; }
            set
            {
                if (obj.control_second != value)
                {
                    obj.control_second = value; control_second_text = string.Format("{0:f2}", value.ToString());
                }
                RaisePropertyChanged();
            }
        }
        public float control_third
        {
            get { return obj.control_third; }
            set
            {
                if (obj.control_third != value)
                {
                    obj.control_third = value; control_third_text = string.Format("{0:f2}", value.ToString());
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<string> control_str
        {
            get { return obj.control_str; }
            set { obj.control_str = value; }
        }
        public ObservableCollection<float> control_float
        {
            get { return obj.control_float; }
            set { if (obj.control_float != value) 
                { 
                    obj.control_float = value;  RaisePropertyChanged();
                    //obj.control_str = string.Format("{0:f2}", value.ToString()); 
                    for (int i = 0; i < value.Count; i++)
                    {
                        obj.control_str[i] = string.Format("{0:f2}", value[i].ToString());
                    }
                } }
        }
        private SerialPort serialPort;
        //command
        public DelegateCommand CarCommand { get; set; }
        public DelegateCommand CarStopCommand { get; set; }
        //清零PID
        public DelegateCommand ClearCommand { get; set; }
        //往charpage.html传递数据
        public DelegateCommand TestCommand { get; set; }
        public delegate void PostDelegate(string webMessageAsJson);
        public PostDelegate postDelegate;
        private void OnTest()
        {
            Task.Run(() =>//测试模拟后台输出文件
            {
                var sePoints1 = new double[20, 2];
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(2000);
                    sePoints1[i, 0] = i;
                    sePoints1[i, 1] = i * 1.2;
                    Console.WriteLine("do Taskwork,i={0}", i);
                }
            });
            //Web.ObjectForScripting.
            var sePoints = new double[20, 2];
            for (int i = 0; i < 20; i++)
            {
                sePoints[i, 0] = i;
                sePoints[i, 1] = i * 1.2;
            }
            postDelegate?.Invoke(JsonConvert.SerializeObject(sePoints));
        }
        /*
         * 车速度帧头为0xFF帧尾为0xFE
         * 大地速度帧头为0xFD帧尾为0xFC
         * 位置闭环帧头为0xFB帧尾为0xFA
         */
        private void OnCar()
        {
            if (serialPort.IsOpen)
            {
                byte[] floatBytes = BitConverter.GetBytes(control_first);
                byte[] floatBytes2 = BitConverter.GetBytes(control_second);
                byte[] floatBytes3 = BitConverter.GetBytes(control_third);
                byte[] combinedBytes = new byte[15]; // 1 byte (frame head) + 12 bytes (floats) + 2 bytes (frame tail)

                switch (control_mode)
                {
                    case View2Model.Car_Control_Mode_t.speed_control_self:
                        combinedBytes[0] = 0xFF;
                        break;
                    case View2Model.Car_Control_Mode_t.speed_control_ground:
                        combinedBytes[0] = 0xFD;
                        break;
                    case View2Model.Car_Control_Mode_t.location_control:
                        combinedBytes[0] = 0xFB;
                        break;
                }

                Array.Copy(floatBytes, 0, combinedBytes, 1, 4);
                Array.Copy(floatBytes2, 0, combinedBytes, 5, 4);
                Array.Copy(floatBytes3, 0, combinedBytes, 9, 4);

                switch (control_mode)
                {
                    case View2Model.Car_Control_Mode_t.speed_control_self:
                        combinedBytes[13] = 0xFE;
                        break;
                    case View2Model.Car_Control_Mode_t.speed_control_ground:
                        combinedBytes[13] = 0xFC;
                        break;
                    case View2Model.Car_Control_Mode_t.location_control:
                        combinedBytes[13] = 0xFA;
                        break;
                }

                serialPort.Write(combinedBytes, 0, 15);
            }
        }
    
    }
}
