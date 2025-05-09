using Prism.Commands;
using Prism.Mvvm;
using prism_serial.Model;
using SharpDX.XInput;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading.Tasks;
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

            //_timer = new Timer(100); // 10 Hz
            //_timer.Elapsed += (s, e) => ReadController();
            //_timer.Start();
            StartReadingController();
        }
        SerialPort _serial ;
        private View3Model _obj = new View3Model();
        public bool IsAPressed
        {
            get => _obj.IsAPressed; set {
                if (_obj.IsAPressed != value)
                {
                    _obj.IsAPressed = value;
                    RaisePropertyChanged();
                }
            }
        }
        private async void StartReadingController()
        {
            // 持续异步读取 Xbox 控制器状态
            while (true)
            {
                await Task.Delay(100); // 100ms 轮询间隔，避免过于频繁

                ReadController(); // 读取控制器数据
            }
        }

        public View3Model.GamepadState _xboxData
        {
            get => _obj.XboxData; set {SetProperty(ref _obj.XboxData,value); RaisePropertyChanged(); }
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
                IsAPressed = (state.Gamepad.Buttons & GamepadButtonFlags.A) != 0,
                IsBPressed = (state.Gamepad.Buttons & GamepadButtonFlags.B) != 0,
                IsXPressed = (state.Gamepad.Buttons & GamepadButtonFlags.X) != 0,
                IsYPressed = (state.Gamepad.Buttons & GamepadButtonFlags.Y) != 0

            };
            IsAPressed = (state.Gamepad.Buttons & GamepadButtonFlags.A) != 0;
            Console.WriteLine($"LX: {_xboxData.LeftThumbX}, LY: {_xboxData.LeftThumbY}, RX: {_xboxData.RightThumbX}, RY: {_xboxData.RightThumbY}, LT: {_xboxData.LeftTrigger}, RT: {_xboxData.RightTrigger}");
        }
    }
}