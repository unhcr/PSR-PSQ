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
public class SelectionCriteriaDEM
{
  public string StartYear { get; set; }
  public string EndYear { get; set; }

  List<string> residenceCodes = new List<string>();
  public List<string> ResidenceCodes
  {
    get { return residenceCodes; }
    set { residenceCodes = value; }
  }

  public bool ShowRES { get; set; }
  public bool ShowLOC { get; set; }
  public bool ShowAGE { get; set; }
  public bool ShowSEX { get; set; }
  public bool ShowNone { get; set; }

  static Regex countryCodePattern = new Regex("^[A-Z]{3}$");  // Regular expression to validate ISO country codes

  public SelectionCriteriaDEM()
  {
    StartYear = "1950";
    EndYear = "9999";
    ResidenceCodes = new List<string>();
    ShowRES = true;
    ShowLOC = true;
    ShowAGE = true;
    ShowSEX = false;
    ShowNone = false;
  }

  public SelectionCriteriaDEM(string startYear, string endYear,
    ListItemCollection residenceCodes,
    bool showRES, bool showLOC, bool showAGE, bool showSEX, bool showNone)
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
    ShowRES = showRES;
    ShowLOC = showLOC;
    ShowAGE = showAGE;
    ShowSEX = showSEX;
    ShowNone = showNone;
  }

  public void AddResidenceCode(string code)
  {
    if (countryCodePattern.IsMatch(code))
    {
      residenceCodes.Add(code);
    }
  }
}

public partial class PSQ_DEM : System.Web.UI.Page
{
  protected SelectionCriteriaDEM selectionCriteria = new SelectionCriteriaDEM();

  protected bool selectionMode = false;

  void SaveToViewState()
  {
    ViewState["SelectionCriteria"] = selectionCriteria;
  }

  void RestoreFromViewState()
  {
    selectionCriteria = (SelectionCriteriaDEM)ViewState["SelectionCriteria"];
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

    // Extract column display parameters from query string.
    if (Request.QueryString["DRES"] != null)
    {
      selectionCriteria.ShowRES = (Request.QueryString["DRES"].ToUpper() != "N");
    }
    if (Request.QueryString["DLOC"] != null)
    {
      selectionCriteria.ShowLOC = (Request.QueryString["DLOC"].ToUpper() != "N");
    }
    if (Request.QueryString["DAGE"] != null)
    {
      selectionCriteria.ShowAGE = (Request.QueryString["DAGE"].ToUpper() != "N");
    }
    if (Request.QueryString["DSEX"] != null)
    {
      selectionCriteria.ShowSEX = (Request.QueryString["DSEX"].ToUpper() != "N");
    }
  }

  SelectionCriteriaDEM GetSelectionDialog()
  {
    return new SelectionCriteriaDEM(ddlSTART_YEAR.Text, ddlEND_YEAR.Text, lbxCOUNTRY.Items,
      cbxRES.Checked, cbxLOC.Checked, rdbAGE.Checked, rdbSEX.Checked, rdbNone.Checked);
  }

  void SetSelectionDialog()
  {
    ddlSTART_YEAR.SelectedValue = selectionCriteria.StartYear;
    ddlEND_YEAR.SelectedValue = selectionCriteria.EndYear;
    cbxRES.Checked = selectionCriteria.ShowRES;
    cbxLOC.Checked = selectionCriteria.ShowLOC;
    rdbAGE.Checked = selectionCriteria.ShowAGE;
    rdbSEX.Checked = selectionCriteria.ShowSEX;
    rdbNone.Checked = selectionCriteria.ShowNone;
  }

