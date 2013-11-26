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
public class SelectionCriteriaTMS
{
  List<string> statisticYears = new List<string>();
  public List<string> StatisticYears
  {
    get { return statisticYears; }
    set { statisticYears = value; }
  }
  
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

  List<string> populationTypes = new List<string>();
  public List<string> PopulationTypes
  {
    get { return populationTypes; }
    set { populationTypes = value; }
  }

  public bool ShowRES { get; set; }
  public bool ShowOGN { get; set; }
  public bool ShowPOPT { get; set; }

  public SelectionCriteriaTMS()
  {
    StatisticYears = new List<string>();
    ResidenceCodes = new List<string>();
    OriginCodes = new List<string>();
    PopulationTypes = new List<string>();
    ShowRES = true;
    ShowOGN = true;
    ShowPOPT = true;
  }

  public SelectionCriteriaTMS(string startYear, string endYear,
    ListItemCollection residenceCodes, ListItemCollection originCodes, ListItemCollection populationTypes,
    bool showRES, bool showOGN, bool showPOPT)
  {
    AddYearRange(startYear, endYear);

    foreach (ListItem item in residenceCodes)
    {
      if (item.Selected)
      {
        this.AddResidenceCode(item.Value);
      }
    }

    foreach (ListItem item in originCodes)
    {
      if (item.Selected)
      {
        this.AddOriginCode(item.Value);
      }
    }

    foreach (ListItem item in populationTypes)
    {
      if (item.Selected)
      {
        this.AddPopulationType(item.Value);
      }
    }

    ShowRES = showRES;
    ShowOGN = showOGN;
    ShowPOPT = showPOPT;
  }

  static Regex yearPattern = new Regex("^[0-9]{4}$");  // Regular expression to validate year numbers
  static Regex countryCodePattern = new Regex("^[A-Z]{3}$");  // Regular expression to validate ISO country codes
  static Regex populationTypePattern = new Regex("^RF|AS|RT|ID|RD|ST|OC$");  // Regular expression to validate population types

  public void AddYear(string code)
  {
    if (yearPattern.IsMatch(code) && String.CompareOrdinal(code, "1950") >= 0)
    {
      statisticYears.Add(code);
    }
  }

  public void AddYearRange(string startYear, string endYear)
  {
    if (yearPattern.IsMatch(startYear) && String.CompareOrdinal(startYear, "1950") >= 0 &&
      yearPattern.IsMatch(endYear) && String.CompareOrdinal(endYear, startYear) >= 0)
    {
      for (int year = Convert.ToInt32(startYear); year <= Convert.ToInt32(endYear); year++)
      {
        statisticYears.Add(year.ToString());
      }
    }
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

  public void AddPopulationType(string code)
  {
    if (populationTypePattern.IsMatch(code))
    {
      populationTypes.Add(code);
    }
  }
}

public class ListViewTemplate : ITemplate
{
  protected SelectionCriteriaTMS sc;

  public ListViewTemplate(SelectionCriteriaTMS selectionCriteria)
  {
    sc = selectionCriteria;
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
    if (sc.ShowRES)
    {
      lc.Text += "<td style=\"min-width: 100px;\">" + DataBinder.Eval(container.DataItem, "COU_NAME_RESIDENCE_EN") + "</td>";
    }
    if (sc.ShowOGN)
    {
      lc.Text += "<td style=\"min-width: 100px;\">" + DataBinder.Eval(container.DataItem, "COU_NAME_ORIGIN_EN") + "</td>";
    }
    if (sc.ShowPOPT)
    {
      lc.Text += "<td class=\"population-type-short\">" +
        DataBinder.Eval(container.DataItem, "POPULATION_TYPE_EN") + "</td>";
    }
    foreach (string year in sc.StatisticYears)
    {
      lc.Text += "<td class=\"numeric\">" +
        DataBinder.Eval(container.DataItem, "Y" + year + "_VALUE", "{0:N0}") + "</td>";
    }
    lc.Text += "</tr>";
  }

}

public partial class PSQ_TMS : System.Web.UI.Page
{
  protected SelectionCriteriaTMS selectionCriteria = new SelectionCriteriaTMS();

  protected bool selectionMode = false;

  void SaveToViewState()
  {
    ViewState["SelectionCriteria"] = selectionCriteria;
  }

  void RestoreFromViewState()
  {
    selectionCriteria = (SelectionCriteriaTMS)ViewState["SelectionCriteria"];
  }

