using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginbtn_Click(object sender, EventArgs e)
        {
            var email = HttpUtility.HtmlEncode(emailtb.Text.Trim());
            var password = HttpUtility.HtmlEncode(pwdtb.Text.Trim());

            var attempt = 0;

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDbSalt(email);
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = password + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);

                    //email is in lockout
                    if (Attempts(email) <= 0)
                    {
                        if (checkLockoutPeriod(email))
                        {
                            errorMsg.Text = "Account is unlocked. Please enter your login details.";
                            errorMsg.ForeColor = System.Drawing.Color.Green;
                        }
                        else 
                        {
                            var minutesLeft = retrieveMinute(email);
                            errorMsg.Text = string.Format("Account is locked. Please try again in {0} minutes.", minutesLeft);
                            errorMsg.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    //if email is not in lockout period
                    else
                    {
                        // If password and email matches
                        if (userHash.Equals(dbHash))
                        {
                            //password and email match and have attempt available
                            validUser(email);

                        }
                        //password is invalid but email is valid
                        else
                        {
                            //password is Invalid and have attempt available
                            SqlConnection connection = new SqlConnection(MYDBConnectionString);
                            string sql = "select * from Account Where Email=@email";
                            SqlCommand command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@email", email);

                            try
                            {
                                connection.Open();

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        if (reader["Email"] != null)
                                        {
                                            if (reader["Email"] != DBNull.Value)
                                            {
                                                attempt = Convert.ToInt32(reader["Attempt"]);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.ToString());
                            }
                            finally { connection.Close(); }

                            string sql2 = "";
                            attempt -= 1;
                            updateLockoutTime(email);
                            sql2 = "update Account set Attempt = " + attempt + " Where Email=@email";
                            try
                            {
                                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                                {
                                    using (SqlCommand cmd = new SqlCommand(sql2))
                                    {
                                        using (SqlDataAdapter sda = new SqlDataAdapter())
                                        {
                                            cmd.CommandType = CommandType.Text;
                                            cmd.Parameters.AddWithValue("@email", email);
                                            cmd.Connection = con;
                                            con.Open();
                                            cmd.ExecuteNonQuery();
                                            con.Close();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.ToString());
                            }
                            

                            errorMsg.Text = "Email or password is incorrect.";
                            errorMsg.ForeColor = System.Drawing.Color.Red;

                        }
                    }
                }
                //email is not valid
                else
                {
                    errorMsg.Text = "Email or password is incorrect.";
                    errorMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
        }

        protected string getDBHash(string email)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash from Account Where Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDbSalt(string email)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordSalt FROM ACCOUNT WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        protected int retrieveAttempt(string email)
        {
            int i = 0;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Attempt FROM ACCOUNT WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Attempt"] != null)
                        {
                            if (reader["Attempt"] != DBNull.Value)
                            {
                                i = Convert.ToInt32(reader["Attempt"]);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return i;
        }

        protected void updateLockoutTime(string email)
        {
            string sql = "update Account set Attempt = @attempt, LockoutTime = @Lockouttime Where Email = @email";
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@attempt", 3);
                            cmd.Parameters.AddWithValue("@Lockouttime", DateTime.Now.AddMinutes(30));
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }

        protected int Attempts(string email)
        {
            int i = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Attempt FROM ACCOUNT WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Attempt"] != null)
                        {
                            if (reader["Attempt"] != DBNull.Value)
                            {
                                i = (int)reader["Attempt"];
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }

            return i;
        }

        protected Boolean lockoutPeriod(string email)
        {
            Boolean check = false;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LockoutTime FROM ACCOUNT WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LockoutTime"] != null)
                        {
                            if (reader["LockoutTime"] != DBNull.Value)
                            {
                                if(DateTime.Now < (DateTime)reader["LockoutTime"])
                                {
                                    check = true;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }

            return check;
        }

        protected Boolean checkLockoutPeriod(string email)
        {
            Boolean check = false;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LockoutTime FROM ACCOUNT WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var lockouttime = (DateTime)command.ExecuteScalar();
            connection.Close();
            if(DateTime.Now >= lockouttime)
            {
                string sql2 = "update Account set Attempt = @attempt, LockoutTime = @Lockouttime Where Email = @email";
                try
                {
                    using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(sql2))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@attempt", 3);
                                cmd.Parameters.AddWithValue("@Lockouttime", DBNull.Value);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());

                }
                check = true;
            }
            else
            {
                check = false;
            }
            return check;
        }

        protected void validUser(string email)
        {
            string sql = "";
            sql = "update Account set Attempt = " + 3 + " Where Email=@email";
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            Session["EmailLogin"] = HttpUtility.HtmlEncode(emailtb.Text.Trim());

            string guid = Guid.NewGuid().ToString();
            Session["AuthToken"] = guid;

            Response.Cookies.Add(new HttpCookie("AuthToken", guid));
            Response.Redirect("HomePage.aspx", false);
        }

        protected int retrieveMinute(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LockoutTime FROM ACCOUNT WHERE Email=@email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var lockouttime = (DateTime)command.ExecuteScalar();
            connection.Close();

            var i = lockouttime - DateTime.Now;
            return (int)i.TotalMinutes;
        }
    }
}