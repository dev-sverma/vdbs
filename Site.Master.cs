using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VDBS
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void ShowMessage(string message)
        {
            Page page = (Page)HttpContext.Current.CurrentHandler;
            messageInfo.Text = message;
            // Check if the event raised was asynchronous as call to modal will be different
            if (ScriptManager.GetCurrent(page).IsInAsyncPostBack)
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "department-async-popup", "toggleMessageModal()", true);
            else
                page.ClientScript.RegisterStartupScript(typeof(Page), "department-sync-popup", "toggleMessageModal()", true);
        }
    }
}