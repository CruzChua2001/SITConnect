using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmailLogin"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
            } 
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var newPassword = HttpUtility.HtmlEncode(newPasswordTB.Text);
            var cPassword = HttpUtility.HtmlEncode(cPasswordTB.Text);

            if(newPassword == "")
            {
                npErr.Text = "Please do not leave blank";
                npErr.ForeColor = System.Drawing.Color.Red;
            } 
            else if (cPassword == "")
            {
                cpErr.Text = "Please do not leave blank";
                cpErr.ForeColor = System.Drawing.Color.Red;
            }
            else if(newPassword != cPassword)
            {
                cpErr.Text = "Password do not match";
                cpErr.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                string status = "";
                int scores = checkPassword(newPassword);

                switch (scores)
                {
                    case 1:
                        status = "Very Weak";
                        break;

                    case 2:
                        status = "Weak";
                        break;

                    case 3:
                        status = "Weak";
                        break;

                    case 4:
                        status = "Weak";
                        break;

                    case 5:
                        status = "Strong";
                        break;

                    case 6:
                        status = "";
                        break;

                    default:
                        break;
                }

                npErr.Text = "Status : " + status;

                if (scores < 5)
                {
                    npErr.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                else
                {

                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    SHA512Managed hashing = new SHA512Managed();

                    string pwdWithSalt = newPassword + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);


                    string sql = "update Account set PasswordHash = @passwordhash, PasswordSalt = @passwordsalt Where Email = @email";
                    try
                    {
                        using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand(sql))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@passwordhash", finalHash);
                                    cmd.Parameters.AddWithValue("@passwordsalt", salt);
                                    cmd.Parameters.AddWithValue("@email", Session["EmailLogin"]);
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

                    Response.Redirect("HomePage.aspx", false);
                }
            }
        }

        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[?=.*[^a-zA-Z0-9]"))
            {
                score++;
            }

            return score;
        }
    }
}