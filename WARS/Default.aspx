<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WARS.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        window.onload = function OpenMenuScreen() {            
            var win = window.open('../Common/WARSAffiliates.aspx', '_self');//for PROD

            //to disable browser options and display full window
            //window.open('', '_parent', '');
            //window.close();

            //var win = window.open('../Common/MenuScreen.aspx', 'name', "top=1,left=0,width=" + (window.screen.width - 15) + ",height=" + (window.screen.height - 50));
            //win.focus();

            //Disable back and forward buttons of browser
            this.history.forward(-1);
            //===========End
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
