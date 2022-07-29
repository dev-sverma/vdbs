# vdbs
Project for VDBS Pvt. Ltd.

Task :

1) Create table in Database with following details

Table Name : CompanyMaster

Column Names : Id (identity column), Name, Status (data will be 0 or 1)

Table Name : FileMaster

Column Names : Id (identity column), FileName, CompanyId (Reference of
CompanyMaster's Id column)

2) Create ASP.net Page with following details

Page Name : CompanyMaster.Aspx

Create a company page with a Name field and fileupload control to upload
multiple documents. On the Submit button click Name should be inserted
in CompanyMaster Table and Files should be uploaded to FileMaster Table
with reference Id of company.

Status column will be null for new insert.

Page Name : CompanyViewPage

Add a Grid/List to show the list of added company lists with 2 buttons
as "Show" and "Approve/Disapprove". Rows with Status value as 1 (means
approved) will be enabled and rows with Status value as 0 (means
disapproved) will be disable (user will not be able to click on the Show
and Approve/Disapprove buttons).

On the 1st button click, show pop-up with a list of uploaded files,
which the user will be able to download on click of filename.

On the 2nd button, Show popup with 2 buttons, approve and disapprove. On
click of approve insert '1' in CompanyMaster Table and On disapprove '0'
will be updated.
