using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Register : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        public class MyObject
        {
            public string success { get; set; }
            public List<String> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LdOeA0eAAAAAKJaaPnfmJAvV4cb1UI8KpUB3mfA &response=" + captchaResponse);

            try
            {
                using(WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void regbtn_Click(object sender, EventArgs e)
        {
            var first_name = HttpUtility.HtmlEncode(fname.Text);
            var last_name = HttpUtility.HtmlEncode(lname.Text);
            var Email = HttpUtility.HtmlEncode(email.Text);
            var card_no = HttpUtility.HtmlEncode(ccno.Text);
            var Password = HttpUtility.HtmlEncode(password.Text);
            var DOB = HttpUtility.HtmlEncode(dob.Text);
            int scores = checkPassword(Password);
            
            

            string status = "";

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

            passwordErr.Text = "Status : " + status;

            if (scores < 5)
            {
                passwordErr.ForeColor = Color.Red;
                return;
            }

            if(ValidateCaptcha())
            {
                if (first_name == "")
                {
                    fnameerr.Text = "Please do not leave blank";
                    fnameerr.ForeColor = System.Drawing.Color.Red;
                }
                else if (last_name == "")
                {
                    lnameerr.Text = "Please do not leave blank";
                    lnameerr.ForeColor = System.Drawing.Color.Red;
                }
                else if (card_no == "")
                {
                    ccerr.Text = "Please do not leave blank";
                    ccerr.ForeColor = System.Drawing.Color.Red;
                }
                else if (Password == "")
                {
                    passwordErr.Text = "Please do not leave blank";
                    passwordErr.ForeColor = System.Drawing.Color.Red;
                }
                else if (!Regex.IsMatch(card_no, @"[0-9]{16}"))
                {
                    ccerr.Text = "Invalid card format. Please try again.";
                    ccerr.ForeColor = System.Drawing.Color.Red;
                }
                else if (!Regex.IsMatch(Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    emailErr.Text = "Invalid email format. Please try again";
                    emailErr.ForeColor = System.Drawing.Color.Red;
                }
                else if(Email == "")
                {
                    emailErr.Text = "Please upload an image";
                    emailErr.ForeColor = System.Drawing.Color.Red;
                }
                else if (!photo.HasFile)
                {
                    photoErr.Text = "Please do not leave blank";
                    photoErr.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    string strFileName = Path.GetFileName(photo.PostedFile.FileName);
                    photo.SaveAs(Server.MapPath("Image/" + strFileName));

                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    SHA512Managed hashing = new SHA512Managed();

                    string pwdWithSalt = Password + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(Password));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;

                    try
                    {
                        using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO Account Values(@Fname, @Lname, @CreditCard, @Email, @PasswordHash, @PasswordSalt, @DOB, @Photo, @IV, @Key, @Attempt, @LockoutTime)"))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@Fname", first_name);
                                cmd.Parameters.AddWithValue("@Lname", last_name);
                                cmd.Parameters.AddWithValue("@CreditCard", encryptData(card_no));
                                cmd.Parameters.AddWithValue("@Email", Email);
                                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                cmd.Parameters.AddWithValue("@DOB", DOB);
                                cmd.Parameters.AddWithValue("@Photo", strFileName);
                                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                                cmd.Parameters.AddWithValue("@Attempt", 3);
                                cmd.Parameters.AddWithValue("@LockoutTime", DBNull.Value);
                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }


                    
                    Response.Redirect("Login.aspx", false);
                }
                
            }
            else
            {
                lbl_captcha.Text = "Invalid Captcha. Please try again.";
                lbl_captcha.ForeColor = System.Drawing.Color.Red;
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

            if (Regex.IsMatch(password, "[?=.*[^a-zA-Z0-9]")) {
                score++;
            }

            return score;
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }
    }
}