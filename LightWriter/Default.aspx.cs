using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LightWriter.App_Code;

namespace LightWriter
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        //protected string URL { get { return HttpContext.Current.Request.Url.AbsoluteUri; } } 
        // <!--script>var url = "<%=URL%>";</!--script> <!-- Make URL available to the js -->
        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserLogin.IsAuthenticated) {
                menuLoginButton.Src = "Content/images/LogoutButton.png";
                presentationLoginButton.Src = "Content/images/PresentationLogoutButton.png";
            }
        }
    }
}