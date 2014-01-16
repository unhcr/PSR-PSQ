<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Persons of Concern Overview"
  CodeFile="PSQPOCS.aspx.cs" Inherits="PSQPOCS" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <asp:Label ID="lb1" runat="server" />

  <asp:SqlDataSource ID="dsYears" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>" />

  <asp:SqlDataSource ID="dsCountries" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>" />

  <asp:SqlDataSource ID="dsOrigins" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSR %>" 
    ProviderName="<%$ ConnectionStrings:PSR.ProviderName %>" />

  <div class="selection">
    <div class="selection-container">

      <fieldset class="year-selection">
        <legend>Time period</legend>
        <asp:ListBox runat="server" ID="lbYL"
          DataSourceID="dsYears" DataTextField="ASR_YEAR" DataValueField="ASR_YEAR"
          SelectionMode="Multiple" CssClass="year year-list"
          OnDataBound="lbYL_DataBound" OnSelectedIndexChanged="lbYL_SelectedIndexChanged" />
      </fieldset>

      <fieldset class="country-selection">
        <legend>Country / territory of residence</legend>
        <asp:RadioButtonList runat="server" ID="rblCS" RepeatDirection="Horizontal" RepeatLayout="Flow"
          AutoPostBack="true" OnSelectedIndexChanged="rblCS_SelectedIndexChanged">
          <asp:ListItem Text="Continents" Value="UNSD" Selected="True" />
          <asp:ListItem Text="UNHCR regions" Value="UNHCR" />
          <asp:ListItem Text="List (search)" Value="LIST" />
        </asp:RadioButtonList>
        <asp:HiddenField runat="server" ID="hfC" Value="UNSD" />
        <asp:ListView runat="server" ID="lvC" DataSourceID="dsCountries" DataKeyNames="CODE"
          OnDataBound="lvC_DataBound">
          <LayoutTemplate>
            <div runat="server" id="dvSC" visible="false" class="search">
              <asp:TextBox runat="server" ID="tbSC" autocomplete="off" OnTextChanged="tbSC_TextChanged" />
              <img src="Images/clear.png" alt="Clear" />
            </div>
            <div class="selection-panel">
              <label runat="server" id="lbSC" visible="false"><input type="checkbox" class="select-all" />Select all</label>
              <asp:PlaceHolder runat="server" ID="itemPlaceHolder" />
            </div>
          </LayoutTemplate>
          <ItemTemplate>
            <%# Eval("PREFIX") %>
              <label class='<%# Eval("NODETYPE") %>'>
                <asp:CheckBox runat="server" ID="cbCT" Checked='<%# Convert.ToBoolean(Eval("EXPANDED")) %>'
                  OnCheckedChanged="cbCT_CheckedChanged" />
              </label>
              <label class="select">
                <asp:CheckBox runat="server" ID="cbCS"
                  OnCheckedChanged="cbCS_CheckedChanged" /><%# Eval("NAME") %>
              </label>
            <%# Eval("SUFFIX") %>
          </ItemTemplate>
        </asp:ListView>
      </fieldset>

      <fieldset class="country-selection">
        <legend>Origin / Returned from</legend>
        <asp:RadioButtonList runat="server" ID="rblOS" RepeatDirection="Horizontal" RepeatLayout="Flow"
          AutoPostBack="true" OnSelectedIndexChanged="rblOS_SelectedIndexChanged">
          <asp:ListItem Text="Continents" Value="UNSD" Selected="True" />
          <asp:ListItem Text="UNHCR regions" Value="UNHCR" />
          <asp:ListItem Text="List (search)" Value="LIST" />
        </asp:RadioButtonList>
        <asp:HiddenField runat="server" ID="hfO" Value="UNSD" />
        <asp:ListView runat="server" ID="lvO" DataSourceID="dsOrigins" DataKeyNames="CODE"
          OnDataBound="lvO_DataBound">
          <LayoutTemplate>
            <div runat="server" id="dvSO" visible="false" class="search">
              <asp:TextBox runat="server" ID="tbSO" autocomplete="off" OnTextChanged="tbSO_TextChanged" />
              <img src="Images/clear.png" alt="Clear" />
            </div>
            <div class="selection-panel">
              <label runat="server" id="lbSO" visible="false"><input type="checkbox" />Select all</label>
              <asp:PlaceHolder runat="server" ID="itemPlaceHolder" />
            </div>
          </LayoutTemplate>
          <ItemTemplate>
            <%# Eval("PREFIX") %>
              <label class='<%# Eval("NODETYPE") %>'>
                <asp:CheckBox runat="server" ID="cbOT" Checked='<%# Convert.ToBoolean(Eval("EXPANDED")) %>'
                  OnCheckedChanged="cbOT_CheckedChanged" />
              </label>
              <label class="select">
                <asp:CheckBox runat="server" ID="cbOS"
                  OnCheckedChanged="cbOS_CheckedChanged" /><%# Eval("NAME") %>
              </label>
            <%# Eval("SUFFIX") %>
          </ItemTemplate>
        </asp:ListView>
      </fieldset>

      <fieldset class="column-selection">
        <legend>Report format</legend>
        <div class="checkbox-list">
          <h3>Break down by:</h3>
          <asp:CheckBoxList ID="cblBD" runat="server" OnSelectedIndexChanged="cblBD_SelectedIndexChanged">
            <asp:ListItem Text="Country / territory of residence " Value="RES" />
            <asp:ListItem Text="Origin / Returned from" Value="OGN" />
          </asp:CheckBoxList>
        </div>
        <div class="checkbox-list">
          <h3>Included population types:</h3>
          <asp:CheckBoxList ID="cblPT" runat="server" OnSelectedIndexChanged="cblPT_SelectedIndexChanged">
            <asp:ListItem Text="Refugees" Value="REF" />
            <asp:ListItem Text="Asylum-seekers" Value="ASY" />
            <asp:ListItem Text="Returned refugees" Value="RET" />
            <asp:ListItem Text="Internally displaced persons" Value="IDP" />
            <asp:ListItem Text="Returned IDPs" Value="RDP" />
            <asp:ListItem Text="Stateless persons" Value="STA" />
            <asp:ListItem Text="Others of concern" Value="OOC" />
            <asp:ListItem Text="Total population" Value="POC" />
          </asp:CheckBoxList>
        </div>
      </fieldset>

    </div> <!-- /.selection-container -->

    <div class="buttons">
      <asp:Button ID="btSb" runat="server" Text="Submit" PostBackUrl="PSQPOCD.aspx" 
        onclick="btSb_Click" />
    </div> <!-- /.buttons -->
  </div> <!-- /.selection -->

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="phS" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#poc-overview").addClass("active");
    });
  </script>
</asp:Content>