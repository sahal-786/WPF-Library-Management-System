using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Xml.Linq;

namespace Library_Managment_System
{
    /// <summary>
    /// Interaction logic for AddBooks.xaml
    /// </summary>
    public partial class AddBooks : Window
    {
        public AddBooks()
        {
            InitializeComponent();
        }
        SqlConnection SqlCon = new SqlConnection(@"Data Source=SAHAL\MSSQLSERVER01;Initial Catalog=crud;Integrated Security=True");
        public void clearTextboxes()
        {
            tbTitle.Clear();
            tbAuthor.Clear();
            tbGerne.Clear();
        }

        public bool validation()
        {
            if (String.IsNullOrEmpty(tbTitle.Text) || String.IsNullOrEmpty(tbAuthor.Text) || String.IsNullOrEmpty(tbGerne.Text))
            {
                return false;
            }
            else if (String.IsNullOrWhiteSpace(tbTitle.Text) || String.IsNullOrWhiteSpace(tbAuthor.Text) || String.IsNullOrWhiteSpace(tbGerne.Text))
            {
                return false;
            }
            return true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AccountMainPage accountMainWindow = new AccountMainPage();
            accountMainWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validation())
                {
                    SqlCommand cmd = new SqlCommand("insert into Book values ( @Title, @Author, @Genre)", SqlCon);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Title", tbTitle.Text);
                    cmd.Parameters.AddWithValue("@Author", tbAuthor.Text);
                    cmd.Parameters.AddWithValue("@Genre", tbGerne.Text);

                    SqlCon.Open();
                    cmd.ExecuteNonQuery();
                    SqlCon.Close();

                    MessageBox.Show("Book Successfully Added. Go to the previous screen to check.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearTextboxes();
                }
                else
                {
                    MessageBox.Show("Some Fields are Empty or have incorrect data filled.\nPlease fill them Correctly", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SqlCon.Close();
                clearTextboxes();
            }
        }
    }
}
