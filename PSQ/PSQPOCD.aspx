<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Persons of Concern Overview" 
  CodeFile="PSQPOCD.aspx.cs" Inherits="PSQPOCD" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <div ID="divMainBody" runat="server" class="results">

    <asp:SqlDataSource ID="dsASR_POC_SUMMARY" runat="server"
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
      <asp:Button ID="btnNewQuery" runat="server" Text="New Query" PostBackUrl="PSQPOCS.aspx" />
    </div>
    <asp:ListView ID="lvwASR_POC_SUMMARY" runat="server" DataSourceID="dsASR_POC_SUMMARY" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_POC_SUMMARY_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption id="Caption1" runat="server">
            <asp:Label ID="capASR_POC_SUMMARY" runat="server" Text="Persons of concern to UNHCR – Overview" />
          </caption>
          <colgroup id="Colgroup1" runat="server">
            <col class="year" />
            <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
               { %>
            <col />
            <% } %>
            <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
               { %>
            <col />
            <% } %>
            <% if (ParameterSet.ContainsItem("INCLUDE", "HCRASS"))
               { if (!ParameterSet.ContainsItem("BREAKDOWN", "RES") ||
                 !ParameterSet.ContainsItem("BREAKDOWN", "OGN") ||
                 ParameterSet["POP_TYPES"].Count < 9)
                 { %>
            <col class="total-assisted-wide" />
              <% } else { %>
            <col class="total-assisted-narrow" />
              <% } %>
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "RF"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "RL"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "RFT"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "AS"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "RT"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "ID"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "IL"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "IDT"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "RD"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "ST"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "OC"))
               { %>
            <col class="digits-7" />
            <% } %>
            <% if (ParameterSet.ContainsItem("POP_TYPES", "TPOC"))
               { %>
            <col class="digits-7" />
            <% } %>
          </colgroup>
          <thead id="Thead1" runat="server">
            <tr>
              <th>Year</th>
              <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
                 { %>
              <th title="Country, territory or region of residence">Residing in</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
                 { %>
              <th title="Country, territory or region of origin or from which persons of concern have returned">Originating / returned from</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("INCLUDE", "HCRASS"))
                 { %>
              <th></th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "RF"))
                 { %>
              <th class="numeric" title="Refugees">Refugees</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "RL"))
                 { %>
              <th class="numeric" title="People in a refugee-like situation">Refugee- like</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "RFT"))
                 { %>
              <th class="numeric" title="Refugees and people in a refugee-like situation">Refugees (including ref.-like)</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "AS"))
                 { %>
              <th class="numeric" title="Asylum seekers (pending cases)">Asylum-seekers</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "RT"))
                 { %>
              <th class="numeric" title="Returned refugees and people in a refugee-like situation">Returned refugees</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "ID"))
                 { %>
              <th class="numeric" title="Internally displaced persons protected / assisted by UNHCR">IDPs</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "IL"))
                 { %>
              <th class="numeric" title="Persons in an IDP-like situation">IDP-like</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "IDT"))
                 { %>
              <th class="numeric" title="IDPs and persons in an IDP-like situation">IDPs (including IDP-like)</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "RD"))
                 { %>
              <th class="numeric" title="Returned internally displaced persons">Returned IDPs</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "ST"))
                 { %>
              <th class="numeric" title="Persons under UNHCR statelessness mandate">Stateless persons</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "OC"))
                 { %>
              <th class="numeric" title="Other persons of concern to UNHCR">Others of concern</th>
              <% } %>
              <% if (ParameterSet.ContainsItem("POP_TYPES", "TPOC"))
                 { %>
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
        <% if (ParameterSet.ContainsItem("INCLUDE", "HCRASS"))
           { %>
          <td rowspan="2" class="year">
            <asp:Label ID="lblASR_YEAR1" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
          <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
             { %>
          <td rowspan="2">
            <asp:Label ID="lblCOU_NAME_RESIDENCE1" runat="server" Text='<%# Eval("COU_NAME_RESIDENCE") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
              { %>
          <td rowspan="2">
            <asp:Label ID="lblCOU_NAME_ORIGIN1" runat="server" Text='<%# Eval("COU_NAME_ORIGIN") %>' />
          </td>
          <% } %>
          <% if (!ParameterSet.ContainsItem("BREAKDOWN", "RES") ||
               !ParameterSet.ContainsItem("BREAKDOWN", "OGN") ||
               ParameterSet["POP_TYPES"].Count < 9)
             { %>
          <td class="total-assisted force-vertical-dividers" title="Total population">Total population:</td>
          <% }
             else
             { %>
          <td class="total-assisted force-vertical-dividers" title="Total population">T</td>
          <% } %>
        <% } else { %>
          <td class="year">
            <asp:Label ID="lblASR_YEAR2" runat="server" Text='<%# Eval("ASR_YEAR") %>' />
          </td>
          <% if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
             { %>
          <td>
            <asp:Label ID="lblCOU_NAME_RESIDENCE2" runat="server" Text='<%# Eval("COU_NAME_RESIDENCE") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
              { %>
          <td>
            <asp:Label ID="lblCOU_NAME_ORIGIN2" runat="server" Text='<%# Eval("COU_NAME_ORIGIN") %>' />
          </td>
          <% } %>
        <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RF"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRF_VALUE" runat="server" Text='<%# Eval("RF_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RL"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRL_VALUE" runat="server" Text='<%# Eval("RL_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RFT"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRFT_VALUE" runat="server" Text='<%# Eval("RFT_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "AS"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblAS_VALUE" runat="server" Text='<%# Eval("AS_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RT"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRT_VALUE" runat="server" Text='<%# Eval("RT_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "ID"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblID_VALUE" runat="server" Text='<%# Eval("ID_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "IL"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblIL_VALUE" runat="server" Text='<%# Eval("IL_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "IDT"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblIDT_VALUE" runat="server" Text='<%# Eval("IDT_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RD"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRD_VALUE" runat="server" Text='<%# Eval("RD_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "ST"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblST_VALUE" runat="server" Text='<%# Eval("ST_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "OC"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblOC_VALUE" runat="server" Text='<%# Eval("OC_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "TPOC"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblTPOC_VALUE" runat="server" Text='<%# Eval("TPOC_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
        </tr>
        <% if (ParameterSet.ContainsItem("INCLUDE", "HCRASS"))
            { %>
        <tr class="accent">
          <% if (!ParameterSet.ContainsItem("BREAKDOWN", "RES") ||
               !ParameterSet.ContainsItem("BREAKDOWN", "OGN") ||
               ParameterSet["POP_TYPES"].Count < 9)
             { %>
          <td class="total-assisted force-vertical-dividers" title="Assisted by UNHCR">UNHCR-assisted:</td>
          <% } else { %>
          <td class="total-assisted force-vertical-dividers" title="Assisted by UNHCR">A</td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RF"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRFA_VALUE" runat="server" Text='<%# Eval("RFA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RL"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRLA_VALUE" runat="server" Text='<%# Eval("RLA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RFT"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRFTA_VALUE" runat="server" Text='<%# Eval("RFTA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "AS"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblASA_VALUE" runat="server" Text='<%# Eval("ASA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RT"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRTA_VALUE" runat="server" Text='<%# Eval("RTA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "ID"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblIDA_VALUE" runat="server" Text='<%# Eval("IDA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "IL"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblILA_VALUE" runat="server" Text='<%# Eval("ILA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "IDT"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblIDTA_VALUE" runat="server" Text='<%# Eval("IDTA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "RD"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblRDA_VALUE" runat="server" Text='<%# Eval("RDA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "ST"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblSTA_VALUE" runat="server" Text='<%# Eval("STA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "OC"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblOCA_VALUE" runat="server" Text='<%# Eval("OCA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
          <% if (ParameterSet.ContainsItem("POP_TYPES", "TPOC"))
              { %>
          <td class="numeric">
            <asp:Label ID="lblTPOCA_VALUE" runat="server" Text='<%# Eval("TPOCA_VALUE", "{0:N0}") %>' />
          </td>
          <% } %>
        </tr>
        <% } %>
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
  </div>

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="phS" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#lbtPOC").addClass("active");
    });
  </script>
</asp:Content>

