<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Persons of Concern Overview" 
  CodeFile="PSQ_POC.aspx.cs" Inherits="PSQ_POC" %>

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

  <asp:SqlDataSource ID="dsORIGIN" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct COU_CODE_ORIGIN, COU_NAME_ORIGIN_EN from ASR_POC_SUMMARY_EN order by nlssort(COU_NAME_ORIGIN_EN, 'NLS_SORT=BINARY_AI')" />

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
          Rows="8" SelectionMode="Multiple" 
          CssClass="country" OnDataBound="lbxCOUNTRY_DataBound" />
      </div>
      <div class="country-selection">
        <label>Origin / Returned from</label>
        <asp:ListBox ID="lbxORIGIN" runat="server" ViewStateMode="Disabled" 
          DataSourceID="dsORIGIN" DataTextField="COU_NAME_ORIGIN_EN" DataValueField="COU_CODE_ORIGIN" 
          Rows="8" SelectionMode="Multiple" 
          CssClass="origin" OnDataBound="lbxORIGIN_DataBound" />
      </div>
    </fieldset>
    <fieldset class="column-selection">
      <legend>Data items to display</legend>
      <div class="dimensions">
        <label><asp:CheckBox ID="cbxRES" runat="server" ViewStateMode="Disabled" Checked="true" />Country / territory of residence</label>
        <label><asp:CheckBox ID="cbxOGN" runat="server" ViewStateMode="Disabled" Checked="true" />Origin / Returned from</label>
      </div>
      <div class="values">
        <label><asp:CheckBox ID="cbxREF" runat="server" ViewStateMode="Disabled" Checked="true" />Refugees</label><br />
        <label><asp:CheckBox ID="cbxASY" runat="server" ViewStateMode="Disabled" Checked="true" />Asylum-seekers</label><br />
        <label><asp:CheckBox ID="cbxRET" runat="server" ViewStateMode="Disabled" Checked="true" />Returned refugees</label><br />
        <label><asp:CheckBox ID="cbxIDP" runat="server" ViewStateMode="Disabled" Checked="true" />Internally displaced persons</label><br />
        <label><asp:CheckBox ID="cbxRDP" runat="server" ViewStateMode="Disabled" Checked="true" />Returned IDPs</label><br />
        <label><asp:CheckBox ID="cbxSTA" runat="server" ViewStateMode="Disabled" Checked="true" />Stateless persons</label><br />
        <label><asp:CheckBox ID="cbxOOC" runat="server" ViewStateMode="Disabled" Checked="true" />Others of concern</label><br />
        <label><asp:CheckBox ID="cbxPOC" runat="server" ViewStateMode="Disabled" Checked="true" />Total population</label>
      </div>
    </fieldset>
    <div class="buttons">
      <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" />
    </div>
  </div>

  <asp:SqlDataSource ID="dsASR_POC_SUMMARY" runat="server"
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
          <asp:ListItem Value="10" />
          <asp:ListItem Value="25" Selected="True" />
          <asp:ListItem Value="50" />
          <asp:ListItem Value="100" />
          <asp:ListItem Value="250" />
          <asp:ListItem Value="500" />
          <asp:ListItem Text="All" Value="0" />
        </asp:DropDownList>
        <asp:DataPager ID="dpgASR_POC_SUMMARY1" runat="server" PagedControlID="lvwASR_POC_SUMMARY" 
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
    <asp:ListView ID="lvwASR_POC_SUMMARY" runat="server" DataSourceID="dsASR_POC_SUMMARY" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_POC_SUMMARY_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption runat="server">
            <asp:Label ID="capASR_POC_SUMMARY" runat="server" Text="Persons of concern to UNHCR – Overview" />
          </caption>
          <colgroup runat="server">
            <col class="year" />
            <% if (selectionCriteria.ShowRES)
               { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowOGN)
               { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowREF)
               { %>
            <col class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowASY)
               { %>
            <col class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowRET)
               { %>
            <col class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowIDP)
               { %>
            <col class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowRDP)
               { %>
            <col class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowSTA)
               { %>
            <col class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowOOC)
               { %>
            <col class="digits-8" />
            <% } %>
            <% if (selectionCriteria.ShowPOC)
               { %>
            <col class="digits-8" />
            <% } %>
          </colgroup>
          <thead runat="server">
            <tr>
              <th>Year</th>
              <% if (selectionCriteria.ShowRES) { %>
              <th title="Country or territory of residence">Country / territory of residence</th>
              <% } %>
              <% if (selectionCriteria.ShowOGN) { %>
              <th title="Country or territory of origin or from which persons of concern have returned">Origin / Returned from</th>
              <% } %>
              <% if (selectionCriteria.ShowREF) { %>
              <th class="numeric" title="Refugees and people in a refugee-like situation">Refugees</th>
              <% } %>
              <% if (selectionCriteria.ShowASY) { %>
              <th class="numeric" title="Asylum seekers (pending cases)">Asylum seekers</th>
              <% } %>
              <% if (selectionCriteria.ShowRET) { %>
              <th class="numeric" title="Returned refugees and people in a refugee-like situation">Returned refugees</th>
              <% } %>
              <% if (selectionCriteria.ShowIDP) { %>
              <th class="numeric" title="Internally displaced persons protected / assisted by UNHCR">IDPs</th>
              <% } %>
              <% if (selectionCriteria.ShowRDP) { %>
              <th class="numeric" title="Returned internally displaced persons">Returned IDPs</th>
              <% } %>
              <% if (selectionCriteria.ShowSTA) { %>
              <th class="numeric" title="Persons under UNHCR statelessness mandate">Stateless persons</th>
              <% } %>
              <% if (selectionCriteria.ShowOOC) { %>
              <th class="numeric" title="Other persons of concern to UNHCR">Others of concern</th>
              <% } %>
              <% if (selectionCriteria.ShowPOC) { %>
              <th class="numeric" title="Total population of concern to UNHCR">Total population</th>
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
        <tr>
          <td class="year">
            <asp:Label ID="lblASR_YEAR" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
          <% if (selectionCriteria.ShowRES)
             { %>
          <td>
            <asp:Label ID="lblCOU_NAME_RESIDENCE_EN" runat="server" Text='<%# Eval("COU_NAME_RESIDENCE_EN") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowOGN)
             { %>
          <td>
            <asp:Label ID="lblCOU_NAME_ORIGIN_EN" runat="server" Text='<%# Eval("COU_NAME_ORIGIN_EN") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowREF)
             { %>
          <td class="numeric">
            <asp:Label ID="lblREFPOP_VALUE" runat="server" Text='<%# Eval("REFPOP_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowASY)
             { %>
          <td class="numeric">
            <asp:Label ID="lblASYPOP_VALUE" runat="server" Text='<%# Eval("ASYPOP_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowRET)
             { %>
          <td class="numeric">
            <asp:Label ID="lblREFRTN_VALUE" runat="server" Text='<%# Eval("REFRTN_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowIDP)
             { %>
          <td class="numeric">
            <asp:Label ID="lblIDPHPOP_VALUE" runat="server" Text='<%# Eval("IDPHPOP_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowRDP)
             { %>
          <td class="numeric">
            <asp:Label ID="lblIDPHRTN_VALUE" runat="server" Text='<%# Eval("IDPHRTN_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowSTA)
             { %>
          <td class="numeric">
            <asp:Label ID="lblSTAPOP_VALUE" runat="server" Text='<%# Eval("STAPOP_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowOOC)
             { %>
          <td class="numeric">
            <asp:Label ID="lblOOCPOP_VALUE" runat="server" Text='<%# Eval("OOCPOP_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowPOC)
             { %>
          <td class="numeric">
            <asp:Label ID="lblTPOC_VALUE" runat="server" Text='<%# Eval("TPOC_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
        </tr>
      </ItemTemplate>
    </asp:ListView>
    <div class="bottom-pager">
      <asp:DataPager ID="dpgASR_POC_SUMMARY2" runat="server" PagedControlID="lvwASR_POC_SUMMARY" 
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
    $( document ).ready(function () {
      "use strict";
      $( "#poc-overview" ).addClass( "active" );
    });
  </script>
</asp:Content>