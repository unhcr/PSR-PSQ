using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PSQPOCS : System.Web.UI.Page, IQueryParameters
{
  public QueryParameters ParameterSet { get; set; }

  #region SQLstatements
  private string dsYears_SelectCommand =
@"select ASR_YEAR from PSQ_POC_YEARS order by ASR_YEAR desc";

  private string dsCountries_CountryList_SelectCommand =
@"select CODE, NAME,
  case when ROW_NUMBER = 1 then '<div class=""list-select""><ul>' end || '<li>' as PREFIX,
  '</li>' || case when ROW_NUMBER is null then '</ul></div>' end as SUFFIX,
  'leaf' as NODETYPE,
  'false' as EXPANDED
from PSQ_POC_COUNTRY_LIST_EN
order by SORT_NAME nulls last";

  private string dsCountries_UNSDTree_SelectCommand =
@"select case when LOCT_CODE != 'COUNTRY' then '*' end || CODE as CODE,
  NAME,
  case when ROW_NUMBER = 1 then '<div class=""tree-select"">' end ||
  case when TREE_LEVEL > PREV_TREE_LEVEL then '<ul>' end || '<li>' as PREFIX,
  case
    when NEXT_TREE_LEVEL < TREE_LEVEL
    then rpad('</li></ul>', (TREE_LEVEL - NEXT_TREE_LEVEL) * 10, '</li></ul>')
  end ||
  case when NEXT_TREE_LEVEL <= TREE_LEVEL and NEXT_TREE_LEVEL > 0 then '</li>' end ||
  case when ROW_NUMBER = ROW_COUNT then '</div>' end as SUFFIX,
  case when LOCT_CODE = 'COUNTRY' then 'leaf' else 'toggle' end as NODETYPE,
  case when LOCT_CODE != 'COUNTRY' and TREE_LEVEL <= 2 then 'true' else 'false' end as EXPANDED
from PSQ_POC_COUNTRY_UNSD_TREE_EN
order by ORDER_SEQ, SORT_NAME nulls first, NAME";

  private string dsCountries_UNHCRTree_SelectCommand =
@"select case when LOCT_CODE != 'COUNTRY' then '*' end || CODE as CODE,
  NAME,
  case when ROW_NUMBER = 1 then '<div class=""tree-select"">' end ||
  case when TREE_LEVEL > PREV_TREE_LEVEL then '<ul>' end || '<li>' as PREFIX,
  case
    when NEXT_TREE_LEVEL < TREE_LEVEL
    then rpad('</li></ul>', (TREE_LEVEL - NEXT_TREE_LEVEL) * 10, '</li></ul>')
  end ||
  case when NEXT_TREE_LEVEL <= TREE_LEVEL and NEXT_TREE_LEVEL > 0 then '</li>' end ||
  case when ROW_NUMBER = ROW_COUNT then '</div>' end as SUFFIX,
  case when LOCT_CODE = 'COUNTRY' then 'leaf' else 'toggle' end as NODETYPE,
  case when LOCT_CODE != 'COUNTRY' and NEXT_LOCT_CODE != 'COUNTRY' then 'true' else 'false' end as EXPANDED
from PSQ_POC_COUNTRY_UNHCR_TREE_EN
order by ORDER_SEQ, SORT_NAME nulls first, NAME";

  private string dsOrigins_CountryList_SelectCommand =
@"select CODE, NAME,
  case when ROW_NUMBER = 1 then '<div class=""list-select""><ul>' end || '<li>' as PREFIX,
  '</li>' || case when ROW_NUMBER is null then '</ul></div>' end as SUFFIX,
  'leaf' as NODETYPE,
  'false' as EXPANDED
from PSQ_POC_ORIGIN_LIST_EN
order by SORT_NAME nulls last";

  private string dsOrigins_UNSDTree_SelectCommand =
@"select case when LOCT_CODE not in ('COUNTRY', 'OTHORIGIN') then '*' end || CODE as CODE,
  NAME,
  case when ROW_NUMBER = 1 then '<div class=""tree-select"">' end ||
  case when TREE_LEVEL > PREV_TREE_LEVEL then '<ul>' end || '<li>' as PREFIX,
  case
    when NEXT_TREE_LEVEL < TREE_LEVEL
    then rpad('</li></ul>', (TREE_LEVEL - NEXT_TREE_LEVEL) * 10, '</li></ul>')
  end ||
  case when NEXT_TREE_LEVEL <= TREE_LEVEL and NEXT_TREE_LEVEL > 0 then '</li>' end ||
  case when ROW_NUMBER = ROW_COUNT then '</div>' end as SUFFIX,
  case when LOCT_CODE in ('COUNTRY', 'OTHORIGIN') then 'leaf' else 'toggle' end as NODETYPE,
  case
    when LOCT_CODE not in ('COUNTRY', 'OTHORIGIN') and TREE_LEVEL <= 2 then 'true'
    else 'false'
  end as EXPANDED
from PSQ_POC_ORIGIN_UNSD_TREE_EN
order by ORDER_SEQ, SORT_NAME nulls first, NAME";

  private string dsOrigins_UNHCRTree_SelectCommand =
@"select case when LOCT_CODE not in ('COUNTRY', 'OTHORIGIN') then '*' end || CODE as CODE,
  NAME,
  case when ROW_NUMBER = 1 then '<div class=""tree-select"">' end ||
  case when TREE_LEVEL > PREV_TREE_LEVEL then '<ul>' end || '<li>' as PREFIX,
  case
    when NEXT_TREE_LEVEL < TREE_LEVEL
    then rpad('</li></ul>', (TREE_LEVEL - NEXT_TREE_LEVEL) * 10, '</li></ul>')
  end ||
  case when NEXT_TREE_LEVEL <= TREE_LEVEL and NEXT_TREE_LEVEL > 0 then '</li>' end ||
  case when ROW_NUMBER = ROW_COUNT then '</div>' end as SUFFIX,
  case when LOCT_CODE in ('COUNTRY', 'OTHORIGIN') then 'leaf' else 'toggle' end as NODETYPE,
  case
    when LOCT_CODE not in ('COUNTRY', 'OTHORIGIN') and NEXT_LOCT_CODE not in ('COUNTRY', 'OTHORIGIN')
    then 'true'
    else 'false'
  end as EXPANDED
from PSQ_POC_ORIGIN_UNHCR_TREE_EN
order by ORDER_SEQ, SORT_NAME nulls first, NAME";
  #endregion
  
  void SetDefaultParameters()
  {
    if (! ParameterSet.ContainsKey("RESSTYLE"))
    {
      ParameterSet.AddSet("RESSTYLE", new SortedSet<string>(new string[] { "UNSD" }));
    }

    if (! ParameterSet.ContainsKey("OGNSTYLE"))
    {
      ParameterSet.AddSet("OGNSTYLE", new SortedSet<string>(new string[] { "UNSD" }));
    }

    if (! ParameterSet.ContainsKey("BREAKDOWN") && ! ParameterSet.ContainsEmptyKey("BREAKDOWN"))
    {
      ParameterSet.AddSet("BREAKDOWN", new SortedSet<string>(new string[] { "RES" }));
    }

    if (!ParameterSet.ContainsKey("SUMRES") || ParameterSet.ContainsItem("SUMRES", "LOCATION"))
    {
      ParameterSet.AddSet("SUMRES", new SortedSet<string>(new string[] { "COUNTRY" }));
    }

    if (! ParameterSet.ContainsKey("SUMOGN"))
    {
      ParameterSet.AddSet("SUMOGN", new SortedSet<string>(new string[] { "COUNTRY" }));
    }

    if (! ParameterSet.ContainsKey("POP_TYPES"))
    {
      ParameterSet.AddSet("POP_TYPES", new SortedSet<string>(
        new string[] { "RFT","AS","RT","IDT","RD","ST","OC","TPOC" }));
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
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    ParameterSet.SaveToViewState(ViewState);

    dsYears.SelectCommand = dsYears_SelectCommand;

    if (ParameterSet.ContainsKey("RESSTYLE"))
    {
      rblCS.SelectedValue = ParameterSet["RESSTYLE"].Max;
    }
    hfC.Value = rblCS.SelectedValue;

    switch (rblCS.SelectedValue)
    {
      case "UNSD":
        dsCountries.SelectCommand = dsCountries_UNSDTree_SelectCommand;
        break;
      case "UNHCR":
        dsCountries.SelectCommand = dsCountries_UNHCRTree_SelectCommand;
        break;
      case "LIST":
        dsCountries.SelectCommand = dsCountries_CountryList_SelectCommand;
        break;
    }

    if (ParameterSet.ContainsKey("OGNSTYLE"))
    {
      rblOS.SelectedValue = ParameterSet["OGNSTYLE"].Max;
    }
    hfO.Value = rblOS.SelectedValue;

    switch (rblOS.SelectedValue)
    {
      case "UNSD":
        dsOrigins.SelectCommand = dsOrigins_UNSDTree_SelectCommand;
        break;
      case "UNHCR":
        dsOrigins.SelectCommand = dsOrigins_UNHCRTree_SelectCommand;
        break;
      case "LIST":
        dsOrigins.SelectCommand = dsOrigins_CountryList_SelectCommand;
        break;
    }

    if (ParameterSet.ContainsKey("BREAKDOWN"))
    {
      foreach (ListItem item in cblBD.Items)
      {
        item.Selected = ParameterSet["BREAKDOWN"].Contains(item.Value);
      }
    }

    if (ParameterSet.ContainsKey("SUMRES"))
    {
      ddlRS.SelectedValue = ParameterSet["SUMRES"].Max;
    }

    if (ParameterSet.ContainsKey("SUMOGN"))
    {
      ddlOG.SelectedValue = ParameterSet["SUMOGN"].Max;
    }

    if (ParameterSet.ContainsKey("POP_TYPES"))
    {
      foreach (ListItem item in cblPT.Items)
      {
        item.Selected = ParameterSet["POP_TYPES"].Contains(item.Value);
      }
    }

    if (ParameterSet.ContainsKey("INCLUDE"))
    {
      foreach (ListItem item in cblIN.Items)
      {
        item.Selected = ParameterSet["INCLUDE"].Contains(item.Value);
      }
    }
  }

  protected void lbYL_DataBound(object sender, EventArgs e)
  {
    int maxRows = 30;
    lbYL.Rows = (lbYL.Items.Count > maxRows) ? maxRows : lbYL.Items.Count;

    if (ParameterSet.ContainsKey("YEAR"))
    {
      foreach (string year in ParameterSet["YEAR"])
      {
        ListItem item = lbYL.Items.FindByValue(year);
        if (item != null)
        {
          item.Selected = true;
        }
      }
    }
    else
    {
      lbYL.Items[0].Selected = true;
      ParameterSet.AddItem("YEAR", lbYL.Items[0].Value);
    }
  }

  protected void lvC_DataBound(object sender, EventArgs e)
  {
    if (ParameterSet.ContainsKey("RESSTYLE"))
    {
      lvC.FindControl("dvSC").Visible = lvC.FindControl("lbSC").Visible =
        (ParameterSet["RESSTYLE"].Max == "LIST");
    }

    if (ParameterSet.ContainsKey("RESSTATE_" + hfC.Value))
    {
      var set = ParameterSet["RESSTATE_" + hfC.Value];
      foreach (ListViewDataItem item in lvC.Items)
      {
        var checkBox = item.FindControl("cbCT") as CheckBox;
        if (checkBox != null)
        {
          checkBox.Checked = set.Contains(lvC.DataKeys[item.DisplayIndex].Value.ToString());
        }
      }
    }

    if (ParameterSet.ContainsKey("RESFILTER"))
    {
      ((TextBox)lvC.FindControl("tbSC")).Text = ParameterSet["RESFILTER"].Max;
    }

    if (ParameterSet.ContainsKey("RES"))
    {
      var set = ParameterSet["RES"];
      foreach (ListViewDataItem item in lvC.Items)
      {
        var checkBox = item.FindControl("cbCS") as CheckBox;
        if (checkBox != null)
        {
          checkBox.Checked = set.Contains(lvC.DataKeys[item.DisplayIndex].Value.ToString());
        }
      }
    }
  }

  protected void lvO_DataBound(object sender, EventArgs e)
  {
    if (ParameterSet.ContainsKey("OGNSTYLE"))
    {
      lvO.FindControl("dvSO").Visible = lvO.FindControl("lbSO").Visible =
        (ParameterSet["OGNSTYLE"].Max == "LIST");
    }

    if (ParameterSet.ContainsKey("OGNSTATE_" + hfO.Value))
    {
      var set = ParameterSet["OGNSTATE_" + hfO.Value];
      foreach (ListViewDataItem item in lvO.Items)
      {
        var checkBox = item.FindControl("cbOT") as CheckBox;
        if (checkBox != null)
        {
          checkBox.Checked = set.Contains(lvO.DataKeys[item.DisplayIndex].Value.ToString());
        }
      }
    }

    if (ParameterSet.ContainsKey("OGNFILTER"))
    {
      ((TextBox)lvO.FindControl("tbSO")).Text = ParameterSet["OGNFILTER"].Max;
    }

    if (ParameterSet.ContainsKey("OGN"))
    {
      var set = ParameterSet["OGN"];
      foreach (ListViewDataItem item in lvO.Items)
      {
        var checkBox = item.FindControl("cbOS") as CheckBox;
        if (checkBox != null)
        {
          checkBox.Checked = set.Contains(lvO.DataKeys[item.DisplayIndex].Value.ToString());
        }
      }
    }
  }

  protected void lbYL_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (lbYL.SelectedIndex == -1)
    {
      lbYL.SelectedIndex = 0;
    }

    ParameterSet.AddSet(
      "YEAR",
      new SortedSet<string>(
        lbYL.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
  }

  protected void rblCS_SelectedIndexChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "RESSTYLE",
      new SortedSet<string>(new string[] { rblCS.SelectedValue }));
  }

  protected void cbCT_CheckedChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "RESSTATE_" + hfC.Value,
      new SortedSet<string>(
        lvC.Items.
        Where(x => x.FindControl("cbCT") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbCT")).Checked).
        Select(x => lvC.DataKeys[x.DisplayIndex].Value.ToString())));
  }

  protected void tbSC_TextChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "RESFILTER",
      new SortedSet<string>(new string[] { ((TextBox)sender).Text }));
  }

  protected void cbCS_CheckedChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "RES",
      new SortedSet<string>(
        lvC.Items.
        Where(x => lvC.DataKeys[x.DisplayIndex].Values[2].ToString() == "leaf" &&
          x.FindControl("cbCS") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbCS")).Checked).
        Select(x => lvC.DataKeys[x.DisplayIndex].Value.ToString())));

    ParameterSet.AddSet(
      "RESNAMES",
      new SortedSet<string>(
        lvC.Items.
        Where(x => lvC.DataKeys[x.DisplayIndex].Values[2].ToString() == "leaf" &&
          x.FindControl("cbCS") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbCS")).Checked).
        Select(x => lvC.DataKeys[x.DisplayIndex].Values[1].ToString()).
        Take(5)));
  }

  protected void rblOS_SelectedIndexChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "OGNSTYLE",
      new SortedSet<string>(new string[] { rblOS.SelectedValue }));
  }

  protected void cbOT_CheckedChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "OGNSTATE_" + hfO.Value,
      new SortedSet<string>(
        lvO.Items.
        Where(x => x.FindControl("cbOT") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbOT")).Checked).
        Select(x => lvO.DataKeys[x.DisplayIndex].Value.ToString())));
  }

  protected void tbSO_TextChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "OGNFILTER",
      new SortedSet<string>(new string[] { ((TextBox)sender).Text }));
  }

  protected void cbOS_CheckedChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "OGN",
      new SortedSet<string>(
        lvO.Items.
        Where(x => lvO.DataKeys[x.DisplayIndex].Values[2].ToString() == "leaf" &&
          x.FindControl("cbOS") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbOS")).Checked).
        Select(x => lvO.DataKeys[x.DisplayIndex].Value.ToString())));

    ParameterSet.AddSet(
      "OGNNAMES",
      new SortedSet<string>(
        lvO.Items.
        Where(x => lvO.DataKeys[x.DisplayIndex].Values[2].ToString() == "leaf" &&
          x.FindControl("cbOS") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbOS")).Checked).
        Select(x => lvO.DataKeys[x.DisplayIndex].Values[1].ToString()).
        Take(5)));
  }

  protected void cblBD_SelectedIndexChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "BREAKDOWN",
      new SortedSet<string>(
        cblBD.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
  }

  protected void cblPT_SelectedIndexChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "POP_TYPES",
      new SortedSet<string>(
        cblPT.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
  }

  protected void ddlRS_SelectedIndexChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet("SUMRES", new SortedSet<string>(new string[] { ddlRS.SelectedValue }));
  }

  protected void ddlOG_SelectedIndexChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet("SUMOGN", new SortedSet<string>(new string[] { ddlOG.SelectedValue }));
  }

  protected void cblIN_SelectedIndexChanged(object sender, EventArgs e)
  {
    ParameterSet.AddSet(
      "INCLUDE",
      new SortedSet<string>(
        cblIN.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
  }
}