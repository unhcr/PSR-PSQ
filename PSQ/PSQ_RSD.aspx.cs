using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[Serializable]
public class SelectionCriteriaRSD
{
  public string StartYear { get; set; }
  public string EndYear { get; set; }

  List<string> residenceCodes = new List<string>();
  public List<string> ResidenceCodes
  {
    get { return residenceCodes; }
    set { residenceCodes = value; }
  }

  List<string> originCodes = new List<string>();
  public List<string> OriginCodes
  {
    get { return originCodes; }
    set { originCodes = value; }
  }

  public bool ShowRES { get; set; }
  public bool ShowOGN { get; set; }
  public bool ShowRSDP { get; set; }
  public bool ShowRSDL { get; set; }

  static Regex countryCodePattern = new Regex("^[A-Z]{3}$");  // Regular expression to validate ISO country codes

  public SelectionCriteriaRSD()
  {
    StartYear = "1950";
    EndYear = "9999";
    ResidenceCodes = new List<string>();
    OriginCodes = new List<string>();
    ShowRES = true;
    ShowOGN = true;
    ShowRSDP = true;
    ShowRSDL = true;
  }

  public SelectionCriteriaRSD(string startYear, string endYear,
    ListItemCollection residenceCodes, ListItemCollection originCodes,
    bool showRES, bool showOGN, bool showRSDP, bool showRSDL)
  {
    StartYear = startYear;
    EndYear = endYear;
    foreach (ListItem item in residenceCodes)
    {
      if (item.Selected && countryCodePattern.IsMatch(item.Value))
      {
        this.AddResidenceCode(item.Value);
      }
    }
    foreach (ListItem item in originCodes)
    {
      if (item.Selected && countryCodePattern.IsMatch(item.Value))
      {
        this.AddOriginCode(item.Value);
      }
    }
    ShowRES = showRES;
    ShowOGN = showOGN;
    ShowRSDP = showRSDP;
    ShowRSDL = showRSDL;
  }

  public void AddResidenceCode(string code)
  {
    if (countryCodePattern.IsMatch(code))
    {
      residenceCodes.Add(code);
    }
  }

  public void AddOriginCode(string code)
  {
    if (countryCodePattern.IsMatch(code))
    {
      originCodes.Add(code);
    }
  }
}

public partial class PSQ_RSD : System.Web.UI.Page
{
  protected SelectionCriteriaRSD selectionCriteria = new SelectionCriteriaRSD();

  protected bool selectionMode = false;

  void SaveToViewState()
  {
    ViewState["SelectionCriteria"] = selectionCriteria;
  }

  void RestoreFromViewState()
  {
    selectionCriteria = (SelectionCriteriaRSD)ViewState["SelectionCriteria"];
  }

  void UnpackQueryString()
  {
    // Extract selection criteria parameters from query string.
    if (Request.QueryString["SYR"] != null)
    {
      selectionCriteria.StartYear = Request.QueryString["SYR"];
    }
    if (Request.QueryString["EYR"] != null)
    {
      selectionCriteria.EndYear = Request.QueryString["EYR"];
    }

    if (Request.QueryString["RES"] != null)
    {
      foreach (string code in Request.QueryString["RES"].ToUpper().Split(',').Distinct())
      {
        selectionCriteria.AddResidenceCode(code);
      }
    }
    if (Request.QueryString["OGN"] != null)
    {
      foreach (string code in Request.QueryString["OGN"].ToUpper().Split(',').Distinct())
      {
        selectionCriteria.AddOriginCode(code);
      }
    }

    // Extract column display parameters from query string.
    if (Request.QueryString["DRES"] != null)
    {
      selectionCriteria.ShowRES = (Request.QueryString["DRES"].ToUpper() != "N");
    }
    if (Request.QueryString["DOGN"] != null)
    {
      selectionCriteria.ShowOGN = (Request.QueryString["DOGN"].ToUpper() != "N");
    }
    if (Request.QueryString["DRSDP"] != null)
    {
      selectionCriteria.ShowRSDP = (Request.QueryString["DRSDP"].ToUpper() != "N");
    }
    if (Request.QueryString["DRSDL"] != null)
    {
      selectionCriteria.ShowRSDL = (Request.QueryString["RSDL"].ToUpper() != "N");
    }
  }

