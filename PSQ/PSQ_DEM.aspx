<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Demographics" 
  CodeFile="PSQ_DEM.aspx.cs" Inherits="PSQ_DEM" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
  <% if (selectionMode)
     { %>
  <style type="text/css"> .main-body { display:none; } </style>
  <% } else
     {%>
  <style type="text/css"> .selection-box { display:none; } </style>
  <% } %>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">

  <asp:Label ID="Label1" runat="server" />

  <asp:SqlDataSource ID="dsASR_YEAR" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct ASR_YEAR from ASR_POC_SUMMARY_EN order by ASR_YEAR desc" />

  <asp:SqlDataSource ID="dsCOUNTRY" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct COU_CODE_RESIDENCE, COU_NAME_RESIDENCE_EN from ASR_POC_SUMMARY_EN order by nlssort(COU_NAME_RESIDENCE_EN, 'NLS_SORT=BINARY_AI')" />
 
  <div ID="divSelectionBox" runat="server" class="selection-box">
    <fieldset>
      <legend>Selection criteria</legend>
      <div class="date-range-selection">
        <label>Date range:</label>
        <asp:DropDownList ID="ddlSTART_YEAR" runat="server" ViewStateMode="Disabled"
          DataSourceID="dsASR_YEAR" DataTextField="ASR_YEAR" DataValueField="ASR_YEAR" 
          CssClass="year start-year" />
        <label>to</label>
        <asp:DropDownList ID="ddlEND_YEAR" runat="server" ViewStateMode="Disabled" 
          DataSourceID="dsASR_YEAR" DataTextField="ASR_YEAR" DataValueField="ASR_YEAR" 
          CssClass="year end-year" />
      </div>
      <div class="country-selection">
        <label>Country / territory of residence</label>
        <asp:ListBox ID="lbxCOUNTRY" runat="server" ViewStateMode="Disabled" 
          DataSourceID="dsCOUNTRY" DataTextField="COU_NAME_RESIDENCE_EN" DataValueField="COU_CODE_RESIDENCE" 
          Rows="12" SelectionMode="Multiple" 
          CssClass="country" OnDataBound="lbxCOUNTRY_DataBound" />
      </div>
    </fieldset>
    <fieldset class="column-selection">
      <legend>Data items to display</legend>
      <div class="dimensions">
        <label><asp:CheckBox ID="cbxRES" runat="server" ViewStateMode="Disabled" Checked="true" />Country / territory of residence</label>
        <label><asp:CheckBox ID="cbxLOC" runat="server" ViewStateMode="Disabled" Checked="true" />Location of residence</label>
      </div>
      <div class="values">
        Breakdown by:<br />
        <asp:RadioButton runat="server" ID="rdbAGE" Text="Sex and age" GroupName="Breakdown" Checked="true" /><br />
        <asp:RadioButton runat="server" ID="rdbSEX" Text="Sex only" GroupName="Breakdown" /><br />
        <asp:RadioButton runat="server" ID="rdbNone" Text="None (total only)" GroupName="Breakdown" /><br />
      </div>
    </fieldset>
    <div class="buttons">
      <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" />
    </div>
  </div>

  <asp:SqlDataSource id="dsASR_DEMOGRAPHICS" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>">
    <SelectParameters>
      <asp:Parameter Name="START_YEAR" Type="String" DefaultValue="1950" />
      <asp:Parameter Name="END_YEAR" Type="String" DefaultValue="9999" />
    </SelectParameters>
  </asp:SqlDataSource>

  <div ID="divMainBody" runat="server" class="main-body">
    <div class="top-pager">
      <asp:Label runat="server" ID="lblNoData" Visible="false" CssClass="no-data">
        No results for these criteria
      </asp:Label>
      <asp:Label runat="server" ID="lblPager" ViewStateMode="Disabled">Page size:
        <asp:DropDownList runat="server" id="ddlPageRows" ViewStateMode="Enabled" 
          OnSelectedIndexChanged="ddlPageRows_SelectedIndexChanged" AutoPostBack="true">              
          <asp:ListItem Value="5" />
          <asp:ListItem Value="12" Selected="True" />
          <asp:ListItem Value="25" />
          <asp:ListItem Value="50" />
          <asp:ListItem Value="100" />
          <asp:ListItem Value="250" />
          <asp:ListItem Text="All" Value="0" />
        </asp:DropDownList>
        <asp:DataPager ID="dpgASR_DEMOGRAPHICS1" runat="server" PagedControlID="lvwASR_DEMOGRAPHICS" 
          PageSize="25" ViewStateMode="Disabled">
          <Fields>
            <asp:NextPreviousPagerField ButtonType="Button"
              ShowFirstPageButton="true" FirstPageText="&lt;&lt;" 
              ShowLastPageButton="true" LastPageText="&gt;&gt;"
              NextPageText="&gt;" PreviousPageText="&lt;" />
          </Fields>
        </asp:DataPager>
      </asp:Label>
      <asp:Button ID="btnNewQuery" runat="server" Text="New Query" onclick="btnNewQuery_Click" />
    </div>
    <asp:ListView ID="lvwASR_DEMOGRAPHICS" runat="server" DataSourceID="dsASR_DEMOGRAPHICS" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_DEMOGRAPHICS_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption>
            <asp:Label ID="capASR_DEMOGRAPHICS" runat="server" Text="Demographic composition of populations of concern to UNHCR" />
          </caption>
          <colgroup runat="server">
            <col class="year" />
            <% if (selectionCriteria.ShowRES) { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowLOC) { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowAGE) { %>
            <col class="sex-label" />
            <col span="5" class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowAGE || selectionCriteria.ShowSEX) { %>
            <col span="2" class="digits-8" />
            <% } %>
            <col class="digits-8" />
          </colgroup>
          <thead runat="server">
            <tr>
              <th>Year</th>
              <% if (selectionCriteria.ShowRES)
                 { %>
              <th title="Country or territory of residence">Country / territory of residence</th>
              <% } %>
              <% if (selectionCriteria.ShowLOC) 
                 { %>
              <th title="Location of residence">Location of residence</th>
              <% } %>
              <% if (selectionCriteria.ShowAGE) 
                 { %>
              <th></th>
              <th class="numeric">0&minus;4</th>
              <th class="numeric">5&minus;11</th>
              <th class="numeric">12&minus;17</th>
              <th class="numeric">18&minus;59</th>
              <th class="numeric">60+</th>
              <th class="numeric">Unknown age</th>
              <th class="numeric">Total by sex</th>
              <th class="numeric">Overall total</th>
              <% } else 
                 {
                   if (selectionCriteria.ShowSEX) 
                   { %>
              <th class="numeric">Female</th>
              <th class="numeric">Male</th>
                <% } %>
              <th class="numeric">Total</th>
              <% } %>
            </tr>
          </thead>
          <tbody>
            <tr ID="itemPlaceholder" runat="server">
            </tr>
          </tbody>
        </table>
      </LayoutTemplate>
      <ItemTemplate>
        <% if (selectionCriteria.ShowAGE)
           { %>
        <tr>
          <td rowspan="2" class="year">
            <asp:Label id="lblASR_YEAR" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
          <% if (selectionCriteria.ShowRES)
             { %>
          <td rowspan="2">
            <asp:Label id="lblCOU_NAME_RESIDENCE_EN" runat="server" Text='<%# Eval("COU_NAME_RESIDENCE_EN") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowLOC)
             { %>
          <td rowspan="2">
            <asp:Label id="lblLOC_NAME_RESIDENCE_EN" runat="server" Text='<%# Eval("LOC_NAME_RESIDENCE_EN") %>' />
          </td>
          <% } %>
          <th>Female:</th>
          <td class="numeric">
            <asp:Label id="lblF0_VALUE" runat="server" Text='<%# Eval("F0_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblF5_VALUE" runat="server" Text='<%# Eval("F5_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblF12_VALUE" runat="server" Text='<%# Eval("F12_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblF18_VALUE" runat="server" Text='<%# Eval("F18_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblF60_VALUE" runat="server" Text='<%# Eval("F60_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblFOTHER_VALUE" runat="server" Text='<%# Eval("FOTHER_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblFTOTAL_VALUE" runat="server" Text='<%# Eval("FTOTAL_VALUE", "{0:N0}") %>' />
          </td>
          <td rowspan="2" class="numeric">
            <asp:Label id="lblTOTAL_VALUE" runat="server" Text='<%# Eval("TOTAL_VALUE", "{0:N0}") %>' />
          </td>
        </tr>
        <tr class="male-row">
          <th class="force-vertical-dividers">Male:</th>
          <td class="numeric">
            <asp:Label id="lblM0_VALUE" runat="server" Text='<%# Eval("M0_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblM5_VALUE" runat="server" Text='<%# Eval("M5_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblM12_VALUE" runat="server" Text='<%# Eval("M12_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblM18_VALUE" runat="server" Text='<%# Eval("M18_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblM60_VALUE" runat="server" Text='<%# Eval("M60_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblMOTHER_VALUE" runat="server" Text='<%# Eval("MOTHER_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblMTOTAL_VALUE" runat="server" Text='<%# Eval("MTOTAL_VALUE", "{0:N0}") %>' />
          </td>
        </tr>
        <% } else
           { %>
        <tr>
          <td class="year">
            <asp:Label id="Label2" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
          <% if (selectionCriteria.ShowRES)
             { %>
          <td>
            <asp:Label id="Label3" runat="server" Text='<%# Eval("COU_NAME_RESIDENCE_EN") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowLOC)
             { %>
          <td>
            <asp:Label id="Label4" runat="server" Text='<%# Eval("LOC_NAME_RESIDENCE_EN") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowSEX)
             { %>
          <td class="numeric">
            <asp:Label id="Label10" runat="server" Text='<%# Eval("FTOTAL_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="Label11" runat="server" Text='<%# Eval("MTOTAL_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <td class="numeric">
            <asp:Label id="Label12" runat="server" Text='<%# Eval("TOTAL_VALUE", "{0:N0}") %>' />
          </td>
        </tr>
        <% } %>
      </ItemTemplate>
    </asp:ListView>
    <div class="bottom-pager">
      <asp:DataPager ID="dpgASR_DEMOGRAPHICS2" runat="server" PagedControlID="lvwASR_DEMOGRAPHICS" 
        PageSize="25" ViewStateMode="Disabled">
        <Fields>
          <asp:NumericPagerField ButtonCount="20" ButtonType="Button" CurrentPageLabelCssClass="current-page-button" /> 
        </Fields>
      </asp:DataPager>
      <asp:Button ID="btnCSV" runat="server" Text="Export to CSV" onclick="btnCSV_Click" />
    </div>
  </div> <!-- /.main-body -->

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#demographics").addClass("active");
    });
  </script>
</asp:Content>