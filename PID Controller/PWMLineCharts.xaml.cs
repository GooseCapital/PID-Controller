using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Windows.Controls;

namespace PID_Controller
{
    /// <summary>
    /// Interaction logic for LineCharts.xaml
    /// </summary>
    public partial class PWMLineCharts : UserControl
    {
        public PWMLineCharts()
        {
            InitializeComponent();
            var dayConfig = Mappers.Xy<DateModel>()
                .X(dayModel => (double)dayModel.DateTime.Ticks / TimeSpan.FromMilliseconds(10).Ticks)
                .Y(dayModel => dayModel.Value);
            SeriesCollection = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    Title = "PWM",
                    Values = new ChartValues<DateModel> {  },
                    PointGeometry = null
                }
            };

            //modifying any series values will also animate and update the chart
            //            SeriesCollection[0].Values.Add(5d);
            Formatter = value => new System.DateTime((long)(value * TimeSpan.FromMilliseconds(10).Ticks)).ToString("T");
            DataContext = this;
        }
        public Func<double, string> Formatter { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
    }
}
