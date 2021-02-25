using System;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using HWIDgbr;
using System.Net;
using System.IO;

namespace Auth_DDOS
{
    public class Auth
    {
        public static string dbip { get; set; }
        public static string table { get; set; }
        public static string dbuser { get; set; }
        public static string dbpass { get; set; }

        public static bool Login(string login, string password, string HWID)
        {
            string cs = "Server = " + dbip + "; Database = " + table + "; User Id = " + dbuser + "; Password = " + dbpass + "";
            MySqlConnection conn = new MySqlConnection(cs);
            MySqlCommand command = new MySqlCommand("Select exp-date from users where pass=@UKEY", conn);
            string bober = "";
            command.Parameters.AddWithValue("@UKEY", password);
            MySqlDataReader da = command.ExecuteReader();
            while (da.Read())
            {
                bober = da.GetValue(0).ToString();
            }


            string data;
            DateTime now = DateTime.Now;
            now.ToString();
            string HWIDRESET = "";
            
            MySqlConnection con = new MySqlConnection(cs);
            MySqlCommand cmd = new MySqlCommand(" Select * from users where login=@login and Password=@password and HWID=@HWID", con);

            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@HWID", HWDI.GetMachineGuid());



            con.Open();
            MySqlDataAdapter adapt = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapt.Fill(ds);
            DateTime datetim;
            DateTime date2;
            DateTime.TryParse(bober, out date2);
            DateTime now1 = DateTime.Now;
            con.Close();
            int count = ds.Tables[0].Rows.Count;
            
            DateTime date1 = DateTime.Now;
            int value = DateTime.Compare(date1, date2);
            MySqlConnection connection1 = new MySqlConnection(cs);
            connection1.Open();

            MySqlCommand command1 = new MySqlCommand("Select HWID from users where login=@User", connection1);

            command1.Parameters.AddWithValue("@User", login);
            MySqlDataReader da1 = command1.ExecuteReader();
            while (da1.Read())
            {
                HWIDRESET = da1.GetValue(0).ToString();
            }


            if (count == 1)
            {
                if (value < 0)
                {

                    Microsoft.Win32.RegistryKey key;
                    key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software");
                    key.SetValue("LOGIN", login);
                    key.Close();


                    MySqlCommand insert = new MySqlCommand("UPDATE licence set OWNER=@UserName WHERE EXISTS (SELECT Used-KEY FROM users WHERE Pass='" + password + "') ;", con);
                    insert.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = login;

                    con.Open();
                    insert.ExecuteNonQuery();


                    return true;



                }
                else if (value > 0)
                {
                    
                    MySqlConnection con1 = new MySqlConnection(cs);
                    MySqlCommand cmd1 = new MySqlCommand(" DELETE LKEY FROM licence WHERE OWNER='" + login + "';", con1);
                    con1.Open();
                    cmd1.ExecuteNonQuery();
                    con1.Close();
                    return false;

                }
            }

            else if (String.IsNullOrEmpty(HWIDRESET))
            {
                MySqlCommand insert = new MySqlCommand("UPDATE users set HWID='" + HWDI.GetMachineGuid() + "' WHERE login=@USERNAME", con);
                //  insert.Parameters.Add("@HWID", SqlDbType.NVarChar).Value = HWID.GetMachineGuid();
                insert.Parameters.Add("@USERNAME", MySqlDbType.VarChar).Value = login;
                con.Open();
                insert.ExecuteNonQuery();

                return false;

            }
            else
            {
                return false;
                
            }
            return false;
        }
        public static bool Register(string user, string password, string email, string key)
        {
            string cs = "Server = " + dbip + "; Database = " + table + "; User Id = " + dbuser + "; Password = " + dbpass + "";
            string address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }
            //tu skonczylem 11:50 18.02/2021
            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);
            //using(SqlCommand cmd = new SqlCommand("UserAdd", con);

