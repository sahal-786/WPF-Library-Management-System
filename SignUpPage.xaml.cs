using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Window
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        SqlConnection SqlCon = new SqlConnection(@"Data Source=SAHAL\MSSQLSERVER01;Initial Catalog=crud;Integrated Security=True");
        DataSet ds = new DataSet();

        public void cleartb()
        {
            tbName.Clear();
            tbEmail.Clear();
            tbPassword.Clear();
            tbCNIC.Clear();
        }
        public bool validation()
        {
            if (String.IsNullOrEmpty(tbName.Text) || String.IsNullOrEmpty(tbEmail.Text) || String.IsNullOrEmpty(tbPassword.Password) || String.IsNullOrEmpty(tbCNIC.Text))
            {
                return false;
            }
            if (String.IsNullOrWhiteSpace(tbName.Text) || String.IsNullOrWhiteSpace(tbEmail.Text) || String.IsNullOrWhiteSpace(tbPassword.Password) || String.IsNullOrEmpty(tbCNIC.Text))
            {
                return false;
            }
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginPage = new MainWindow();
            loginPage.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            cleartb();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validation() == true)
                {
                SqlCommand cmd1 = new SqlCommand("select * from User_Info where  CNIC='" + tbCNIC.Text + "'", SqlCon);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd1);
                adapter.Fill(ds);
                int i = ds.Tables[0].Rows.Count;
                if (i > 0)
                {
                    MessageBox.Show("The CNIC: " + tbCNIC.Text + " already taken.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ds.Clear();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("insert into User_Info values (@Name, @CNIC, @Email, @Password)", SqlCon);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Name", tbName.Text);
                    cmd.Parameters.AddWithValue("@CNIC", tbCNIC.Text);
                    cmd.Parameters.AddWithValue("@Email", tbEmail.Text);
                    cmd.Parameters.AddWithValue("@Password", tbPassword.Password);
                    //cmd.Parameters.AddWithValue("@Password", SecureData.HashString(tbPassword.Password));
                    

                    SqlCon.Open();
                    cmd.ExecuteNonQuery();
                    SqlCon.Close();

                    MessageBox.Show("Successfully Added Account\nNow you can LogIn", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    cleartb();

                   // Open the AccountMainPage or perform other actions
                   MainWindow loginWindow = new MainWindow();
                   loginWindow.Show();
                   this.Close();
                                
                }
                }
                else
                {
                    MessageBox.Show("Some Fields are Empty.\nPlease fill them", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SqlCon.Close();
                cleartb();
            }
        }
    }
}
