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

namespace Library_Managment_System
{
    /// <summary>
    /// Interaction logic for SelectedBooks.xaml
    /// </summary>
    public partial class SelectedBooks : Window
    {
        public SelectedBooks()
        {
            InitializeComponent();
            LoadGrid();
        }
        SqlConnection SqlCon = new SqlConnection(@"Data Source=SAHAL\MSSQLSERVER01;Initial Catalog=crud;Integrated Security=True");
        DataSet DataSet = new DataSet();
        public void clearTextboxes()
        {
            tbSearch.Clear();
        }

        public void LoadGrid()
        {
            int userID = AppContext.LoggedInUserId;
            string query = "SELECT * FROM SelectedBook WHERE UserID = @UserID";

            SqlCommand sqlCommand = new SqlCommand(query, SqlCon);
            sqlCommand.Parameters.AddWithValue("@UserID", userID);

            DataTable dt = new DataTable();
            try
            {
                SqlCon.Open();
                SqlDataReader sdr = sqlCommand.ExecuteReader();
                dt.Load(sdr);
                SqlCon.Close();
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SqlCon.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AccountMainPage acp = new AccountMainPage();
            acp.Show(); 
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadGrid();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                // Checking if the textbox is empty or not a valid integer
                if (string.IsNullOrWhiteSpace(tbSearch.Text))
                {
                    MessageBox.Show("Please enter a valid search string.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    SqlCon.Open();
                    string query = "select * from SelectedBook where Title like @Title";

                    using (SqlCommand cmd = new SqlCommand(query, SqlCon))
                    {
                        cmd.Parameters.AddWithValue("@Title", "%" + tbSearch.Text + "");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGrid.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                SqlCon.Close();
            }
        }
    }
}
