<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="DashboardApplication.Resigter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registraion</title>
    <style type="text/css">
        .maindiv
        {
            margin: 0px auto;
            width: 25%;
            border: solid 1px Black;
            border-radius: 3px;
            padding: 10px;
            display: flex;
            margin-top: 150px;
            flex-direction: column;
            align-items: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin: 0px auto; width: 15%;">
        <asp:Label ID="lblmsg" runat="server" ForeColor="Red"></asp:Label></div>
    <div class="maindiv">
        <div>
            <h2>
                Registration Form</h2>
        </div>
        <table border="2">
            <tr>
                <td>
                    <asp:TextBox ID="txtUserName" MaxLength="10" runat="server" placeholder="Enter UserName"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUserName"
                        ForeColor="Red" ErrorMessage="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtPassword" MaxLength="10" runat="server" placeholder="Enter Password"
                        TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                        ForeColor="Red" ErrorMessage="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <div style="margin: 20px;">
            <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" /></div>
    </div>
    <div class="maindiv" style="margin-top: 20px">
        <h2>
            Decrypt Password</h2>
        <asp:TextBox ID="txtencpassword" runat="server"></asp:TextBox><br />
        <asp:Button ID="btndecrypt" runat="server" Text="Decrypt" OnClick="btndecrypt_click"
            CausesValidation="false" /><br />
        <asp:Label ID="lbldecryptpassword" runat="server"></asp:Label>
    </div>
    </form>
</body>
</html>