  void UnpackQueryString()
  {
    string startYear = "1950";
    string endYear = "9999";

    // Extract selection criteria parameters from query string.
    if (Request.QueryString["SYR"] != null)
    {
      startYear = Request.QueryString["SYR"];
    }
    if (Request.QueryString["EYR"] != null)
    {
      endYear = Request.QueryString["EYR"];
    }
    selectionCriteria.AddYearRange(startYear, endYear);

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
    if (Request.QueryString["POPT"] != null)
    {
      foreach (string code in Request.QueryString["POPT"].ToUpper().Split(',').Distinct())
      {
        selectionCriteria.AddPopulationType(code);
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
    if (Request.QueryString["DPOPT"] != null)
    {
      selectionCriteria.ShowPOPT = (Request.QueryString["DPOPT"].ToUpper() != "N");
    }
  }

  SelectionCriteriaTMS GetSelectionDialog()
  {
    return new SelectionCriteriaTMS(ddlSTART_YEAR.Text, ddlEND_YEAR.Text,
      lbxCOUNTRY.Items, lbxORIGIN.Items, cblPOP_TYPE.Items,
      cbxRES.Checked, cbxOGN.Checked, cbxPOPT.Checked);
  }

  void SetSelectionDialog()
  {
    cbxRES.Checked = selectionCriteria.ShowRES;
    cbxOGN.Checked = selectionCriteria.ShowOGN;
    cbxPOPT.Checked = selectionCriteria.ShowPOPT;
  }

  void ConstructSelectStatement()
  {
    var selectStatement = new StringBuilder("select ", 2000);
    string separator = "";

    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(separator + "COU_NAME_RESIDENCE_EN");
      separator = ", ";
    }
    if (selectionCriteria.ShowOGN)
    {
      selectStatement.Append(separator + "COU_NAME_ORIGIN_EN");
      separator = ", ";
    }
    if (selectionCriteria.ShowPOPT)
    {
      selectStatement.Append(separator + "POPULATION_TYPE_EN");
      separator = ", ";
    }
    foreach (string year in selectionCriteria.StatisticYears)
    {
      selectStatement.Append(separator + "case when Y" + year + "_VALUE is null and Y" + year +
        "_REDACTED_FLAG = 1 then '*' else trim(to_char(Y" + year +
        "_VALUE, '999,999,999')) end as Y" + year + "_VALUE");
      separator = ", ";
    }
    selectStatement.Append(" from (select ");
    separator = "";
    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(separator + "COU_NAME_RESIDENCE_EN");
      separator = ", ";
    }
    if (selectionCriteria.ShowOGN)
    {
      selectStatement.Append(separator + "COU_NAME_ORIGIN_EN");
      separator = ", ";
    }
    if (selectionCriteria.ShowPOPT)
    {
      selectStatement.Append(separator + "replace(POPULATION_TYPE_EN, ' persons', '') as POPULATION_TYPE_EN, POPULATION_TYPE_SEQ");
      separator = ", ";
    }
    selectStatement.Append(separator + "ASR_YEAR, VALUE, REDACTED_FLAG from ASR_POC_DETAILS_EN where ASR_YEAR in (");
    separator = "";
    foreach (string year in selectionCriteria.StatisticYears)
    {
      selectStatement.Append(separator + "'" + year + "'");
      separator = ",";
    }
    selectStatement.Append(")");
    separator = "";
    if (selectionCriteria.ResidenceCodes != null && selectionCriteria.ResidenceCodes.Count > 0)
    {
      selectStatement.Append(" and COU_CODE_RESIDENCE in (");
      foreach (string code in selectionCriteria.ResidenceCodes)
      {
        selectStatement.Append(separator + "'" + code + "'");
        separator = ",";
      }
      selectStatement.Append(")");
    }
    separator = "";
    if (selectionCriteria.OriginCodes != null && selectionCriteria.OriginCodes.Count > 0)
    {
      selectStatement.Append(" and COU_CODE_ORIGIN in (");
      foreach (string code in selectionCriteria.OriginCodes)
      {
        selectStatement.Append(separator + "'" + code + "'");
        separator = ",";
      }
      selectStatement.Append(")");
    }
    selectStatement.Append(" and POPULATION_TYPE_CODE in (''");
    if (selectionCriteria.PopulationTypes != null && selectionCriteria.PopulationTypes.Count > 0)
    {
      foreach (string code in selectionCriteria.PopulationTypes)
      {
        selectStatement.Append(",'" + code + "'");
      }
    }
    selectStatement.Append(")) pivot (sum(VALUE) as VALUE, max(REDACTED_FLAG) as REDACTED_FLAG for ASR_YEAR in (");
    separator = "";
    foreach (string year in selectionCriteria.StatisticYears)
    {
      selectStatement.Append(separator + "'" + year + "' as Y" + year);
      separator = ", ";
    }
    selectStatement.Append("))");
    separator = " order by ";
    if (selectionCriteria.ShowRES)
    {
      selectStatement.Append(separator + "nlssort(COU_NAME_RESIDENCE_EN, 'NLS_SORT=BINARY_AI')");
      separator = ", ";
    }
    if (selectionCriteria.ShowOGN)
    {
      selectStatement.Append(separator + "nlssort(decode(COU_NAME_ORIGIN_EN, 'Various', 'ZZZ', COU_NAME_ORIGIN_EN), 'NLS_SORT=BINARY_AI')");
      separator = ", ";
    }
    if (selectionCriteria.ShowPOPT)
    {
      selectStatement.Append(separator + "POPULATION_TYPE_SEQ");
      separator = ", ";
    }

    dsASR_POC_DETAILS.SelectCommand = selectStatement.ToString();

    //Label1.Text = selectStatement.ToString() + "<br />" + DateTime.Now;
  }

