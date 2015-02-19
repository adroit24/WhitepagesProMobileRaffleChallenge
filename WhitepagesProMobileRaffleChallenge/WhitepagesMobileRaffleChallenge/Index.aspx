<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WhitePagesPhoneLookup.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="css/mobile-challenge.css" type="text/css" />

    <title>Whitepages Pro Mobile Challenge - Test our data against your number.</title>
   
    <script type="text/javascript" language="javascript">
        function ClearTextboxes() {
            document.getElementById('textBoxPhoneNumber').value = '';
        }
    </script>
</head>
<body>
    <div class="container"></div>
    <div class="content">
      <div class="left-col">
        <img class="phone" src="img/phonegraph-full.png" />
      </div>
      <div class="right-col">
        <img class="logo" src="img/prologo-2x.png" />
        <h1 class="headline"><span>MOBILE</span> CHALLENGE</h1>
        <form id="index" runat="server">
            <div id="phone-field">
                <asp:TextBox ID="textBoxPhoneNumber" CssClass="inputbox" placeholder="Your mobile number" runat="server"></asp:TextBox>
                <span class="icon"></span>
            </div>
            <div class="subfield">
                <div class="check-wrap">
                  <input type="checkbox" id="check" class="check">
                  <label for="check">I have the Whitepages Caller ID app.</label>
                </div>
                <asp:Button id="sendPhone" runat="server" CssClass="find_btn" Text="CHALLENGE ACCEPTED" OnClick="ButtonFindClick" />
            </div>
            </form>
        <p class="desc">Test our data against your phone. Get entered to win a sweet prize.</p>
      </div>
    </div>
 <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js" type="text/javascript"></script>
    <script src="js/prettyCheckable.min.js"></script>
    <script src="js/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        jQuery(function ($) {
            var addFields = $('.subfield');
            var phoneField = $('#phone-field input');
            var check = $('.check');
            var icon = $('.icon');
            var desc = $('.desc');

            addFields.hide();
            phoneField.mask("( 999 ) 999 - 9999", { placeholder: "  " });
            check.prettyCheckable();
            phoneField.on('keyup', function () {
                var keyVal = $(this).val().length;
                if (keyVal >= 13) {
                    addFields.slideDown("slow");
                    desc.slideUp("slow");
                }
            });
            phoneField.on('blur', function () {
                var keyVal = $(this).val().length;
                if (keyVal < 13) {
                    addFields.slideUp("slow");
                    desc.slideDown("slow");
                }
            });
        });
    </script>
</body>
</html>
