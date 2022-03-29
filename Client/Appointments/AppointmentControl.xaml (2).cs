using Client.ManagerService;
using Framework;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for AppointmentControl.xaml
    /// </summary>
    public partial class AppointmentControl : UserControl
    {
        MainWindow main = null;
        DateTime from = DateTime.Now;
        DateTime to = DateTime.Now;
        public int id = -1;

        public AppointmentControl()
        {
            InitializeComponent();

            SetUp();
        }

        public AppointmentControl(MainWindow main, AppointmentObject appobj, bool notificate)
        {
            InitializeComponent();

            this.main = main;

            this.id = appobj.id;
            lbl_appname.Content = appobj.name;
            tBlock_apptext.Text = appobj.description;
            this.from = new DateTime(appobj.year, appobj.month, appobj.day, appobj.fromHour, appobj.fromMinute, 0);
            this.to = new DateTime(appobj.year, appobj.month, appobj.day, appobj.toHour, appobj.toMinute, 0);
            SetUp();

            main.AddApp(this);
            if (notificate)
            {
                main.ShowNotification("New appointment", "On " + appobj.month.ToString().PadLeft(2, '0') + "/" + appobj.day.ToString().PadLeft(2, '0') + "/" + appobj.year + "\r\n" + "From: " + from.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" + from.TimeOfDay.Minutes.ToString().PadLeft(2, '0') + "\r\n" + "To: " + to.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" + to.TimeOfDay.Minutes.ToString().PadLeft(2, '0'));
            }
        }

        void SetUp()
        {
            lbl_dayname.Content = from.DayOfWeek.ToString();
            lbl_daynumber.Content = from.Day;
            lbl_month.Content = GetMonth(from.Month);

            lbl_fromto.Content = from.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" + from.TimeOfDay.Minutes.ToString().PadLeft(2, '0') + " - " + to.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" + to.TimeOfDay.Minutes.ToString().PadLeft(2, '0');
        }

        string GetMonth(int month)
        {
            string retMonth = "";
            switch (month)
            {
                case 1:
                    retMonth = "January"; 
                    break;
                case 2:
                    retMonth = "February"; 
                    break;
                case 3:
                    retMonth = "March"; 
                    break;
                case 4:
                    retMonth = "April"; 
                    break;
                case 5:
                    retMonth = "May"; 
                    break;
                case 6:
                    retMonth = "June"; 
                    break;
                case 7:
                    retMonth = "July"; 
                    break;
                case 8:
                    retMonth = "August"; 
                    break;
                case 9:
                    retMonth = "September"; 
                    break;
                case 10:
                    retMonth = "October"; 
                    break;
                case 11:
                    retMonth = "November"; 
                    break;
                case 12:
                    retMonth = "December"; 
                    break;
            }

            return retMonth;
        }

        private void btn_remove_Click(object sender, RoutedEventArgs e)
        {
            main.RemoveApp(this);
        }
    }
}
