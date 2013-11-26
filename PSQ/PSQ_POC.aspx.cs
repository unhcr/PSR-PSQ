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
public class SelectionCriteriaPOC
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
  public bool ShowREF { get; set; }
  public bool ShowASY { get; set; }
  public bool ShowRET { get; set; }
  public bool ShowIDP { get; set; }
  public bool ShowRDP { get; set; }
  public bool ShowSTA { get; set; }
  public bool ShowOOC { get; set; }
  public bool ShowPOC { get; set; }

  static Regex countryCodePattern = new Regex("^[A-Z]{3}$");  // Regular expression to validate ISO country codes

  public SelectionCriteriaPOC()
  {
    StartYear = "1950";
    EndYear = "9999";
    ResidenceCodes = new List<string>();
    OriginCodes = new List<string>();
    ShowRES = true;
    ShowOGN = true;
    ShowREF = true;
    ShowASY = true;
    ShowRET = true;
    ShowIDP = true;
    ShowRDP = true;
    ShowSTA = true;
    ShowOOC = true;
    ShowPOC = true;
  }

  public SelectionCriteriaPOC(string startYear, string endYear,
    ListItemCollection residenceCodes, ListItemCollection originCodes,
    bool showRES, bool showOGN, bool showREF, bool showASY, bool showRET,
    bool showIDP, bool showRDP, bool showSTA, bool showOOC, bool showPOC)
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
    ShowREF = showREF;
    ShowASY = showASY;
    ShowRET = showRET;
    ShowIDP = showIDP;
    ShowRDP = showRDP;
    ShowSTA = showSTA;
    ShowOOC = showOOC;
    ShowPOC = showPOC;
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

public partial class PSQ_POC : System.Web.UI.Page
{
  protected SelectionCriteriaPOC selectionCriteria = new SelectionCriteriaPOC();

  protected bool selectionMode = false;

  void SaveToViewState()
  {
    ViewState["SelectionCriteria"] = selectionCriteria;
  }

  void RestoreFromViewState()
  {
    selectionCriteria = (SelectionCriteriaPOC)ViewState["SelectionCriteria"];
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
    if (Request.QueryString["DREF"] != null)
    {
      selectionCriteria.ShowREF = (Request.QueryString["DREF"].ToUpper() != "N");
    }
    if (Request.QueryString["DASY"] != null)
    {
      selectionCriteria.ShowASY = (Request.QueryString["DASY"].ToUpper() != "N");
    }
    if (Request.QueryString["DRET"] != null)
    {
      selectionCriteria.ShowRET = (Request.QueryString["DRET"].ToUpper() != "N");
    }
    if (Request.QueryString["DIDP"] != null)
    {
      selectionCriteria.ShowIDP = (Request.QueryString["DIDP"].ToUpper() != "N");
    }
    if (Request.QueryString["DRDP"] != null)
    {
      selectionCriteria.ShowRDP = (Request.QueryString["DRDP"].ToUpper() != "N");
    }
    if (Request.QueryString["DSTA"] != null)
    {
      selectionCriteria.ShowSTA = (Request.QueryString["DSTA"].ToUpper() != "N");
    }
    if (Request.QueryString["DOOC"] != null)
    {
      selectionCriteria.ShowOOC = (Request.QueryString["DOOC"].ToUpper() != "N");
    }
    if (Request.QueryString["DPOC"] != null)
    {
      selectionCriteria.ShowPOC = (Request.QueryString["DPOC"].ToUpper() != "N");
    }
  }

  SelectionCriteriaPOC GetSelectionDialog()
  {
    return new SelectionCriteriaPOC(ddlSTART_YEAR.Text, ddlEND_YEAR.Text,
      lbxCOUNTRY.Items, lbxORIGIN.Items,
      cbxRES.Checked, cbxOGN.Checked, cbxREF.Checked, cbxASY.Checked, cbxRET.Checked,
      cbxIDP.Checked, cbxRDP.Checked, cbxSTA.Checked, cbxOOC.Checked, cbxPOC.Checked);
  }

