<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Persons of Concern Time Series" 
  CodeFile="PSQTMSD.aspx.cs" Inherits="PSQTMSD" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <div ID="divMainBody" runat="server" class="results">

    <asp:SqlDataSource ID="dsASR_POC_DETAILS" runat="server"
      ConnectionString="<%$ ConnectionStrings:PSQ %>" 
      ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>">
    </asp:SqlDataSource>

    <%--<asp:Label ID="Label1" runat="server" /><br />--%>

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
      <asp:Button ID="btnNewQuery" runat="server" Text="New Query" PostBackUrl="PSQTMSS.aspx" />
    </div>
    <asp:ListView ID="lvwASR_POC_DETAILS" runat="server" DataSourceID="dsASR_POC_DETAILS" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_POC_DETAILS_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption runat="server">
            <asp:Label ID="capASR_POC_DETAILS" runat="server" Text="Persons of concern to UNHCR – Time Series" />
          </caption>
          <colgroup runat="server">
            <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
               { %>
            <col />
            <% } %>
            <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
               { %>
            <col />
            <% } %>
            <% if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
               { %>
            <col class="population-type-short" />
            <% } %>
            <% foreach (string year in ParameterSet["YEAR"])
               { %>
            <col class="digits-7" />
            <% } %>
          </colgroup>
          <thead runat="server">
            <tr>
              <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
                 { %>
              <th title="Country, territory or region of residence">Residing in</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
                 { %>
              <th title="Country, territory or region of origin or from which persons of concern have returned">Originating / returned from</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
                 { %>
              <th class="population-type-short" title="Type of population of concern to UNHCR">Population type</th>
              <% } %>
              <% foreach (string year in ParameterSet["YEAR"])
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
  </div>

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="phS" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#lbtTMS").addClass("active");
    });
  </script>
</asp:Content>

