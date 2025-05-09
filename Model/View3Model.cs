
using System.Collections.ObjectModel;

namespace prism_serial.Model
{
    public class View3Model
    {
        public View3Model()
        {
            
        }
        public GamepadState XboxData = new GamepadState();
        public class GamepadState
        {
            public short LeftThumbX { get; set; }
            public short LeftThumbY { get; set; }
            public short RightThumbX { get; set; }
            public short RightThumbY { get; set; }
            public byte LeftTrigger { get; set; }
            public byte RightTrigger { get; set; }
        }
    }
}