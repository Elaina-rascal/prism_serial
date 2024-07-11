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

namespace prism_simpletemplate.ViewModels
{
    public class View2ViewModel : BindableBase
    {
        public View2ViewModel(SerialPort serialPort_in) 
        {
            this.serialPort = serialPort_in;
            TestCommand = new DelegateCommand(OnTest);
        }
        private View2Model obj = new View2Model();
        public string text_list_selected
        {
            get { return obj.text_list_selected; }
            set { obj.text_list_selected = value; }
        }
        public List<string> text_list_control
        {
            get { return obj.text_list_control; }
            set { obj.text_list_control = value; }
        }
        //第一第二第三个控制量
        public string control_first_text
        {
            get { return obj.control_first_text; }
            set { obj.control_first_text = value; control_first = float.Parse(value); }
        }
        public string control_second_text
        {
            get { return obj.control_second_text; }
            set { obj.control_second_text = value;control_second = float.Parse(value); }
        }
        public string control_third_text
        {
            get { return obj.control_third_text; }
            set { obj.control_third_text = value;control_third = float.Parse(value); }
        }
        public float control_first
        {
            get { return obj.control_first; }
            set { obj.control_first = value; control_first_text = value.ToString(); }
        }
        public float control_second
        {
            get { return obj.control_second; }
            set { obj.control_second = value; control_second_text = value.ToString(); }
        }
        public float control_third
        {
            get { return obj.control_third; }
            set { obj.control_third = value;control_third_text = value.ToString();}
        }

        private SerialPort serialPort;
        //command
        public DelegateCommand CarCommand { get; set; }
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
        
    }
}
