using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PSQDEMD : System.Web.UI.Page, IQueryParameters
{
  public QueryParameters ParameterSet { get; set; }

  private int[] largePageSizes = { 10, 25, 50, 100, 250, 500, 0 };
  private int[] smallPageSizes = { 5, 12, 25, 50, 100, 250, 0 };

  struct ColumnTemplate
  {
    public readonly string key1;
    public readonly string item1;
    public readonly string key2;
    public readonly string item2;
    public readonly bool isNumeric;
    public readonly string title;

    public ColumnTemplate(string key1, string item1, string key2, string item2, bool isNumeric, string title)
    {
      this.key1 = key1;
      this.item1 = item1;
      this.key2 = key2;
      this.item2 = item2;
      this.isNumeric = isNumeric;
      this.title = title;
    }
  }

  private static readonly IList<ColumnTemplate> columnTemplateArray = new ReadOnlyCollection<ColumnTemplate>
    (new[] {
      new ColumnTemplate("", "", "", "", true, "Year"),
      new ColumnTemplate("BREAKDOWN", "RES", "X", "X", false, "Residing in"),
      new ColumnTemplate("BREAKDOWN", "RES", "SUMRES", "LOCATION", false, "Location"),
      new ColumnTemplate("BREAKDOWN", "OGN", "X", "X", false, "Originating / returned from"),
      new ColumnTemplate("BREAKDOWN", "POPT", "X", "X", false, "Population type"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Female 0-4"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Female 5-11"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Female 12-17"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Female 18-59"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Female 60+"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Female unknown age"),
      new ColumnTemplate("SEXAGE", "AGE", "SEXAGE", "SEX", true, "Female total"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Male 0-4"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Male 5-11"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Male 12-17"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Male 18-59"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Male 60+"),
      new ColumnTemplate("SEXAGE", "AGE", "", "", true, "Male unknown age"),
      new ColumnTemplate("SEXAGE", "AGE", "SEXAGE", "SEX", true, "Male total"),
      new ColumnTemplate("", "", "", "", true, "Overall total")
    });

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
    foreach (ColumnTemplate template in columnTemplateArray)
    {
      if ((template.key1 == "" || ParameterSet.ContainsItem(template.key1, template.item1)) ||
        (template.key2 == "" || ParameterSet.ContainsItem(template.key2, template.item2)))
      {
        csv.Append(sep + template.title);
        sep = ",";
      }
    }
    csv.AppendLine();

    ConstructSelectStatement();

    int i;
    foreach (DataRow row in ((DataView)dsASR_POC_SUMMARY.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
    {
      i = 0;
      sep = "";
      foreach (ColumnTemplate template in columnTemplateArray)
      {
        if ((template.key1 == "" || ParameterSet.ContainsItem(template.key1, template.item1)) ||
          (template.key2 == "" || ParameterSet.ContainsItem(template.key2, template.item2)))
        {
          if (template.isNumeric)
          {
            csv.Append(sep);
            if (row.ItemArray[i].GetType() == typeof(string))
            {
              csv.Append(((String)(row.ItemArray[i])).Replace(",", ""));
            }
          }
          else
          {
            if (row.ItemArray[i].GetType() == typeof(string))
            {
              if (((String)(row.ItemArray[i])).Contains(","))
              {
                csv.Append(sep + "\"" + ((String)(row.ItemArray[i])).Replace("\"", "\"\"") + "\"");
              }
              else
              {
                csv.Append(sep + row.ItemArray[i]);
              }
            }
            else
            {
              csv.Append(sep);
            }
          }
          sep = ",";
        }
        i++;
      }
      csv.AppendLine();
    }

    Response.Clear();
    Response.AddHeader("content-disposition", "attachment; filename=PSQDEM.csv");
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
    var selectStatement = new StringBuilder(10000);

    selectStatement.Append(@"
select to_char(ASR_YEAR) as ASR_YEAR,
  COU_NAME_RESIDENCE,
  LOC_NAME_RESIDENCE,
  COU_NAME_ORIGIN,
  DST_DESCRIPTION,
  case
    when sum(F0_VALUE) is null and max(F0_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(F0_VALUE), '999,999,999'))
  end as F0_VALUE,
  case
    when sum(F5_VALUE) is null and max(F5_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(F5_VALUE), '999,999,999'))
  end as F5_VALUE,
  case
    when sum(F12_VALUE) is null and max(F12_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(F12_VALUE), '999,999,999'))
  end as F12_VALUE,
  case
    when sum(F18_VALUE) is null and max(F18_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(F18_VALUE), '999,999,999'))
  end as F18_VALUE,
  case
    when sum(F60_VALUE) is null and max(F60_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(F60_VALUE), '999,999,999'))
  end as F60_VALUE,
  case
    when sum(FOTHER_VALUE) is null and max(FOTHER_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(FOTHER_VALUE), '999,999,999'))
  end as FOTHER_VALUE,
  case
    when sum(FTOTAL_VALUE) is null and max(FTOTAL_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(FTOTAL_VALUE), '999,999,999'))
  end as FTOTAL_VALUE,
  case
    when sum(M0_VALUE) is null and max(M0_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(M0_VALUE), '999,999,999'))
  end as M0_VALUE,
  case
    when sum(M5_VALUE) is null and max(M0_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(M5_VALUE), '999,999,999'))
  end as M5_VALUE,
  case
    when sum(M12_VALUE) is null and max(M12_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(M12_VALUE), '999,999,999'))
  end as M12_VALUE,
  case
    when sum(M18_VALUE) is null and max(M18_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(M18_VALUE), '999,999,999'))
  end as M18_VALUE,
  case
    when sum(M60_VALUE) is null and max(M60_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(M60_VALUE), '999,999,999'))
  end as M60_VALUE,
  case
    when sum(MOTHER_VALUE) is null and max(MOTHER_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(MOTHER_VALUE), '999,999,999'))
  end as MOTHER_VALUE,
  case
    when sum(MTOTAL_VALUE) is null and max(MTOTAL_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(MTOTAL_VALUE), '999,999,999'))
  end as MTOTAL_VALUE,
  case
    when sum(TOTAL_VALUE) is null and max(TOTAL_REDACTED_FLAG) = 1 then '*'
    else trim(to_char(sum(TOTAL_VALUE), '999,999,999'))
  end as TOTAL_VALUE
