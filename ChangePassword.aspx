<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="SITConnect.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            height: 23px;
        }
        .auto-style3 {
            height: 50px;
        }
        .auto-style4 {
            width: 258px;
        }
        .auto-style5 {
            height: 23px;
            width: 258px;
        }
        .auto-style6 {
            height: 50px;
            width: 258px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
            <legend><h1>Change Password</h1></legend>
            <br />
            <table class="auto-style1">
                <tr>
                    <td class="auto-style4">New Password</td>
                    <td>
                        <asp:TextBox ID="newPasswordTB" runat="server" Width="215px" onkeyup="javascript:validatePassword()" TextMode="Password"></asp:TextBox>
&nbsp;<asp:Label ID="npErr" runat="server" Text="Required"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style5">Confirm Password</td>
                    <td class="auto-style2">
                        <asp:TextBox ID="cPasswordTB" runat="server" Width="215px" onkeyup="javascript:validateCPassword()" TextMode="Password"></asp:TextBox>
&nbsp;<asp:Label ID="cpErr" runat="server" Text="Required"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style6">
                        <asp:Button ID="Button1" runat="server" Text="Submit" Width="151px" OnClick="Button1_Click" />
                    </td>
                    <td class="auto-style3">&nbsp;</td>
                </tr>
            </table>
        </fieldset>
    </form>
</body>
</html>

<script>
    document.getElementById("npErr").style.color = "Red";
    document.getElementById("cpErr").style.color = "Red";

    function validatePassword() {
        var str = document.getElementById("<%=newPasswordTB.ClientID %>").value;
        if (str.length < 12) {
            document.getElementById("npErr").innerHTML = "Password length must be at least 12 characters";
        } else if (str.search(/[0-9]/) == -1) {
            document.getElementById("npErr").innerHTML = "Password requre at least 1 numeral";
        } else if (str.search(/[a-z]/) == -1) {
            document.getElementById("npErr").innerHTML = "Password requre at least 1 lowercase alphabet";
        } else if (str.search(/[A-Z]/) == -1) {
            document.getElementById("npErr").innerHTML = "Password requre at least 1 uppercase alphabet";
        } else if (str.search(/[\W_]/) == -1) {
            document.getElementById("npErr").innerHTML = "Password requre at least 1 special character";
        } else {
            document.getElementById("npErr").innerHTML = "";
        }
    }

    function validateCPassword() {
        var str = document.getElementById("<%=newPasswordTB.ClientID %>").value;
        var str2 = document.getElementById("<%=cPasswordTB.ClientID %>").value;

        if (str != str2) {
            document.getElementById("cpErr").innerHTML = "Password do not match";
        } else {
            document.getElementById("cpErr").innerHTML = "";
        }
    }
</script>
