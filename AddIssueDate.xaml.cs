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

namespace Library_Managment_System
{
    /// <summary>
    /// Interaction logic for AddIssueDate.xaml
    /// </summary>
    public partial class AddIssueDate : Window
    {
        
        public string SelectedDate { get; private set; }


        public AddIssueDate()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the input (txtIssueDate.Text should be a valid date)
                if (!string.IsNullOrWhiteSpace(tbIssueDate.Text))
                {
                    // Set the SelectedDateString property with the date string
                    SelectedDate = tbIssueDate.Text;

                    // Set the DialogResult to true, indicating the user clicked the "Add" button
                    this.DialogResult = true;

                    // Close the window
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter a valid date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
