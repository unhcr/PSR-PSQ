<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Asylum Seekers Status Determination" 
  CodeFile="PSQRSDD.aspx.cs" Inherits="PSQRSDD" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <div ID="divMainBody" runat="server" class="results">

    <asp:SqlDataSource ID="dsASR_RSD" runat="server"
      ConnectionString="<%$ ConnectionStrings:PSQ %>" 
      ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>">
    </asp:SqlDataSource>

    <asp:Label ID="Label1" runat="server" /><br />

    <div class="top-pager">
      <asp:Label runat="server" ID="lblNoData" Visible="false" CssClass="no-data">
        No results for these criteria
      </asp:Label>
      <asp:Label runat="server" ID="lblPager" ViewStateMode="Disabled">
        <label>Page size:
          <asp:DropDownList runat="server" id="ddlPageRows" ViewStateMode="Enabled"
            OnSelectedIndexChanged="ddlPageRows_SelectedIndexChanged" AutoPostBack="true">
            <asp:ListItem Value="10" />
            <asp:ListItem Value="25" Selected="True" />
            <asp:ListItem Value="50" />
            <asp:ListItem Value="100" />
            <asp:ListItem Value="250" />
            <asp:ListItem Value="500" />
            <asp:ListItem Text="All" Value="0" />
          </asp:DropDownList>
        </label>
        <asp:DataPager ID="dpgASR_RSD1" runat="server" PagedControlID="lvwASR_RSD" 
          PageSize="25" ViewStateMode="Disabled">
          <Fields>
            <asp:NextPreviousPagerField ButtonType="Button"
              ShowFirstPageButton="true" FirstPageText="&lt;&lt;" 
              ShowLastPageButton="true" LastPageText="&gt;&gt;"
              NextPageText="&gt;" PreviousPageText="&lt;" />
          </Fields>
        </asp:DataPager>
      </asp:Label>
      <asp:Button ID="btnNewQuery" runat="server" Text="New Query" PostBackUrl="PSQRSDS.aspx" />
    </div>
    <asp:ListView ID="lvwASR_RSD" runat="server" DataSourceID="dsASR_RSD" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_RSD_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption runat="server">
            <asp:Label ID="capASR_RSD" runat="server" Text="Persons of concern to UNHCR – Overview" />
          </caption>
          <colgroup runat="server">
            <col class="year" />
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
   { %>
            <col />
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
   { %>
            <col />
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RSDP") ||
       ParameterSet.ContainsItem("BREAKDOWN", "RSDL"))
   { %>
            <col class="rsd-codes" />
<% } %>
            <col span="10" class="digits-7" />
          </colgroup>
          <thead runat="server">
            <tr>
              <th rowspan="2">Year</th>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
   { %>
              <th rowspan="2" title="Country or territory of asylum">Country / territory of asylum</th>
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
   { %>
              <th rowspan="2" title="Country or territory of origin">Origin</th>
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RSDP") ||
       ParameterSet.ContainsItem("BREAKDOWN", "RSDL"))
   { %>
              <th rowspan="2" class="rsd-codes" title="Refugee status determination procedure type and level">RSD proc.</th>
<% } %>
              <th colspan="2">Pending start of year</th>
              <th rowspan="2" class="numeric">Applied during year</th>
              <th colspan="2" class="numeric">Positive decisions</th>
              <th rowspan="2" class="numeric">Rejected</th>
              <th rowspan="2" class="numeric">Otherwise closed</th>
              <th rowspan="2" class="numeric">Total decisions</th>
              <th colspan="2" class="numeric">Pending end of year</th>
            </tr>
            <tr>
              <th class="numeric force-vertical-dividers">Total persons</th>
              <th class="numeric">of which UNHCR-assisted</th>
              <th class="numeric">Conven&shy;tion status</th>
              <th class="numeric">Complem. protection status</th>
              <th class="numeric">Total persons</th>
              <th class="numeric">of which UNHCR-assisted</th>
            </tr>
          </thead>
          <tbody>
            <tr ID="itemPlaceholder" runat="server">
            </tr>
          </tbody>
        </table>
      </LayoutTemplate>
      <ItemTemplate>
        <tr>
          <td class="year">
            <asp:Label ID="lblASR_YEAR" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
   { %>
          <td>
            <asp:Label ID="lblCOU_NAME_ASYLUM" runat="server" Text='<%# Eval("COU_NAME_ASYLUM") %>' />
          </td>
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
   { %>
          <td>
            <asp:Label ID="lblCOU_NAME_ORIGIN" runat="server" Text='<%# Eval("COU_NAME_ORIGIN") %>' />
          </td>
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RSDP"))
   {
     if (ParameterSet.ContainsItem("BREAKDOWN", "RSDL"))
     { %>
          <td class="rsd-codes">
            <asp:Label ID="lblRSD_PROC_TYPE_CODE1" runat="server" Text='<%# Eval("RSD_PROC_TYPE_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_TYPE_DESCRIPTION") %>' /> /
            <asp:Label ID="lblRSD_PROC_LEVEL_CODE1" runat="server" Text='<%# Eval("RSD_PROC_LEVEL_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_LEVEL_DESCRIPTION") %>' />
          </td>
  <% } else { %>
          <td class="rsd-codes">
            <asp:Label ID="lblRSD_PROC_TYPE_CODE2" runat="server" Text='<%# Eval("RSD_PROC_TYPE_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_TYPE_DESCRIPTION") %>' />
          </td>
  <% }
   }
   else
   {
     if (ParameterSet.ContainsItem("BREAKDOWN", "RSDL"))
     { %>
          <td class="rsd-codes">
            <asp:Label ID="lblRSD_PROC_LEVEL_CODE2" runat="server" Text='<%# Eval("RSD_PROC_LEVEL_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_LEVEL_DESCRIPTION") %>' />
          </td>
  <% }
   } %>
          <td class="numeric">
            <asp:Label ID="lblASYPOP_START_VALUE" runat="server" Text='<%# Eval("ASYPOP_START_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYPOP_AH_START_VALUE" runat="server" Text='<%# Eval("ASYPOP_AH_START_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYAPP_VALUE" runat="server" Text='<%# Eval("ASYAPP_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYREC_CV_VALUE" runat="server" Text='<%# Eval("ASYREC_CV_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYREC_CP_VALUE" runat="server" Text='<%# Eval("ASYREC_CP_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYREJ_VALUE" runat="server" Text='<%# Eval("ASYREJ_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYOTHCL_VALUE" runat="server" Text='<%# Eval("ASYOTHCL_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblTOTAL_DECISIONS_VALUE" runat="server" Text='<%# Eval("TOTAL_DECISIONS_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYPOP_END_VALUE" runat="server" Text='<%# Eval("ASYPOP_END_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label ID="lblASYPOP_AH_END_VALUE" runat="server" Text='<%# Eval("ASYPOP_AH_END_VALUE", "{0:N0}") %>' />
          </td>
        </tr>
      </ItemTemplate>
    </asp:ListView>
    <div class="bottom-pager">
      <asp:DataPager ID="dpgASR_RSD2" runat="server" PagedControlID="lvwASR_RSD" 
        PageSize="25" ViewStateMode="Disabled">
        <Fields>
          <asp:NumericPagerField ButtonCount="20" ButtonType="Button" CurrentPageLabelCssClass="current-page-button" />
        </Fields>
      </asp:DataPager>
      <asp:Button ID="btnCSV" runat="server" Text="Export to CSV" onclick="btnCSV_Click" />
    </div>
  </div>

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="phS" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#lbtRSD").addClass("active");
    });
  </script>
</asp:Content>
