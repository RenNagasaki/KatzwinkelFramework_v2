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
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public string Name
        {
            get { return text_name.Text; }
        }

        public InputWindow()
        {
            InitializeComponent();
        }

        private void btn_name_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(text_name.Text))
            {
                DialogResult = true;
                this.Hide();
            }
            else
            {
                MessageBox.Show("No name entered...", "Error");
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Convert.ToBoolean(DialogResult))
            {
                e.Cancel = true;
                DialogResult = false;
                this.Hide();
            }
        }
    }
}