  SelectionCriteriaRSD GetSelectionDialog()
  {
    return new SelectionCriteriaRSD(ddlSTART_YEAR.Text, ddlEND_YEAR.Text,
      lbxCOUNTRY.Items, lbxORIGIN.Items,
      cbxRES.Checked, cbxOGN.Checked, cbxRSDP.Checked, cbxRSDL.Checked);
  }

  void SetSelectionDialog()
  {
    ddlSTART_YEAR.SelectedValue = selectionCriteria.StartYear;
    ddlEND_YEAR.SelectedValue = selectionCriteria.EndYear;
    cbxRES.Checked = selectionCriteria.ShowRES;
    cbxOGN.Checked = selectionCriteria.ShowOGN;
    cbxRSDP.Checked = selectionCriteria.ShowRSDP;
    cbxRSDL.Checked = selectionCriteria.ShowRSDL;
  }

  void ConstructSelectStatement()
  {
    var selectStatement =
      new StringBuilder("select ASR_YEAR, COU_NAME_ASYLUM_EN, COU_NAME_ORIGIN_EN, " +
        "RSD_PROC_TYPE_CODE, RSD_PROC_TYPE_DESCRIPTION_EN, RSD_PROC_LEVEL_CODE, RSD_PROC_LEVEL_DESCRIPTION_EN, " +
        "case when ASYPOP_START_VALUE is null and ASYPOP_START_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYPOP_START_VALUE, '999,999,999')) end as ASYPOP_START_VALUE, " +
        "case when ASYPOP_AH_START_VALUE is null and ASYPOP_AH_START_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYPOP_AH_START_VALUE, '999,999,999')) end as ASYPOP_AH_START_VALUE, " +
        "case when ASYAPP_VALUE is null and ASYAPP_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYAPP_VALUE, '999,999,999')) end as ASYAPP_VALUE, " +
        "case when ASYREC_CV_VALUE is null and ASYREC_CV_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYREC_CV_VALUE, '999,999,999')) end as ASYREC_CV_VALUE, " +
        "case when ASYREC_CP_VALUE is null and ASYREC_CP_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYREC_CP_VALUE, '999,999,999')) end as ASYREC_CP_VALUE, " +
        "case when ASYREJ_VALUE is null and ASYREJ_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYREJ_VALUE, '999,999,999')) end as ASYREJ_VALUE, " +
        "case when ASYOTHCL_VALUE is null and ASYOTHCL_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYOTHCL_VALUE, '999,999,999')) end as ASYOTHCL_VALUE, " +
        "case when TOTAL_DECISIONS_VALUE is null and TOTAL_DECISIONS_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(TOTAL_DECISIONS_VALUE, '999,999,999')) end as TOTAL_DECISIONS_VALUE, " +
        "case when ASYPOP_END_VALUE is null and ASYPOP_END_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYPOP_END_VALUE, '999,999,999')) end as ASYPOP_END_VALUE, " +
        "case when ASYPOP_AH_END_VALUE is null and ASYPOP_AH_END_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYPOP_AH_END_VALUE, '999,999,999')) end as ASYPOP_AH_END_VALUE " +
        "from (select ASR_YEAR, ", 4000);

    selectStatement.Append((selectionCriteria.ShowRES ? String.Empty : "null as ") + "COU_NAME_ASYLUM_EN, ");
    selectStatement.Append((selectionCriteria.ShowOGN ? String.Empty : "null as ") + "COU_NAME_ORIGIN_EN, ");
    selectStatement.Append((selectionCriteria.ShowRSDP ? String.Empty : "null as ") + "RSD_PROC_TYPE_CODE, ");
    selectStatement.Append((selectionCriteria.ShowRSDP ? String.Empty : "null as ") + "RSD_PROC_TYPE_DESCRIPTION_EN, ");
    selectStatement.Append((selectionCriteria.ShowRSDL ? String.Empty : "null as ") + "RSD_PROC_LEVEL_CODE, ");
    selectStatement.Append((selectionCriteria.ShowRSDL ? String.Empty : "null as ") + "RSD_PROC_LEVEL_DESCRIPTION_EN, ");
    selectStatement.Append("sum(ASYPOP_START_VALUE) as ASYPOP_START_VALUE, max(ASYPOP_START_REDACTED_FLAG) as ASYPOP_START_REDACTED_FLAG, " +
      "sum(ASYPOP_AH_START_VALUE) as ASYPOP_AH_START_VALUE, max(ASYPOP_AH_START_REDACTED_FLAG) as ASYPOP_AH_START_REDACTED_FLAG, " +
      "sum(ASYAPP_VALUE) as ASYAPP_VALUE, max(ASYAPP_REDACTED_FLAG) as ASYAPP_REDACTED_FLAG, " +
      "sum(ASYREC_CV_VALUE) as ASYREC_CV_VALUE, max(ASYREC_CV_REDACTED_FLAG) as ASYREC_CV_REDACTED_FLAG, " +
      "sum(ASYREC_CP_VALUE) as ASYREC_CP_VALUE, max(ASYREC_CP_REDACTED_FLAG) as ASYREC_CP_REDACTED_FLAG, " +
      "sum(ASYREJ_VALUE) as ASYREJ_VALUE, max(ASYREJ_REDACTED_FLAG) as ASYREJ_REDACTED_FLAG, " +
      "sum(ASYOTHCL_VALUE) as ASYOTHCL_VALUE, max(ASYOTHCL_REDACTED_FLAG) as ASYOTHCL_REDACTED_FLAG, " +
      "case when coalesce(sum(ASYREC_CV_VALUE), sum(ASYREC_CP_VALUE), sum(ASYREJ_VALUE), sum(ASYOTHCL_VALUE)) is not null " +
      "then nvl(sum(ASYREC_CV_VALUE), 0) + nvl(sum(ASYREC_CP_VALUE), 0) + nvl(sum(ASYREJ_VALUE), 0) + nvl(sum(ASYOTHCL_VALUE), 0) " +
      "end as TOTAL_DECISIONS_VALUE, " +
      "coalesce(max(ASYREC_CV_REDACTED_FLAG), max(ASYREC_CP_REDACTED_FLAG), max(ASYREJ_REDACTED_FLAG), max(ASYOTHCL_REDACTED_FLAG)) as TOTAL_DECISIONS_REDACTED_FLAG, " +
      "sum(ASYPOP_END_VALUE) as ASYPOP_END_VALUE, max(ASYPOP_END_REDACTED_FLAG) as ASYPOP_END_REDACTED_FLAG, " +
      "sum(ASYPOP_AH_END_VALUE) as ASYPOP_AH_END_VALUE, max(ASYPOP_AH_END_REDACTED_FLAG) as ASYPOP_AH_END_REDACTED_FLAG " +
      "from ASR_RSD_EN where ASR_YEAR between :START_YEAR and :END_YEAR ");
    if (selectionCriteria.ResidenceCodes != null && selectionCriteria.ResidenceCodes.Count > 0)
    {
      selectStatement.Append("and COU_CODE_ASYLUM in ('");
      foreach (string code in selectionCriteria.ResidenceCodes)
      {
        selectStatement.Append("','" + code);
      }
      selectStatement.Append("') ");
    }
    if (selectionCriteria.OriginCodes != null && selectionCriteria.OriginCodes.Count > 0)
    {
      selectStatement.Append("and COU_CODE_ORIGIN in ('");
      foreach (string code in selectionCriteria.OriginCodes)
      {
        selectStatement.Append("','" + code);
      }
      selectStatement.Append("') ");
    }
    selectStatement.Append("group by ASR_YEAR");
    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(", COU_NAME_ASYLUM_EN");
    }
    if (selectionCriteria.ShowOGN)
    {
      selectStatement.Append(", COU_NAME_ORIGIN_EN");
    }
    if (selectionCriteria.ShowRSDP)
    {
      selectStatement.Append(", RSD_PROC_TYPE_CODE, RSD_PROC_TYPE_DESCRIPTION_EN");
    }
    if (selectionCriteria.ShowRSDL)
    {
      selectStatement.Append(", RSD_PROC_LEVEL_CODE, RSD_PROC_LEVEL_DESCRIPTION_EN");
    }
    selectStatement.Append(") order by ASR_YEAR desc");
    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(", nlssort(COU_NAME_ASYLUM_EN, 'NLS_SORT=BINARY_AI')");
    }
    if (selectionCriteria.ShowOGN)
    {
      selectStatement.Append(", nlssort(decode(COU_NAME_ORIGIN_EN, 'Various', '_', COU_NAME_ORIGIN_EN), 'NLS_SORT=BINARY_AI')");
    }
    if (selectionCriteria.ShowRSDP)
    {
      selectStatement.Append(", RSD_PROC_TYPE_CODE");
    }
    if (selectionCriteria.ShowRSDL)
    {
      selectStatement.Append(", RSD_PROC_LEVEL_CODE");
    }

    dsASR_RSD.SelectCommand = selectStatement.ToString();

    foreach (Parameter param in dsASR_RSD.SelectParameters)
    {
      switch (param.Name)
      {
        case "START_YEAR":
          param.DefaultValue = selectionCriteria.StartYear;
          break;
        case "END_YEAR":
          param.DefaultValue = selectionCriteria.EndYear;
          break;
      }
    }

    //Label1.Text = selectStatement.ToString() + "<br />" +
    //  selectionCriteria.StartYear + " / " + selectionCriteria.EndYear + "<br />" +
    //  DateTime.Now;
  }

  string GetCaption()
  {
    var caption = new StringBuilder("Asylum applications and refugee status determination");
    string conjunction = " for asylum seekers";
    int limit = selectionCriteria.ResidenceCodes.Count - 1;

    if (!selectionCriteria.ShowRES && limit >= 0 && limit < 5)
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
        caption.Append(lbxCOUNTRY.Items.FindByValue(selectionCriteria.ResidenceCodes[i]).Text);
        conjunction = " and";
      }
    }

    limit = selectionCriteria.OriginCodes.Count - 1;

    if (!selectionCriteria.ShowOGN && limit >= 0 && limit < 5)
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
        caption.Append(lbxORIGIN.Items.FindByValue(selectionCriteria.OriginCodes[i]).Text);
      }
    }

    return caption.ToString();
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (IsPostBack)
    {
      RestoreFromViewState();
    }
    else if (Request.QueryString.Count > 0)
    {
      UnpackQueryString();
    }
    else
    {
      selectionMode = true;
    }
  }

  protected void Page_PreRender(Object sender, EventArgs e)
  {
    SaveToViewState();
    if (!selectionMode)
    {
      ConstructSelectStatement();
    }
  }

  protected void ddlPageRows_SelectedIndexChanged(Object sender, EventArgs e)
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

  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    selectionCriteria = GetSelectionDialog();

    dpgASR_RSD1.SetPageProperties(0,
      (ddlPageRows.SelectedValue == "0") ? 100000000 : Convert.ToInt32(ddlPageRows.SelectedValue),
      true);
    dpgASR_RSD2.SetPageProperties(0,
      (ddlPageRows.SelectedValue == "0") ? 100000000 : Convert.ToInt32(ddlPageRows.SelectedValue),
      true);

    selectionMode = false;
  }

  protected void btnNewQuery_Click(object sender, EventArgs e)
  {
    SetSelectionDialog();
    selectionMode = true;
  }

  protected void btnCSV_Click(object sender, EventArgs e)
  {
    StringBuilder csv = new StringBuilder();

    csv.AppendLine("\"Extracted from the UNHCR Population Statistics Reference Database, " +
      "United Nations High Commissioner for Refugees.\"");
    csv.AppendLine("Date extracted: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm K"));
    csv.AppendLine();
    csv.AppendLine('"' + GetCaption() + '"');
    csv.AppendLine();

    csv.Append("Year");
    if (selectionCriteria.ShowRES)
    {
      csv.Append(",Country / territory of residence");
    }
    if (selectionCriteria.ShowOGN)
    {
      csv.Append(",Origin / Returned from");
    }
    if (selectionCriteria.ShowRSDP)
    {
      csv.Append(",RSD procedure type code,RSD procedure type description");
    }
    if (selectionCriteria.ShowRSDL)
    {
      csv.Append(",RSD procedure level code,RSD procedure level description");
    }
    csv.AppendLine(",Total persons pending at start of year,Persons assisted by UNHCR at start of year" +
      ",Persons applied during year,Positive decisions (convention status)," +
      "Positive decisions (Complementary protection status),Rejected,Otherwise closed,Total decisions" +
      ",Total persons pending at end of year,Persons assisted by UNHCR at end of year");

    ConstructSelectStatement();

    foreach (DataRow row in ((DataView)dsASR_RSD.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
    {
      csv.Append(row.ItemArray[0]);
      if (selectionCriteria.ShowRES)
      {
        if (((String)(row.ItemArray[1])).Contains(","))
        {
          csv.Append(",\"" + ((String)(row.ItemArray[1])).Replace("\"", "\"\"") + "\"");
        }
        else
        {
          csv.Append("," + row.ItemArray[1]);
        }
      }
      if (selectionCriteria.ShowOGN)
      {
        if (((String)(row.ItemArray[2])).Contains(","))
        {
          csv.Append(",\"" + ((String)(row.ItemArray[2])).Replace("\"", "\"\"") + "\"");
        }
        else
        {
          csv.Append("," + row.ItemArray[2]);
        }
      }
      if (selectionCriteria.ShowRSDP)
      {
        csv.Append("," + row.ItemArray[3] + "," + row.ItemArray[4]);
      }
      if (selectionCriteria.ShowRSDL)
      {
        csv.Append("," + row.ItemArray[5] + "," + row.ItemArray[6]);
      }
      csv.Append(",");
      if (row.ItemArray[7].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[7])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[8].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[8])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[9].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[9])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[10].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[10])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[11].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[11])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[12].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[12])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[13].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[13])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[14].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[14])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[15].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[15])).Replace(",", ""));
      }
      csv.Append(",");
      if (row.ItemArray[16].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[16])).Replace(",", ""));
      }
      csv.AppendLine();
    }

    Response.Clear();
    Response.AddHeader("content-disposition", "attachment; filename=PSQ_RSD.csv");
    Response.ContentType = "application/csv";
    Response.ContentEncoding = Encoding.UTF8;
    Response.BinaryWrite(Encoding.UTF8.GetPreamble());
    Response.Write(csv.ToString());
    Response.End();
  }

  protected void lbxCOUNTRY_DataBound(object sender, EventArgs e)
  {
    lbxCOUNTRY.Items.Insert(0, new ListItem { Text = "All countries / territories", Value = "0", Selected = true });
  }

  protected void lbxORIGIN_DataBound(object sender, EventArgs e)
  {
    lbxORIGIN.Items.Insert(0, new ListItem { Text = "All origins", Value = "0", Selected = true });
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