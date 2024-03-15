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
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Data.Common;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

namespace Library_Managment_System
{
    /// <summary>
    /// Interaction logic for AccountMainPage.xaml
    /// </summary>
    public partial class AccountMainPage : Window
    {
        private int userID;
        public AccountMainPage()
        {
            InitializeComponent();
            this.userID = AppContext.LoggedInUserId;
            UpdateWelcomeMessage();
            LoadGrid();

            //MessageBox.Show("User ID is: " + AppContext.LoggedInUserId + ".", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateWelcomeMessage()
        {
            // Update the TextBlock to display the welcome message with the user ID
            welcometb.Text += userID.ToString();
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
            string query = "SELECT * FROM Book ";

            SqlCommand sqlCommand = new SqlCommand(query, SqlCon);

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

        //Logout Bitton Event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginPage = new MainWindow();
            loginPage.Show();
            this.Close();
        }

        //Reload Button Event
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            LoadGrid();
        }

        //Addbooks Button Event
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddBooks ab= new AddBooks();
            ab.Show();
            this.Close();
        }
        //Search Event
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
                    string query = "select * from Book where Title like @Title";

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
            SelectedBooks sb= new SelectedBooks();
            sb.Show();
            this.Close();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ReturnBooks returnBooks = new ReturnBooks();
            returnBooks.Show();
            this.Close ();
        }

        AddIssueDate AID;

        //Select Buttton Functionality
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlCon.Open();

                // Validate the input (tbSearch.Text should be a valid BookID)
                if (string.IsNullOrWhiteSpace(tbSearch.Text) || !int.TryParse(tbSearch.Text, out int bookID))
                {
                    MessageBox.Show("Please enter a valid BookID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AID = new AddIssueDate();
                bool? result = AID.ShowDialog();
                // Check if the user clicked the "Add" button on the DateEntryWindow
                if (result == true)
                {
                    // Retrieve the selected date from the DateEntryWindow
                    String issueDate = AID.SelectedDate;

                    // Begin a SQL transaction
                    using (SqlTransaction transaction = SqlCon.BeginTransaction())
                    {
                        try
                        {
                            // Step 1: Insert the record into Selected Books
                            string insertQuery = "INSERT INTO SelectedBook (UserID, BookID, Title, Author, Genre, DateIssued) "+" SELECT U.UserID, B.BookID,  B.Title, B.Author,  B.Genre, '"+ issueDate + "' AS DateIssued  FROM Book B INNER JOIN User_Info U ON U.UserID = @UserID WHERE B.BookID = @BookID;";

                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, SqlCon, transaction))
                            {
                                insertCommand.Parameters.AddWithValue("@BookID", bookID);
                                insertCommand.Parameters.AddWithValue("@UserID", AppContext.LoggedInUserId);
                                insertCommand.ExecuteNonQuery();
                            }


                            // Step 2: Update the records with whitespace in Book table
                            string updateQuery = "UPDATE Book SET Title = ' ', Author = ' ', Genre = ' ' WHERE BookID = @BookID";

                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, SqlCon, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@BookID", bookID);

                                updateCommand.ExecuteNonQuery();
                            }

                            // If both steps succeed, commit the transaction
                            transaction.Commit();

                            MessageBox.Show("Selected book is added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            clearTextboxes(); SqlCon.Close();
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
        //Remove Button Event
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlCon.Open();

                // Checking if the textbox is empty or not a valid integer
                if (string.IsNullOrWhiteSpace(tbSearch.Text) )
                {
                    MessageBox.Show("Please enter a valid Book Title.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Check if the record exists
                    SqlCommand checkCmd = new SqlCommand($"SELECT COUNT(*) FROM Book WHERE Title = '{tbSearch.Text}'", SqlCon);
                    int recordCount = (int)checkCmd.ExecuteScalar();

                    if (recordCount > 0)
                    {
                        // Record exists, proceed with deletion
                        SqlCommand deleteCmd = new SqlCommand($"DELETE FROM Book WHERE Title = '{tbSearch.Text}'", SqlCon);
                        deleteCmd.ExecuteNonQuery();

                        MessageBox.Show("The selected Book has been deleted.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                        clearTextboxes();
                        SqlCon.Close();
                        LoadGrid();
                    }
                    else
                    {
                        MessageBox.Show("The Book does not exist or has already been deleted from Library.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("In this application main page there are following buttons\nAdd Book: going to that button you can add a New book\nSelected Book: This screen show the selected book User selected.\nReturn Book: In this screen user will see the selected book by entering in the search box the selected book ID and pressing returned button the book will be returned to the library Books\nSelect: For this button firstly enter the BookID and press Select button, a screen appears which asks for the Date to be issued. Enter Date and press ok the book will be issued from library and can be seen in selected books.\nRemove: This button works when you enter the book title in TextBox of the main screen and then press Remove button the book will removed from library.", "About Library Management System", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