from
 (select DEM.ASR_YEAR, ");

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "RES",
        ParameterSet.Alternative("SUMRES", "LOCATION",
          "DEM.COU_NAME_RESIDENCE, " +
          "decode(DEM.ISO3166_ALPHA3_CODE_RESIDENCE, 'XXX', 2, 1) as ORDER_SEQ_RESIDENCE, " +
          "DEM.LOC_NAME_RESIDENCE, DEM.LOC_ORDER_SEQ, ",
          ParameterSet.Alternative("SUMRES", "COUNTRY",
            "DEM.COU_NAME_RESIDENCE, " +
            "decode(DEM.ISO3166_ALPHA3_CODE_RESIDENCE, 'XXX', 2, 1) as ORDER_SEQ_RESIDENCE, ",
            "nvl(NAM1.NAME, DEM.COU_NAME_RESIDENCE) as COU_NAME_RESIDENCE, nvl(" +
            ParameterSet.Alternative("SUMRES", "UNSD_MGR",
              "DEM.UNSD_MGR_ORDER_SEQ_RESIDENCE",
              String.Empty) +
            ParameterSet.Alternative("SUMRES", "UNSD_GSR",
              "DEM.UNSD_GSR_ORDER_SEQ_RESIDENCE",
              String.Empty) +
            ParameterSet.Alternative("SUMRES", "UNHCR_BUR",
              "DEM.UNHCR_BUR_ORDER_SEQ_RESIDENCE",
              String.Empty) +
            ParameterSet.Alternative("SUMRES", "UNHCR_ROP",
              "DEM.UNHCR_ROP_ORDER_SEQ_RESIDENCE",
              String.Empty) +
            ", decode(DEM.ISO3166_ALPHA3_CODE_RESIDENCE, 'XXX', 2e6, 1e6)) as ORDER_SEQ_RESIDENCE, ") +
          "null as LOC_NAME_RESIDENCE, null as LOC_ORDER_SEQ, "),
        "null as COU_NAME_RESIDENCE, null as ORDER_SEQ_RESIDENCE, " +
        "null as LOC_NAME_RESIDENCE, null as LOC_ORDER_SEQ, "));

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "OGN",
        ParameterSet.Alternative("SUMOGN", "COUNTRY",
          "DEM.COU_NAME_ORIGIN, " +
          "decode(DEM.ISO3166_ALPHA3_CODE_ORIGIN, 'XXX', 2, 1) as ORDER_SEQ_ORIGIN, ",
          "nvl(NAM2.NAME, DEM.COU_NAME_ORIGIN) as COU_NAME_ORIGIN, nvl(" +
          ParameterSet.Alternative("SUMOGN", "UNSD_MGR",
            "DEM.UNSD_MGR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNSD_GSR",
            "DEM.UNSD_GSR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNHCR_BUR",
            "DEM.UNHCR_BUR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNHCR_ROP",
            "DEM.UNHCR_ROP_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ", decode(DEM.ISO3166_ALPHA3_CODE_ORIGIN, 'XXX', 2e6, 1e6)) as ORDER_SEQ_ORIGIN, "),
        "null as COU_NAME_ORIGIN, null as ORDER_SEQ_ORIGIN, "));

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "POPT",
        "DEM.DST_DESCRIPTION, DEM.DST_ORDER_SEQ, ",
        "null as DST_DESCRIPTION, null as DST_ORDER_SEQ, "));

    selectStatement.Append(
      ParameterSet.Alternative("SEXAGE", "AGE",
        "DEM.F0_VALUE, DEM.F0_REDACTED_FLAG, DEM.F5_VALUE, DEM.F5_REDACTED_FLAG, " +
        "DEM.F12_VALUE, DEM.F12_REDACTED_FLAG, DEM.F18_VALUE, DEM.F18_REDACTED_FLAG, " +
        "DEM.F60_VALUE, DEM.F60_REDACTED_FLAG, DEM.FOTHER_VALUE, DEM.FOTHER_REDACTED_FLAG, ",
        "null as F0_VALUE, null as F0_REDACTED_FLAG, null as F5_VALUE, null as F5_REDACTED_FLAG, " +
        "null as F12_VALUE, null as F12_REDACTED_FLAG, null as F18_VALUE, null as F18_REDACTED_FLAG, " +
        "null as F60_VALUE, null as F60_REDACTED_FLAG, null as FOTHER_VALUE, null as FOTHER_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("SEXAGE", "NONE",
        "null as FTOTAL_VALUE, null as FTOTAL_REDACTED_FLAG, ",
        "DEM.FTOTAL_VALUE, DEM.FTOTAL_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("SEXAGE", "AGE",
        "DEM.M0_VALUE, DEM.M0_REDACTED_FLAG, DEM.M5_VALUE, DEM.M5_REDACTED_FLAG, " +
        "DEM.M12_VALUE, DEM.M12_REDACTED_FLAG, DEM.M18_VALUE, DEM.M18_REDACTED_FLAG, " +
        "DEM.M60_VALUE, DEM.M60_REDACTED_FLAG, DEM.MOTHER_VALUE, DEM.MOTHER_REDACTED_FLAG, ",
        "null as M0_VALUE, null as M0_REDACTED_FLAG, null as M5_VALUE, null as M5_REDACTED_FLAG, " +
        "null as M12_VALUE, null as M12_REDACTED_FLAG, null as M18_VALUE, null as M18_REDACTED_FLAG, " +
        "null as M60_VALUE, null as M60_REDACTED_FLAG, null as MOTHER_VALUE, null as MOTHER_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("SEXAGE", "NONE",
        "null as MTOTAL_VALUE, null as MTOTAL_REDACTED_FLAG, ",
        "DEM.MTOTAL_VALUE, DEM.MTOTAL_REDACTED_FLAG, "));

    selectStatement.Append(
      "DEM.TOTAL_VALUE, DEM.TOTAL_REDACTED_FLAG from PSQ_DEMOGRAPHICS DEM ");

    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNSD_MGR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = DEM.UNSD_MGR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNSD_GSR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = DEM.UNSD_GSR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNHCR_BUR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = DEM.UNHCR_BUR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNHCR_ROP",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = DEM.UNHCR_ROP_ITM_ID_RESIDENCE ",
        String.Empty));

    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNSD_MGR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = DEM.UNSD_MGR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNSD_GSR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = DEM.UNSD_GSR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNHCR_BUR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = DEM.UNHCR_BUR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNHCR_ROP",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = DEM.UNHCR_ROP_ITM_ID_ORIGIN ",
        String.Empty));

    string conjunction = "where ";

    if (ParameterSet.ContainsKey("YEAR"))
    {
      selectStatement.Append(conjunction + "DEM.ASR_YEAR in (" +
        ParameterSet.CommaSeparatedList("YEAR") + ") ");
      conjunction = "and ";
    }

    if (ParameterSet.ContainsKey("RES"))
    {
      selectStatement.Append(conjunction + "DEM.COU_CODE_RESIDENCE in (" +
        ParameterSet.QuotedCommaSeparatedList("RES") + ") ");
    }

    if (ParameterSet.ContainsKey("OGN"))
    {
      selectStatement.Append(conjunction + "DEM.COU_CODE_ORIGIN in (" +
        ParameterSet.QuotedCommaSeparatedList("OGN") + ") ");
    }

    if (ParameterSet.ContainsKey("DST"))
    {
      selectStatement.Append(conjunction + "DEM.DST_CODE in (" +
        ParameterSet.QuotedCommaSeparatedList("DST"));
      if (ParameterSet.ContainsItem("DST", "REF") || ParameterSet.ContainsItem("DST", "ASY"))
      {
        selectStatement.Append(",'RAS'");
      }
      selectStatement.Append(") ");
    }

    selectStatement.Append(") group by ASR_YEAR, " +
      "COU_NAME_RESIDENCE, ORDER_SEQ_RESIDENCE, LOC_NAME_RESIDENCE, LOC_ORDER_SEQ, " +
      "COU_NAME_ORIGIN, ORDER_SEQ_ORIGIN, DST_DESCRIPTION, DST_ORDER_SEQ " +
      "order by ASR_YEAR desc, " +
      "ORDER_SEQ_RESIDENCE, nlssort(COU_NAME_RESIDENCE, 'NLS_SORT=BINARY_AI'), " +
      "LOC_ORDER_SEQ, nlssort(LOC_NAME_RESIDENCE, 'NLS_SORT=BINARY_AI'), " +
      "ORDER_SEQ_ORIGIN, nlssort(COU_NAME_ORIGIN, 'NLS_SORT=BINARY_AI'), " +
      "DST_ORDER_SEQ, DST_DESCRIPTION");

    dsASR_POC_SUMMARY.SelectCommand = selectStatement.ToString();

    //Label1.Text += selectStatement.ToString() + "<br />" + DateTime.Now;
  }

  private string GetCaption()
  {
    var caption = new StringBuilder("Demographic composition of populations of ");
    string conjunction = "";

    int limit = (ParameterSet.ContainsKey("DST") ? ParameterSet["DST"].Count : 0) - 1;

    if (!ParameterSet.ContainsItem("BREAKDOWN", "POPT") && limit >= 0 && limit < 8)
    {
      int i = 0;
      if (ParameterSet.ContainsItem("DST", "REF"))
      {
        caption.Append(conjunction + "refugees");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "ROC"))
      {
        caption.Append(conjunction + "persons in a refugee-like situation");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "ASY"))
      {
        caption.Append(conjunction + "asylum-seekers");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "RET"))
      {
        caption.Append(conjunction + "returned refugees");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "IDP"))
      {
        caption.Append(conjunction + "internally displaced persons");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "IOC"))
      {
        caption.Append(conjunction + "persons in an IDP-like situation");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "RDP"))
      {
        caption.Append(conjunction + "returned IDPs");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "STA"))
      {
        caption.Append(conjunction + "persons under UNHCR's statelessness mandate");
        conjunction = (++i == limit) ? " and " : ", ";
      }
      if (ParameterSet.ContainsItem("DST", "OOC") || ParameterSet.ContainsItem("DST", "VAR"))
      {
        caption.Append(conjunction + "others of concern to UNHCR");
        conjunction = (++i == limit) ? " and " : ", ";
      }
    }
    else
    {
      caption.Append("concern to UNHCR");
    }

    limit = (ParameterSet.ContainsKey("RES") ? ParameterSet["RES"].Count : 0) - 1;

    if (!ParameterSet.ContainsItem("BREAKDOWN", "RES") && limit >= 0 && limit < 5)
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

    limit = ParameterSet.ContainsKey("OGN") ? ParameterSet["OGN"].Count - 1 : -1;

    if (!ParameterSet.ContainsItem("BREAKDOWN", "OGN") && limit >= 0 && limit < 5)
    {
      caption.Append(conjunction);
      conjunction = "";

      if (ParameterSet.ContainsItem("DST", "REF") ||
        ParameterSet.ContainsItem("DST", "ROC") ||
        ParameterSet.ContainsItem("DST", "ASY") ||
        ParameterSet.ContainsItem("DST", "IDP") ||
        ParameterSet.ContainsItem("DST", "IOC") ||
        ParameterSet.ContainsItem("DST", "STA") ||
        ParameterSet.ContainsItem("DST", "OOC") ||
        ParameterSet.ContainsItem("DST", "VAR"))
      {
        caption.Append(" originating");
        conjunction = " or";
      }

      if (ParameterSet.ContainsItem("DST", "RET") ||
        ParameterSet.ContainsItem("DST", "RDP"))
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
      dpgASR_POC_SUMMARY1.PageSize = 100000000;
      dpgASR_POC_SUMMARY2.PageSize = 100000000;
    }
    else
    {
      dpgASR_POC_SUMMARY1.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
      dpgASR_POC_SUMMARY2.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
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

    ConstructSelectStatement();
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    ParameterSet.SaveToViewState(ViewState);

    if (ParameterSet.ContainsItem("SEXAGE", "AGE"))
    {
      foreach (ListItem item in ddlPageRows.Items)
      {
        item.Value = smallPageSizes[ddlPageRows.Items.IndexOf(item)].ToString();
      }
    }
    else
    {
      foreach (ListItem item in ddlPageRows.Items)
      {
        item.Value = largePageSizes[ddlPageRows.Items.IndexOf(item)].ToString();
      }
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

  protected void lvwASR_POC_SUMMARY_DataBound(object sender, EventArgs e)
  {
    lblNoData.Visible = (dpgASR_POC_SUMMARY1.TotalRowCount == 0);
    lblPager.Visible = (dpgASR_POC_SUMMARY1.TotalRowCount > 0);
    btnCSV.Visible = (dpgASR_POC_SUMMARY1.TotalRowCount > 0);
    dpgASR_POC_SUMMARY2.Visible = (dpgASR_POC_SUMMARY2.TotalRowCount > dpgASR_POC_SUMMARY2.PageSize);

    var caption = (Label)(lvwASR_POC_SUMMARY.FindControl("capASR_POC_SUMMARY"));
    if (caption != null)
    {
      caption.Text = GetCaption();
    }
  }
}