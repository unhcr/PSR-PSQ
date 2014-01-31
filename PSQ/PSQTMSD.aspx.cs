using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public class ListViewTemplate : ITemplate
{
  protected QueryParameters parameters;

  public ListViewTemplate(QueryParameters parameterSet)
  {
    parameters = parameterSet;
  }

  public void InstantiateIn(Control container)
  {
    var lc = new Literal();
    lc.DataBinding += new EventHandler(DataBinding);
    container.Controls.Add(lc);
  }

  private void DataBinding(object sender, EventArgs e)
  {
    var lc = (Literal)sender;
    var container = (ListViewDataItem)lc.NamingContainer;
    lc.Text = "<tr>";
    if (parameters.ContainsItem("BREAKDOWN", "RES"))
    {
      lc.Text += "<td style=\"min-width: 100px;\">" + DataBinder.Eval(container.DataItem, "COU_NAME_RESIDENCE") + "</td>";
    }
    if (parameters.ContainsItem("BREAKDOWN", "OGN"))
    {
      lc.Text += "<td style=\"min-width: 100px;\">" + DataBinder.Eval(container.DataItem, "COU_NAME_ORIGIN") + "</td>";
    }
    if (parameters.ContainsItem("BREAKDOWN", "POPT"))
    {
      lc.Text += "<td class=\"population-type-short\">" +
        DataBinder.Eval(container.DataItem, "POP_TYPE_DESCRIPTION") + "</td>";
    }
    foreach (string year in parameters["YEAR"])
    {
      lc.Text += "<td class=\"numeric\">" +
        DataBinder.Eval(container.DataItem, "Y" + year + "_VALUE", "{0:N0}") + "</td>";
    }
    lc.Text += "</tr>";
  }
}

public partial class PSQTMSD : System.Web.UI.Page, IQueryParameters
{
  public QueryParameters ParameterSet { get; set; }

  private int[] pageSizes = { 10, 25, 50, 100, 250, 500, 0 };

  private void BuildCSV()
  {
    StringBuilder csv = new StringBuilder(10000);

    csv.AppendLine("\"Extracted from the UNHCR Population Statistics Reference Database, " +
      "United Nations High Commissioner for Refugees.\"");
    csv.AppendLine("Date extracted: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm K"));
    csv.AppendLine();
    csv.AppendLine('"' + GetCaption() + '"');
    csv.AppendLine();

    string sep = "";

    if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
    {
      csv.Append(sep + "Residing in");
      sep = ",";
    }
    if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
    {
      csv.Append(sep + "Originating / returned from");
      sep = ",";
    }
    if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
    {
      csv.Append(sep + "Population type");
      sep = ",";
    }
    foreach (string year in ParameterSet["YEAR"])
    {
      csv.Append(sep + year);
      sep = ",";
    }
    csv.AppendLine();

    ConstructSelectStatement();

