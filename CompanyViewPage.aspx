<%@ Page Title="VDBS - Company View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CompanyViewPage.aspx.cs" Inherits="VDBS.CompanyViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyPlaceHolder" runat="server">
    <script>
        $(function () {
            SetActiveLink('#companyViewLink');
        });

        function showFilesModal() {
            $('#filesModal').modal('show');
        }

        function showApprovalModal() {
            $('#approvalModal').modal('show');
        }

        function hideApprovalModal() {
            $('#approvalModal').modal('hide');
        }
    </script>
    <br />
    <div class="general-container title-heading-center-small">
        Company View
    </div>
    <br />
    <div class="general-container form-container">
        <asp:UpdatePanel ID="mainUpdPnl" runat="server" UpdateMode="Conditional">
            <ContentTemplate>


                <asp:GridView ID="companyGrid" runat="server" AllowPaging="false" AutoGenerateColumns="false" CssClass="table table-sm table-bordered table-hover" 
                    DataKeyNames="Id, Status" EmptyDataText="No Company Data Available" OnRowDataBound="companyGrid_RowDataBound">
                    <HeaderStyle CssClass="thead-light" HorizontalAlign="Center" />
                    <Columns>
                        <asp:TemplateField HeaderText="S No" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Company Name">
                            <ItemTemplate>
                                <asp:Label ID="lblCompanyName" runat="server" Style="word-wrap: break-word;" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnShow" runat="server" Text="Show" CssClass="btn btn-primary mr-2" OnClick="btnShow_Click" />
                                <asp:Button ID="btnApproveDisapprove" runat="server" Text="Approve / Disapprove" CssClass="btn btn-primary" OnClick="btnApproveDisapprove_Click" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
        <!-- Company Files Modal -->
        <div class="modal fade" id="filesModal">
            <div class="modal-dialog">
                <asp:UpdatePanel ID="filesModalUpdPnl" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="contentPnlfilesModal" runat="server" CssClass="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title">Company Files</h4>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <div class="form-row">
                                    <asp:GridView ID="filesGrid" runat="server" AllowPaging="false" AutoGenerateColumns="false" CssClass="table table-sm table-bordered table-hover w-100" EmptyDataText="No Company Files Available">
                                        <HeaderStyle CssClass="thead-light" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="S No" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Company File">
                                                <ItemTemplate>
                                                    <a href='<%# ResolveClientUrl(Eval("FilePath").ToString()) %>' target="_blank"><%# Eval("FileName") %></a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <br />
                                <div class="form-row justify-content-end">
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <!-- Approve / Disapprove Modal -->
        <div class="modal fade" id="approvalModal">
            <div class="modal-dialog">
                <asp:UpdatePanel ID="approvalUpdPnl" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="contentPnlapprovalModal" runat="server" CssClass="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title">Take Approval / Disapproval Decision</h4>
                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hdnCompanyId" runat="server" />
                                <div class="btn-group" role="group">
                                    <asp:Button ID="btnApprove" runat="server" CssClass="btn btn-success" Text="Approve" OnClick="btnApprove_Click" />
                                    <asp:Button ID="btnDisapprove" runat="server" CssClass="btn btn-danger" Text="Disapprove" OnClick="btnDisapprove_Click" />
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