            using (MySqlConnection sqlCon = new MySqlConnection(cs))
            {
                //SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);











                //Pobieranie pierwszego
                MySqlConnection connection1 = new MySqlConnection(cs);
                connection1.Open();
                MySqlCommand command1 = new MySqlCommand("Select start-date from users where Used-Key=@UKEY", connection1);

                command1.Parameters.AddWithValue("@UKEY", key);
                MySqlDataReader da1 = command1.ExecuteReader();
                string firstd = "";
                while (da1.Read())
                {
                    firstd = da1.GetValue(0).ToString();
                }
                

                double firstdate;
                double.TryParse(firstd, out firstdate);

                if (firstdate == 0)
                {
                    MySqlConnection connection = new MySqlConnection(cs);
                    connection.Open();
                    //pobieranie długości klucza
                    MySqlCommand command = new MySqlCommand("Select LENGTH from licence where LKEY=@UKEY", connection);

                    command.Parameters.AddWithValue("@UKEY", key);
                    MySqlDataReader da = command.ExecuteReader();
                    string length = "";
                    while (da.Read())
                    {
                        length = da.GetValue(0).ToString();
                    }
                    
                    double number;
                    double.TryParse(length, out number);
                    DateTime exp = DateTime.Now;
                    DateTime expiration = exp.AddDays(number);
                    MySqlConnection con = new MySqlConnection(cs);


                    MySqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "INSERT INTO users (login, pass, email, ip, HWID, UKEY,start-date,exp-date) VALUES (@UserName, @Password,@email, @IP, @HWID, @UKEY, @FIRST, @EXP)";
                    cmd.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = user;
                    cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = password;
                    cmd.Parameters.Add("@IP", MySqlDbType.VarChar).Value = address;
                    cmd.Parameters.Add("@FIRST", MySqlDbType.VarChar).Value = exp;
                    cmd.Parameters.Add("@HWID", MySqlDbType.VarChar).Value = HWDI.GetMachineGuid();

                    MySqlCommand insert = new MySqlCommand("UPDATE licence set OWNER=@UserName WHERE EXISTS (SELECT Used-Key FROM ludzie WHERE Used-Key='" + key + "') ;", con);
                    insert.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = user;
                    con.Open();


                    insert.ExecuteNonQuery();
                    cmd.Parameters.Add("@UKEY", MySqlDbType.VarChar).Value = key;
                    cmd.Parameters.Add("@EXP", MySqlDbType.VarChar).Value = expiration;


                    if (user == "" || password == "" || email == "" || key == "")
                    {

                       // MessageBox.Show("Enter something", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;

                    }
                    else
                    {




                        cmd.ExecuteNonQuery();

                       // DialogResult dialog = MessageBox.Show("Your License Expire in " + expiration + "", "Registred Successful!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //if (dialog == DialogResult.OK)
                        //{
                            Microsoft.Win32.RegistryKey key1;
                            key1 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software");
                            key1.SetValue("LOGIN", user);
                            key1.Close();
                        return true;
                        //logged
                          //  this.Hide();
                          //  Mainmenu fm = new Mainmenu();
                          //  fm.Show();


                        //}



                    }
                    return false;
                }


                else
                {
                    MySqlConnection connection = new MySqlConnection(cs);
                    connection.Open();
                    //pobieranie długości klucza
                    MySqlCommand command = new MySqlCommand("Select LENGTH from klucze where LICENSE=@UKEY", connection);

                    command.Parameters.AddWithValue("@UKEY", key);
                    MySqlDataReader da = command.ExecuteReader();
                    string length = "";
                    while (da.Read())
                    {
                        length = da.GetValue(0).ToString();
                    }
                    
                    double number;
                    double.TryParse(length, out number);
                    DateTime exp = DateTime.Now;
                    DateTime expiration = exp.AddDays(number);





                    MySqlCommand cmd = sqlCon.CreateCommand();
                    cmd.CommandText = "INSERT INTO users (login, pass,email, IP, HWID, Used-Key,start-date, exp-date) VALUES (@UserName, @Password,@email, @IP, @HWID, @UKEY, @FIRST, @EXP)";
                    cmd.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = user;
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                    cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = password;
                    cmd.Parameters.Add("@IP", MySqlDbType.VarChar).Value = address;
                    cmd.Parameters.Add("@FIRST", MySqlDbType.VarChar).Value = firstdate;
                    cmd.Parameters.Add("@HWID", MySqlDbType.VarChar).Value = HWDI.GetMachineGuid();

                    MySqlCommand insert = new MySqlCommand("UPDATE licence set OWNER=@UserName WHERE EXISTS (SELECT Used-Key FROM users WHERE LICENSE='" + key + "') ;", sqlCon);
                    insert.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = user;


                    sqlCon.Open();
                    insert.ExecuteNonQuery();
                    cmd.Parameters.Add("@UKEY", MySqlDbType.VarChar).Value = key;
                    cmd.Parameters.Add("@EXP", MySqlDbType.VarChar).Value = expiration;


                    if (user == "" || password == "" || email == "" || key == "")
                    {

                       // MessageBox.Show("Enter something", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;

                    }
                    else
                    {




                        cmd.ExecuteNonQuery();
                        // DialogResult dialog = MessageBox.Show("Your License Expire in " + expiration + "", "Registred Successful!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // if (dialog == DialogResult.OK)
                        //  {
                        //      this.Hide();
                        //      Mainmenu fm = new Mainmenu();
                        //     fm.Show();
                        return true;

                      //  }



                    }
                }
            }
            return false;
        }
    }
}
