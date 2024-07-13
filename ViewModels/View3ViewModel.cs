using OxyPlot;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using prism_serial.Model;
using System;
using System.Collections.ObjectModel;

namespace prism_serial.ViewModels
{
    public class View3ViewModel : BindableBase
    {
        public View3ViewModel()
        {
            TestCommand = new DelegateCommand(OnTest);
        }

        private View3Model _obj = new View3Model();

        public PlotModel Plot
        {
            get => _obj.Plot;
            set => _obj.Plot = value;
        }

        public ObservableCollection<LineSeries> SerialCollection
        {
            get => _obj.SerialCollection;
            set { _obj.SerialCollection = value; RaisePropertyChanged(); }
        }

        public DelegateCommand TestCommand { get; set; }

        private void OnTest()
        {
            foreach (var series in SerialCollection)
            {
                series.Points.Add(new DataPoint(DateTime.Now.ToOADate(), GetNewMotorValue()));
                if (series.Points.Count > 1000)
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