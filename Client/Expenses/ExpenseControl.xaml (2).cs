using Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for ExpenseControl.xaml
    /// </summary>
    public partial class ExpenseControl : UserControl
    {
        internal BindingList<ExpenseObject> expenseItems = new BindingList<ExpenseObject>();
        internal int id = -1;
        bool colDataSet = false;

        public ExpenseControl()
        {
            InitializeComponent();

            expenseItems.Add(new ExpenseObject() { Name = "Miete", Type = ExpenseType.Household, Recurring = true, Amount = 500 });
            data_expenses.ItemsSource = expenseItems;
            expenseItems.Clear();
        }

        internal void SetColData()
        {
            if (!colDataSet)
            {
                data_expenses.Columns[0].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                data_expenses.Columns[1].MinWidth = 100;
                data_expenses.Columns[2].MinWidth = 50;
                data_expenses.Columns[3].MinWidth = 75;

                Style s = new Style();
                s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                data_expenses.Columns[3].CellStyle = s;

                colDataSet = true;
            }
        }

        private void SetBackground()
        {
            foreach (ExpenseObject item in expenseItems)
            {
                var row = data_expenses.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                switch (item.Type)
                {
                    case ExpenseType.Income:
                        row.Background = Brushes.Green;
                        break;
                    case ExpenseType.DailyNeeds:
                        row.Background = Brushes.Blue;
                        break;
                    case ExpenseType.Food:
                        row.Background = Brushes.Yellow;
                        break;
                    case ExpenseType.Games:
                        row.Background = Brushes.Red;
                        break;
                    case ExpenseType.Hardware:
                        row.Background = Brushes.DarkRed;
                        break;
                    case ExpenseType.Household:
                        row.Background = Brushes.Brown;
                        break;
                    case ExpenseType.Others:
                        row.Background = Brushes.Gray;
                        break;
                    case ExpenseType.SpecialFood:
                        row.Background = Brushes.GreenYellow;
                        break;
                }
            }
        }

        private void data_expenses_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SetColData();
            SetBackground();
            SetSum();
        }

        private void data_expenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetColData();
            SetBackground();
            SetSum();
        }

        private void data_expenses_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            SetColData();
            SetBackground();
            SetSum();
        }


        private void SetSum()
        {
            int amount = 0;
            foreach (ExpenseObject exp in expenseItems)
            {
                if (exp.Type == ExpenseType.Income)
                {
                    amount += exp.Amount;
                }
                else
                {
                    amount -= exp.Amount;
                }
            }

            if (amount < 0)
            {
                text_sum.Foreground = Brushes.Red;
            }
            else
            {
                text_sum.Foreground = Brushes.Black;
            }

            text_sum.Text = amount + "€";
        }
    }
}
