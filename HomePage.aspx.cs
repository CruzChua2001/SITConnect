using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class HomePage : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Test1");
            if (Session["EmailLogin"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null) 
            {
                Console.WriteLine("Test");
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    SqlConnection connection = new SqlConnection(MYDBConnectionString);
                    string sql = "select * FROM Account WHERE Email=@USERID";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@USERID", Session["EmailLogin"]);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(reader["Photo"] != null)
                                {
                                    photoIMG.ImageUrl = "Image/" + reader["Photo"].ToString();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { connection.Close(); }
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

        }

        protected void logoutbtn_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if(Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if(Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["Authtoken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }
    }
}