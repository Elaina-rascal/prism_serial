using Microsoft.Web.WebView2.WinForms;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace prism_simpletemplate.Model
{
    public class View2Model
    {
        public View2Model()
        {
            text_list_selected = "车身速度";
           
        }
        public string text_list_selected { get; set; }
        public List<string> text_list_control { get; set; } =new() { "车身速度", "大地速度", "位置闭环" };
        public float control_first { get; set; }
        public float control_second { get; set; }
        public float control_third { get; set; }
        public string control_first_text { get; set; }
        public string control_second_text { get; set; }
        public string control_third_text { get; set; }
        public enum Car_Control_Mode_t
        {
            location_control,
            speed_control_self,
            speed_control_ground,
        }
    }
}
