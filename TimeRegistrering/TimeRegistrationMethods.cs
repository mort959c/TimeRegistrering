using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TimeRegistrering
{
    class TimeRegistrationMethods
    {
        static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"\\\\cv.local\\dfs\\Students\\mort959c\\Documents\\Visual Studio 2015\\Projects\\TimeRegistrering\\TimeRegistrering\\TimeRegistreringDB.mdf\";Integrated Security=True;Connect Timeout=30";
        SqlConnection myConnection = new SqlConnection(connectionString);
        SqlDataReader myReader = null;
        SqlCommand myCommand = null;

        public List<Person> GetPeople() //Gets all the peopledata from the database and puts it in a list
        {
            List<Person> people = new List<Person>(); //contains all the person object created from the data in the database

            //SQL string
            string PeopleSql =
            @"select 
            p.id,
            p.name,
            p.adress,
            p.areacode,
            ac.city,
            p.email,
            p.agreedsalary,
            p.phonenumber,
            p.birthdate
            from persontable p
            join areacodecitytable ac on p.areacode = ac.areacode ";
            myCommand = new SqlCommand(PeopleSql, myConnection);

            try
            {
                myConnection.Open();
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    Person p = new Person();
                    p.Id = Convert.ToInt32(myReader["id"]);
                    p.Name = myReader["name"].ToString();
                    p.Adress = myReader["adress"].ToString();
                    p.AreaCode = Convert.ToInt32(myReader["areacode"]);
                    p.PhoneNumber = Convert.ToInt32(myReader["phonenumber"]);
                    p.Email = myReader["email"].ToString();
                    p.Birthdate = Convert.ToDateTime(myReader["birthdate"]);
                    p.AgreedSalary = Convert.ToDouble(myReader["agreedsalary"]);
                    p.City = myReader["city"].ToString();

                    people.Add(p);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                myConnection.Close();
            }

            return people;
        }

        public int HoursThisMonth(string month, int personId)
        {
            int hours = 0;
            month = month.ToUpper();
            int monthNumber = 0;
            switch (month)
            {
                case "JANUAR":
                    monthNumber = 1;
                    break;
                case "FEBRUAR":
                    monthNumber = 2;
                    break;
                case "MARTS":
                    monthNumber = 3;
                    break;
                case "APRIL":
                    monthNumber = 4;
                    break;
                case "MAJ":
                    monthNumber = 5;
                    break;
                case "JUNI":
                    monthNumber = 6;
                    break;
                case "JULI":
                    monthNumber = 7;
                    break;
                case "AUGUST":
                    monthNumber = 8;
                    break;
                case "SEPTEMBER":
                    monthNumber = 9;
                    break;
                case "OKTOBER":
                    monthNumber = 10;
                    break;
                case "NOVEMBER":
                    monthNumber = 11;
                    break;
                case "DECEMBER":
                    monthNumber = 12;
                    break;
            }

            if (monthNumber != 0)
            {
                myCommand = new SqlCommand($@"DECLARE @PersonId int
                                              SET @PersonId = {personId}
                                              DECLARE @Month int
                                              SET @Month = {monthNumber}

                                              SELECT
                                              hr.month
                                              FROM HourRegistrationTable hr
                                              WHERE PersonId = @PersonId and DATEPART(MM, date) = @Month and DATEPART(YYYY, date) = 2016", myConnection);//Gets the hours for the selectedPerson from the selectedMonth
                try
                {
                    myConnection.Open();
                    myReader = myCommand.ExecuteReader();

                    while (myReader.Read())
                    {
                        if (myReader["month"] != DBNull.Value)
                        {
                            hours = Convert.ToInt32(myReader["month"]);
                        }
                    }
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
            return hours;
        }

        public int HoursThisYear()
        {
            return 0;
        }

        public void RegisterHours(Person selectedPerson, DateTime? date, string startTime, string endTime)
        {
            DateTime tempDate = (DateTime)date;
            var sqlFormattedDate = tempDate.Date.ToString("yyyy-MM-dd");
            string sqlTimeRegistration = "INSERT INTO HourRegistrationTable (PersonId, Date, StartTime, EndTime) values(@PersonId, @Date, @StartTime, @EndTime)";
            myCommand = new SqlCommand(sqlTimeRegistration, myConnection);

            myCommand.Parameters.AddWithValue("@PersonId", selectedPerson.Id);
            myCommand.Parameters.AddWithValue("@Date", sqlFormattedDate);
            myCommand.Parameters.AddWithValue("@StartTime", startTime);
            myCommand.Parameters.AddWithValue("@EndTime", endTime);

            try
            {
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
    }

    

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public int AreaCode { get; set; }
        public string City { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public double AgreedSalary { get; set; }

        public override string ToString()
        {
            return Id + " - " + Name + " - " + Adress + " - " + PhoneNumber;
        }
    }
}
