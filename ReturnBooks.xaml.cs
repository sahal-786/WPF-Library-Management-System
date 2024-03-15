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
    /// Interaction logic for ReturnBooks.xaml
    /// </summary>
    public partial class ReturnBooks : Window
    {
        int userID;
        public ReturnBooks()
        {
            InitializeComponent();
            userID = AppContext.LoggedInUserId;
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
            AccountMainPage accountMainPage = new AccountMainPage();
            accountMainPage.Show();
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlCon.Open();

                // Validate the input (tbSearch.Text should be a valid TaskID)
                if (string.IsNullOrWhiteSpace(tbSearch.Text) || !int.TryParse(tbSearch.Text, out int SelectedBookID))
                {
                    MessageBox.Show("Please enter a valid SelectedBookID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Begin a SQL transaction
                using (SqlTransaction transaction = SqlCon.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Insert the record into CompletedTasks
                        string updateQuery = "UPDATE B SET B.Title = SB.Title, B.Author = SB.Author, B.Genre = SB.Genre FROM Book B INNER JOIN SelectedBook SB ON B.BookID = SB.BookID WHERE SB.SelectedBookID = @SelectedBookID AND SB.UserID = @UserID;";

                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, SqlCon, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@SelectedBookID", SelectedBookID);
                            updateCommand.Parameters.AddWithValue("@UserID", userID);
                            updateCommand.ExecuteNonQuery();
                        }

                        // Step 2: Delete the record from UniversityTasks
                        string deleteQuery = "DELETE FROM SelectedBook WHERE SelectedBookID = @SelectedBookID";

                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, SqlCon, transaction))
                        {
                            deleteCommand.Parameters.AddWithValue("@SelectedBookID", SelectedBookID);

                            deleteCommand.ExecuteNonQuery();
                        }

                        // If both steps succeed, commit the transaction
                        transaction.Commit();

                        MessageBox.Show("Book Succeccfully Returned.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        SqlCon.Close();

                        // Refresh the data grid or perform any other necessary actions
                        LoadGrid();
                    }
                    catch (Exception ex)
                    {
                        // If an exception occurs during the transaction, roll back the changes
                        transaction.Rollback();
                        MessageBox.Show("An error occurred. Transaction rolled back. " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SqlCon.Close();
            }
        }
    }
}