  void ConstructSelectStatement()
  {
    var selectStatement = new StringBuilder("select ASR_YEAR, COU_NAME_RESIDENCE_EN, LOC_NAME_RESIDENCE_EN, " +
      "case when F0_VALUE is null and F0_REDACTED_FLAG = 1 then '*' else trim(to_char(F0_VALUE, '999,999,999')) end as F0_VALUE, " +
      "case when F5_VALUE is null and F0_REDACTED_FLAG = 1 then '*' else trim(to_char(F5_VALUE, '999,999,999')) end as F5_VALUE, " +
      "case when F12_VALUE is null and F12_REDACTED_FLAG = 1 then '*' else trim(to_char(F12_VALUE, '999,999,999')) end as F12_VALUE, " +
      "case when F18_VALUE is null and F18_REDACTED_FLAG = 1 then '*' else trim(to_char(F18_VALUE, '999,999,999')) end as F18_VALUE, " +
      "case when F60_VALUE is null and F60_REDACTED_FLAG = 1 then '*' else trim(to_char(F60_VALUE, '999,999,999')) end as F60_VALUE, " +
      "case when FOTHER_VALUE is null and FOTHER_REDACTED_FLAG = 1 then '*' else trim(to_char(FOTHER_VALUE, '999,999,999')) end as FOTHER_VALUE, " +
      "case when FTOTAL_VALUE is null and FTOTAL_REDACTED_FLAG = 1 then '*' else trim(to_char(FTOTAL_VALUE, '999,999,999')) end as FTOTAL_VALUE, " +
      "case when M0_VALUE is null and M0_REDACTED_FLAG = 1 then '*' else trim(to_char(M0_VALUE, '999,999,999')) end as M0_VALUE, " +
      "case when M5_VALUE is null and M0_REDACTED_FLAG = 1 then '*' else trim(to_char(M5_VALUE, '999,999,999')) end as M5_VALUE, " +
      "case when M12_VALUE is null and M12_REDACTED_FLAG = 1 then '*' else trim(to_char(M12_VALUE, '999,999,999')) end as M12_VALUE, " +
      "case when M18_VALUE is null and M18_REDACTED_FLAG = 1 then '*' else trim(to_char(M18_VALUE, '999,999,999')) end as M18_VALUE, " +
      "case when M60_VALUE is null and M60_REDACTED_FLAG = 1 then '*' else trim(to_char(M60_VALUE, '999,999,999')) end as M60_VALUE, " +
      "case when MOTHER_VALUE is null and MOTHER_REDACTED_FLAG = 1 then '*' else trim(to_char(MOTHER_VALUE, '999,999,999')) end as MOTHER_VALUE, " +
      "case when MTOTAL_VALUE is null and MTOTAL_REDACTED_FLAG = 1 then '*' else trim(to_char(MTOTAL_VALUE, '999,999,999')) end as MTOTAL_VALUE, " +
      "case when TOTAL_VALUE is null and TOTAL_REDACTED_FLAG = 1 then '*' else trim(to_char(TOTAL_VALUE, '999,999,999')) end as TOTAL_VALUE " +
      "from (select ASR_YEAR, ",
      1000);


    selectStatement.Append((selectionCriteria.ShowRES ? String.Empty : "null as ") + "COU_NAME_RESIDENCE_EN, ");
    selectStatement.Append((selectionCriteria.ShowLOC ? String.Empty : "null as ") + "LOC_NAME_RESIDENCE_EN, ");
    if (selectionCriteria.ShowAGE)
    {
      selectStatement.Append("sum(F0_VALUE) as F0_VALUE, max(F0_REDACTED_FLAG) as F0_REDACTED_FLAG, " +
        "sum(F5_VALUE) as F5_VALUE, max(F5_REDACTED_FLAG) as F5_REDACTED_FLAG, " +
        "sum(F12_VALUE) as F12_VALUE, max(F12_REDACTED_FLAG) as F12_REDACTED_FLAG, " +
        "sum(F18_VALUE) as F18_VALUE, max(F18_REDACTED_FLAG) as F18_REDACTED_FLAG, " +
        "sum(F60_VALUE) as F60_VALUE, max(F60_REDACTED_FLAG) as F60_REDACTED_FLAG, " +
        "sum(FOTHER_VALUE) as FOTHER_VALUE, max(FOTHER_REDACTED_FLAG) as FOTHER_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as F0_VALUE, null as F0_REDACTED_FLAG, null as F5_VALUE, null as F5_REDACTED_FLAG, " +
        "null as F12_VALUE, null as F12_REDACTED_FLAG, null as F18_VALUE, null as F18_REDACTED_FLAG, " +
        "null as F60_VALUE, null as F60_REDACTED_FLAG, null as FOTHER_VALUE, null as FOTHER_REDACTED_FLAG, ");
    }
    if (!selectionCriteria.ShowNone)
    {
      selectStatement.Append("sum(FTOTAL_VALUE) as FTOTAL_VALUE, max(FTOTAL_REDACTED_FLAG) as FTOTAL_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as FTOTAL_VALUE, null as FTOTAL_REDACTED_FLAG, ");
    }
    if (selectionCriteria.ShowAGE)
    {
      selectStatement.Append("sum(M0_VALUE) as M0_VALUE, max(M0_REDACTED_FLAG) as M0_REDACTED_FLAG, " +
        "sum(M5_VALUE) as M5_VALUE, max(M5_REDACTED_FLAG) as M5_REDACTED_FLAG, " +
        "sum(M12_VALUE) as M12_VALUE, max(M12_REDACTED_FLAG) as M12_REDACTED_FLAG, " +
        "sum(M18_VALUE) as M18_VALUE, max(M18_REDACTED_FLAG) as M18_REDACTED_FLAG, " +
        "sum(M60_VALUE) as M60_VALUE, max(M60_REDACTED_FLAG) as M60_REDACTED_FLAG, " +
        "sum(MOTHER_VALUE) as MOTHER_VALUE, max(MOTHER_REDACTED_FLAG) as MOTHER_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as M0_VALUE, null as M0_REDACTED_FLAG, null as M5_VALUE, null as M5_REDACTED_FLAG, " +
        "null as M12_VALUE, null as M12_REDACTED_FLAG, null as M18_VALUE, null as M18_REDACTED_FLAG, " +
        "null as M60_VALUE, null as M60_REDACTED_FLAG, null as MOTHER_VALUE, null as MOTHER_REDACTED_FLAG, ");
    }
    if (!selectionCriteria.ShowNone)
    {
      selectStatement.Append("sum(MTOTAL_VALUE) as MTOTAL_VALUE, max(MTOTAL_REDACTED_FLAG) as MTOTAL_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as MTOTAL_VALUE, null as MTOTAL_REDACTED_FLAG, ");
    }
    selectStatement.Append("sum(TOTAL_VALUE) as TOTAL_VALUE, max(TOTAL_REDACTED_FLAG) as TOTAL_REDACTED_FLAG " +
      "from ASR_DEMOGRAPHICS_EN where ASR_YEAR between :START_YEAR and :END_YEAR ");
    if (selectionCriteria.ResidenceCodes != null && selectionCriteria.ResidenceCodes.Count > 0)
    {
      selectStatement.Append("and COU_CODE_RESIDENCE in ('");
      foreach (string code in selectionCriteria.ResidenceCodes)
      {
        selectStatement.Append("','" + code);
      }
      selectStatement.Append("') ");
    }
    selectStatement.Append("group by ASR_YEAR");
    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(", COU_NAME_RESIDENCE_EN");
    }
    if (selectionCriteria.ShowLOC)
    {
      selectStatement.Append(", LOC_NAME_RESIDENCE_EN");
    }
    selectStatement.Append(") order by ASR_YEAR desc");
    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(", nlssort(COU_NAME_RESIDENCE_EN, 'NLS_SORT=BINARY_AI')");
    }
    if (selectionCriteria.ShowLOC)
    {
      selectStatement.Append(", decode(substr(LOC_NAME_RESIDENCE_EN, 1, 12), 'Dispersed in', '_', LOC_NAME_RESIDENCE_EN)");
    }

    dsASR_DEMOGRAPHICS.SelectCommand = selectStatement.ToString();

    foreach (Parameter param in dsASR_DEMOGRAPHICS.SelectParameters)
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
    var caption = new StringBuilder("Demographic composition of populations of concern to UNHCR");
    int limit = selectionCriteria.ResidenceCodes.Count - 1;

    if (!selectionCriteria.ShowRES && limit >= 0 && limit < 5)
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
        caption.Append(lbxCOUNTRY.Items.FindByValue(selectionCriteria.ResidenceCodes[i]).Text);
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
      // Switch off paging. Note that 966,367,641 is the largest page size accepted without misbehaviour of the DataPager.
      dpgASR_DEMOGRAPHICS1.PageSize = 100000000;
      dpgASR_DEMOGRAPHICS2.PageSize = 100000000;
    }
    else
    {
      dpgASR_DEMOGRAPHICS1.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
      dpgASR_DEMOGRAPHICS2.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
    }
  }

  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    selectionCriteria = GetSelectionDialog();

    dpgASR_DEMOGRAPHICS1.SetPageProperties(0,
      (ddlPageRows.SelectedValue == "0") ? 100000000 : Convert.ToInt32(ddlPageRows.SelectedValue),
      true);
    dpgASR_DEMOGRAPHICS2.SetPageProperties(0,
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
      csv.Append(",Country/territory of residence");
    }
    if (selectionCriteria.ShowLOC)
    {
      csv.Append(",Location of residence");
    }
    if (selectionCriteria.ShowAGE)
    {
      csv.Append(",Female 0-4,Female 5-11,Female 12-17,Female 18-59,Female 60+,Female unknown age");
    }
    if (!selectionCriteria.ShowNone)
    {
      csv.Append(",Female total");
    }
    if (selectionCriteria.ShowAGE)
    {
      csv.Append(",Male 0-4,Male 5-11,Male 12-17,Male 18-59,Male 60+,Male unknown age");
    }
    if (!selectionCriteria.ShowNone)
    {
      csv.Append(",Male total");
    }
    csv.AppendLine(",Overall total");

    ConstructSelectStatement();

    foreach (DataRow row in ((DataView)dsASR_DEMOGRAPHICS.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
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
      if (selectionCriteria.ShowLOC)
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
      if (selectionCriteria.ShowAGE)
      {
        csv.Append(",");
        if (row.ItemArray[3].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[3])).Replace(",", ""));
        }
        csv.Append(",");
        if (row.ItemArray[4].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[4])).Replace(",", ""));
        }
        csv.Append(",");
        if (row.ItemArray[5].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[5])).Replace(",", ""));
        }
        csv.Append(",");
        if (row.ItemArray[6].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[6])).Replace(",", ""));
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
      }
      if (!selectionCriteria.ShowNone)
      {
        csv.Append(",");
        if (row.ItemArray[9].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[9])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowAGE)
      {
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
      }
      if (!selectionCriteria.ShowNone)
      {
        csv.Append(",");
        if (row.ItemArray[16].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[16])).Replace(",", ""));
        }
      }
      csv.Append(",");
      if (row.ItemArray[17].GetType() == typeof(string))
      {
        csv.Append(((String)(row.ItemArray[17])).Replace(",", ""));
      }
      csv.AppendLine();
    }

    Response.Clear();
    Response.AddHeader("content-disposition", "attachment; filename=PSQ_DEM.csv");
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

  protected void lvwASR_DEMOGRAPHICS_DataBound(object sender, EventArgs e)
  {
    lblNoData.Visible = (dpgASR_DEMOGRAPHICS1.TotalRowCount == 0);
    lblPager.Visible = (dpgASR_DEMOGRAPHICS1.TotalRowCount > 0);
    btnCSV.Visible = (dpgASR_DEMOGRAPHICS1.TotalRowCount > 0);
    dpgASR_DEMOGRAPHICS2.Visible = (dpgASR_DEMOGRAPHICS2.TotalRowCount > dpgASR_DEMOGRAPHICS2.PageSize);

    var caption = (Label)(lvwASR_DEMOGRAPHICS.FindControl("capASR_DEMOGRAPHICS"));
    if (caption != null)
    {
      caption.Text = GetCaption();
    }
  }

}