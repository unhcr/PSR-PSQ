<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Asylum Seekers Status Determination" 
  CodeFile="PSQ_RSD.aspx.cs" Inherits="PSQ_RSD" %>

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
    SelectCommand="select distinct ASR_YEAR from ASR_RSD_EN order by ASR_YEAR desc" />

  <asp:SqlDataSource ID="dsCOUNTRY" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct COU_CODE_ASYLUM, COU_NAME_ASYLUM_EN from ASR_RSD_EN order by nlssort(COU_NAME_ASYLUM_EN, 'NLS_SORT=BINARY_AI')" />

  <asp:SqlDataSource ID="dsORIGIN" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand="select distinct COU_CODE_ORIGIN, COU_NAME_ORIGIN_EN from ASR_RSD_EN order by nlssort(COU_NAME_ORIGIN_EN, 'NLS_SORT=BINARY_AI')" />

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
        <label>Country / territory of asylum</label>
        <asp:ListBox ID="lbxCOUNTRY" runat="server" ViewStateMode="Disabled" 
          DataSourceID="dsCOUNTRY" DataTextField="COU_NAME_ASYLUM_EN" DataValueField="COU_CODE_ASYLUM" 
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
        <label><asp:CheckBox ID="cbxRES" runat="server" ViewStateMode="Disabled" Checked="true" />Country / territory of asylum</label>
        <label><asp:CheckBox ID="cbxOGN" runat="server" ViewStateMode="Disabled" Checked="true" />Origin / Returned from</label>
        <label><asp:CheckBox ID="cbxRSDP" runat="server" ViewStateMode="Disabled" Checked="true" />RSD procedure type</label>
        <label><asp:CheckBox ID="cbxRSDL" runat="server" ViewStateMode="Disabled" Checked="true" />RSD level</label>
      </div>
    </fieldset>
    <div class="buttons">
      <asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" />
    </div>
  </div>

  <asp:SqlDataSource ID="dsASR_RSD" runat="server"
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
      <asp:Button ID="btnNewQuery" runat="server" Text="New Query" onclick="btnNewQuery_Click" />
    </div>
    <asp:ListView ID="lvwASR_RSD" runat="server" DataSourceID="dsASR_RSD" 
      ItemPlaceholderID="itemPlaceholder" OnDataBound="lvwASR_RSD_DataBound"> 
      <LayoutTemplate>
        <table class="standard-table">
          <caption>
            <asp:Label ID="capASR_RSD" runat="server" Text="Asylum applications and refugee status determination" />
          </caption>
          <colgroup id="Colgroup1" runat="server">
            <col class="year" />
            <% if (selectionCriteria.ShowRES)
               { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowOGN)
               { %>
            <col />
            <% } %>
            <% if (selectionCriteria.ShowRSDP || selectionCriteria.ShowRSDL)
               { %>
            <col style="width: 42px;" />
            <% } %>
            <col span="10" class="digits-7" />
          </colgroup>
          <thead runat="server">
            <tr>
              <th rowspan="2">Year</th>
              <% if (selectionCriteria.ShowRES) { %>
              <th rowspan="2" title="Country or territory of asylum">Country / territory of asylum</th>
              <% } %>
              <% if (selectionCriteria.ShowOGN) { %>
              <th rowspan="2" title="Country or territory of origin">Origin</th>
              <% } %>
              <% if (selectionCriteria.ShowRSDP || selectionCriteria.ShowRSDL)
                 { %>
              <th rowspan="2" class="numeric" title="Refugee status determination procedure type and level">RSD proc.</th>
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
          <% if (selectionCriteria.ShowRES)
             { %>
          <td>
            <asp:Label ID="lblCOU_NAME_ASYLUM_EN" runat="server" Text='<%# Eval("COU_NAME_ASYLUM_EN") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowOGN)
             { %>
          <td>
            <asp:Label ID="lblCOU_NAME_ORIGIN_EN" runat="server" Text='<%# Eval("COU_NAME_ORIGIN_EN") %>' />
          </td>
          <% } %>
          <% if (selectionCriteria.ShowRSDP)
             {
               if (selectionCriteria.ShowRSDL)
               { %>
          <td class="centred">
            <asp:Label ID="lblRSD_PROC_TYPE_CODE1" runat="server" Text='<%# Eval("RSD_PROC_TYPE_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_TYPE_DESCRIPTION_EN") %>' /> /
            <asp:Label ID="lblRSD_PROC_LEVEL_CODE1" runat="server" Text='<%# Eval("RSD_PROC_LEVEL_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_LEVEL_DESCRIPTION_EN") %>' />
          </td>
            <% }
               else
               { %>
          <td class="centred">
            <asp:Label ID="lblRSD_PROC_TYPE_CODE2" runat="server" Text='<%# Eval("RSD_PROC_TYPE_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_TYPE_DESCRIPTION_EN") %>' />
          </td>
            <% }
             }
             else
             {
               if (selectionCriteria.ShowRSDL)
               { %>
          <td class="centred">
            <asp:Label ID="lblRSD_PROC_LEVEL_CODE2" runat="server" Text='<%# Eval("RSD_PROC_LEVEL_CODE") %>'
              ToolTip='<%# Eval("RSD_PROC_LEVEL_DESCRIPTION_EN") %>' />
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
  </div> <!-- /.main-body -->

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#rsd").addClass("active");
    });
  </script>
</asp:Content>