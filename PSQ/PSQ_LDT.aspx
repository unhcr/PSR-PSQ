<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master"
  Title="UNHCR Population Statistics"CodeFile="PSQ_LDT.aspx.cs" Inherits="PSQ_LDT" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
  <div class="main-body introduction">
    <h1>Latest Data (Mid-2013 Statistics)</h1>
    <div class="recent-data">
      <p>
        The latest data in the Population Statistics Database represents UNHCR's best estimate
        of the situation at the end of 2012. For some countries and populations, further data has
        been collected during 2013 and, while this has not yet been incorporated into the
        online database, we are pleased to make it available in the form of
        <a href="http://www.unhcr.org/statistics/mid2013stats.zip">zipped Excel tables</a>.
      </p>
    </div>
  </div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      "use strict";
      $("#latest-data").addClass("active");
    });
  </script>
</asp:Content>