  string GetCaption()
  {
    var caption = new StringBuilder("Time Series – ");
    string conjunction = "";
    int limit = selectionCriteria.PopulationTypes.Count - 1;

    if (!selectionCriteria.ShowPOPT && limit >= 0 && limit < 6)
    {
      for (int i = 0; i <= limit; i++)
      {
        if (i > 0)
        {
          if (i == limit)
          {
            caption.Append(" and ");
          }
          else
          {
            caption.Append(", ");
          }
        }
        switch (selectionCriteria.PopulationTypes[i])
        {
          case "RF":
            caption.Append("Refugees");
            break;
          case "AS":
            caption.Append("Asylum seekers");
            break;
          case "RT":
            caption.Append("Returned refugees");
            break;
          case "ID":
            caption.Append("Internally displaced persons");
            break;
          case "RD":
            caption.Append("Returned IDPs");
            break;
          case "ST":
            caption.Append("Persons under UNHCR's statelessness mandate");
            break;
          case "OC":
            caption.Append("Others of concern to UNHCR");
            break;
        }
      }
    }
    else
    {
      caption.Append("Persons of concern to UNHCR");
    }

    limit = selectionCriteria.ResidenceCodes.Count - 1;

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
      lvwASR_POC_DETAILS.ItemTemplate = new ListViewTemplate(selectionCriteria);
    }
    else if (Request.QueryString.Count > 0)
    {
      UnpackQueryString();
      lvwASR_POC_DETAILS.ItemTemplate = new ListViewTemplate(selectionCriteria);
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
      dpgASR_POC_DETAILS1.PageSize = 100000000;
      dpgASR_POC_DETAILS2.PageSize = 100000000;
    }
    else
    {
      dpgASR_POC_DETAILS1.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
      dpgASR_POC_DETAILS2.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
    }
  }

  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    selectionCriteria = GetSelectionDialog();
    lvwASR_POC_DETAILS.ItemTemplate = new ListViewTemplate(selectionCriteria);

    dpgASR_POC_DETAILS1.SetPageProperties(0,
      (ddlPageRows.SelectedValue == "0") ? 100000000 : Convert.ToInt32(ddlPageRows.SelectedValue),
      true);
    dpgASR_POC_DETAILS2.SetPageProperties(0,
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
    string separator = "";

    csv.AppendLine("\"Extracted from the UNHCR Population Statistics Reference Database, " +
      "United Nations High Commissioner for Refugees.\"");
    csv.AppendLine("Date extracted: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm K"));
    csv.AppendLine();
    csv.AppendLine('"' + GetCaption() + '"');
    csv.AppendLine();

    if (selectionCriteria.ShowRES)
    {
      csv.Append(separator + "Country / territory of residence");
      separator = ",";
    }
    if (selectionCriteria.ShowOGN)
    {
      csv.Append(separator + "Origin / Returned from");
      separator = ",";
    }
    if (selectionCriteria.ShowPOPT)
    {
      csv.Append(separator + "Population type");
      separator = ",";
    }
    foreach (string year in selectionCriteria.StatisticYears)
    {
      csv.Append(separator + year);
      separator = ",";
    }
    csv.AppendLine();

    ConstructSelectStatement();

    foreach (DataRow row in ((DataView)dsASR_POC_DETAILS.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
    {
      int i = 0;
      separator = "";
      if (selectionCriteria.ShowRES)
      {
        if (((String)(row.ItemArray[i])).Contains(","))
        {
          csv.Append(separator + "\"" + ((String)(row.ItemArray[i])).Replace("\"", "\"\"") + "\"");
        }
        else
        {
          csv.Append(separator + row.ItemArray[i]);
        }
        separator = ",";
        i++;
      }
      if (selectionCriteria.ShowOGN)
      {
        if (((String)(row.ItemArray[i])).Contains(","))
        {
          csv.Append(separator + "\"" + ((String)(row.ItemArray[i])).Replace("\"", "\"\"") + "\"");
        }
        else
        {
          csv.Append(separator + row.ItemArray[i]);
        }
        separator = ",";
        i++;
      }
      if (selectionCriteria.ShowPOPT)
      {
        csv.Append(separator + row.ItemArray[i++]);
        separator = ",";
      }
      foreach (string year in selectionCriteria.StatisticYears)
      {
        csv.Append(",");
        if (row.ItemArray[i].GetType() == typeof(string))
        {
          csv.Append(((String)(row.ItemArray[i])).Replace(",", ""));
        }
        i++;
      }
      csv.AppendLine();
    }

    Response.Clear();
    Response.AddHeader("content-disposition", "attachment; filename=PSQ_TMS.csv");
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

  protected void cblPOP_TYPE_DataBound(object sender, EventArgs e)
  {
    foreach (ListItem item in cblPOP_TYPE.Items)
    {
      item.Selected = true;
    }
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