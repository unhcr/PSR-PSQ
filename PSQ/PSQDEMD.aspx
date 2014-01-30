<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master"
  Title="UNHCR Population Statistics - Demographics"
  CodeFile="PSQDEMD.aspx.cs" Inherits="PSQDEMD" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <div ID="divMainBody" runat="server" class="results">

    <asp:SqlDataSource ID="dsASR_DEMOGRAPHICS" runat="server"
      ConnectionString="<%$ ConnectionStrings:PSQ %>" 
      ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>">
      <SelectParameters>
        <asp:Parameter Name="START_YEAR" Type="String" DefaultValue="1950" />
        <asp:Parameter Name="END_YEAR" Type="String" DefaultValue="9999" />
      </SelectParameters>
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
      <asp:Button ID="btnNewQuery" runat="server" Text="New Query" PostBackUrl="PSQDEMS.aspx" />
    </div>
    <asp:ListView ID="lvwASR_DEMOGRAPHICS" runat="server" DataSourceID="dsASR_DEMOGRAPHICS" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_DEMOGRAPHICS_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption runat="server">
            <asp:Label ID="capASR_DEMOGRAPHICS" runat="server" Text="Persons of concern to UNHCR – Overview" />
          </caption>
          <colgroup runat="server">
            <col class="year" />
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
   { %>
            <col />
  <% if (ParameterSet.ContainsItem("SUMRES", "LOCATION"))
     { %>
            <col />
  <% } %>
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
   { %>
            <col />
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
   { %>
            <col class="population-type-short" />
<% } %>
<% if (ParameterSet.ContainsItem("SEXAGE", "AGE"))
   { %>
            <col class="sex-label" />
            <col span="5" class="digits-7" />
<% } %>
<% if (!ParameterSet.ContainsItem("SEXAGE", "NONE"))
   { %>
            <col span="2" class="digits-7" />
<% } %>
            <col class="digits-7" />
          </colgroup>
          <thead runat="server">
            <tr>
              <th>Year</th>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
   { %>
              <th title="Country or territory of residence">Residing in</th>
  <% if (ParameterSet.ContainsItem("SUMRES", "LOCATION")) 
     { %>
              <th title="Location of residence">Location</th>
  <% } %>
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN")) 
   { %>
              <th title="Location of residence">Originating / returned from</th>
<% } %>
<% if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
   { %>
              <th class="population-type-short" title="Type of population of concern to UNHCR">Population type</th>
<% } %>
<% if (ParameterSet.ContainsItem("SEXAGE", "AGE")) 
   { %>
              <th>Sex</th>
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
     if (ParameterSet.ContainsItem("SEXAGE", "SEX")) 
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
<% if (ParameterSet.ContainsItem("SEXAGE", "AGE"))
   { %>
        <tr>
          <td rowspan="2" class="year">
            <asp:Label id="lblASR_YEAR1" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
  <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
     { %>
          <td rowspan="2">
            <asp:Label id="lblCOU_NAME_RESIDENCE1" runat="server" Text='<%# Eval("COU_NAME_RESIDENCE") %>' />
          </td>
    <% if (ParameterSet.ContainsItem("SUMRES", "LOCATION"))
       { %>
          <td rowspan="2">
            <asp:Label id="lblLOC_NAME_RESIDENCE1" runat="server" Text='<%# Eval("LOC_NAME_RESIDENCE") %>' />
          </td>
    <% } %>
  <% } %>
  <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
     { %>
          <td rowspan="2">
            <asp:Label id="lblCOU_NAME_ORIGIN1" runat="server" Text='<%# Eval("COU_NAME_ORIGIN") %>' />
          </td>
  <% } %>
  <% if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
     { %>
          <td rowspan="2">
            <asp:Label id="lblDST_DESCRIPTION1" runat="server" Text='<%# Eval("DST_DESCRIPTION") %>' />
          </td>
  <% } %>
          <td class="sex-label">F</td>
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
            <asp:Label id="lblFTOTAL_VALUE1" runat="server" Text='<%# Eval("FTOTAL_VALUE", "{0:N0}") %>' />
          </td>
          <td rowspan="2" class="numeric">
            <asp:Label id="lblTOTAL_VALUE1" runat="server" Text='<%# Eval("TOTAL_VALUE", "{0:N0}") %>' />
          </td>
        </tr>
        <tr class="accent">
          <td class="sex-label force-vertical-dividers">M</td>
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
            <asp:Label id="lblMTOTAL_VALUE1" runat="server" Text='<%# Eval("MTOTAL_VALUE", "{0:N0}") %>' />
          </td>
        </tr>
<% } else
   { %>
        <tr>
          <td class="year">
            <asp:Label id="lblASR_YEAR2" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
  <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
     { %>
          <td>
            <asp:Label id="lblCOU_NAME_RESIDENCE2" runat="server" Text='<%# Eval("COU_NAME_RESIDENCE") %>' />
          </td>
    <% if (ParameterSet.ContainsItem("SUMRES", "LOCATION"))
       { %>
          <td>
            <asp:Label id="lblLOC_NAME_RESIDENCE" runat="server" Text='<%# Eval("LOC_NAME_RESIDENCE") %>' />
          </td>
    <% } %>
  <% } %>
  <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
      { %>
          <td>
            <asp:Label id="lblCOU_NAME_ORIGIN" runat="server" Text='<%# Eval("COU_NAME_ORIGIN") %>' />
          </td>
  <% } %>
  <% if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
     { %>
          <td>
            <asp:Label id="lblDST_DESCRIPTION2" runat="server" Text='<%# Eval("DST_DESCRIPTION") %>' />
          </td>
  <% } %>
  <% if (ParameterSet.ContainsItem("SEXAGE", "SEX"))
     { %>
          <td class="numeric">
            <asp:Label id="lblFTOTAL_VALUE2" runat="server" Text='<%# Eval("FTOTAL_VALUE", "{0:N0}") %>' />
          </td>
          <td class="numeric">
            <asp:Label id="lblMTOTAL_VALUE2" runat="server" Text='<%# Eval("MTOTAL_VALUE", "{0:N0}") %>' />
          </td>
  <% } %>
          <td class="numeric">
            <asp:Label id="lblTOTAL_VALUE2" runat="server" Text='<%# Eval("TOTAL_VALUE", "{0:N0}") %>' />
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
  </div>

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="phS" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#lbtDEM").addClass("active");
    });
  </script>
</asp:Content>

