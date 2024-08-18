using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace prism_serial.Model
{
    public class View2Model
    {
        public View2Model()
        {
            TextListSelected = "车身速度";
            ControlMode = CarControlModeT.SpeedControlSelf;

        }

        public string TextListSelected { get; set; }
        public List<string> TextListControl { get; set; } = new() { "车身速度", "大地速度", "位置闭环" };


        public ObservableCollection<string> ControlStr { get; set; } = new() { "0.0", "0.0", "0.0" };
        public ObservableCollection<float> ControlFloat { get; set; } = new() { 0.0f, 0.0f, 0.0f };
        public CarControlModeT ControlMode { get; set; }

        public enum CarControlModeT
        {
            LocationControl,
            SpeedControlSelf,
            SpeedControlGround,
        }
        //发给html的数据
        public class SerialPoints
        {
            public List<double> _x { set; get; } = new();
            public List<double> _y { set; get; } = new();
        }

    }
}