<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Persons of Concern Time Series" 
  CodeFile="PSQ_TMS.aspx.cs" Inherits="PSQ_TMS" %>

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
    SelectCommand="select distinct ASR_YEAR from ASR_POC_DETAILS_EN order by ASR_YEAR desc" />

  <asp:SqlDataSource ID="dsCOUNTRY" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct COU_CODE_RESIDENCE, COU_NAME_RESIDENCE_EN from ASR_POC_DETAILS_EN order by nlssort(COU_NAME_RESIDENCE_EN, 'NLS_SORT=BINARY_AI')" />

  <asp:SqlDataSource ID="dsORIGIN" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct COU_CODE_ORIGIN, COU_NAME_ORIGIN_EN from ASR_POC_DETAILS_EN order by nlssort(COU_NAME_ORIGIN_EN, 'NLS_SORT=BINARY_AI')" />

  <asp:SqlDataSource ID="dsPOP_TYPE" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct POPULATION_TYPE_CODE, POPULATION_TYPE_EN, POPULATION_TYPE_SEQ from ASR_POC_DETAILS_EN order by POPULATION_TYPE_SEQ" />

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
      <div class="poptype-selection">
        <label>Population type</label>
        <asp:CheckBoxList ID="cblPOP_TYPE" runat="server" ViewStateMode="Disabled" 
          DataSourceID="dsPOP_TYPE" DataTextField="POPULATION_TYPE_EN" DataValueField="POPULATION_TYPE_CODE" 
          OnDataBound="cblPOP_TYPE_DataBound" />
      </div>
    </fieldset>
    <fieldset class="column-selection">
      <legend>Data items to display</legend>
      <div class="dimensions">
        <label><asp:CheckBox ID="cbxRES" runat="server" ViewStateMode="Disabled" Checked="true" />Country / territory of residence</label>
        <label><asp:CheckBox ID="cbxOGN" runat="server" ViewStateMode="Disabled" Checked="true" />Origin / Returned from</label>
        <label><asp:CheckBox ID="cbxPOPT" runat="server" ViewStateMode="Disabled" Checked="true" />Population type</label>
      </div>
    </fieldset>
    <div class="buttons">
      <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" />
    </div>
  </div>

  <asp:SqlDataSource ID="dsASR_POC_DETAILS" runat="server"
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>" />

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
        <asp:DataPager ID="dpgASR_POC_DETAILS1" runat="server" PagedControlID="lvwASR_POC_DETAILS" 
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
    <asp:ListView ID="lvwASR_POC_DETAILS" runat="server" DataSourceID="dsASR_POC_DETAILS" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_POC_DETAILS_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption>
            <asp:Label ID="capASR_POC_DETAILS" runat="server" Text="Persons of concern to UNHCR – Time series" />
          </caption>
          <colgroup runat="server">
            <% if (selectionCriteria.ShowRES)
               { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowOGN)
               { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowPOPT)
               { %>
            <col class="population-type-short" />
            <% } %>
            <% foreach (string year in selectionCriteria.StatisticYears)
               { %>
            <col class="digits-7" />
            <% } %>
          </colgroup>
          <thead runat="server">
            <tr>
              <% if (selectionCriteria.ShowRES) { %>
              <th title="Country or territory of residence">Country / territory of residence</th>
              <% } %>
              <% if (selectionCriteria.ShowOGN) { %>
              <th title="Country or territory of origin or from which persons of concern have returned">Origin / Returned from</th>
              <% } %>
              <% if (selectionCriteria.ShowPOPT) { %>
              <th class="population-type-short" title="Type of population of concern to UNHCR">Population type</th>
              <% } %>
              <% foreach (string year in selectionCriteria.StatisticYears)
                 { %>
              <th class="numeric"><%= year %></th>
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
      </ItemTemplate>
    </asp:ListView>
    <div class="bottom-pager">
      <asp:DataPager ID="dpgASR_POC_DETAILS2" runat="server" PagedControlID="lvwASR_POC_DETAILS" 
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
      $("#poc-time-series").addClass("active");
    });
  </script>
</asp:Content>