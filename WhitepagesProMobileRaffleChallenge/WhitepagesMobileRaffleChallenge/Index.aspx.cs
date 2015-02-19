using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace WhitePagesPhoneLookup
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
             
            }
        }

        protected void ButtonFindClick(object sender, EventArgs e)
        {
            try
            {
              

                if (!string.IsNullOrEmpty(textBoxPhoneNumber.Text))
                {
                    string description = string.Empty;
                    string errorMessage = string.Empty;

                    Response.Redirect("PhoneLookup.aspx?phone=" + textBoxPhoneNumber.Text);

                }
               

            }
            catch (Exception ex)
            {
            }
        }
    }
}