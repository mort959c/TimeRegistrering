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
using System.Data.SqlClient;

namespace TimeRegistrering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TimeRegistrationMethods trm = new TimeRegistrationMethods();
        List<Person> people = new List<Person>(); //list that will be filled with the person objects created from the data in the database
        Person selectedPerson = null; //this helps keeping track of which person is selected in the person combobox

        TimeSpan duration;

        public MainWindow()
        {
            InitializeComponent();

            FillPeopleCombobox(); //Fills the combobox with the people objects created from the data in the database 

            FillTimeComboboxes(); //Fills the comboboxes for the timeregistration "group"
        }

        private void cboSelectPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPerson = (Person)cboSelectPerson.SelectedItem; //When a person is selected, selectedPerson is being set to contain that persons data
            FillTxtForPerson();
        }
        private void cboSelectPayoffMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedMonth = ((ComboBoxItem)cboSelectPayoffMonth.SelectedItem).Content.ToString();

            try
            {
                txtHoursThisMonth.Text = trm.HoursThisMonth(selectedMonth, selectedPerson.Id).ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Noget gik galt, det var ikke muligt at hente timer for denne måned");
            }

            int payoff = Convert.ToInt32(txtAgreedSalary.Text) * Convert.ToInt32(txtHoursThisMonth.Text); 
            txtPayoffSelectedMonth.Text = payoff.ToString();
        }
        private void btnSaveTimer_Click(object sender, RoutedEventArgs e)
        {
            string startTime = cboStartHour.SelectedItem + ":" + cboStartMinute.SelectedItem + ":00";
            string endTime = cboEndHour.SelectedItem + ":" + cboEndMinute.SelectedItem + ":00";
            CalculateHours();

            if (selectedPerson != null)
            {
                if (duration > TimeSpan.Zero)
                {
                    if (dtpSelectDate.SelectedDate != null)
                    {
                        try
                        {
                            trm.RegisterHours(selectedPerson, dtpSelectDate.SelectedDate, startTime, endTime);
                            MessageBox.Show("Succes!");
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("noget gik galt det var ikke muligt at registrerer de valgte timer");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Du har ikke valgt en dato");
                    }
                }
                else
                {
                    MessageBox.Show("Du har valgt et ugyldigt antal timer");
                }
            }
            else
            {
                MessageBox.Show("Du har ikke valgt hvilken person du vil registrere timer for");
            }
        }

        #region cboHours
        private void cboStartHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
            txtHours.Text = duration.ToString();
        }

        private void cboStartMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
            txtHours.Text = duration.ToString();
        }

        private void cboEndHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
            txtHours.Text = duration.ToString();
        }

        private void cboEndMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
            txtHours.Text = duration.ToString();
        }

        private void btnCreateEdit_Click(object sender, RoutedEventArgs e) //opens the create/edit persons window whenever it's clicked
        {
            CreateEditPerson createEditPerson = new CreateEditPerson();
            createEditPerson.ShowDialog(); //Disables other windows as long as CreateEditPerson is open.
        }
        #endregion

        #region Methods
        public void FillPeopleCombobox() //Select person combobox with the person objects
        {
            try
            {
                people = trm.GetPeople();
            }
            catch (Exception)
            {
                MessageBox.Show("noget gik galt, det var ikke muligt at indlæse data fra databasen");
            }
            cboSelectPerson.ItemsSource = people;
        }

        public void FillTxtForPerson() //Fills the textboxes with the selectedPerson's data
        {
            txtName.Text = selectedPerson.Name;
            txtAdress.Text = selectedPerson.Adress;
            txtAreaCode.Text = selectedPerson.AreaCode.ToString();
            txtCity.Text = selectedPerson.City;
            txtPhoneNumber.Text = selectedPerson.PhoneNumber.ToString();
            txtEmail.Text = selectedPerson.Email;

            txtAgreedSalary.Text = selectedPerson.AgreedSalary.ToString();   
        }

        public void FillTimeComboboxes() //Fills the time registration "group" with loops
        {
            for (int i = 8; i <= 16; i++)
            {
                if (i < 10)
                {
                    cboStartHour.Items.Add(i.ToString().PadLeft(2, '0'));
                    cboEndHour.Items.Add(i.ToString().PadLeft(2, '0'));
                }
                else
                {
                    cboStartHour.Items.Add(i);
                    cboEndHour.Items.Add(i);
                }
            }

            for (int i = 0; i <= 60; i++)
            {
                if (i < 10)
                {
                    cboStartMinute.Items.Add(i.ToString().PadLeft(2, '0'));
                    cboEndMinute.Items.Add(i.ToString().PadLeft(2, '0'));
                }
                else
                {
                    cboStartMinute.Items.Add(i);
                    cboEndMinute.Items.Add(i);
                }
            }
        }

        public void CalculateHours() //calculates the duration between start and end time
        {
            if (cboStartHour.SelectedItem != null && 
                cboStartMinute.SelectedItem != null &&
                cboEndHour.SelectedItem != null &&
                CheckForValidTime() &&
                cboEndMinute.SelectedItem != null)
            {
                string startTime = cboStartHour.SelectedItem.ToString() + ":" + cboStartMinute.SelectedItem.ToString();

                string endTime = cboEndHour.SelectedItem.ToString() + ":" + cboEndMinute.SelectedItem.ToString();

                duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));
            }
        }

        public bool CheckForValidTime() //checks if work hours selected is valid. 
        {
            bool validTime = true;
            if (Convert.ToInt32(cboStartHour.SelectedItem) == Convert.ToInt32(cboEndHour.SelectedItem) ||
                Convert.ToInt32(cboStartHour.SelectedItem) > Convert.ToInt32(cboEndHour.SelectedItem))
            {
                if (Convert.ToInt32(cboStartHour.SelectedItem) > Convert.ToInt32(cboEndHour.SelectedItem))
                {
                    MessageBox.Show("Du kan ikke vælge en sluttid der ligger før starttiden");
                    duration = TimeSpan.Zero; //sets the duration (hours between start and end time) to zero 00:00:00
                    validTime = false;
                }
                else if (Convert.ToInt32(cboStartHour.SelectedItem) == Convert.ToInt32(cboEndHour.SelectedItem))
                {
                    if (Convert.ToInt32(cboStartMinute.SelectedItem) < Convert.ToInt32(cboEndMinute.SelectedItem))
                    {
                        MessageBox.Show("Du kan ikke vælge en sluttid der ligger før starttiden");
                        duration = TimeSpan.Zero;
                        validTime = false;
                    }
                    else if (Convert.ToInt32(cboStartMinute.SelectedItem) == Convert.ToInt32(cboEndMinute.SelectedItem))
                    {
                        MessageBox.Show("Sluttid og starttid må ikke være det samme");
                        duration = TimeSpan.Zero;
                        validTime = false;
                    }
                }
            }
            
            return validTime;
        }

        #endregion

        
    }
}