    int i;
    foreach (DataRow row in ((DataView)dsASR_POC_DETAILS.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
    {
      i = 0;
      sep = "";

      if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
      {
        if (((String)(row.ItemArray[i])).Contains(","))
        {
          csv.Append(sep + "\"" + ((String)(row.ItemArray[i])).Replace("\"", "\"\"") + "\"");
        }
        else
        {
          csv.Append(sep + row.ItemArray[i]);
        }
        sep = ",";
        i++;
      }
      if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
      {
        if (((String)(row.ItemArray[i])).Contains(","))
        {
          csv.Append(sep + "\"" + ((String)(row.ItemArray[i])).Replace("\"", "\"\"") + "\"");
        }
        else
        {
          csv.Append(sep + row.ItemArray[i]);
        }
        sep = ",";
        i++;
      }
      if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
      {
        if (((String)(row.ItemArray[i])).Contains(","))
        {
          csv.Append(sep + "\"" + ((String)(row.ItemArray[i])).Replace("\"", "\"\"") + "\"");
        }
        else
        {
          csv.Append(sep + row.ItemArray[i]);
        }
        sep = ",";
        i++;
      }
      foreach (string year in ParameterSet["YEAR"])
      {
        csv.Append(sep);
        if (row.ItemArray[i].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[i])).Replace(",", ""));
        }
        sep = ",";
        i++;
      }
      csv.AppendLine();
    }

    Response.Clear();
    Response.AddHeader("content-disposition", "attachment; filename=PSQTMS.csv");
    Response.ContentType = "application/csv";
    Response.ContentEncoding = Encoding.UTF8;
    Response.BinaryWrite(Encoding.UTF8.GetPreamble());
    Response.Write(csv.ToString());
    Response.End();
  }

  private void SetDefaultParameters()
  {
    if (!ParameterSet.ContainsKey("PAGESIZEINDEX"))
    {
      ParameterSet.AddSet("PAGESIZEINDEX", new SortedSet<string>(new string[] { "1" }));
    }
  }

  private void ConstructSelectStatement()
  {
    var selectStatement = new StringBuilder("select ", 2000);
    string separator = "";

    if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
    {
      selectStatement.Append(separator + "COU_NAME_RESIDENCE");
      separator = ", ";
    }
    if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
    {
      selectStatement.Append(separator + "COU_NAME_ORIGIN");
      separator = ", ";
    }
    if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
    {
      selectStatement.Append(separator + "POP_TYPE_DESCRIPTION");
      separator = ", ";
    }
    
    foreach (string year in ParameterSet["YEAR"])
    {
      selectStatement.Append(separator + "case when Y" + year + "_VALUE is null and Y" + year +
        "_REDACTED_FLAG = 1 then '*' else trim(to_char(Y" + year +
        "_VALUE, '999,999,999')) end as Y" + year + "_VALUE");
      separator = ", ";
    }
    
    selectStatement.Append(" from (select COU_NAME_RESIDENCE, ORDER_SEQ_RESIDENCE, " +
      "COU_NAME_ORIGIN, ORDER_SEQ_ORIGIN, POP_TYPE_DESCRIPTION, POP_TYPE_SEQ, " +
      "ASR_YEAR, VALUE, REDACTED_FLAG from (select POC.ASR_YEAR");

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "RES",
        ParameterSet.Alternative("SUMRES", "COUNTRY",
          ", POC.COU_NAME_RESIDENCE" +
          ", decode(POC.ISO3166_ALPHA3_CODE_RESIDENCE, 'XXX', 2, 1) as ORDER_SEQ_RESIDENCE",
          ", nvl(NAM1.NAME, POC.COU_NAME_RESIDENCE) as COU_NAME_RESIDENCE, nvl(" +
          ParameterSet.Alternative("SUMRES", "UNSD_MGR",
            "POC.UNSD_MGR_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ParameterSet.Alternative("SUMRES", "UNSD_GSR",
            "POC.UNSD_GSR_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ParameterSet.Alternative("SUMRES", "UNHCR_BUR",
            "POC.UNHCR_BUR_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ParameterSet.Alternative("SUMRES", "UNHCR_ROP",
            "POC.UNHCR_ROP_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ", decode(POC.ISO3166_ALPHA3_CODE_RESIDENCE, 'XXX', 2e6, 1e6)) as ORDER_SEQ_RESIDENCE"),
        ", null as COU_NAME_RESIDENCE, null as ORDER_SEQ_RESIDENCE"));

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "OGN",
        ParameterSet.Alternative("SUMOGN", "COUNTRY",
          ", POC.COU_NAME_ORIGIN" +
          ", decode(POC.ISO3166_ALPHA3_CODE_ORIGIN, 'XXX', 2, 1) as ORDER_SEQ_ORIGIN",
          ", nvl(NAM2.NAME, POC.COU_NAME_ORIGIN) as COU_NAME_ORIGIN, nvl(" +
          ParameterSet.Alternative("SUMOGN", "UNSD_MGR",
            "POC.UNSD_MGR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNSD_GSR",
            "POC.UNSD_GSR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNHCR_BUR",
            "POC.UNHCR_BUR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNHCR_ROP",
            "POC.UNHCR_ROP_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ", decode(POC.ISO3166_ALPHA3_CODE_ORIGIN, 'XXX', 2e6, 1e6)) as ORDER_SEQ_ORIGIN"),
        ", null as COU_NAME_ORIGIN, null as ORDER_SEQ_ORIGIN"));

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "POPT",
        ", POC.POP_TYPE_DESCRIPTION, POC.POP_TYPE_SEQ",
        ", null as POP_TYPE_DESCRIPTION, null as POP_TYPE_SEQ"));

    selectStatement.Append(", POC.VALUE, POC.REDACTED_FLAG from PSQ_POC_DETAILS POC ");

    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNSD_MGR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNSD_MGR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNSD_GSR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNSD_GSR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNHCR_BUR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNHCR_BUR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNHCR_ROP",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNHCR_ROP_ITM_ID_RESIDENCE ",
        String.Empty));

    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNSD_MGR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNSD_MGR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNSD_GSR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNSD_GSR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNHCR_BUR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNHCR_BUR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNHCR_ROP",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNHCR_ROP_ITM_ID_ORIGIN ",
        String.Empty));

    separator = "where ";

    if (ParameterSet.ContainsKey("YEAR"))
    {
      selectStatement.Append(separator + "POC.ASR_YEAR in (" +
        ParameterSet.CommaSeparatedList("YEAR") + ") ");
      separator = "and ";
    }

    if (ParameterSet.ContainsKey("RES"))
    {
      selectStatement.Append(separator + "POC.COU_CODE_RESIDENCE in (" +
        ParameterSet.QuotedCommaSeparatedList("RES") + ") ");
    }

    if (ParameterSet.ContainsKey("OGN"))
    {
      selectStatement.Append(separator + "POC.COU_CODE_ORIGIN in (" +
        ParameterSet.QuotedCommaSeparatedList("OGN") + ") ");
    }

    if (ParameterSet.ContainsKey("POPT"))
    {
      selectStatement.Append(separator + "POC.POP_TYPE_CODE in (" +
        ParameterSet.QuotedCommaSeparatedList("POPT") + ") ");
    }

    selectStatement.Append(")) pivot (sum(VALUE) as VALUE, max(REDACTED_FLAG) as REDACTED_FLAG " +
      "for ASR_YEAR in (");
    separator = "";
    foreach (string year in ParameterSet["YEAR"])
    {
      selectStatement.Append(separator + year + " as Y" + year);
      separator = ", ";
    }
    selectStatement.Append("))");

    separator = " order by ";
    if (ParameterSet.ContainsItem("BREAKDOWN", "RES"))
    {
      selectStatement.Append(separator +
        "ORDER_SEQ_RESIDENCE, nlssort(COU_NAME_RESIDENCE, 'NLS_SORT=BINARY_AI')");
      separator = ", ";
    }
    if (ParameterSet.ContainsItem("BREAKDOWN", "OGN"))
    {
      selectStatement.Append(separator +
        "ORDER_SEQ_ORIGIN, nlssort(COU_NAME_ORIGIN, 'NLS_SORT=BINARY_AI')");
      separator = ", ";
    }
    if (ParameterSet.ContainsItem("BREAKDOWN", "POPT"))
    {
      selectStatement.Append(separator + "POP_TYPE_SEQ");
      separator = ", ";
    }

    dsASR_POC_DETAILS.SelectCommand = selectStatement.ToString();

    //Label1.Text += selectStatement.ToString() + "<br />" + DateTime.Now;
  }

  private string GetCaption()
  {
    var caption = new StringBuilder("Time Series – ");
    string conjunction = "";

    int limit = (ParameterSet.ContainsKey("POPT") ? ParameterSet["POPT"].Count : 0) - 1;

    if (! ParameterSet.ContainsItem("BREAKDOWN", "POPT") && limit >= 0 && limit < 8)
    {
      int i = 0;
      if (ParameterSet.ContainsItem("POPT", "RF"))
      {
        caption.Append(conjunction + "Refugees");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "RL"))
      {
        caption.Append(conjunction + (i == 0 ? "P" : "p") + "ersons in a refugee-like situation");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "AS"))
      {
        caption.Append(conjunction + (i == 0 ? "A" : "a") + "sylum-seekers");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "RT"))
      {
        caption.Append(conjunction + (i == 0 ? "R" : "r") + "eturned refugees");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "ID"))
      {
        caption.Append(conjunction + (i == 0 ? "I" : "i") + "nternally displaced persons");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "IL"))
      {
        caption.Append(conjunction + (i == 0 ? "P" : "p") + "ersons in an IDP-like situation");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "RD"))
      {
        caption.Append(conjunction + (i == 0 ? "R" : "r") + "eturned IDPs");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "ST"))
      {
        caption.Append(conjunction + (i == 0 ? "P" : "p") + "ersons under UNHCR's statelessness mandate");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("POPT", "OC"))
      {
        caption.Append(conjunction + (i == 0 ? "O" : "o") + "thers of concern to UNHCR");
        conjunction = (++i == limit) ? " and " : ", ";
      }
    }
    else
    {
      caption.Append("Persons of concern to UNHCR");
    }

    limit = (ParameterSet.ContainsKey("RES") ? ParameterSet["RES"].Count : 0) - 1;
    if (ParameterSet.ContainsKey("RESNAMES"))
    {
      limit = limit >= ParameterSet["RESNAMES"].Count ? ParameterSet["RESNAMES"].Count - 1 : limit;
    }
    else
    {
      limit = -1;
    }

    conjunction = "";

    if (! ParameterSet.ContainsItem("BREAKDOWN", "RES") && limit >= 0 && limit < 5)
    {
      caption.Append(" residing in ");
      for (int i = 0; i <= limit; i++)
      {
        if (i > 0)
        {
          if (i == limit)
          {
            caption.Append(" or ");
          }
          else
          {
            caption.Append(", ");
          }
        }
        caption.Append(ParameterSet["RESNAMES"].ElementAt<string>(i));
        conjunction = " and";
      }
    }

    limit = (ParameterSet.ContainsKey("OGN") ? ParameterSet["OGN"].Count : 0) - 1;
    if (ParameterSet.ContainsKey("OGNNAMES"))
    {
      limit = limit >= ParameterSet["OGNNAMES"].Count ? ParameterSet["OGNNAMES"].Count - 1 : limit;
    }
    else
    {
      limit = -1;
    }

    if (! ParameterSet.ContainsItem("BREAKDOWN", "OGN") && limit >= 0 && limit < 5)
    {
      caption.Append(conjunction);
      conjunction = "";

      if (ParameterSet.ContainsItem("POPT", "RF") ||
        ParameterSet.ContainsItem("POPT", "RL") ||
        ParameterSet.ContainsItem("POPT", "AS") ||
        ParameterSet.ContainsItem("POPT", "ID") ||
        ParameterSet.ContainsItem("POPT", "IL") ||
        ParameterSet.ContainsItem("POPT", "ST") ||
        ParameterSet.ContainsItem("POPT", "OC"))
      {
        caption.Append(" originating");
        conjunction = " or";
      }

      if (ParameterSet.ContainsItem("POPT", "RT") ||
        ParameterSet.ContainsItem("POPT", "RD"))
      {
        caption.Append(conjunction + " returning");
      }
      caption.Append(" from ");

      for (int i = 0; i <= limit; i++)
      {
        if (i > 0)
        {
          if (i == limit)
          {
            caption.Append(" or ");
          }
          else
          {
            caption.Append(", ");
          }
        }
        caption.Append(ParameterSet["OGNNAMES"].ElementAt<string>(i));
        conjunction = " and";
      }
    }

    return caption.ToString();
  }

  private void SetPageSize()
  {
    if (ddlPageRows.SelectedValue == "0")
    {
      // Switch off paging. Note that 966367641 is the largest page size accepted without misbehaviour of the DataPager.
      dpgASR_POC_DETAILS1.PageSize = 100000000;
      dpgASR_POC_DETAILS2.PageSize = 100000000;
    }
    else
    {
      dpgASR_POC_DETAILS1.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
      dpgASR_POC_DETAILS2.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
    }
  }
  
  protected void Page_Load(object sender, EventArgs e)
  {
    if (IsPostBack)
    {
      ParameterSet = new QueryParameters(ViewState);
    }
    else if (PreviousPage == null)
    {
      ParameterSet = new QueryParameters();
    }
    else
    {
      ParameterSet = (PreviousPage as IQueryParameters).ParameterSet;
    }

    SetDefaultParameters();

    //Label1.Text += ParameterSet.ToString();

    lvwASR_POC_DETAILS.ItemTemplate = new ListViewTemplate(ParameterSet);
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    ParameterSet.SaveToViewState(ViewState);

    ConstructSelectStatement();

    foreach (ListItem item in ddlPageRows.Items)
    {
      item.Value = pageSizes[ddlPageRows.Items.IndexOf(item)].ToString();
    }

    if (ParameterSet.ContainsKey("PAGESIZEINDEX"))
    {
      ddlPageRows.SelectedIndex = Convert.ToInt32(ParameterSet["PAGESIZEINDEX"].Max);
    }

    SetPageSize();
  }

  protected void ddlPageRows_SelectedIndexChanged(Object sender, EventArgs e)
  {
    SetPageSize();
    ParameterSet.AddSet("PAGESIZEINDEX", new SortedSet<string>(new string[] { ddlPageRows.SelectedIndex.ToString() }));
  }

  protected void btnCSV_Click(object sender, EventArgs e)
  {
    BuildCSV();
  }

  protected void lvwASR_POC_DETAILS_DataBound(object sender, EventArgs e)
  {
    lblNoData.Visible = (dpgASR_POC_DETAILS1.TotalRowCount == 0);
    lblPager.Visible = (dpgASR_POC_DETAILS1.TotalRowCount > 0);
    btnCSV.Visible = (dpgASR_POC_DETAILS1.TotalRowCount > 0);
    dpgASR_POC_DETAILS2.Visible = (dpgASR_POC_DETAILS2.TotalRowCount > dpgASR_POC_DETAILS2.PageSize);

    var caption = (Label)(lvwASR_POC_DETAILS.FindControl("capASR_POC_DETAILS"));
    if (caption != null)
    {
      caption.Text = GetCaption();
    }
  }
}