using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PSQRSDD : System.Web.UI.Page, IQueryParameters
{
  public QueryParameters ParameterSet { get; set; }

  private int[] pageSizes = { 10, 25, 50, 100, 250, 500, 0 };

  struct ColumnTemplate
  {
    public readonly string key;
    public readonly string item;
    public readonly bool isNumeric;
    public readonly string title;

    public ColumnTemplate(string key, string item, bool isNumeric, string title)
    {
      this.key = key;
      this.item = item;
      this.isNumeric = isNumeric;
      this.title = title;
    }
  }

  private static readonly IList<ColumnTemplate> columnTemplateArray = new ReadOnlyCollection<ColumnTemplate>
    (new[] {
      new ColumnTemplate("", "", true, "Year"),
      new ColumnTemplate("BREAKDOWN", "RES", false, "Country / territory of asylum"),
      new ColumnTemplate("BREAKDOWN", "OGN", false, "Origin / Returned from"),
      new ColumnTemplate("BREAKDOWN", "RSDP", false, "RSD procedure type code"),
      new ColumnTemplate("BREAKDOWN", "RSDP", false, "RSD procedure type description"),
      new ColumnTemplate("BREAKDOWN", "RSDL", false, "RSD procedure level code"),
      new ColumnTemplate("BREAKDOWN", "RSDL", false, "RSD procedure level description"),
      new ColumnTemplate("", "", true, "Total persons pending at start of year"),
      new ColumnTemplate("", "", true, "Persons assisted by UNHCR at start of year"),
      new ColumnTemplate("", "", true, "Persons applied during year"),
      new ColumnTemplate("", "", true, "Positive decisions (convention status)"),
      new ColumnTemplate("", "", true, "Positive decisions (Complementary protection status)"),
      new ColumnTemplate("", "", true, "Rejected"),
      new ColumnTemplate("", "", true, "Otherwise closed"),
      new ColumnTemplate("", "", true, "Total decisions"),
      new ColumnTemplate("", "", true, "Total persons pending at end of year"),
      new ColumnTemplate("", "", true, "Persons assisted by UNHCR at end of year"),
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
      if ((template.key == "" || ParameterSet.ContainsItem(template.key, template.item)))
      {
        csv.Append(sep + template.title);
        sep = ",";
      }
    }
    csv.AppendLine();

    ConstructSelectStatement();

    int i;
    foreach (DataRow row in ((DataView)dsASR_RSD.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
    {
      i = 0;
      sep = "";
      foreach (ColumnTemplate template in columnTemplateArray)
      {
        if ((template.key == "" || ParameterSet.ContainsItem(template.key, template.item)))
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
    Response.AddHeader("content-disposition", "attachment; filename=PSQRSD.csv");
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
    var selectStatement = new StringBuilder(4000);

    selectStatement.Append(@"
select to_char(ASR_YEAR) as ASR_YEAR,
  COU_NAME_ASYLUM, COU_NAME_ORIGIN,
  RSD_PROC_TYPE_CODE, RSD_PROC_TYPE_DESCRIPTION,
  RSD_PROC_LEVEL_CODE, RSD_PROC_LEVEL_DESCRIPTION,
  case
    when ASYPOP_START_VALUE is null and ASYPOP_START_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYPOP_START_VALUE, '999,999,999'))
  end as ASYPOP_START_VALUE,
  case
    when ASYPOP_AH_START_VALUE is null and ASYPOP_AH_START_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYPOP_AH_START_VALUE, '999,999,999'))
  end as ASYPOP_AH_START_VALUE,
  case
    when ASYAPP_VALUE is null and ASYAPP_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYAPP_VALUE, '999,999,999'))
  end as ASYAPP_VALUE,
  case
    when ASYREC_CV_VALUE is null and ASYREC_CV_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYREC_CV_VALUE, '999,999,999'))
  end as ASYREC_CV_VALUE,
  case
    when ASYREC_CP_VALUE is null and ASYREC_CP_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYREC_CP_VALUE, '999,999,999'))
  end as ASYREC_CP_VALUE,
  case
    when ASYREJ_VALUE is null and ASYREJ_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYREJ_VALUE, '999,999,999'))
  end as ASYREJ_VALUE,
  case
    when ASYOTHCL_VALUE is null and ASYOTHCL_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYOTHCL_VALUE, '999,999,999'))
  end as ASYOTHCL_VALUE,
  case
    when TOTAL_DECISIONS_VALUE is null and TOTAL_DECISIONS_REDACTED_FLAG = 1 then '*'
    else trim(to_char(TOTAL_DECISIONS_VALUE, '999,999,999'))
  end as TOTAL_DECISIONS_VALUE,
  case
    when ASYPOP_END_VALUE is null and ASYPOP_END_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYPOP_END_VALUE, '999,999,999'))
  end as ASYPOP_END_VALUE,
  case
    when ASYPOP_AH_END_VALUE is null and ASYPOP_AH_END_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASYPOP_AH_END_VALUE, '999,999,999'))
  end as ASYPOP_AH_END_VALUE
