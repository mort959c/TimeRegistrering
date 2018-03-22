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
using System.Text.RegularExpressions;

namespace TimeRegistrering
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CreateEditPerson : Window
    {
        TimeRegistrationMethods trm = new TimeRegistrationMethods();
        List<Person> people = new List<Person>();
        Person selectedPerson = new Person();

        static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"\\\\cv.local\\dfs\\Students\\mort959c\\Documents\\Visual Studio 2015\\Projects\\TimeRegistrering\\TimeRegistrering\\TimeRegistreringDB.mdf\";Integrated Security=True;Connect Timeout=30";
        SqlConnection myConnection = new SqlConnection(connectionString);
        SqlCommand myCommand = null;

        public CreateEditPerson()
        {
            InitializeComponent();
            FillPeopleCombobox();
        }

        private void cboSelectPersonUpdate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPerson = (Person)cboSelectPersonUpdate.SelectedItem;
            FillTxtForUpdatePerson();
        }

        private void chkUpdatePerson_Checked(object sender, RoutedEventArgs e) //when updatePerson is checked the updatePerson groupbox will be showed
        {
            gboUpdatePerson.Visibility = System.Windows.Visibility.Visible;
            chkCreatePerson.IsChecked = false;
            gboCreatePerson.Visibility = System.Windows.Visibility.Hidden;
        }

        private void chkCreatePerson_Checked(object sender, RoutedEventArgs e) //when createPerson is checked the createPerson groupbox will be showed
        {
            gboCreatePerson.Visibility = System.Windows.Visibility.Visible;
            chkUpdatePerson.IsChecked = false;
            gboUpdatePerson.Visibility = System.Windows.Visibility.Hidden;
        }

        private void buttonUpdatePerson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdatePerson();
            }
            catch (Exception)
            {
                MessageBox.Show("Noget gik galt det var ikke muligt at redigere den valgte ansatte");
            }
        }

        private void buttonCreatePerson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreatePerson();
                MessageBox.Show("Succes!");
            }
            catch (Exception)
            {
                MessageBox.Show("Noget gik galt det var ikke muligt at oprette den ansatte");
            }
        }

        #region Methods
        private void FillPeopleCombobox()
        {
            try
            {
                people = trm.GetPeople();
            }
            catch (Exception)
            {
                MessageBox.Show("noget gik galt, det var ikke muligt at indlæse data fra databasen");
            }
            cboSelectPersonUpdate.ItemsSource = people;
        }

        public void FillTxtForUpdatePerson() //Fills the textboxes with the selectedPerson's data
        {
            txtUpdateName.Text = selectedPerson.Name;
            txtUpdateAdress.Text = selectedPerson.Adress;
            txtUpdateAreaCode.Text = selectedPerson.AreaCode.ToString();
            txtUpdateCity.Text = selectedPerson.City;
            txtUpdatePhoneNumber.Text = selectedPerson.PhoneNumber.ToString();
            txtUpdateEmail.Text = selectedPerson.Email;
            txtUpdateBirthdate.Text = selectedPerson.Birthdate.ToString();

            txtUpdateAgreedSalary.Text = selectedPerson.AgreedSalary.ToString();
        }

        public void UpdatePerson()
        {
            string sqlUpdatePerson = $@"
                                     @Name,
                                     @Adress,
                                     @AreaCode,
                                     @PhoneNumber,
                                     @Email,
                                     @Birthdate,
                                     @AgreedSalary

                                     UPDATE PersonTable
                                     SET
                                     Name = @Name,
                                     Adress = @Adress,
                                     AreaCode = @AreaCode,
                                     PhoneNumber = @PhoneNumber,
                                     Email = @Email,
                                     Birthdate = @Birthdate,
                                     AgreedSalary = @AgreedSalary

                                     WHERE Id = {selectedPerson.Id}";
            myCommand = new SqlCommand(sqlUpdatePerson, myConnection);
            if (selectedPerson != null)
            {
                if (!string.IsNullOrEmpty(txtUpdateName.Text) ||
                !string.IsNullOrEmpty(txtUpdateAdress.Text) ||
                !string.IsNullOrEmpty(txtUpdateAreaCode.Text) ||
                !string.IsNullOrEmpty(txtUpdatePhoneNumber.Text) ||
                !string.IsNullOrEmpty(txtUpdateEmail.Text) ||
                !string.IsNullOrEmpty(txtUpdateBirthdate.Text) ||
                !string.IsNullOrEmpty(txtUpdateAgreedSalary.Text))
                {
                    if (Regex.IsMatch(txtUpdateName.Text, "^[a-å A-Å]+$"))
                    {
                        try
                        {
                            myCommand.Parameters.AddWithValue("@Name", txtUpdateName.Text);
                            myCommand.Parameters.AddWithValue("@Adress", txtUpdateAdress.Text);
                            myCommand.Parameters.AddWithValue("@AreaCode", txtUpdateAreaCode.Text);
                            myCommand.Parameters.AddWithValue("@PhoneNumber", txtUpdatePhoneNumber.Text);
                            myCommand.Parameters.AddWithValue("@Email", txtUpdateEmail.Text);
                            myCommand.Parameters.AddWithValue("@Birthdate", txtUpdateBirthdate.Text);
                            myCommand.Parameters.AddWithValue("@AgreedSalary", txtUpdateAgreedSalary.Text);

                            myConnection.Open();
                            myCommand.ExecuteNonQuery();
                            MessageBox.Show("Succes!");
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        finally
                        {
                            myConnection.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Navn må ikke indeholde tal");
                    }
                }
                else
                {
                    MessageBox.Show("Tomme felter er ikke tilladt");
                }
            }
            else
            {
                MessageBox.Show("Skal vælge den person du vil redigere");
            }
            
        }

        private void CreatePerson()
        {
            string sqlCreatePerson = @" @Name NVARCHAR,
                                        @Adress,
                                        @AreaCode,
                                        @PhoneNumber,
                                        @Email,
                                        @Birthdate,
                                        @AgreedSalary

                                        INSERT INTO PersonTable (Name, Adress, AreaCode, PhoneNumber, Email, Birthdate, AgreedSalary)
                                        VALUES (@Name, @Adress, @AreaCode, @PhoneNumber, @Email, @Birthdate, @AgreedSalary)";
            myCommand = new SqlCommand(sqlCreatePerson, myConnection);

            if (string.IsNullOrEmpty(txtCreateName.Text) ||
                string.IsNullOrEmpty(txtCreateAdress.Text) ||
                string.IsNullOrEmpty(txtCreateAreaCode.Text) ||
                string.IsNullOrEmpty(txtCreatePhoneNumber.Text) ||
                string.IsNullOrEmpty(txtCreateEmail.Text) ||
                string.IsNullOrEmpty(txtCreateBirthdate.Text) ||
                string.IsNullOrEmpty(txtCreateAgreedSalary.Text))
            {
                if (!Regex.IsMatch(txtCreateName.Text, "^[a-å A-Å]"))
                {
                    try
                    {
                        myCommand.Parameters.AddWithValue("@Name", txtCreateName.Text);
                        myCommand.Parameters.AddWithValue("@Adress", txtCreateAdress.Text);
                        myCommand.Parameters.AddWithValue("@AreaCode", txtCreateAreaCode.Text);
                        myCommand.Parameters.AddWithValue("@PhoneNumber", txtCreatePhoneNumber.Text);
                        myCommand.Parameters.AddWithValue("@Email", txtCreateEmail.Text);
                        myCommand.Parameters.AddWithValue("@Birthdate", txtCreateBirthdate.Text);
                        myCommand.Parameters.AddWithValue("@AgreedSalary", txtCreateAgreedSalary.Text);

                        myConnection.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Navn må ikke indeholde tal");
                }
            }
            else
            {
                MessageBox.Show("Tomme felter er ikke tilladt");
            }

        }
        #endregion
    }
}
