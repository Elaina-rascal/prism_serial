using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using prism_serial.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace prism_serial.ViewModels
{
    public class View2ViewModel : BindableBase
    {
        public View2ViewModel(SerialPort serialPortIn)
        {
            this._serialPort = serialPortIn;
            TestCommand = new DelegateCommand(OnTest);
            CarCommand = new DelegateCommand(OnCar);
            CarStopCommand = new DelegateCommand(() =>
            {
                if (_serialPort.IsOpen)
                {
                    byte[] dataToSend = new byte[] { 0xFF,0x00,
                        0x00, 0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFE };

                    // 发送数据
                    _serialPort.Write(dataToSend, 0, dataToSend.Length);
                }
            });
            ClearCommand = new DelegateCommand(() => { ControlFirst = (float)0.0; ControlSecond = (float)0.0; ControlThird = (float)0.0; });
        }

        private View2Model _obj = new View2Model();

        public string TextListSelected
        {
            get => _obj.TextListSelected;
            set
            {
                _obj.TextListSelected = value;
                if (value == "车身速度")
                {
                    ControlMode = View2Model.CarControlModeT.SpeedControlSelf;
                }
                else if (value == "大地速度")
                {
                    ControlMode = View2Model.CarControlModeT.SpeedControlGround;
                }
                else if (value == "位置闭环")
                {
                    ControlMode = View2Model.CarControlModeT.LocationControl;
                }
            }
        }

        public List<string> TextListControl
        {
            get => _obj.TextListControl;
            set => _obj.TextListControl = value;
        }

        public View2Model.CarControlModeT ControlMode
        {
            get => _obj.ControlMode;
            set => _obj.ControlMode = value;
        }

        //第一第二第三个控制量
        public string ControlFirstText
        {
            get => _obj.ControlFirstText;
            set { _obj.ControlFirstText = value; ControlFirst = float.Parse(value); RaisePropertyChanged(); }
        }

        public string ControlSecondText
        {
            get => _obj.ControlSecondText;
            set { _obj.ControlSecondText = value; ControlSecond = float.Parse(value); RaisePropertyChanged(); }
        }

        public string ControlThirdText
        {
            get => _obj.ControlThirdText;
            set { _obj.ControlThirdText = value; ControlThird = float.Parse(value); RaisePropertyChanged(); }
        }

        public float ControlFirst
        {
            get => _obj.ControlFirst;
            set
            {
                if (_obj.ControlFirst != value)
                {
                    _obj.ControlFirst = value;
                    ControlFirstText = string.Format("{0:f2}", value.ToString());
                    RaisePropertyChanged();
                }
            }
        }

        public float ControlSecond
        {
            get => _obj.ControlSecond;
            set
            {
                if (_obj.ControlSecond != value)
                {
                    _obj.ControlSecond = value; ControlSecondText = string.Format("{0:f2}", value.ToString());
                }
                RaisePropertyChanged();
            }
        }

        public float ControlThird
        {
            get => _obj.ControlThird;
            set
            {
                if (_obj.ControlThird != value)
                {
                    _obj.ControlThird = value; ControlThirdText = string.Format("{0:f2}", value.ToString());
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<string> ControlStr
        {
            get => _obj.ControlStr;
            set => _obj.ControlStr = value;
        }

        public ObservableCollection<float> ControlFloat
        {
            get => _obj.ControlFloat;
            set
            {
                if (_obj.ControlFloat == value) return;
                _obj.ControlFloat = value; RaisePropertyChanged();
                //obj.control_str = string.Format("{0:f2}", value.ToString());
                for (int i = 0; i < value.Count; i++)
                {
                    _obj.ControlStr[i] = string.Format("{0:f2}", value[i].ToString());
                }
            }
        }

        private readonly SerialPort _serialPort;

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
                    Console.WriteLine("do Task work,i={0}", i);
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
            if (_serialPort.IsOpen)
            {
                byte[] floatBytes = BitConverter.GetBytes(ControlFirst);
                byte[] floatBytes2 = BitConverter.GetBytes(ControlSecond);
                byte[] floatBytes3 = BitConverter.GetBytes(ControlThird);
                byte[] combinedBytes = new byte[15]; // 1 byte (frame head) + 12 bytes (floats) + 2 bytes (frame tail)

                switch (ControlMode)
                {
                    case View2Model.CarControlModeT.SpeedControlSelf:
                        combinedBytes[0] = 0xFF;
                        break;

                    case View2Model.CarControlModeT.SpeedControlGround:
                        combinedBytes[0] = 0xFD;
                        break;

                    case View2Model.CarControlModeT.LocationControl:
                        combinedBytes[0] = 0xFB;
                        break;
                }

                Array.Copy(floatBytes, 0, combinedBytes, 1, 4);
                Array.Copy(floatBytes2, 0, combinedBytes, 5, 4);
                Array.Copy(floatBytes3, 0, combinedBytes, 9, 4);

                switch (ControlMode)
                {
                    case View2Model.CarControlModeT.SpeedControlSelf:
                        combinedBytes[13] = 0xFE;
                        break;

                    case View2Model.CarControlModeT.SpeedControlGround:
                        combinedBytes[13] = 0xFC;
                        break;

                    case View2Model.CarControlModeT.LocationControl:
                        combinedBytes[13] = 0xFA;
                        break;
                }

                _serialPort.Write(combinedBytes, 0, 15);
            }
        }
    }
}