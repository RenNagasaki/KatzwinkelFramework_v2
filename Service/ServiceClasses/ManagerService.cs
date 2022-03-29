using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ManagerService : IManagerService
    {
        string CryptionPassPhrase = "Katzwinkel_Framework2017@Julian-The|Master@MainService";
        static List<string> loggedInUsers = new List<string>();

        public bool Login(string username, string password)
        {
            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                if (!loggedInUsers.Contains(username))
                {
                    loggedInUsers.Add(username);
                }

                return true;
            }

            return false;
        }

        public AppointmentObject AddApointment(string username, AppointmentObject app)
        {
            if (loggedInUsers.Contains(username))
            {
                // DoSomething

                return app;
            }

            return null;
        }

        public bool RemoveAppointment(string username, int appId)
        {
            if (loggedInUsers.Contains(username))
            {
                return true;
            }

            return false;
        }

        public AppointmentObject[] GetAppointments(string username)
        {
            List<AppointmentObject> apps = new List<AppointmentObject>();
            if (loggedInUsers.Contains(username))
            {
                AppointmentObject app1 = new AppointmentObject();
                app1.id = 1;
                app1.name = "Test1";
                app1.description = "TestDesc1";
                app1.year = 2017;
                app1.month = 3;
                app1.day = 16;
                app1.fromHour = 15;
                app1.fromMinute = 45;
                app1.toHour = 20;
                app1.toMinute = 15;

                AppointmentObject app2 = new AppointmentObject();
                app2.id = 2;
                app2.name = "Test2";
                app2.description = "TestDesc2";
                app2.year = 2017;
                app2.month = 3;
                app2.day = 16;
                app2.fromHour = 14;
                app2.fromMinute = 10;
                app2.toHour = 15;
                app2.toMinute = 30;

                apps.Add(app1);
                apps.Add(app2);
            }

            return apps.ToArray();
        }

        public ExpenseTable AddExpenseTable(string username, string name)
        {
            if (loggedInUsers.Contains(username))
            {
                return new ExpenseTable() { tableName = name, id = 1, expenseItems = new List<ExpenseObject>().ToArray() };
            }

            return null;
        }

        public bool RemoveExpenseTable(string username, int id)
        {
            if (loggedInUsers.Contains(username))
            {
                return true;
            }

            return false;
        }

        public ExpenseTable[] GetExpenseTables(string username)
        {
            List<ExpenseTable> expenses = new List<ExpenseTable>();
            if (loggedInUsers.Contains(username))
            {
                ExpenseTable tab = new ExpenseTable();
                tab.id = 1;
                tab.tableName = "General";
                List<ExpenseObject> tabobjs = new List<ExpenseObject>();
                tabobjs.Add(new ExpenseObject() { Amount = 500, Name = "Miete", Recurring = true, Type = ExpenseType.Household });
                tabobjs.Add(new ExpenseObject() { Amount = 1500, Name = "Gehalt", Recurring = true, Type = ExpenseType.Income });
                tab.expenseItems = tabobjs.ToArray();

                ExpenseTable tab2 = new ExpenseTable();
                tab2.id = 1;
                tab2.tableName = "TestTable1";
                List<ExpenseObject> tabobjs2 = new List<ExpenseObject>();
                tabobjs2.Add(new ExpenseObject() { Amount = 500, Name = "Miete", Recurring = true, Type = ExpenseType.Household });
                tabobjs2.Add(new ExpenseObject() { Amount = 1500, Name = "Gehalt", Recurring = true, Type = ExpenseType.Income });
                tab2.expenseItems = tabobjs.ToArray();

                expenses.Add(tab);
                expenses.Add(tab2);
            }

            return expenses.ToArray();
        }


        public bool UpdateExpenseTables(string username, ExpenseTable[] expenseTables)
        {
            if (loggedInUsers.Contains(username))
            {
                return true;
            }

            return false;
        }
    }
}
