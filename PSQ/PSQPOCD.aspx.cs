using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PSQPOCD : System.Web.UI.Page, IQueryParameters
{
  public QueryParameters Parameters { get; set; }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (IsPostBack)
    {
      Parameters = new QueryParameters(ViewState);
    }
    else if (PreviousPage == null)
    {
      Parameters = new QueryParameters();
    }
    else
    {
      Parameters = (PreviousPage as IQueryParameters).Parameters;

      if (Parameters.Parameters != null)
      {
        foreach (string key in Parameters.Parameters.Keys)
        {
          string sep = key + ": ";
          foreach (string item in Parameters.Parameters[key])
          {
            Label1.Text += sep + item;
            sep = ", ";
          }
          Label1.Text += "<br />";
        }
      }

      //if (Parameters.Parameters != null && Parameters.Parameters.ContainsKey("YEAR"))
      //{
      //  string sep = "YEAR: ";
      //  foreach (string item in Parameters.Parameters["YEAR"])
      //  {
      //    Label1.Text += sep + item;
      //    sep = ",";
      //  }
      //  Label1.Text += "<br />";
      //}

      //if (Parameters.Parameters != null && Parameters.Parameters.ContainsKey("RES"))
      //{
      //  string sep = "RES: ";
      //  foreach (string item in Parameters.Parameters["RES"])
      //  {
      //    Label1.Text += sep + item;
      //    sep = ",";
      //  }
      //  Label1.Text += "<br />";
      //}
    }
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    Parameters.SaveToViewState(ViewState);
  }
}