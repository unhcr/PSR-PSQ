﻿using System;
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
  public QueryParameters Parameters { get; set; }

  #region SQLstatements
  private string dsYears_SelectCommand =
@"select ASR_YEAR from PSQ_POC_YEARS order by ASR_YEAR desc";

  private string dsCountries_CountryList_SelectCommand =
@"select CODE, NAME,
  case when ROW_NUMBER = 1 then '<div class=""list-select""><ul>' end || '<li>' as PREFIX,
  '</li>' || case when ROW_NUMBER = ROW_COUNT then '</ul></div>' end as SUFFIX,
  'leaf' as NODETYPE,
  'false' as EXPANDED
from
 (select CODE, NAME,
    row_number() over (order by SORT_NAME) as ROW_NUMBER,
    count(*) over () as ROW_COUNT,
    SORT_NAME
  from COUNTRY_SELECTION
  where CODE in (select COU_CODE_RESIDENCE from PSQ_POC_SUMMARY_DATA))
order by SORT_NAME";

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
from
 (select CODE, NAME, LOCT_CODE, TREE_LEVEL,
    lag(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as PREV_TREE_LEVEL,
    lead(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as NEXT_TREE_LEVEL,
    row_number() over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as ROW_NUMBER,
    count(*) over () as ROW_COUNT,
    ORDER_SEQ, SORT_NAME
  from
   (select ID, CODE, NAME, LOCT_CODE, TREE_LEVEL,
      ORDER_SEQ, null as SORT_NAME
    from UNSD_REGION_TREE REG
    union all
    select COU.ID, COU.CODE, COU.NAME, COU.LOCT_CODE, REG.TREE_LEVEL + 1 as TREE_LEVEL,
      REG.ORDER_SEQ, COU.SORT_NAME
    from UNSD_REGION_TREE REG
    inner join T_LOCATION_RELATIONSHIPS LOCR
      on LOCR.LOC_ID_FROM = REG.ID
      and LOCR.LOCRT_CODE = 'UNSD'
    inner join COUNTRY_SELECTION COU
      on COU.ID = LOCR.LOC_ID_TO
    where COU.CODE in (select COU_CODE_RESIDENCE from PSQ_POC_SUMMARY_DATA)))
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
from
 (select CODE, NAME, LOCT_CODE, TREE_LEVEL,
    lead(LOCT_CODE) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as NEXT_LOCT_CODE,
    lag(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as PREV_TREE_LEVEL,
    lead(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as NEXT_TREE_LEVEL,
    row_number() over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as ROW_NUMBER,
    count(*) over () as ROW_COUNT,
    ORDER_SEQ, SORT_NAME
  from
   (select ID, CODE, NAME, LOCT_CODE, TREE_LEVEL, ORDER_SEQ, null as SORT_NAME
    from UNHCR_REGION_TREE REG
    union all
    select COU.ID, COU.CODE, COU.NAME, COU.LOCT_CODE, REG.TREE_LEVEL + 1 as TREE_LEVEL,
      REG.ORDER_SEQ, COU.SORT_NAME
    from UNHCR_REGION_TREE REG
    inner join T_LOCATION_RELATIONSHIPS LOCR
      on LOCR.LOC_ID_FROM = REG.ID
      and LOCR.LOCRT_CODE = 'HCRRESP'
    inner join COUNTRY_SELECTION COU
      on COU.ID = LOCR.LOC_ID_TO
      and COU.CODE in (select COU_CODE_RESIDENCE from PSQ_POC_SUMMARY_DATA)))
order by ORDER_SEQ, SORT_NAME nulls first, NAME";

  private string dsOrigins_CountryList_SelectCommand =
@"select CODE, NAME,
  case when ROW_NUMBER = 1 then '<div class=""list-select""><ul>' end || '<li>' as PREFIX,
  '</li>' || case when ROW_NUMBER = ROW_COUNT then '</ul></div>' end as SUFFIX,
  'leaf' as NODETYPE,
  'false' as EXPANDED
from
 (select CODE, NAME,
    row_number() over (order by SORT_NAME) as ROW_NUMBER,
    count(*) over () as ROW_COUNT,
    SORT_NAME
  from ORIGIN_SELECTION
  where CODE in (select COU_CODE_ORIGIN from PSQ_POC_SUMMARY_DATA))
order by SORT_NAME";

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
  case when LOCT_CODE not in ('COUNTRY', 'OTHORIGIN') and TREE_LEVEL <= 2 then 'true' else 'false' end as EXPANDED
from
 (select CODE, NAME, LOCT_CODE, TREE_LEVEL,
    lag(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as PREV_TREE_LEVEL,
    lead(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as NEXT_TREE_LEVEL,
    row_number() over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as ROW_NUMBER,
    count(*) over () as ROW_COUNT,
    ORDER_SEQ, SORT_NAME
  from
   (select ID, CODE, NAME, LOCT_CODE, TREE_LEVEL,
      ORDER_SEQ, null as SORT_NAME
    from UNSD_REGION_TREE REG
    union all
    select OGN.ID, OGN.CODE, OGN.NAME, OGN.LOCT_CODE, REG.TREE_LEVEL + 1 as TREE_LEVEL,
      REG.ORDER_SEQ, OGN.SORT_NAME
    from UNSD_REGION_TREE REG
    inner join T_LOCATION_RELATIONSHIPS LOCR
      on LOCR.LOC_ID_FROM = REG.ID
      and LOCR.LOCRT_CODE = 'UNSD'
    inner join ORIGIN_SELECTION OGN
      on OGN.ID = LOCR.LOC_ID_TO
    where OGN.CODE in (select COU_CODE_ORIGIN from PSQ_POC_SUMMARY_DATA)))
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
  case when LOCT_CODE = 'COUNTRY' then 'leaf' else 'toggle' end as NODETYPE,
  case
    when LOCT_CODE not in ('COUNTRY', 'OTHORIGIN') and NEXT_LOCT_CODE not in ('COUNTRY', 'OTHORIGIN')
    then 'true'
    else 'false'
  end as EXPANDED
from
 (select CODE, NAME, LOCT_CODE, TREE_LEVEL,
    lead(LOCT_CODE) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as NEXT_LOCT_CODE,
    lag(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as PREV_TREE_LEVEL,
    lead(TREE_LEVEL, 1, 0) over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as NEXT_TREE_LEVEL,
    row_number() over (order by ORDER_SEQ, SORT_NAME nulls first, NAME) as ROW_NUMBER,
    count(*) over () as ROW_COUNT,
    ORDER_SEQ, SORT_NAME
  from
   (select ID, CODE, NAME, LOCT_CODE, TREE_LEVEL, ORDER_SEQ, null as SORT_NAME
    from UNHCR_REGION_TREE REG
    union all
    select COU.ID, COU.CODE, COU.NAME, COU.LOCT_CODE, REG.TREE_LEVEL + 1 as TREE_LEVEL, REG.ORDER_SEQ, COU.SORT_NAME
    from UNHCR_REGION_TREE REG
    inner join T_LOCATION_RELATIONSHIPS LOCR
      on LOCR.LOC_ID_FROM = REG.ID
      and LOCR.LOCRT_CODE = 'HCRRESP'
    inner join ORIGIN_SELECTION COU
      on COU.ID = LOCR.LOC_ID_TO
      and COU.CODE in (select COU_CODE_ORIGIN from PSQ_POC_SUMMARY_DATA)))
order by ORDER_SEQ, SORT_NAME nulls first, NAME";
  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {
    if (IsPostBack)
    {
      Parameters = new QueryParameters(ViewState);
    }
    else if (PreviousPage == null)
    {
      Parameters = new QueryParameters();
      Parameters.AddSet(
        "BREAKDOWN",
        new SortedSet<string>(new string[] { "RES","OGN" }));
      Parameters.AddSet(
        "POP_TYPES",
        new SortedSet<string>(new string[] { "REF","ASY","RET","IDP","RDP","STA","OOC","POC" }));
    }
    else
    {
      Parameters = (PreviousPage as IQueryParameters).Parameters;
    }
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    Parameters.SaveToViewState(ViewState);

    dsYears.SelectCommand = dsYears_SelectCommand;

    if (Parameters.ContainsKey("RESSTYLE"))
    {
      rblCS.SelectedValue = Parameters["RESSTYLE"].Max;
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

    if (Parameters.ContainsKey("OGNSTYLE"))
    {
      rblOS.SelectedValue = Parameters["OGNSTYLE"].Max;
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

    if (Parameters.ContainsKey("BREAKDOWN"))
    {
      foreach (ListItem item in cblBD.Items)
      {
        item.Selected = Parameters["BREAKDOWN"].Contains(item.Value);
      }
    }

    if (Parameters.ContainsKey("POP_TYPES"))
    {
      foreach (ListItem item in cblPT.Items)
      {
        item.Selected = Parameters["POP_TYPES"].Contains(item.Value);
      }
    }
  }

  protected void lbYL_DataBound(object sender, EventArgs e)
  {
    int maxRows = 30;
    lbYL.Rows = (lbYL.Items.Count > maxRows) ? maxRows : lbYL.Items.Count;

    if (Parameters.ContainsKey("YEAR"))
    {
      foreach (string year in Parameters["YEAR"])
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
      Parameters.AddItem("YEAR", lbYL.Items[0].Value);
    }
  }

  protected void lvC_DataBound(object sender, EventArgs e)
  {
    if (Parameters.ContainsKey("RESSTYLE"))
    {
      lvC.FindControl("dvSC").Visible = lvC.FindControl("lbSC").Visible =
        (Parameters["RESSTYLE"].Max == "LIST");
    }

    if (Parameters.ContainsKey("RESSTATE_" + hfC.Value))
    {
      var set = Parameters["RESSTATE_" + hfC.Value];
      foreach (ListViewDataItem item in lvC.Items)
      {
        var checkBox = item.FindControl("cbCT") as CheckBox;
        if (checkBox != null)
        {
          checkBox.Checked = set.Contains(lvC.DataKeys[item.DisplayIndex].Value.ToString());
        }
      }
    }

    if (Parameters.ContainsKey("RESFILTER"))
    {
      ((TextBox)lvC.FindControl("tbSC")).Text = Parameters["RESFILTER"].Max;
    }

    if (Parameters.ContainsKey("RES"))
    {
      var set = Parameters["RES"];
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
    if (Parameters.ContainsKey("OGNSTYLE"))
    {
      lvO.FindControl("dvSO").Visible = lvO.FindControl("lbSO").Visible =
        (Parameters["OGNSTYLE"].Max == "LIST");
    }

    if (Parameters.ContainsKey("OGNSTATE_" + hfO.Value))
    {
      var set = Parameters["OGNSTATE_" + hfO.Value];
      foreach (ListViewDataItem item in lvO.Items)
      {
        var checkBox = item.FindControl("cbOT") as CheckBox;
        if (checkBox != null)
        {
          checkBox.Checked = set.Contains(lvO.DataKeys[item.DisplayIndex].Value.ToString());
        }
      }
    }

    if (Parameters.ContainsKey("OGNFILTER"))
    {
      ((TextBox)lvO.FindControl("tbSO")).Text = Parameters["OGNFILTER"].Max;
    }

    if (Parameters.ContainsKey("OGN"))
    {
      var set = Parameters["OGN"];
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
    Parameters.AddSet(
      "YEAR",
      new SortedSet<string>(
        lbYL.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
  }

  protected void rblCS_SelectedIndexChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "RESSTYLE",
      new SortedSet<string>(new string[] { rblCS.SelectedValue }));
  }

  protected void cbCT_CheckedChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "RESSTATE_" + hfC.Value,
      new SortedSet<string>(
        lvC.Items.
        Where(x => x.FindControl("cbCT") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbCT")).Checked).
        Select(x => lvC.DataKeys[x.DisplayIndex].Value.ToString())));
  }

  protected void tbSC_TextChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "RESFILTER",
      new SortedSet<string>(new string[] { ((TextBox)sender).Text }));
  }

  protected void cbCS_CheckedChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "RES",
      new SortedSet<string>(
        lvC.Items.
        Where(x => x.FindControl("cbCS") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbCS")).Checked).
        Select(x => lvC.DataKeys[x.DisplayIndex].Value.ToString()).
        Where(x => x[0] != '*')));
  }

  protected void rblOS_SelectedIndexChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "OGNSTYLE",
      new SortedSet<string>(new string[] { rblOS.SelectedValue }));
  }

  protected void cbOT_CheckedChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "OGNSTATE_" + hfO.Value,
      new SortedSet<string>(
        lvO.Items.
        Where(x => x.FindControl("cbOT") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbOT")).Checked).
        Select(x => lvO.DataKeys[x.DisplayIndex].Value.ToString())));
  }

  protected void tbSO_TextChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "OGNFILTER",
      new SortedSet<string>(new string[] { ((TextBox)sender).Text }));
  }

  protected void cbOS_CheckedChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "OGN",
      new SortedSet<string>(
        lvO.Items.
        Where(x => x.FindControl("cbOS") as CheckBox != null &&
          ((CheckBox)x.FindControl("cbOS")).Checked).
        Select(x => lvO.DataKeys[x.DisplayIndex].Value.ToString()).
        Where(x => x[0] != '*')));
  }

  protected void cblBD_SelectedIndexChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "BREAKDOWN",
      new SortedSet<string>(
        cblBD.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
  }

  protected void cblPT_SelectedIndexChanged(object sender, EventArgs e)
  {
    Parameters.AddSet(
      "POP_TYPES",
      new SortedSet<string>(
        cblPT.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value)));
  }

  protected void btSb_Click(object sender, EventArgs e)
  {
  }
}