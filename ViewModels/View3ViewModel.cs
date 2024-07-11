
using OxyPlot;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using prism_serial.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prism_serial.ViewModels
{
    public class View3ViewModel:BindableBase
    {
        public View3ViewModel() 
        {
            TestCommand = new DelegateCommand(OnTest);
            
        }

        private View3Model obj=new View3Model();
        public PlotModel Plot
        {
            get { return obj.Plot; }
            set { obj.Plot = value;}
        }
        public ObservableCollection<LineSeries> SerialCollection
        {  get { return obj.SerialCollection; } set { obj.SerialCollection = value;RaisePropertyChanged(); } }

        public DelegateCommand TestCommand {  get; set; }

       
        private void OnTest()
        {

            foreach (var series in SerialCollection)
            {
                series.Points.Add(new DataPoint(DateTime.Now.ToOADate(), GetNewMotorValue()));
                if(series.Points.Count>1000)
                {
                    series.Points.RemoveAt(0);
                }
            }
            
            Plot.InvalidatePlot(true);

        }
        private double GetNewMotorValue()
        {
            // Replace with your actual method for getting new motor values
            return new Random().NextDouble();
        }

    }
}