  void SetSelectionDialog()
  {
    ddlSTART_YEAR.SelectedValue = selectionCriteria.StartYear;
    ddlEND_YEAR.SelectedValue = selectionCriteria.EndYear;
    cbxRES.Checked = selectionCriteria.ShowRES;
    cbxOGN.Checked = selectionCriteria.ShowOGN;
    cbxREF.Checked = selectionCriteria.ShowREF;
    cbxASY.Checked = selectionCriteria.ShowASY;
    cbxRET.Checked = selectionCriteria.ShowRET;
    cbxIDP.Checked = selectionCriteria.ShowIDP;
    cbxRDP.Checked = selectionCriteria.ShowRDP;
    cbxSTA.Checked = selectionCriteria.ShowSTA;
    cbxOOC.Checked = selectionCriteria.ShowOOC;
    cbxPOC.Checked = selectionCriteria.ShowPOC;
  }

  void ConstructSelectStatement()
  {
    var selectStatement =
      new StringBuilder("select ASR_YEAR, COU_NAME_RESIDENCE_EN, COU_NAME_ORIGIN_EN, " +
        "case when REFPOP_VALUE is null and REFPOP_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(REFPOP_VALUE, '999,999,999')) end as REFPOP_VALUE, " +
        "case when ASYPOP_VALUE is null and ASYPOP_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(ASYPOP_VALUE, '999,999,999')) end as ASYPOP_VALUE, " +
        "case when REFRTN_VALUE is null and REFRTN_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(REFRTN_VALUE, '999,999,999')) end as REFRTN_VALUE, " +
        "case when IDPHPOP_VALUE is null and IDPHPOP_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(IDPHPOP_VALUE, '999,999,999')) end as IDPHPOP_VALUE, " +
        "case when IDPHRTN_VALUE is null and IDPHRTN_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(IDPHRTN_VALUE, '999,999,999')) end as IDPHRTN_VALUE, " +
        "case when STAPOP_VALUE is null and STAPOP_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(STAPOP_VALUE, '999,999,999')) end as STAPOP_VALUE, " +
        "case when OOCPOP_VALUE is null and OOCPOP_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(OOCPOP_VALUE, '999,999,999')) end as OOCPOP_VALUE, " +
        "case when TPOC_VALUE is null and TPOC_REDACTED_FLAG = 1 then '*' " +
        "else trim(to_char(TPOC_VALUE, '999,999,999')) end as TPOC_VALUE " +
        "from (select ASR_YEAR, ",
        2000);

    selectStatement.Append((selectionCriteria.ShowRES ? String.Empty : "null as ") + "COU_NAME_RESIDENCE_EN, ");
    selectStatement.Append((selectionCriteria.ShowOGN ? String.Empty : "null as ") + "COU_NAME_ORIGIN_EN, ");
    if (selectionCriteria.ShowREF)
    {
      selectStatement.Append("sum(REFPOP_VALUE) as REFPOP_VALUE, sign(sum(REFPOP_REDACTED_FLAG)) as REFPOP_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as REFPOP_VALUE, null as REFPOP_REDACTED_FLAG, ");
    }
    if (selectionCriteria.ShowASY)
    {
      selectStatement.Append("sum(ASYPOP_VALUE) as ASYPOP_VALUE, sign(sum(ASYPOP_REDACTED_FLAG)) as ASYPOP_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as ASYPOP_VALUE, null as ASYPOP_REDACTED_FLAG, ");
    }
    if (selectionCriteria.ShowRET)
    {
      selectStatement.Append("sum(REFRTN_VALUE) as REFRTN_VALUE, sign(sum(REFRTN_REDACTED_FLAG)) as REFRTN_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as REFRTN_VALUE, null as REFRTN_REDACTED_FLAG, ");
    }
    if (selectionCriteria.ShowIDP)
    {
      selectStatement.Append("sum(IDPHPOP_VALUE) as IDPHPOP_VALUE, sign(sum(IDPHPOP_REDACTED_FLAG)) as IDPHPOP_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as IDPHPOP_VALUE, null as IDPHPOP_REDACTED_FLAG, ");
    }
    if (selectionCriteria.ShowRDP)
    {
      selectStatement.Append("sum(IDPHRTN_VALUE) as IDPHRTN_VALUE, sign(sum(IDPHRTN_REDACTED_FLAG)) as IDPHRTN_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as IDPHRTN_VALUE, null as IDPHRTN_REDACTED_FLAG, ");
    }
    if (selectionCriteria.ShowSTA)
    {
      selectStatement.Append("sum(STAPOP_VALUE) as STAPOP_VALUE, sign(sum(STAPOP_REDACTED_FLAG)) as STAPOP_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as STAPOP_VALUE, null as STAPOP_REDACTED_FLAG, ");
    }
    if (selectionCriteria.ShowOOC)
    {
      selectStatement.Append("sum(OOCPOP_VALUE) as OOCPOP_VALUE, sign(sum(OOCPOP_REDACTED_FLAG)) as OOCPOP_REDACTED_FLAG, ");
    }
    else
    {
      selectStatement.Append("null as OOCPOP_VALUE, null as OOCPOP_REDACTED_FLAG, ");
    }

    if (selectionCriteria.ShowPOC)
    {
      selectStatement.Append("sum(case when nvl(REFPOP_VALUE,0) + nvl(ASYPOP_VALUE,0) + nvl(REFRTN_VALUE,0) + " +
        "nvl(IDPHPOP_VALUE,0) + nvl(IDPHRTN_VALUE,0) + nvl(STAPOP_VALUE,0) + nvl(OOCPOP_VALUE,0) > 0 " +
        "then nvl(REFPOP_VALUE,0) + nvl(ASYPOP_VALUE,0) + nvl(REFRTN_VALUE,0) + nvl(IDPHPOP_VALUE,0) + " +
        "nvl(IDPHRTN_VALUE,0) + nvl(STAPOP_VALUE,0) + nvl(OOCPOP_VALUE,0) end) as TPOC_VALUE, " +
        "sign(sum(coalesce(REFPOP_REDACTED_FLAG, ASYPOP_REDACTED_FLAG, REFRTN_REDACTED_FLAG, " +
        "IDPHPOP_REDACTED_FLAG, IDPHRTN_REDACTED_FLAG, STAPOP_REDACTED_FLAG, OOCPOP_REDACTED_FLAG))) " +
        "as TPOC_REDACTED_FLAG ");
    }
    else
    {
      selectStatement.Append("null as TPOC_VALUE, null as TPOC_REDACTED_FLAG ");
    }
    selectStatement.Append("from ASR_POC_SUMMARY_EN where ASR_YEAR between :START_YEAR and :END_YEAR ");
    if (selectionCriteria.ResidenceCodes != null && selectionCriteria.ResidenceCodes.Count > 0)
    {
      selectStatement.Append("and COU_CODE_RESIDENCE in ('");
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
      selectStatement.Append(", COU_NAME_RESIDENCE_EN");
    }
    if (selectionCriteria.ShowOGN)
    {
      selectStatement.Append(", COU_NAME_ORIGIN_EN");
    }
    selectStatement.Append(") where coalesce(REFPOP_VALUE, ASYPOP_VALUE, REFRTN_VALUE, " +
      "IDPHPOP_VALUE, IDPHRTN_VALUE, STAPOP_VALUE, OOCPOP_VALUE, TPOC_VALUE, " +
      "REFPOP_REDACTED_FLAG, ASYPOP_REDACTED_FLAG, REFRTN_REDACTED_FLAG, IDPHPOP_REDACTED_FLAG, " +
      "IDPHRTN_REDACTED_FLAG, STAPOP_REDACTED_FLAG, OOCPOP_REDACTED_FLAG, TPOC_REDACTED_FLAG) is not null " +
      "order by ASR_YEAR desc");
    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(", nlssort(COU_NAME_RESIDENCE_EN, 'NLS_SORT=BINARY_AI')");
    }
    if (selectionCriteria.ShowOGN)
    {
      selectStatement.Append(", nlssort(decode(COU_NAME_ORIGIN_EN, 'Various', '_', COU_NAME_ORIGIN_EN), 'NLS_SORT=BINARY_AI')");
    }

