<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="SITConnect.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LdOeA0eAAAAAFlYY7HpLPynf4bVPCR8gTgfGJtF"></script>
    <style type="text/css">
        .auto-style13 {
            width: 312px;
            height: 69px;
        }
        .auto-style14 {
            height: 69px;
        }
        .auto-style15 {
            width: 312px;
            height: 72px;
        }
        .auto-style16 {
            height: 72px;
        }
        .auto-style17 {
            width: 312px;
            height: 70px;
        }
        .auto-style18 {
            height: 70px;
        }
        .auto-style19 {
            width: 312px;
            height: 71px;
        }
        .auto-style20 {
            height: 71px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
            <legend><h1>Registration Form</h1></legend>
            

                <br />
                <table align="left">
                    <tr>
                        <td class="auto-style13">First Name:</td>
                        <td class="auto-style14">
                            <asp:TextBox ID="fname" runat="server" Width="526px" Height="30px" onkeyup="javascript:validateFname()"></asp:TextBox>
                            <asp:Label ID="fnameerr" runat="server" Text="Required"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style15">Last Name:</td>
                        <td class="auto-style16">
                            <asp:TextBox ID="lname" runat="server" Width="526px" Height="30px" onkeyup="javascript:validateLname()"></asp:TextBox>
                            <asp:Label ID="lnameerr" runat="server" Text="Required" EnableViewState="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style15">Credit Card:</td>
                        <td class="auto-style16">
                            <asp:TextBox ID="ccno" runat="server" Width="526px" Height="30px" onkeyup="javascript:validateCredit()"></asp:TextBox>
                            <asp:Label ID="ccerr" runat="server" Text="Required" EnableViewState="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style17">Email:</td>
 
                        <td class="auto-style18">
                            <asp:TextBox ID="email" runat="server" Width="526px" Height="30px" onkeyup="javascript:validateEmail()"></asp:TextBox>
                            <asp:Label ID="emailErr" runat="server" Text="Required"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style13">Password</td>
                        <td class="auto-style14">
                            <asp:TextBox ID="password" runat="server" Width="526px" Height="30px" TextMode="Password" onkeyup="javascript:validatePassword()"></asp:TextBox>
                            <asp:Label ID="passwordErr" runat="server" Text="Required"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style19">Date Of Birth:</td>
                        <td class="auto-style20">
                            <asp:TextBox ID="dob" runat="server" Width="526px" Height="30px" TextMode="Date" onchange="javascript:validateDOB()"></asp:TextBox>
                        &nbsp;<asp:Label ID="dobErr" runat="server" Text="Required"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style17">Photo:</td>
                        <td class="auto-style18">
                            <asp:FileUpload ID="photo" runat="server" Height="30px" Width="529px" accept=".png,.jpg,.jpeg,.gif" />
                        &nbsp;<asp:Label ID="photoErr" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style17" colspan="2">
                            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style19" align="center">
                            <asp:Button ID="regbtn" runat="server" Text="Register" Width="179px" OnClick="regbtn_Click" />
                        </td>
                        <td class="auto-style20">
                            <asp:Label ID="lbl_captcha" runat="server"></asp:Label>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Click <a href="Login.aspx">here</a> to login</td>
                    </tr>
                </table>
                <br />

                <br />

           
        </fieldset>
        
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LdOeA0eAAAAAFlYY7HpLPynf4bVPCR8gTgfGJtF', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>

<script type="text/javascript">
    document.getElementById("passwordErr").style.color = "Red";
    document.getElementById("emailErr").style.color = "Red";
    document.getElementById("fnameerr").style.color = "Red";
    document.getElementById("lnameerr").style.color = "Red";
    document.getElementById("ccerr").style.color = "Red";
    document.getElementById("dobErr").style.color = "Red";

    function validatePassword() {
        var str = document.getElementById("<%=password.ClientID %>").value;
        document.getElementById("passwordErr").style.color = "Red";
        if (str.length < 12) {
            document.getElementById("passwordErr").innerHTML = "Very Weak: Password length must be at least 12 characters";
        } else if (str.search(/[0-9]/) == -1){
            document.getElementById("passwordErr").innerHTML = "Weak: Password requre at least 1 numeral";
        } else if (str.search(/[a-z]/) == -1) {
            document.getElementById("passwordErr").innerHTML = "Weak: Password requre at least 1 lowercase alphabet";
        } else if (str.search(/[A-Z]/) == -1) {
            document.getElementById("passwordErr").innerHTML = "Good: Password requre at least 1 uppercase alphabet";
        } else if (str.search(/[\W_]/) == -1) {
            document.getElementById("passwordErr").innerHTML = "Good: Password requre at least 1 special character";
        } else {
            document.getElementById("passwordErr").innerHTML = "Excellent";
        }
    }

    function validateEmail() {
        var str = document.getElementById("<%=email.ClientID %>").value;
        document.getElementById("emailErr").style.color = "Red";
        var pattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (str.match(pattern)) {
            document.getElementById("emailErr").innerHTML = "";
        } else if (str.length == 0) {
            document.getElementById("emailErr").innerHTML = "Required";
        } else {
            document.getElementById("emailErr").innerHTML = "Invalid Format";
        }
    }

    function validateFname() {
        var str = document.getElementById("<%=fname.ClientID %>").value;
        if (str.length > 0) {
            document.getElementById("fnameerr").innerHTML = "";
        } else {
            document.getElementById("fnameerr").innerHTML = "Required";
        }
    }

    function validateLname() {
        var str = document.getElementById("<%=lname.ClientID %>").value;
        if (str.length > 0) {
            document.getElementById("lnameerr").innerHTML = "";
        } else {
            document.getElementById("lnameerr").innerHTML = "Required";
        }
    }

    function validateCredit() {
        var str = document.getElementById("<%=ccno.ClientID %>").value;
        if (!str.match(/^[0-9]+$/)) {
            document.getElementById("ccerr").innerHTML = "Only number is accepted";
        }else if (str.length != 16) {
            document.getElementById("ccerr").innerHTML = "Must be 16 digit";
        } else {
            document.getElementById("ccerr").innerHTML = "";
        }
    }

    function validateDOB() {
        var str = document.getElementById("<%=dob.ClientID %>").value;
        if (str == "") {
            document.getElementById("dobErr").innerHTML = "Required";
        } else {
            document.getElementById("dobErr").innerHTML = "";
        }
    }
</script>
