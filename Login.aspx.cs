using System;
using System.Collections.Generic;
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

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDbSalt(email);
            System.Diagnostics.Debug.WriteLine("test1");
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("test2");
                    string pwdWithSalt = password + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);

                    if (userHash.Equals(dbHash))
                    {
                        System.Diagnostics.Debug.WriteLine("test3");
                        Session["EmailLogin"] = HttpUtility.HtmlEncode(emailtb.Text.Trim());

                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;

                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                        Response.Redirect("HomePage.aspx", false);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("test4");
                        errorMsg.Text = "Email or password is incorrect. Please try again";
                        Response.Redirect("Login.aspx");
                    }
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

        
    }
}