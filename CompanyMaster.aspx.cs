using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VDBS.App_Code;

namespace VDBS
{
    public partial class CompanyMaster : System.Web.UI.Page
    {
        BusinessRules business = new BusinessRules();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnCreateCompany_Click(object sender, EventArgs e)
        {
            try
            {
                HttpFileCollection companyFiles = Request.Files;

                if (string.IsNullOrEmpty(txtCompanyName.Text))
                {
                    SetPageMessage("Company Name is required");
                    return;
                }

                for (int index = 0; index < companyFiles.Count; index++)
                {
                    if (IsValidFile(companyFiles[index], true))
                        break;
                    else
                        return;
                }
                if(companyFiles == null || companyFiles.Count == 0)
                {
                    SetPageMessage("Atleast one company file is required");
                    return;
                }

                var fileNames = new List<string>();
                string currentFileName = string.Empty;
                // Save Multiple files
                for (int index = 0; index < companyFiles.Count; index++)
                {
                    currentFileName = business.GetFileName(companyFiles[index].FileName);
                    fileNames.Add(currentFileName);
                    companyFiles[index].SaveAs(Server.MapPath(business.GetFilePath(currentFileName)));
                }

                if (business.SaveCompanyMaster(txtCompanyName.Text.Trim(), fileNames))
                {
                    txtCompanyName.Text = string.Empty;
                    SetPageMessage("Company details submitted successfully");
                }
                else
                    SetPageMessage("Unable to store Company details. Please try again");
            }
            catch (Exception ex)
            {
                SetPageMessage(string.Format("Error Occured: {0}", ex.Message));
            }
        }
        private void SetPageMessage(string message)
        {
            var masterPage = (this.Master as Site);
            masterPage.ShowMessage(message);
        }
        private bool IsValidFile(HttpPostedFile file, bool setPageMessage)
        {
            bool response = false;

            if(file != null && file.ContentLength != 0)
            {
                var allowedExtensions = new List<string>() { ".PDF", ".JPG", ".JPEG", ".PNG" };
                var fileExtension = Path.GetExtension(file.FileName).ToUpper();

                if(allowedExtensions.Contains(fileExtension))
                {
                    if (file.ContentLength < 11534336) // File Max size 11 MB = 11 * 1024 * 1024 Bytes
                        response = true;
                    else
                    {
                        if (setPageMessage == true)
                            SetPageMessage(string.Format("File: {0} is more than allowed limit of 10 MB.", file.FileName));
                    }
                }
                else
                {
                    if(setPageMessage == true)
                       SetPageMessage(string.Format("File: {0} is not of allowed type.", file.FileName));
                }
            }
            else
            {
                if (setPageMessage == true)
                    SetPageMessage("Atleast one company file is to be uploaded.");
            }
            return response;
        }
    }
}