<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Quick Queries" CodeFile="PSQFRS.aspx.cs" Inherits="PSQFRS" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="phH" Runat="Server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <asp:SqlDataSource ID="dsYears" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSQ %>" 
    ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>"
    SelectCommand=
      "select ASR_YEAR
       from PSQ_POC_YEARS
       where ASR_YEAR >
        (select max(ASR_YEAR) - 12
         from PSQ_POC_YEARS)
       order by ASR_YEAR" />

  <asp:ListBox runat="server" ID="lbYL" Visible="false"
    DataSourceID="dsYears" DataTextField="ASR_YEAR" DataValueField="ASR_YEAR" />

  <asp:SqlDataSource ID="dsCOUNTRY" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSQ %>" 
    ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>"
    SelectCommand=
      "select CODE, NAME
       from PSQ_COUNTRY_SELECTION
       where CODE in (select COU_CODE from PSQ_POC_COUNTRIES)
       order by SORT_NAME" />

  <asp:SqlDataSource ID="dsORIGIN" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSQ %>" 
    ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>"
    SelectCommand=
      "select CODE, NAME
       from PSQ_ORIGIN_SELECTION
       where CODE in (select COU_CODE from PSQ_POC_ORIGINS)
       order by SORT_NAME" />

  <div class="main-body">
    <div class="frs">
      <h1>Frequently Requested Statistics: Quick Queries</h1>
      <div>
        <div class="residing">
          <h2>Refugees by country / territory of asylum</h2>
          <ul>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_PAK" CommandArgument="586" Text="Pakistan"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_IRN" CommandArgument="364" Text="Islamic Republic of Iran"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_DEU" CommandArgument="276" Text="Germany"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_KEN" CommandArgument="404" Text="Kenya"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_SYR" CommandArgument="760" Text="Syrian Arab Republic"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_ETH" CommandArgument="231" Text="Ethiopia"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_TCD" CommandArgument="148" Text="Chad"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_JOR" CommandArgument="400" Text="Jordan"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_TUR" CommandArgument="792" Text="Turkey"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_YRM" CommandArgument="887" Text="Yemen"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_BGD" CommandArgument="050" Text="Bangladesh"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtRES_SDN" CommandArgument="729,736" Text="Sudan"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeAsylum_Command" />
            </li>
            <li>
              <asp:DropDownList ID="lbxCOUNTRY" runat="server" ViewStateMode="Disabled"
                DataSourceID="dsCOUNTRY" DataTextField="NAME" DataValueField="CODE" 
                OnDataBound="lbxCOUNTRY_DataBound" OnSelectedIndexChanged="lbxCOUNTRY_SelectedIndexChanged"
                AutoPostBack="true" CssClass="country" />
            </li>
          </ul>
        </div>
        <div class="origin">
          <h2>Refugees by origin</h2>
          <ul>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_AFG" CommandArgument="004" Text="Afghanistan"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_SOM" CommandArgument="706" Text="Somalia"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_IRQ" CommandArgument="368" Text="Iraq"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_SYR" CommandArgument="760" Text="Syrian Arab Republic"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_SDN" CommandArgument="729,736" Text="Sudan"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_COD" CommandArgument="180" Text="Democratic Republic of the Congo"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_MMR" CommandArgument="104" Text="Myanmar"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_COL" CommandArgument="170" Text="Colombia"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_ERI" CommandArgument="232" Text="Eritrea"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_CHN" CommandArgument="156" Text="China"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_SRB" CommandArgument="688,891" Text="Serbia (and Kosovo: S/RES/1244 (1999))"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtOGN_MLI" CommandArgument="466" Text="Mali"
                PostBackUrl="PSQTMSD.aspx" OnCommand="RefugeeOrigin_Command" />
            </li>
            <li>
              <asp:DropDownList ID="lbxORIGIN" runat="server" ViewStateMode="Disabled" 
                DataSourceID="dsORIGIN" DataTextField="NAME" DataValueField="CODE" 
                OnDataBound="lbxORIGIN_DataBound" OnSelectedIndexChanged="lbxORIGIN_SelectedIndexChanged"
                AutoPostBack="true" CssClass="origin" />
            </li>
          </ul>
        </div>
        <div class="others">
          <ul>
            <li>
              <asp:LinkButton runat="server" ID="lbtIDP" Text="Internally displaced persons protected/assisted by UNHCR"
                PostBackUrl="PSQTMSD.aspx" OnClick="lbtIDP_Click" />
            </li>
            <li>
              <asp:LinkButton runat="server" ID="lbtSTA" Text="Persons falling under UNHCR's statelessness mandate"
                PostBackUrl="PSQTMSD.aspx" OnClick="lbtSTA_Click" />
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="phS" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#lbtFRS").addClass("active");
    });
  </script>
</asp:Content>