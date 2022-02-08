<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style3 {
            width: 312px;
            height: 68px;
        }
        .auto-style4 {
            height: 68px;
        }
        .auto-style5 {
            width: 312px;
            height: 70px;
        }
        .auto-style6 {
            height: 70px;
        }
        .auto-style7 {
            width: 312px;
            height: 71px;
        }
        .auto-style8 {
            height: 71px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
            <legend><h1>Login</h1></legend>

            <table class="auto-style1">
                <tr>
                    <td class="auto-style3">Email:</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="emailtb" runat="server" Height="30px" Width="529px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style5">Password</td>
                    <td class="auto-style6">
                        <asp:TextBox ID="pwdtb" runat="server" Height="30px" Width="529px" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style7">
                            <asp:Button ID="loginbtn" runat="server" Text="Login" Width="179px" OnClick="loginbtn_Click" />
                        </td>
                    <td class="auto-style8">
                        <asp:Label ID="errorMsg" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">Click <a href="Register.aspx">here</a> to register</td>
                </tr>
            </table>

        </fieldset>
    </form>
</body>
</html>
