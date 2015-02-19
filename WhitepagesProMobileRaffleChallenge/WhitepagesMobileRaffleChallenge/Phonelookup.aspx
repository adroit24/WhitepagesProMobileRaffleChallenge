<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Phonelookup.aspx.cs" Inherits="WhitePagesPhoneLookup.PhoneLookup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Whitepages Pro Mobile Challenge - Test our data against your number.</title>
    <link rel="stylesheet" href="css/mobile-challenge.css?v=helloworldhibye" type="text/css" />

</head>
<body>
    <div class="container"></div>
    <form id="phonelookup" runat="server">
    <div class="content output">
      <div class="left-col">
        <img class="h1img" src="img/yougot.png" />
        <h1 class="headline"><span><asp:Literal ID="LiteralResultCounter" runat="server"></asp:Literal></span></h1>
         <asp:Button id="correct" runat="server" Text="SWEET DATA, YOU NAILED IT." OnClick="ButtonAccurateClick" />
         <asp:Button id="incorrect" runat="server" Text="HMMM, THIS LOOKS OFF." OnClick="ButtonInAccurateClick" />
        <p><asp:Literal ID="LiteralRaffleTickets" runat="server"></asp:Literal></p>
      </div>
      <div class="right-col">
        <img class="logo" src="img/prologo-2x.png" />
        <div class="entity phone-data">
          <div class="circle">
            <div class="icon-pro">
              <span>&#xe602;</span>
            </div>
          </div>
          <ul>
            <li><asp:Literal ID="LitralPhoneNumber" runat="server"></asp:Literal></li>
            <li><span>Carrier:   </span><asp:Literal ID="LiteralPhoneCarrier" runat="server"></asp:Literal></li>
            <li><span>Phone Type:     </span><asp:Literal ID="LiteralPhoneType" runat="server"></asp:Literal></li>
            <li><span>Do Not Call: </span><asp:Literal ID="LiteralDndStatus" runat="server"></asp:Literal></li>
            <li><span>Spam Score:     </span><asp:Literal ID="LiteralSpamScore" runat="server"></asp:Literal></li>
            <li><span>Is Prepaid:     </span><asp:Literal ID="LiteralIsPrepaid" runat="server"></asp:Literal></li>
         </ul>
        </div>
        <div id="personDataDiv" runat="server" visible="true" class="entity person-data">
          <div class="circle">
            <div class="icon-pro">
              <span>&#xe629;</span>
            </div>
          </div>
            <ul>
                <asp:Literal ID="LiteralPeopleDetails" runat="server"></asp:Literal>
            </ul> 
        </div>
        <div class="entity address-data">
          <div class="circle">
              <div class="icon-pro">
              <span>&#xe604;</span>
            </div>
          </div>
          <ul>
            <asp:Literal ID="LiteralLocationDetails" runat="server"></asp:Literal>
          </ul>
       
        </div>
      </div>
    </div>
    </form>
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
            phoneField.mask("( 999 ) 999 - 9999");
            check.prettyCheckable();
            phoneField.on('keyup', function () {
                var keyVal = $(this).val().length;
                if (keyVal >= 18) {
                    addFields.slideDown("slow");
                    desc.slideUp("slow");
                }
            });
            phoneField.on('blur', function () {
                var keyVal = $(this).val().length;
                if (keyVal < 18) {
                    addFields.slideUp("slow");
                    desc.slideDown("slow");
                }
            });
        });
</script>
</body>
</html>
