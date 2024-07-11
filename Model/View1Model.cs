using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prism_simpletemplate.Model
{
    public class View1Model
    {
        public View1Model() 
        {
            Received_text = string.Empty;
            Trans_text = string.Empty;
            show_text = string.Empty;
            com = new ObservableCollection<string>();
            //log=new ObservableCollection<string>();
            //log = new ObservableCollection<MessageItem>();
            baudrate_select = 9600;
            IsComBaudEnable = true;
            Isvisual_data = false;
        }
        
        
        
        
        
        public string Received_text
        { get; set; }
        public string Trans_text
        { get; set; }
        public string show_text
        { get; set; }


        public ObservableCollection<string> com { get; set; }
        //public ObservableCollection<string> log {  get; set; }

        //public ObservableCollection<MessageItem> log { get; set; }
        public List<int> baudrate = new() { 9600, 115200, 1152000, 2000000 };
        public bool IsComBaudEnable { get; set; }
        public bool Isvisual_data { get; set; }
        public int baudrate_select { get; set; }
        public string com_select { get; set; }
    }
}
