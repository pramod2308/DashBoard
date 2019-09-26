<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DashboardApplication.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/LoginStyle.css" rel="stylesheet" type="text/css" />
    <script src="JS/jquery.js" type="text/javascript"></script>
    <script src="bootstrap/dist/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="JS/PBaes.js" type="text/javascript"></script>
    <script type="text/javascript">

        function uuidv4() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        function PBSAESEncryption() {

            var text = $('#txtpassword').val();
            var bytessize = 256;
            var passphase = uuidv4();

            var key = PBCryptoJS.enc.Utf8.parse('AMINHAKEYTEM32NYTES1234567891234');
            var iv = PBCryptoJS.enc.Utf8.parse('7061737323313233');
            var blocksize = bytessize / 2;
            var encrypted = PBCryptoJS.AES.encrypt(PBCryptoJS.enc.Utf8.parse(text), passphase, key,
	{
	    keySize: bytessize,
	    iv: iv,
	    mode: PBCryptoJS.mode.CBC,
	    padding: PBCryptoJS.pad.Pkcs7
	});
            var dta = String(encrypted);
            $('#hdpass').val(dta);
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center;">
        <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
    </div>
    <div id="login" style="margin: 0px auto; width: 28%;">
        <div class="container">
            <div id="login-row" class="row justify-content-center align-items-center">
                <div id="login-column" class="col-md-6">
                    <div id="login-box" class="col-md-12">
                        <form id="login-form" class="form" action="" method="post">
                        <asp:HiddenField ID="hdpass" runat="server" />
                        <h3 class="text-center text-info">
                            Login</h3>
                        <div class="form-group">
                            <label for="username" class="text-info">
                                Username:</label><br />
                            <asp:TextBox ID="txtUsername" MaxLength="10" class="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="password" class="text-info">
                                Password:</label><br />
                            <asp:TextBox ID="txtpassword" MaxLength="10" class="form-control" runat="server"
                                TextMode="Password"></asp:TextBox>
                        </div>
                        <div class="form-group" style="text-align: center">
                            <asp:Button ID="btnlogin" runat="server" OnClientClick="PBSAESEncryption()" class="btn btn-info btn-md"
                                Text="LOGIN" OnClick="btnlogin_click" />
                        </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
