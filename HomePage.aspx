<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="SITConnect.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Welcome to homepage.
            <br />
            Click <a href="ChangePassword.aspx">here</a> to change password

            <br />
            <br />
            <asp:Image ID="photoIMG" runat="server" Height="200px" Width="200px"/>

            <br />
            <br />
            <asp:Button ID="logoutbtn" runat="server" OnClick="logoutbtn_Click" Text="Logout" />

        </div>
    </form>
</body>
</html>
