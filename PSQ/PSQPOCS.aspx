﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Persons of Concern Overview"
  CodeFile="PSQPOCS.aspx.cs" Inherits="PSQPOCS" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <asp:SqlDataSource ID="dsYears" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSQ %>" 
    ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>" />

  <asp:SqlDataSource ID="dsCountries" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSQ %>" 
    ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>" />

  <asp:SqlDataSource ID="dsOrigins" runat="server" 
    ConnectionString="<%$ ConnectionStrings:PSQ %>" 
    ProviderName="<%$ ConnectionStrings:PSQ.ProviderName %>" />

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
        <legend>Residing in</legend>
        <asp:RadioButtonList runat="server" ID="rblCS" RepeatDirection="Vertical" RepeatLayout="Flow"
           AutoPostBack="true" OnSelectedIndexChanged="rblCS_SelectedIndexChanged">
          <asp:ListItem Text="UN continental regions and sub-regions" Value="UNSD" Selected="True" />
          <asp:ListItem Text="UNHCR bureaux and regional operations" Value="UNHCR" />
          <asp:ListItem Text="Alphabetical list (search)" Value="LIST" />
        </asp:RadioButtonList>
        <asp:HiddenField runat="server" ID="hfC" Value="UNSD" />
        <asp:ListView runat="server" ID="lvC" DataSourceID="dsCountries" DataKeyNames="CODE,NAME,NODETYPE"
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
        <legend>Originating / returned from</legend>
        <asp:RadioButtonList runat="server" ID="rblOS" RepeatDirection="Vertical" RepeatLayout="Flow"
          AutoPostBack="true" OnSelectedIndexChanged="rblOS_SelectedIndexChanged">
          <asp:ListItem Text="UN continental regions and sub-regions" Value="UNSD" Selected="True" />
          <asp:ListItem Text="UNHCR bureaux and regional operations" Value="UNHCR" />
          <asp:ListItem Text="Alphabetical list (search)" Value="LIST" />
        </asp:RadioButtonList>
        <asp:HiddenField runat="server" ID="hfO" Value="UNSD" />
        <asp:ListView runat="server" ID="lvO" DataSourceID="dsOrigins" DataKeyNames="CODE,NAME,NODETYPE"
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
            <asp:ListItem Text="Residing in" Value="RES" />
            <asp:ListItem Text="Originating / returned from" Value="OGN" />
          </asp:CheckBoxList>
        </div>
        <div class="listbox-list">
          <h3>Summarise by:</h3>
          <label>Residing in
            <asp:DropDownList ID="ddlRS" runat="server" Rows="1" OnSelectedIndexChanged="ddlRS_SelectedIndexChanged">
              <asp:ListItem Text="Country / territory" Value="COUNTRY" />
              <asp:ListItem Text="Sub-region" Value="UNSD_GSR" />
              <asp:ListItem Text="Continent" Value="UNSD_MGR" />
              <asp:ListItem Text="UNHCR bureau" Value="UNHCR_BUR" />
              <asp:ListItem Text="UNHCR bureau / regional operation" Value="UNHCR_ROP" />
            </asp:DropDownList>
          </label>
          <label>Originating / returned from
            <asp:DropDownList ID="ddlOG" runat="server" Rows="1" OnSelectedIndexChanged="ddlOG_SelectedIndexChanged">
              <asp:ListItem Text="Country / territory" Value="COUNTRY" />
              <asp:ListItem Text="Sub-region" Value="UNSD_GSR" />
              <asp:ListItem Text="Continent" Value="UNSD_MGR" />
              <asp:ListItem Text="UNHCR bureau" Value="UNHCR_BUR" />
              <asp:ListItem Text="UNHCR bureau / regional operation" Value="UNHCR_ROP" />
            </asp:DropDownList>
          </label>
        </div>
        <div class="checkbox-list population-types">
          <h3>Include population types:</h3>
          <asp:CheckBoxList ID="cblPT" runat="server" OnSelectedIndexChanged="cblPT_SelectedIndexChanged">
            <asp:ListItem Text="Refugees" Value="RF" />
            <asp:ListItem Text="Persons in a refugee-like situation" Value="RL" />
            <asp:ListItem Text="Refugees (including refugee-like)" Value="RFT" />
            <asp:ListItem Text="Asylum-seekers" Value="AS" />
            <asp:ListItem Text="Returned refugees" Value="RT" />
            <asp:ListItem Text="Internally displaced persons" Value="ID" />
            <asp:ListItem Text="Persons in an IDP-like situation" Value="IL" />
            <asp:ListItem Text="IDPs (including IDP-like)" Value="IDT" />
            <asp:ListItem Text="Returned IDPs" Value="RD" />
            <asp:ListItem Text="Stateless persons" Value="ST" />
            <asp:ListItem Text="Others of concern" Value="OC" />
            <asp:ListItem Text="Total population" Value="TPOC" />
          </asp:CheckBoxList>
        </div>
        <div class="checkbox-list">
          <asp:CheckBoxList ID="cblIN" runat="server" OnSelectedIndexChanged="cblIN_SelectedIndexChanged">
            <asp:ListItem Text="Show UNHCR-assisted figures?" Value="HCRASS" />
          </asp:CheckBoxList>
        </div>
      </fieldset>

    </div> <!-- /.selection-container -->

    <div class="buttons">
      <asp:Button ID="btSb" runat="server" Text="Submit" PostBackUrl="PSQPOCD.aspx" />
    </div> <!-- /.buttons -->
  </div> <!-- /.selection -->

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