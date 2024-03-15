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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace Library_Managment_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        SqlConnection SqlCon = new SqlConnection(@"Data Source=SAHAL\MSSQLSERVER01;Initial Catalog=crud;Integrated Security=True");
        public void cleartb()
        {
            tbCNIC.Clear();
            tbPassword.Clear();
        }

        public bool validation()
        {
            if (String.IsNullOrEmpty(tbCNIC.Text) || String.IsNullOrEmpty(tbPassword.Password))
            {
                return false;
            }
            if (String.IsNullOrWhiteSpace(tbCNIC.Text) || String.IsNullOrWhiteSpace(tbPassword.Password))
            {
                return false;
            }
            return true;
        }
        //Clear button event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cleartb();
        }
        //Login button event
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            validation();
            try
            {
                if (SqlCon.State == ConnectionState.Closed)
                {
                    SqlCon.Open();
                }

                string query = "SELECT COUNT(1) AS UserCount, UserID FROM User_Info WHERE CNIC = @CNIC AND Password = @Password GROUP BY UserID";

                SqlCommand cmd = new SqlCommand(query, SqlCon);

                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@CNIC", tbCNIC.Text);
                cmd.Parameters.AddWithValue("@Password", tbPassword.Password);

                // Using SqlDataReader to get both the count and UserId in a single query
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Move to the first row of the result set
                        reader.Read();

                        int count = reader.GetInt32(0);
                        int userId = reader.GetInt32(1);

                        if (count == 1)
                        {
                            // Set the global variable
                            AppContext.SetLoggedInUserId(userId);

                            // Open the AccountMainPage or perform other actions
                            AccountMainPage accountMainWindow = new AccountMainPage();
                            accountMainWindow.Show();
                            this.Close();
                        }
                        
                    }
                    else
                        {
                            MessageBox.Show("User CNIC or Password incorrect!");
                            cleartb();
                        }
                }
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
        //Signup button Event
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SignUpPage signUpPage = new SignUpPage();
            signUpPage.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    }


   


