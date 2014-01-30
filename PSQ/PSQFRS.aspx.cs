using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PSQFRS : System.Web.UI.Page, IQueryParameters
{
  public QueryParameters ParameterSet { get; set; }

  private SortedSet<string> GetYears()
  {
    return new SortedSet<string>(lbYL.Items.Cast<ListItem>().Select(x => x.Value));
  }

  private void RefugeesInCountry(SortedSet<string> countryCodes, string countryName)
  {
    ParameterSet.AddSet("RES", countryCodes);
    ParameterSet.AddSet("RESNAMES", new SortedSet<string>(new string[] { countryName }));
    ParameterSet.AddSet("YEAR", GetYears());
    ParameterSet.RemoveSet("OGN");
    ParameterSet.AddSet("POPT", new SortedSet<string>(new string[] { "RF", "RL" }));
    ParameterSet.AddSet("BREAKDOWN", new SortedSet<string>(new string[] { "OGN" }));
    ParameterSet.AddSet("SUMOGN", new SortedSet<string>(new string[] { "COUNTRY" }));
  }

  private void RefugeesFromOrigin(SortedSet<string> originCodes, string originName)
  {
    ParameterSet.AddSet("OGN", originCodes);
    ParameterSet.AddSet("OGNNAMES", new SortedSet<string>(new string[] { originName }));
    ParameterSet.AddSet("YEAR", GetYears());
    ParameterSet.RemoveSet("RES");
    ParameterSet.AddSet("POPT", new SortedSet<string>(new string[] { "RF", "RL" }));
    ParameterSet.AddSet("BREAKDOWN", new SortedSet<string>(new string[] { "RES" }));
    ParameterSet.AddSet("SUMRES", new SortedSet<string>(new string[] { "COUNTRY" }));
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
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    ParameterSet.SaveToViewState(ViewState);

    lbYL.DataBind();
  }

  protected void lbxCOUNTRY_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (lbxCOUNTRY.SelectedValue != "0")
    {
      RefugeesInCountry(new SortedSet<string>(new string[] { lbxCOUNTRY.SelectedValue }), lbxCOUNTRY.SelectedItem.Text);
      ParameterSet.SaveToViewState(ViewState);
      Server.Transfer("PSQTMSD.aspx");
    }
  }

  protected void lbxORIGIN_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (lbxORIGIN.SelectedValue != "0")
    {
      RefugeesFromOrigin(new SortedSet<string>(new string[] { lbxORIGIN.SelectedValue }), lbxORIGIN.SelectedItem.Text);
      ParameterSet.SaveToViewState(ViewState);
      Server.Transfer("PSQTMSD.aspx");
    }
  }

  protected void lbxCOUNTRY_DataBound(object sender, EventArgs e)
  {
    lbxCOUNTRY.Items.Insert(0, new ListItem { Text = "– Select country / territory of asylum –", Value = "0", Selected = true });
  }

  protected void lbxORIGIN_DataBound(object sender, EventArgs e)
  {
    lbxORIGIN.Items.Insert(0, new ListItem { Text = "– Select origin – ", Value = "0", Selected = true });
  }

  protected void RefugeeAsylum_Command(object sender, CommandEventArgs e)
  {
    RefugeesInCountry(new SortedSet<string>(((string)e.CommandArgument).Split(',')), ((LinkButton)sender).Text);
  }

  protected void RefugeeOrigin_Command(object sender, CommandEventArgs e)
  {
    RefugeesFromOrigin(new SortedSet<string>(((string)e.CommandArgument).Split(',')), ((LinkButton)sender).Text);
  }

  protected void lbtIDP_Click(object sender, EventArgs e)
  {
    ParameterSet.RemoveSet("RES");
    ParameterSet.RemoveSet("OGN");
    ParameterSet.AddSet("YEAR", GetYears());
    ParameterSet.AddSet("POPT", new SortedSet<string>(new string[] { "ID", "IL" }));
    ParameterSet.AddSet("BREAKDOWN", new SortedSet<string>(new string[] { "RES" }));
    ParameterSet.AddSet("SUMRES", new SortedSet<string>(new string[] { "COUNTRY" }));
  }

  protected void lbtSTA_Click(object sender, EventArgs e)
  {
    ParameterSet.RemoveSet("RES");
    ParameterSet.RemoveSet("OGN");
    ParameterSet.AddSet("YEAR", GetYears());
    ParameterSet.AddSet("POPT", new SortedSet<string>(new string[] { "ST" }));
    ParameterSet.AddSet("BREAKDOWN", new SortedSet<string>(new string[] { "RES" }));
    ParameterSet.AddSet("SUMRES", new SortedSet<string>(new string[] { "COUNTRY" }));
  }
}