using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PSQ_FRS : System.Web.UI.Page
{

  protected void lbxCOUNTRY_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (lbxCOUNTRY.SelectedValue != "0")
    {
      Response.Redirect("PSQ_TMS.aspx?SYR=2000&EYR=2012&RES=" + lbxCOUNTRY.SelectedValue + "&POPT=RF&DRES=N&DPOPT=N");
    }
  }

  protected void lbxORIGIN_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (lbxORIGIN.SelectedValue != "0")
    {
      Response.Redirect("PSQ_TMS.aspx?SYR=2000&EYR=2012&OGN=" + lbxORIGIN.SelectedValue + "&POPT=RF&DOGN=N&DPOPT=N");
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

}