from (select ASR_YEAR, ");

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "RES",
      "COU_NAME_ASYLUM, ",
      "null as COU_NAME_ASYLUM, "));

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "OGN",
      "COU_NAME_ORIGIN, ",
      "null as COU_NAME_ORIGIN, "));

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "RSDP",
      "RSD_PROC_TYPE_CODE, RSD_PROC_TYPE_DESCRIPTION, ",
      "null as RSD_PROC_TYPE_CODE, null as RSD_PROC_TYPE_DESCRIPTION, "));

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "RSDL",
      "RSD_PROC_LEVEL_CODE, RSD_PROC_LEVEL_DESCRIPTION, ",
      "null as RSD_PROC_LEVEL_CODE, null as RSD_PROC_LEVEL_DESCRIPTION, "));

    selectStatement.Append(@"
    sum(ASYPOP_START_VALUE) as ASYPOP_START_VALUE,
    max(ASYPOP_START_REDACTED_FLAG) as ASYPOP_START_REDACTED_FLAG,
    sum(ASYPOP_AH_START_VALUE) as ASYPOP_AH_START_VALUE,
    max(ASYPOP_AH_START_REDACTED_FLAG) as ASYPOP_AH_START_REDACTED_FLAG,
    sum(ASYAPP_VALUE) as ASYAPP_VALUE,
    max(ASYAPP_REDACTED_FLAG) as ASYAPP_REDACTED_FLAG,
    sum(ASYREC_CV_VALUE) as ASYREC_CV_VALUE,
    max(ASYREC_CV_REDACTED_FLAG) as ASYREC_CV_REDACTED_FLAG,
    sum(ASYREC_CP_VALUE) as ASYREC_CP_VALUE,
    max(ASYREC_CP_REDACTED_FLAG) as ASYREC_CP_REDACTED_FLAG,
    sum(ASYREJ_VALUE) as ASYREJ_VALUE,
    max(ASYREJ_REDACTED_FLAG) as ASYREJ_REDACTED_FLAG,
    sum(ASYOTHCL_VALUE) as ASYOTHCL_VALUE,
    max(ASYOTHCL_REDACTED_FLAG) as ASYOTHCL_REDACTED_FLAG,
    case
      when coalesce(sum(ASYREC_CV_VALUE), sum(ASYREC_CP_VALUE),
                    sum(ASYREJ_VALUE), sum(ASYOTHCL_VALUE)) is not null
      then nvl(sum(ASYREC_CV_VALUE), 0) + nvl(sum(ASYREC_CP_VALUE), 0) +
        nvl(sum(ASYREJ_VALUE), 0) + nvl(sum(ASYOTHCL_VALUE), 0)
    end as TOTAL_DECISIONS_VALUE,
    coalesce(max(ASYREC_CV_REDACTED_FLAG), max(ASYREC_CP_REDACTED_FLAG),
             max(ASYREJ_REDACTED_FLAG), max(ASYOTHCL_REDACTED_FLAG))
      as TOTAL_DECISIONS_REDACTED_FLAG,
    sum(ASYPOP_END_VALUE) as ASYPOP_END_VALUE,
    max(ASYPOP_END_REDACTED_FLAG) as ASYPOP_END_REDACTED_FLAG,
    sum(ASYPOP_AH_END_VALUE) as ASYPOP_AH_END_VALUE,
    max(ASYPOP_AH_END_REDACTED_FLAG) as ASYPOP_AH_END_REDACTED_FLAG
  from PSQ_RSD ");

    string conjunction = "where ";

    if (ParameterSet.ContainsKey("YEAR"))
    {
      selectStatement.Append(conjunction + "ASR_YEAR in (" +
        ParameterSet.CommaSeparatedList("YEAR") + ") ");
      conjunction = "and ";
    }

    if (ParameterSet.ContainsKey("RES"))
    {
      selectStatement.Append(conjunction + "COU_CODE_ASYLUM in (" +
        ParameterSet.QuotedCommaSeparatedList("RES") + ") ");
    }

    if (ParameterSet.ContainsKey("OGN"))
    {
      selectStatement.Append(conjunction + "COU_CODE_ORIGIN in (" +
        ParameterSet.QuotedCommaSeparatedList("OGN") + ") ");
    }

    selectStatement.Append("group by ASR_YEAR");

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "RES",
      ", COU_NAME_ASYLUM", String.Empty));

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "OGN",
      ", COU_NAME_ORIGIN", String.Empty));

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "RSDP",
      ", RSD_PROC_TYPE_CODE, RSD_PROC_TYPE_DESCRIPTION", String.Empty));

    selectStatement.Append(ParameterSet.Alternative("BREAKDOWN", "RSDL",
      ", RSD_PROC_LEVEL_CODE, RSD_PROC_LEVEL_DESCRIPTION", String.Empty));

    selectStatement.Append(") order by ASR_YEAR desc, " +
      "nlssort(COU_NAME_ASYLUM, 'NLS_SORT=BINARY_AI'), " +
      "nlssort(decode(COU_NAME_ORIGIN, 'Various', '_', COU_NAME_ORIGIN), 'NLS_SORT=BINARY_AI'), " +
      "RSD_PROC_TYPE_CODE, RSD_PROC_LEVEL_CODE");

    dsASR_RSD.SelectCommand = selectStatement.ToString();

    //Label1.Text += selectStatement.ToString() + "<br />" + DateTime.Now;
  }

  private string GetCaption()
  {
    var caption = new StringBuilder("Asylum applications and refugee status determination");
    string conjunction = " for asylum seekers";

    int limit = (ParameterSet.ContainsKey("RES") ? ParameterSet["RES"].Count : 0) - 1;

    if (!ParameterSet.ContainsItem("BREAKDOWN", "RES") && limit >= 0 && limit < 5)
    {
      caption.Append(conjunction + " residing in ");
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

    if (!ParameterSet.ContainsItem("BREAKDOWN", "OGN") && limit >= 0 && limit < 5)
    {
      caption.Append(conjunction + " originating from ");
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
      dpgASR_RSD1.PageSize = 100000000;
      dpgASR_RSD2.PageSize = 100000000;
    }
    else
    {
      dpgASR_RSD1.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
      dpgASR_RSD2.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
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

  protected void lvwASR_RSD_DataBound(object sender, EventArgs e)
  {
    lblNoData.Visible = (dpgASR_RSD1.TotalRowCount == 0);
    lblPager.Visible = (dpgASR_RSD1.TotalRowCount > 0);
    btnCSV.Visible = (dpgASR_RSD1.TotalRowCount > 0);
    dpgASR_RSD2.Visible = (dpgASR_RSD2.TotalRowCount > dpgASR_RSD2.PageSize);

    var caption = (Label)(lvwASR_RSD.FindControl("capASR_RSD"));
    if (caption != null)
    {
      caption.Text = GetCaption();
    }
  }
}