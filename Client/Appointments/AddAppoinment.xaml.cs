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

namespace Client
{
    /// <summary>
    /// Interaction logic for AddAppoinment.xaml
    /// </summary>
    public partial class AddAppoinment : Window
    {
        public AddAppoinment()
        {
            InitializeComponent();
        }

        private void btn_done_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(text_name.Text))
            {
                if (!String.IsNullOrEmpty(text_from.Text) && !String.IsNullOrEmpty(text_to.Text))
                {
                    DialogResult = true;
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("No time set", "Error");
                }
            }
            else
            {
                MessageBox.Show("No appointment name set", "Error");
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Hide();
        }
    }
}
