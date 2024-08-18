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


        private readonly SerialPort _serialPort;

        private View2Model.SerialPoints serialData = new();

      

        //往charpage.html传递数据
        public DelegateCommand TestCommand { get; set; }

      
        public delegate void PostDelegate(string webMessageAsJson);

        //给Web页面传递数据
        public PostDelegate postDelegate;

        private void OnTest()
        {
            //Task.Run(() =>//测试模拟后台输出文件
            //{
            //    var sePoints1 = new double[20, 2];
            //    for (int i = 0; i < 20; i++)
            //    {
            //        Thread.Sleep(2000);
            //        sePoints1[i, 0] = i;
            //        sePoints1[i, 1] = i * 1.2;
            //        Console.WriteLine("do Task work,i={0}", i);
            //    }
            //});
            ////Web.ObjectForScripting.
            //var sePoints = new double[20, 2];
            //for (int i = 0; i < 20; i++)
            //{
            //    sePoints[i, 0] = i;
            //    sePoints[i, 1] = i * 1.2;
            //}
            for (int i = 0;i< 20; i++)
            {
                serialData._x.Add(i);
                serialData._y.Add(i * 1.2);
            }
            //serialData._x.Add();
            postDelegate?.Invoke(JsonConvert.SerializeObject(serialData));
        }

        /*
         * 车速度帧头为0xFF帧尾为0xFE
         * 大地速度帧头为0xFD帧尾为0xFC
         * 位置闭环帧头为0xFB帧尾为0xFA
         */

        
    }
}