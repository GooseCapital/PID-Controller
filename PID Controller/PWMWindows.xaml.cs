using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PID_Controller
{
    /// <summary>
    /// Interaction logic for PWMWindows.xaml
    /// </summary>
    public partial class PWMWindows : Window
    {
        public PWMWindows()
        {
            InitializeComponent();
        }

        public void AddPwmData(double x)
        {
            //add value to collection
            PWMChart.SeriesCollection[0].Values.Add(new DateModel()
            {
                DateTime = DateTime.Now,
                Value = x
            });
            if (PWMChart.SeriesCollection[0].Values.Count > 50)
                PWMChart.SeriesCollection[0].Values.RemoveAt(0);
        }
    }
}
