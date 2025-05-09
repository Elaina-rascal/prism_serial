using Prism.Commands;
using Prism.Mvvm;
using prism_serial.Model;
using SharpDX.XInput;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Timers;
using static prism_serial.Model.View3Model;
//using v
namespace prism_serial.ViewModels
{
    public class View3ViewModel : BindableBase
    {
        public View3ViewModel(SerialPort serialPortin)
        {
            _serial = serialPortin;
            _controller = new Controller(UserIndex.One);
            if (!_controller.IsConnected)
            {
                Console.WriteLine("Xbox Controller not connected.");
                return;
            }

            _timer = new Timer(100); // 10 Hz
            _timer.Elapsed += (s, e) => ReadController();
            _timer.Start();
        }
        SerialPort _serial ;
        private View3Model _obj = new View3Model();
        public View3Model.GamepadState _xboxData
        {
            get => _obj.XboxData; set { _obj.XboxData = value; RaisePropertyChanged(); }
        }
        private Timer _timer;
        private Controller _controller;
        private GamepadState _state = new GamepadState();
        private void ReadController()
        {
            var state = _controller.GetState();
            _xboxData = new GamepadState
            {
                LeftThumbX = state.Gamepad.LeftThumbX,
                LeftThumbY = state.Gamepad.LeftThumbY,
                RightThumbX = state.Gamepad.RightThumbX,
                RightThumbY = state.Gamepad.RightThumbY,
                LeftTrigger = state.Gamepad.LeftTrigger,
                RightTrigger = state.Gamepad.RightTrigger,
            };
            Console.WriteLine($"LX: {_xboxData.LeftThumbX}, LY: {_xboxData.LeftThumbY}, RX: {_xboxData.RightThumbX}, RY: {_xboxData.RightThumbY}, LT: {_xboxData.LeftTrigger}, RT: {_xboxData.RightTrigger}");
        }
    }
}