    dsASR_POC_SUMMARY.SelectCommand = selectStatement.ToString();

    foreach (Parameter param in dsASR_POC_SUMMARY.SelectParameters)
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
    var caption = new StringBuilder("Overview – Persons of concern to UNHCR");
    string conjunction = "";
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
        conjunction = " and";
      }
    }

    limit = selectionCriteria.OriginCodes.Count - 1;

    if (!selectionCriteria.ShowOGN && limit >= 0 && limit < 5)
    {
      caption.Append(conjunction);
      conjunction = "";
      if (selectionCriteria.ShowREF || selectionCriteria.ShowASY || selectionCriteria.ShowIDP ||
        selectionCriteria.ShowSTA || selectionCriteria.ShowOOC || selectionCriteria.ShowPOC)
      {
        caption.Append(" originating");
        conjunction = " or";
      }
      if (selectionCriteria.ShowRET || selectionCriteria.ShowRDP || selectionCriteria.ShowPOC)
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
      dpgASR_POC_SUMMARY1.PageSize = 100000000;
      dpgASR_POC_SUMMARY2.PageSize = 100000000;
    }
    else
    {
      dpgASR_POC_SUMMARY1.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
      dpgASR_POC_SUMMARY2.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
    }
  }
  
  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    selectionCriteria = GetSelectionDialog();

    dpgASR_POC_SUMMARY1.SetPageProperties(0,
      (ddlPageRows.SelectedValue == "0") ? 100000000 : Convert.ToInt32(ddlPageRows.SelectedValue),
      true);
    dpgASR_POC_SUMMARY2.SetPageProperties(0,
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
    if (selectionCriteria.ShowOGN)
    {
      csv.Append(",Origin / Returned from");
    }
    if (selectionCriteria.ShowREF)
    {
      csv.Append(",Refugees");
    }
    if (selectionCriteria.ShowASY)
    {
      csv.Append(",Asylum seekers");
    }
    if (selectionCriteria.ShowRET)
    {
      csv.Append(",Returned refugees");
    }
    if (selectionCriteria.ShowIDP)
    {
      csv.Append(",IDPs");
    }
    if (selectionCriteria.ShowRDP)
    {
      csv.Append(",Returned IDPs");
    }
    if (selectionCriteria.ShowSTA)
    {
      csv.Append(",Stateless");
    }
    if (selectionCriteria.ShowOOC)
    {
      csv.Append(",Others of concern");
    }
    if (selectionCriteria.ShowPOC)
    {
      csv.Append(",Total population");
    }
    csv.AppendLine();

    ConstructSelectStatement();

    foreach (DataRow row in ((DataView)dsASR_POC_SUMMARY.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
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
      if (selectionCriteria.ShowREF)
      {
        csv.Append(",");
        if (row.ItemArray[3].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[3])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowASY)
      {
        csv.Append(",");
        if (row.ItemArray[4].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[4])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowRET)
      {
        csv.Append(",");
        if (row.ItemArray[5].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[5])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowIDP)
      {
        csv.Append(",");
        if (row.ItemArray[6].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[6])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowRDP)
      {
        csv.Append(",");
        if (row.ItemArray[7].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[7])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowSTA)
      {
        csv.Append(",");
        if (row.ItemArray[8].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[8])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowOOC)
      {
        csv.Append(",");
        if (row.ItemArray[9].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[9])).Replace(",", ""));
        }
      }
      if (selectionCriteria.ShowPOC)
      {
        csv.Append(",");
        if (row.ItemArray[10].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[10])).Replace(",", ""));
        }
      }
      csv.AppendLine();
    }

    Response.Clear();
    Response.AddHeader("content-disposition", "attachment; filename=PSQ_POC.csv");
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