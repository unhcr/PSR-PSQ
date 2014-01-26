﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PSQ.master"
  Title="UNHCR Population Statistics" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="phB" Runat="Server">
  <div class="main-body introduction">
    <h1>UNHCR Population Statistics</h1>
    <div class="main-content">
      <p>
        Welcome to the UNHCR Population Statistics Database. The database currently contains data
        about UNHCR's populations of concern from the year 2000 and you can use it to investigate
        different aspects of these populations: their general composition by location of residence
        or origin, their status (refugees, asylum seekers, internally displaced persons, etc.),
        their evolution over time, and so on.
      </p>
      <p>
        In each of the screens in the system you start by selecting the sub-set of data you are
        interested in, choosing one or more countries or territories of residence and/or origin.
        You can focus on specific types of population by checking the boxes for only those you are
        concerned with, and you can summarise the data by checking the boxes for only those data
        items by which you wish the data to be broken down, and by selecting the type of
        geographical unit to summarise to.
      </p>
      <p>
        In the <asp:LinkButton runat="server" ID="lbtPOC" Text="Overview" PostBackUrl="PSQPOCS.aspx" />
        page, each row of data presents the information about UNHCR's populations of concern for a
        given year and country of residence and/or origin. Figures for the different types of
        population are presented across the page.
      </p>
      <p>
        The <asp:LinkButton runat="server" ID="lbtTMS" Text="Time Series" PostBackUrl="PSQTMSS.aspx" />
        page presents the same data set as the Overview page, but arranges the figures as a yearly
        time series across the page.
      </p>
      <p>
        The <asp:LinkButton runat="server" ID="lbtDEM" Text="Demographics" PostBackUrl="PSQDEMS.aspx" />
        page presents information about persons of concern broken down by sex and age, as well as by
        location within the country of residence (where such information is available). Note that
        data broken down in this way is not always available, so it may not be possible to reconcile
        the figures on this page with those on the Overview and Time Series pages.
      </p>
      <p>
        The <asp:LinkButton runat="server" ID="lbtRSD" Text="Asylum-seekers" PostBackUrl="PSQRSDS.aspx" />
        page presents information about asylum applications in a given year and the progress of
        asylum seekers through the refugee status determination process.
      </p>
      <p>
        The <asp:LinkButton runat="server" ID="lbtFRS" Text="Quick Queries" PostBackUrl="PSQFRS.aspx" />
        page contains a selection of links to data that are frequently requested, bypassing the need
        to enter specific query criteria. Note that these queries will open in a new page in your
        browser.
      </p>
      <h2>General notes</h2>
      <p>
        A number of statistics are not shown in this system but are displayed as asterisks (*).
        These represent situations where the figures are being kept confidential to protect the
        anonymity of persons of concern. Note that such figures are not included in any totals.
      </p>
      <p>
        On each page you have the option to download the data you have selected to a comma-separated
        variable (CSV) format file, from which you can import the data into a spreadsheet
        application such as Microsoft Excel and analyse it in greater depth.
      </p>
      <p>
        UNHCR's Field Information and Coordination Support Section is responsible for this system.
        If you have any questions or require further information, please email us at
        <a href="mailto:stats@unhcr.org">FICSS</a>. 
      </p>
    </div>
    <div class="poc-definitions">
      <h2>UNHCR's populations of concern</h2>
      <p>
        <em>Refugees</em> include individuals recognised under the 1951 Convention relating to the Status of
        Refugees; its 1967 Protocol; the 1969 OAU Convention Governing the Specific Aspects of Refugee Problems
        in Africa; those recognised in accordance with the UNHCR Statute; individuals granted complementary
        forms of protection; or those enjoying temporary protection. The refugee population also includes people
        in a refugee-like situation.
      </p>
      <p>
        <em>Asylum-seekers</em> are individuals who have sought international protection and whose claims for
        refugee status have not yet been determined, irrespective of when they may have been lodged.
      </p>
      <p>
        <em>Returned refugees</em> are former refugees who have returned to their country of origin spontaneously
        or in an organised fashion but are yet to be fully integrated. Such return would normally only take
        place in conditions of safety and dignity.
      </p>
      <p>
        <em>Internally displaced persons</em> (IDPs) are people or groups of individuals who have been forced
        to leave their homes or places of habitual residence, in particular as a result of, or in order to avoid
        the effects of armed conflict, situations of generalised violence, violations of human rights, or natural
        or man-made disasters, and who have not crossed an international border. For the purposes of UNHCR's
        statistics, this population only includes conflict-generated IDPs to whom the Office extends protection
        and/or assistance. The IDP population also includes people in an IDP-like situation.
      </p>
      <p>
        <em>Returned IDPs</em> refer to those IDPs who were beneficiaries of UNHCR's protection and assistance
        activities and who returned to their areas of origin or habitual residence during the year.
      </p>
      <p>
        <em>Stateless persons</em> are defined under international law as persons who are not considered as
        nationals by any State under the operation of its law. In other words, they do not possess the nationality
        of any State. UNHCR statistics refer to persons who fall under the agency’s statelessness mandate because
        they are stateless according to this international definition, but data from some countries may also
        include persons with undetermined nationality.
      </p>
      <p>
        <em>Others of concern</em> refers to individuals who do not necessarily fall directly into any of the
        groups above, but to whom UNHCR extends its protection and/or assistance services, based on humanitarian
        or other special grounds.
      </p>
    </div>
  </div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="phS" Runat="Server">
  <script type="text/javascript" src="Scripts/PSQ.js"></script>
</asp:Content>