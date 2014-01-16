<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master" 
  Title="UNHCR Population Statistics - Persons of Concern Overview" 
  CodeFile="PSQPOCD.aspx.cs" Inherits="PSQPOCD" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">

  <div ID="divMainBody" runat="server" class="results">

    <asp:Label ID="Label1" runat="server" /><br />

    <div class="top-pager">
      <asp:Button ID="btnNewQuery" runat="server" Text="New Query" PostBackUrl="PSQPOCS.aspx" />
    </div>

  </div>

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

