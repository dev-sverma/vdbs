<%@ Page Title="VDBS - Company Master" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CompanyMaster.aspx.cs" Inherits="VDBS.CompanyMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyPlaceHolder" runat="server">
    <script>
        $(function () {
            SetActiveLink('#companyMasterLink');
        })

        // courtsey of method: https://stackoverflow.com/questions/48613992/bootstrap-4-file-input-doesnt-show-the-file-name
        function file(fileControl) {
            var fileName = $(fileControl).val();
            $(fileControl).next('.custom-file-label').html(fileName.substring(fileName.lastIndexOf('\\') + 1));
        }
    </script>
    <br />
    <div class="general-container title-heading-center-small">
        Create Company
    </div>
    <br />
    <div class="general-container form-container">
        <div>
            <span class="text-danger">All red star (*) marked fields are mandatory</span>
        </div>
        <br />
        <div class="form-group">
            <asp:Label ID="lblCompanyName" runat="server" AssociatedControlID="txtCompanyName" Text="Company Name"></asp:Label><span class="text-danger">*</span>
            <asp:TextBox ID="txtCompanyName" runat="server" CssClass="form-control form-control-sm"
                AutoCompleteType="None" MaxLength="50" autocomplete="none"></asp:TextBox>
            <asp:RequiredFieldValidator ID="reqTxtCompanyName" runat="server"
                ControlToValidate="txtCompanyName" ErrorMessage="Please Enter Company Name" Font-Bold="True"
                Font-Names="Verdana" Font-Size="10px" ForeColor="Red" SetFocusOnError="True" Enabled="true"
                ValidationGroup="submitForm" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        <div class="form-group">
            <div class="custom-file">
                <asp:FileUpload ID="companyFiles" runat="server" CssClass="custom-file-input" onchange="file(this);" AllowMultiple="true" />
                <label class="custom-file-label">Choose file</label>
            </div>
            <span class="text-danger">Allowed types: .pdf, .jpg, .jpeg / Max Size: 10MB for each file</span>
        </div>
        <asp:Button ID="btnCreateCompany" runat="server" Text="Submit" CssClass="btn btn-info btn-block" OnClick="btnCreateCompany_Click" ValidationGroup="submitForm" />
    </div>
</asp:Content>
