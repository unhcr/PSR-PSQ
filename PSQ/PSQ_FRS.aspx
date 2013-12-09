<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics" CodeFile="PSQ_FRS.aspx.cs" Inherits="PSQ_FRS" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">

  <asp:SqlDataSource ID="dsCOUNTRY" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand=
      "select distinct COU_CODE_RESIDENCE, COU_NAME_RESIDENCE_EN
       from ASR_POC_SUMMARY_EN
       where COU_CODE_RESIDENCE not in ('PAK','IRN','DEU','KEN','SYR','ETH','TCD','JOR','TUR','YEM','BGD','SDN')
       order by nlssort(COU_NAME_RESIDENCE_EN, 'NLS_SORT=BINARY_AI')" />

  <asp:SqlDataSource ID="dsORIGIN" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>"
    SelectCommand=
      "select distinct COU_CODE_ORIGIN, COU_NAME_ORIGIN_EN
       from ASR_POC_SUMMARY_EN
       where COU_CODE_ORIGIN not in ('AFG','SOM','IRQ','SYR','SDN','COD','MMR','COL','ERI','CHN','SRB','SCG','YUG','MLI')
       order by nlssort(COU_NAME_ORIGIN_EN, 'NLS_SORT=BINARY_AI')" />

  <div class="main-body">
    <h1>Frequently Requested Statistics: Quick Queries</h1>
    <div class="frs">
      <div class="residing">
        <h2>Refugees by country / territory of asylum</h2>
        <ul>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=PAK&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Pakistan</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=IRN&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Islamic Republic of Iran</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=DEU&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Germany</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=KEN&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Kenya</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=SYR&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Syrian Arab Republic</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=ETH&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Ethiopia</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=TCD&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Chad</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=JOR&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Jordan</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=TUR&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Turkey</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=YEM&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Yemen</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=BGD&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Bangladesh</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;RES=SDN&amp;POPT=RF&amp;DRES=N&amp;DPOPT=N" target="_blank">Sudan</a></li>
          <li>
            <asp:DropDownList ID="lbxCOUNTRY" runat="server" ViewStateMode="Disabled" 
              DataSourceID="dsCOUNTRY" DataTextField="COU_NAME_RESIDENCE_EN" DataValueField="COU_CODE_RESIDENCE" 
              OnDataBound="lbxCOUNTRY_DataBound" CssClass="country" />
          </li>
        </ul>
      </div>
      <div class="origin">
        <h2>Refugees by origin</h2>
        <ul>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=AFG&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Afghanistan</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=SOM&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Somalia</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=IRQ&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Iraq</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=SYR&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Syrian Arab Republic</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=SDN&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Sudan</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=COD&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Democratic Republic of the Congo</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=MMR&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Myanmar</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=COL&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Colombia</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=ERI&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Eritrea</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=CHN&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">China</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=SRB,YUG,SCG&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Serbia (and Kosovo: S/RES/1244 (1999))</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;OGN=MLI&amp;POPT=RF&amp;DOGN=N&amp;DPOPT=N" target="_blank">Mali</a></li>
          <li>
            <asp:DropDownList ID="lbxORIGIN" runat="server" ViewStateMode="Disabled" 
              DataSourceID="dsORIGIN" DataTextField="COU_NAME_ORIGIN_EN" DataValueField="COU_CODE_ORIGIN" 
              OnDataBound="lbxORIGIN_DataBound" CssClass="origin" />
          </li>
        </ul>
      </div>
      <div class="others">
        <ul>
          <li><a href="PSQ_TMS.aspx?SYR=2000&amp;EYR=2012&amp;POPT=ID&amp;DOGN=N&amp;DPOPT=N" target="_blank">Internally displaced persons protected/assisted by UNHCR</a></li>
          <li><a href="PSQ_TMS.aspx?SYR=2004&amp;EYR=2012&amp;POPT=ST&amp;DOGN=N&amp;DPOPT=N" target="_blank">Persons falling under UNHCR's statelessness mandate</a></li>
        </ul>
      </div>
    </div>
  </div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#frs").addClass("active");
    });
  </script>
</asp:Content>