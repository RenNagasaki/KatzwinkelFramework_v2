using Client.ManagerService;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
using Hardcodet.Wpf.TaskbarNotification;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using Client.Chat;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static string username = Environment.UserName + "_" + DateTime.Now.Millisecond;
        ManagerServiceClient client = null;
        TaskbarIcon tbi = new TaskbarIcon();
        double height = 0;
        ChatControl chat = null;
        Dictionary<string, ExpenseControl> expenseTables = new Dictionary<string, ExpenseControl>();

        public MainWindow()
        {
            InitializeComponent();
            chat = new ChatControl(this);
            tab_chat.Content = chat;
        }

        ~MainWindow()
        {
            chat.rc.Stop(username);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CloseConnection();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (tabCtrl.SelectedItem == tab_appointments)
            {
                scroll_apps.Value -= e.Delta / 4;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenConnection();

            if (client.State == System.ServiceModel.CommunicationState.Opened)
            {
                cliprect.Rect = new Rect(0, 0, grid_appointments.ActualWidth, grid_appointments.ActualHeight);
                SetPositions();
                GetAllApps();
                GetAllExpenseTables();

                tabCtrl.SelectedIndex = 1;
            }
        }

        internal void ShowNotification(string title, string message)
        {
            tbi.ShowBalloonTip(title, message, BalloonIcon.Info);
        }

        void OpenConnection()
        {
            try
            {
                client = new ManagerServiceClient();
                client.Open();
                bool res = client.Login(username, "abcd");

                if (res)
                {
                    ShowNotification("Logged in", "Successfull logged in");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("No connection possible.\r\nClient will now close itself.\r\nErrormessage: " + e.Message, "Error while connecting to service", MessageBoxButton.OK);
                Environment.Exit(0);
            }
        }

        void CloseConnection()
        {
            client.Close();
        }

        #region Appointments
        void SetPositions()
        {
            int i = 0;
            AppointmentControl app = new AppointmentControl();
            for (i = 0; i < grid_apps.Children.Count; i++)
            {
                app = grid_apps.Children[i] as AppointmentControl;
                if (app != null)
                {
                    app.Margin = new Thickness(0, (app.Height + 5) * (i), 0, 0);
                    app.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                }
            }

            height = ((i - 1) * (app.Height + 5)) + app.Height;
            scroll_apps.Minimum = 0;
            scroll_apps.Maximum = height - grid_appointments.ActualHeight;
        }

        internal void GetAllApps()
        {
            AppointmentObject[] apps = client.GetAppointments(username);

            foreach (AppointmentObject appobj in apps)
            {
                AppointmentControl app = new AppointmentControl(this, appobj, false);
            }
        }

        internal void AddApp(AppointmentControl app)
        {
            grid_apps.Children.Add(app);
            SetPositions();
        }

        internal void RemoveApp(AppointmentControl app)
        {
            bool res = client.RemoveAppointment(username, app.id);

            if (res)
            {
                grid_apps.Children.Remove(app);
                SetPositions();
            }
        }

        private void scroll_apps_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            grid_apps.Margin = new Thickness(grid_apps.Margin.Left, scroll_apps.Value * -1, grid_apps.Margin.Right, grid_apps.Margin.Bottom);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cliprect.Rect = new Rect(0, 0, grid_appointments.ActualWidth, grid_appointments.ActualHeight);
            scroll_apps.Maximum = height - grid_appointments.ActualHeight;
            chat.UpdateRect();
        }

        private void menu_add_Click(object sender, RoutedEventArgs e)
        {
            AddAppoinment add = new AddAppoinment();

            bool result = Convert.ToBoolean(add.ShowDialog());

            if (result)
            {
                string name = add.text_name.Text;
                string desc = new TextRange(add.text_description.Document.ContentStart, add.text_description.Document.ContentEnd).Text;
                string[] dateFromArr = add.date_app.Text.Split(".".ToCharArray());
                string[] fromArr = add.text_from.Text.Split(":".ToCharArray());
                //string[] dateToArr = add.date_to.Text.Split(".".ToCharArray());
                string[] toArr = add.text_to.Text.Split(":".ToCharArray());

                int fromyear = Convert.ToInt32(dateFromArr[2]);
                int frommonth = Convert.ToInt32(dateFromArr[1]);
                int fromday = Convert.ToInt32(dateFromArr[0]);

                int fromhour = Convert.ToInt32(fromArr[0]);
                int fromminute = 0;
                if (fromArr.Length > 1)
                {
                    fromminute = Convert.ToInt32(fromArr[1]);
                }

                //int toyear = Convert.ToInt32(dateToArr[2]);
                //int tomonth = Convert.ToInt32(dateToArr[1]);
                //int today = Convert.ToInt32(dateToArr[0]);

                int tohour = Convert.ToInt32(toArr[0]);
                int tominute = 0;
                if (toArr.Length > 1)
                {
                    tominute = Convert.ToInt32(toArr[1]);
                }

                DateTime dateFrom = new DateTime(fromyear, frommonth, fromday, fromhour, fromminute, 0);
                DateTime dateTo = new DateTime(fromyear, frommonth, fromday, tohour, tominute, 0);

                AppointmentObject appobj = new AppointmentObject();
                appobj.userName = "Test";
                appobj.name = name;
                appobj.description = desc;
                appobj.year = fromyear;
                appobj.month = frommonth;
                appobj.day = fromday;
                appobj.fromHour = fromhour;
                appobj.fromMinute = fromminute;
                appobj.toHour = tohour;
                appobj.toMinute = tominute;

                appobj = client.AddApointment(username, appobj);

                if (appobj != null)
                {
                    AppointmentControl app = new AppointmentControl(this, appobj, true);
                }
            }
        }
        #endregion

        #region Expenses
        private void GetAllExpenseTables()
        {
            ExpenseTable[] tables = client.GetExpenseTables(username);

            foreach (ExpenseTable table in tables)
            {
                AddTable(table);
            }
        }

        private void AddTable(ExpenseTable table)
        {
            TabItem tab = new TabItem();
            tab.Header = table.tableName;

            ExpenseControl exp = new ExpenseControl();
            exp.id = table.id;

            foreach (ExpenseObject expo in table.expenseItems)
            {
                exp.expenseItems.Add(expo);
            }

            tab.Content = exp;
            tabCtrl_expenses.Items.Add(tab);
            tabCtrl_expenses.SelectedItem = tab;
        }

        private void btn_expenses_reset_Click(object sender, RoutedEventArgs e)
        {
            tabCtrl_expenses.Items.Clear();
            GetAllExpenseTables();
        }

        private void btn_expenses_save_Click(object sender, RoutedEventArgs e)
        {
            List<ExpenseTable> tables = new List<ExpenseTable>();
            foreach (TabItem item in tabCtrl_expenses.Items)
            {
                ExpenseControl control = item.Content as ExpenseControl;

                if (control != null)
                {
                    ExpenseTable table = new ExpenseTable();
                    table.id = control.id;
                    table.tableName = item.Header.ToString();
                    table.expenseItems = control.expenseItems.ToArray();

                    tables.Add(table);
                }
            }

            bool res = client.UpdateExpenseTables(username, tables.ToArray());

            if (res)
            {
                ShowNotification("Successfully saved", "The tables got saved successfully.");
            }
        }

        private void menu_expense_add_Click(object sender, RoutedEventArgs e)
        {
            InputWindow input = new InputWindow();

            bool res = Convert.ToBoolean(input.ShowDialog());

            if (res)
            {
                ExpenseTable table = client.AddExpenseTable(username, input.Name);
                AddTable(table);
            }
        }

        private void menu_expense_remove_Click(object sender, RoutedEventArgs e)
        {
            TabItem item = tabCtrl_expenses.SelectedItem as TabItem;

            if (item != null && item.Header.ToString() != "General")
            {
                int id = ((ExpenseControl)item.Content).id;
                bool res = client.RemoveExpenseTable(username, id);

                if (res)
                {
                    tabCtrl_expenses.Items.Remove(item);
                }
            }
        }
        #endregion
    }
}