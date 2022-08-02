using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VDBS.App_Code;

namespace VDBS
{
    public partial class CompanyViewPage : System.Web.UI.Page
    {
        BusinessRules business = new BusinessRules();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetCompanyGrid();
            }
        }

        public void SetCompanyGrid()
        {
            try
            {
                companyGrid.DataSource = business.GetCompanyInfo();
                companyGrid.DataBind();
            }
            catch (Exception ex)
            {
                SetPageMessage(string.Format("Error occured: {0}", ex.Message));
            }
        }
        private void SetPageMessage(string message)
        {
            var masterPage = (this.Master as Site);
            masterPage.ShowMessage(message);
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                var companyId = companyGrid.DataKeys[((sender as Button).NamingContainer as GridViewRow).RowIndex]["Id"].ToString();
                filesGrid.DataSource = business.GetCompanyFiles(companyId);
                filesGrid.DataBind();
                filesModalUpdPnl.Update();
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "showFiles-async-popup", "showFilesModal()", true);
                //Page page = (Page)HttpContext.Current.CurrentHandler;
                //page.ClientScript.RegisterStartupScript(typeof(Page), "department-sync-popup", "showFilesModal()", true);
            }
            catch (Exception ex)
            {
                SetPageMessage(string.Format("Error Occured: {0}", ex.Message));
            }
        }

        protected void btnApproveDisapprove_Click(object sender, EventArgs e)
        {
            try
            {
                hdnCompanyId.Value = companyGrid.DataKeys[((sender as Button).NamingContainer as GridViewRow).RowIndex]["Id"].ToString();
                approvalUpdPnl.Update();
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "showApproval-async-popup", "showApprovalModal()", true);
                //Page page = (Page)HttpContext.Current.CurrentHandler;
                //page.ClientScript.RegisterStartupScript(typeof(Page), "department-sync-popup", "showApprovalModal()", true);
            }
            catch (Exception ex)
            {
                SetPageMessage(string.Format("Error Occured: {0}", ex.Message));
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "approve-async-popup", "hideApprovalModal()", true);
            string companyId = hdnCompanyId.Value;
            hdnCompanyId.Value = string.Empty;
            approvalUpdPnl.Update();

            try
            {
                if (business.ApproveDisapproveCompany(companyId, true))
                    SetPageMessage("Company approved");
                else
                    SetPageMessage("Unable to approve company. Please try again");

                SetCompanyGrid();
                mainUpdPnl.Update();
            }
            catch (Exception ex)
            {
                SetPageMessage(string.Format("Error Occured: {0}", ex.Message));
            }
        }

        protected void btnDisapprove_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "disapprove-async-popup", "hideApprovalModal()", true);
            string companyId = hdnCompanyId.Value;
            hdnCompanyId.Value = string.Empty;
            approvalUpdPnl.Update();

            try
            {
                if (business.ApproveDisapproveCompany(companyId, false))
                    SetPageMessage("Company Disapproved");
                else
                    SetPageMessage("Unable to Disapprove company. Please try again");

                SetCompanyGrid();
                mainUpdPnl.Update();
            }
            catch (Exception ex)
            {
                SetPageMessage(string.Format("Error Occured: {0}", ex.Message));
            }
        }

        protected void companyGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.DataRow)
            {
                var showBtn = e.Row.FindControl("btnShow") as Button;
                var approveDisapproveBtn = e.Row.FindControl("btnApproveDisapprove") as Button;

                var status = companyGrid.DataKeys[e.Row.RowIndex]["Status"].ToString();
                bool enableStatus = false;
                if(string.IsNullOrEmpty(status) || status.ToString() == "1")
                    enableStatus = true;

                showBtn.Enabled = enableStatus;
                approveDisapproveBtn.Enabled = enableStatus;
            }
        }
    }
}