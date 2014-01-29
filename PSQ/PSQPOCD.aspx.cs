using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PSQPOCD : System.Web.UI.Page, IQueryParameters
{
  public QueryParameters ParameterSet { get; set; }

  private int[] largePageSizes = { 10, 25, 50, 100, 250, 500, 0 };
  private int[] smallPageSizes = { 5, 12, 25, 50, 100, 250, 0 };

  struct ColumnTemplate
  {
    public readonly string key1;
    public readonly string item1;
    public readonly string key2;
    public readonly string item2;
    public readonly bool isNumeric;
    public readonly string title;

    public ColumnTemplate(string key1, string item1, string key2, string item2, bool isNumeric, string title)
    {
      this.key1 = key1;
      this.item1 = item1;
      this.key2 = key2;
      this.item2 = item2;
      this.isNumeric = isNumeric;
      this.title = title;
    }
  }

  private static readonly IList<ColumnTemplate> columnTemplateArray = new ReadOnlyCollection<ColumnTemplate>
    (new[] {
      new ColumnTemplate("", "", "", "", true, "Year"),
      new ColumnTemplate("BREAKDOWN", "RES", "", "", false, "Residing in"),
      new ColumnTemplate("BREAKDOWN", "OGN", "", "", false, "Originating / returned from"),
      new ColumnTemplate("POP_TYPES", "RF", "", "", true, "Refugees"),
      new ColumnTemplate("POP_TYPES", "RL", "", "", true, "Refugee-like"),
      new ColumnTemplate("POP_TYPES", "RFT", "", "", true, "Refugees (including ref.-like)"),
      new ColumnTemplate("POP_TYPES", "AS", "", "", true, "Asylum-seekers"),
      new ColumnTemplate("POP_TYPES", "RT", "", "", true, "Returned refugees"),
      new ColumnTemplate("POP_TYPES", "ID", "", "", true, "IDPs"),
      new ColumnTemplate("POP_TYPES", "IL", "", "", true, "IDP-like"),
      new ColumnTemplate("POP_TYPES", "IDT", "", "", true, "IDPs (including IDP-like)"),
      new ColumnTemplate("POP_TYPES", "RD", "", "", true, "Returned IDPs"),
      new ColumnTemplate("POP_TYPES", "ST", "", "", true, "Stateless persons"),
      new ColumnTemplate("POP_TYPES", "OC", "", "", true, "Others of concern"),
      new ColumnTemplate("POP_TYPES", "TPOC", "", "", true, "Total population"),
      new ColumnTemplate("POP_TYPES", "RF", "INCLUDE", "HCRASS", true, "Refugees (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "RL", "INCLUDE", "HCRASS", true, "Refugee-like (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "RFT", "INCLUDE", "HCRASS", true, "Refugees (including ref.-like) (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "AS", "INCLUDE", "HCRASS", true, "Asylum-seekers (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "RT", "INCLUDE", "HCRASS", true, "Returned refugees (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "ID", "INCLUDE", "HCRASS", true, "IDPs (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "IL", "INCLUDE", "HCRASS", true, "IDP-like (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "IDT", "INCLUDE", "HCRASS", true, "IDPs (including IDP-like) (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "RD", "INCLUDE", "HCRASS", true, "Returned IDPs (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "ST", "INCLUDE", "HCRASS", true, "Stateless persons (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "OC", "INCLUDE", "HCRASS", true, "Others of concern (UNHCR-assisted)"),
      new ColumnTemplate("POP_TYPES", "TPOC", "INCLUDE", "HCRASS", true, "Total population (UNHCR-assisted)")
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
      if ((template.key1 == "" || ParameterSet.ContainsItem(template.key1, template.item1)) &&
        (template.key2 == "" || ParameterSet.ContainsItem(template.key2, template.item2)))
      {
        csv.Append(sep + template.title);
        sep = ",";
      }
    }
    csv.AppendLine();

    ConstructSelectStatement();

    int i;
    foreach (DataRow row in ((DataView)dsASR_POC_SUMMARY.Select(DataSourceSelectArguments.Empty)).ToTable().Rows)
    {
      i = 0;
      sep = "";
      foreach (ColumnTemplate template in columnTemplateArray)
      {
        if ((template.key1 == "" || ParameterSet.ContainsItem(template.key1, template.item1)) &&
          (template.key2 == "" || ParameterSet.ContainsItem(template.key2, template.item2)))
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
            if (((String)(row.ItemArray[i])).Contains(","))
            {
              csv.Append(sep + "\"" + ((String)(row.ItemArray[i])).Replace("\"", "\"\"") + "\"");
            }
            else
            {
              csv.Append(sep + row.ItemArray[i]);
            }
          }
          sep = ",";
        }
        i++;
      }
      csv.AppendLine();
    }

    Response.Clear();
    Response.AddHeader("content-disposition", "attachment; filename=PSQPOC.csv");
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
    var selectStatement = new StringBuilder(10000);

    selectStatement.Append(@"
select to_char(ASR_YEAR) as ASR_YEAR,
  COU_NAME_RESIDENCE,
  COU_NAME_ORIGIN,
  case
    when RF_VALUE is null and RF_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RF_VALUE, '999,999,999'))
  end as RF_VALUE,
  case
    when RL_VALUE is null and RL_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RL_VALUE, '999,999,999'))
  end as RL_VALUE,
  case
    when RFT_VALUE is null and RFT_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RFT_VALUE, '999,999,999'))
  end as RFT_VALUE,
  case
    when AS_VALUE is null and AS_REDACTED_FLAG = 1 then '*'
    else trim(to_char(AS_VALUE, '999,999,999'))
  end as AS_VALUE,
  case
    when RT_VALUE is null and RT_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RT_VALUE, '999,999,999'))
  end as RT_VALUE,
  case
    when ID_VALUE is null and ID_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ID_VALUE, '999,999,999'))
  end as ID_VALUE,
  case
    when IL_VALUE is null and IL_REDACTED_FLAG = 1 then '*'
    else trim(to_char(IL_VALUE, '999,999,999'))
  end as IL_VALUE,
  case
    when IDT_VALUE is null and IDT_REDACTED_FLAG = 1 then '*'
    else trim(to_char(IDT_VALUE, '999,999,999'))
  end as IDT_VALUE,
  case
    when RD_VALUE is null and RD_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RD_VALUE, '999,999,999'))
  end as RD_VALUE,
  case
    when ST_VALUE is null and ST_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ST_VALUE, '999,999,999'))
  end as ST_VALUE,
  case
    when OC_VALUE is null and OC_REDACTED_FLAG = 1 then '*'
    else trim(to_char(OC_VALUE, '999,999,999'))
  end as OC_VALUE,
  case
    when TPOC_VALUE is null and TPOC_REDACTED_FLAG = 1 then '*'
    else trim(to_char(TPOC_VALUE, '999,999,999'))
  end as TPOC_VALUE,
  case
    when RFA_VALUE is null and RFA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RFA_VALUE, '999,999,999'))
  end as RFA_VALUE,
  case
    when RLA_VALUE is null and RLA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RLA_VALUE, '999,999,999'))
  end as RLA_VALUE,
  case
    when RFTA_VALUE is null and RFTA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RFTA_VALUE, '999,999,999'))
  end as RFTA_VALUE,
  case
    when ASA_VALUE is null and ASA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ASA_VALUE, '999,999,999'))
  end as ASA_VALUE,
  case
    when RTA_VALUE is null and RTA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RTA_VALUE, '999,999,999'))
  end as RTA_VALUE,
  case
    when IDA_VALUE is null and IDA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(IDA_VALUE, '999,999,999'))
  end as IDA_VALUE,
  case
    when ILA_VALUE is null and ILA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(ILA_VALUE, '999,999,999'))
  end as ILA_VALUE,
  case
    when IDTA_VALUE is null and IDTA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(IDTA_VALUE, '999,999,999'))
  end as IDTA_VALUE,
  case
    when RDA_VALUE is null and RDA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(RDA_VALUE, '999,999,999'))
  end as RDA_VALUE,
  case
    when STA_VALUE is null and STA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(STA_VALUE, '999,999,999'))
  end as STA_VALUE,
  case
    when OCA_VALUE is null and OCA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(OCA_VALUE, '999,999,999'))
  end as OCA_VALUE,
  case
    when TPOCA_VALUE is null and TPOCA_REDACTED_FLAG = 1 then '*'
    else trim(to_char(TPOCA_VALUE, '999,999,999'))
  end as TPOCA_VALUE
from
 (select ASR_YEAR, COU_NAME_RESIDENCE, ORDER_SEQ_RESIDENCE, COU_NAME_ORIGIN, ORDER_SEQ_ORIGIN,");

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "RF",
        "sum(RF_VALUE) as RF_VALUE, sign(sum(RF_REDACTED_FLAG)) as RF_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(RFA_VALUE) as RFA_VALUE, sign(sum(RFA_REDACTED_FLAG)) as RFA_REDACTED_FLAG, ",
          "null as RFA_VALUE, null as RFA_REDACTED_FLAG, "),
        "null as RF_VALUE, null as RF_REDACTED_FLAG, null as RFA_VALUE, null as RFA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "RL",
        "sum(RL_VALUE) as RL_VALUE, sign(sum(RL_REDACTED_FLAG)) as RL_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(RLA_VALUE) as RLA_VALUE, sign(sum(RLA_REDACTED_FLAG)) as RLA_REDACTED_FLAG, ",
          "null as RLA_VALUE, null as RLA_REDACTED_FLAG, "),
        "null as RL_VALUE, null as RL_REDACTED_FLAG, null as RLA_VALUE, null as RLA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "RFT",
        "sum(RFT_VALUE) as RFT_VALUE, sign(sum(RFT_REDACTED_FLAG)) as RFT_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(RFTA_VALUE) as RFTA_VALUE, sign(sum(RFTA_REDACTED_FLAG)) as RFTA_REDACTED_FLAG, ",
          "null as RFTA_VALUE, null as RFTA_REDACTED_FLAG, "),
        "null as RFT_VALUE, null as RFT_REDACTED_FLAG, null as RFTA_VALUE, null as RFTA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "AS",
        "sum(AS_VALUE) as AS_VALUE, sign(sum(AS_REDACTED_FLAG)) as AS_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(ASA_VALUE) as ASA_VALUE, sign(sum(ASA_REDACTED_FLAG)) as ASA_REDACTED_FLAG, ",
          "null as ASA_VALUE, null as ASA_REDACTED_FLAG, "),
        "null as AS_VALUE, null as AS_REDACTED_FLAG, null as ASA_VALUE, null as ASA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "RT",
        "sum(RT_VALUE) as RT_VALUE, sign(sum(RT_REDACTED_FLAG)) as RT_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(RTA_VALUE) as RTA_VALUE, sign(sum(RTA_REDACTED_FLAG)) as RTA_REDACTED_FLAG, ",
          "null as RTA_VALUE, null as RTA_REDACTED_FLAG, "),
        "null as RT_VALUE, null as RT_REDACTED_FLAG, null as RTA_VALUE, null as RTA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "ID",
        "sum(ID_VALUE) as ID_VALUE, sign(sum(ID_REDACTED_FLAG)) as ID_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(IDA_VALUE) as IDA_VALUE, sign(sum(IDA_REDACTED_FLAG)) as IDA_REDACTED_FLAG, ",
          "null as IDA_VALUE, null as IDA_REDACTED_FLAG, "),
        "null as ID_VALUE, null as ID_REDACTED_FLAG, null as IDA_VALUE, null as IDA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "IL",
        "sum(IL_VALUE) as IL_VALUE, sign(sum(IL_REDACTED_FLAG)) as IL_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(ILA_VALUE) as ILA_VALUE, sign(sum(ILA_REDACTED_FLAG)) as ILA_REDACTED_FLAG, ",
          "null as ILA_VALUE, null as ILA_REDACTED_FLAG, "),
        "null as IL_VALUE, null as IL_REDACTED_FLAG, null as ILA_VALUE, null as ILA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "IDT",
        "sum(IDT_VALUE) as IDT_VALUE, sign(sum(IDT_REDACTED_FLAG)) as IDT_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(IDTA_VALUE) as IDTA_VALUE, sign(sum(IDTA_REDACTED_FLAG)) as IDTA_REDACTED_FLAG, ",
          "null as IDTA_VALUE, null as IDTA_REDACTED_FLAG, "),
        "null as IDT_VALUE, null as IDT_REDACTED_FLAG, null as IDTA_VALUE, null as IDTA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "RD",
        "sum(RD_VALUE) as RD_VALUE, sign(sum(RD_REDACTED_FLAG)) as RD_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(RDA_VALUE) as RDA_VALUE, sign(sum(RDA_REDACTED_FLAG)) as RDA_REDACTED_FLAG, ",
          "null as RDA_VALUE, null as RDA_REDACTED_FLAG, "),
        "null as RD_VALUE, null as RD_REDACTED_FLAG, null as RDA_VALUE, null as RDA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "ST",
        "sum(ST_VALUE) as ST_VALUE, sign(sum(ST_REDACTED_FLAG)) as ST_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(STA_VALUE) as STA_VALUE, sign(sum(STA_REDACTED_FLAG)) as STA_REDACTED_FLAG, ",
          "null as STA_VALUE, null as STA_REDACTED_FLAG, "),
        "null as ST_VALUE, null as ST_REDACTED_FLAG, null as STA_VALUE, null as STA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "OC",
        "sum(OC_VALUE) as OC_VALUE, sign(sum(OC_REDACTED_FLAG)) as OC_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(OCA_VALUE) as OCA_VALUE, sign(sum(OCA_REDACTED_FLAG)) as OCA_REDACTED_FLAG, ",
          "null as OCA_VALUE, null as OCA_REDACTED_FLAG, "),
        "null as OC_VALUE, null as OC_REDACTED_FLAG, null as OCA_VALUE, null as OCA_REDACTED_FLAG, "));

    selectStatement.Append(
      ParameterSet.Alternative("POP_TYPES", "TPOC",
        "sum(TPOC_VALUE) as TPOC_VALUE, sign(sum(TPOC_REDACTED_FLAG)) as TPOC_REDACTED_FLAG, " +
        ParameterSet.Alternative("INCLUDE", "HCRASS",
          "sum(TPOCA_VALUE) as TPOCA_VALUE, sign(sum(TPOCA_REDACTED_FLAG)) as TPOCA_REDACTED_FLAG ",
          "null as TPOCA_VALUE, null as TPOCA_REDACTED_FLAG "),
        "null as TPOC_VALUE, null as TPOC_REDACTED_FLAG, null as TPOCA_VALUE, null as TPOCA_REDACTED_FLAG "));

    selectStatement.Append("from (select POC.ASR_YEAR, ");

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "RES",
        ParameterSet.Alternative("SUMRES", "COUNTRY",
          "POC.COU_NAME_RESIDENCE, " +
          "decode(POC.ISO3166_ALPHA3_CODE_RESIDENCE, 'XXX', 2, 1) as ORDER_SEQ_RESIDENCE, ",
          "nvl(NAM1.NAME, POC.COU_NAME_RESIDENCE) as COU_NAME_RESIDENCE, nvl(" +
          ParameterSet.Alternative("SUMRES", "UNSD_MGR",
            "POC.UNSD_MGR_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ParameterSet.Alternative("SUMRES", "UNSD_GSR",
            "POC.UNSD_GSR_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ParameterSet.Alternative("SUMRES", "UNHCR_BUR",
            "POC.UNHCR_BUR_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ParameterSet.Alternative("SUMRES", "UNHCR_ROP",
            "POC.UNHCR_ROP_ORDER_SEQ_RESIDENCE",
            String.Empty) +
          ", decode(POC.ISO3166_ALPHA3_CODE_RESIDENCE, 'XXX', 2e6, 1e6)) as ORDER_SEQ_RESIDENCE, "),
        "null as COU_NAME_RESIDENCE, null as ORDER_SEQ_RESIDENCE, "));

    selectStatement.Append(
      ParameterSet.Alternative("BREAKDOWN", "OGN",
        ParameterSet.Alternative("SUMOGN", "COUNTRY",
          "POC.COU_NAME_ORIGIN, " +
          "decode(POC.ISO3166_ALPHA3_CODE_ORIGIN, 'XXX', 2, 1) as ORDER_SEQ_ORIGIN, ",
          "nvl(NAM2.NAME, POC.COU_NAME_ORIGIN) as COU_NAME_ORIGIN, nvl(" +
          ParameterSet.Alternative("SUMOGN", "UNSD_MGR",
            "POC.UNSD_MGR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNSD_GSR",
            "POC.UNSD_GSR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNHCR_BUR",
            "POC.UNHCR_BUR_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ParameterSet.Alternative("SUMOGN", "UNHCR_ROP",
            "POC.UNHCR_ROP_ORDER_SEQ_ORIGIN",
            String.Empty) +
          ", decode(POC.ISO3166_ALPHA3_CODE_ORIGIN, 'XXX', 2e6, 1e6)) as ORDER_SEQ_ORIGIN, "),
        "null as COU_NAME_ORIGIN, null as ORDER_SEQ_ORIGIN, "));

    selectStatement.Append(@"
POC.RF_VALUE, POC.RF_REDACTED_FLAG, POC.RFA_VALUE, POC.RFA_REDACTED_FLAG,
POC.RL_VALUE, POC.RL_REDACTED_FLAG, POC.RLA_VALUE, POC.RLA_REDACTED_FLAG,
case
  when coalesce(POC.RF_VALUE, POC.RL_VALUE) is not null
  then nvl(POC.RF_VALUE, 0) + nvl(POC.RL_VALUE, 0)
end as RFT_VALUE,
coalesce(POC.RF_REDACTED_FLAG, POC.RL_REDACTED_FLAG) as RFT_REDACTED_FLAG,
case
  when coalesce(POC.RFA_VALUE, POC.RLA_VALUE) is not null
  then nvl(POC.RFA_VALUE, 0) + nvl(POC.RLA_VALUE, 0)
end as RFTA_VALUE,
coalesce(POC.RFA_REDACTED_FLAG, POC.RLA_REDACTED_FLAG) as RFTA_REDACTED_FLAG,
POC.AS_VALUE, POC.AS_REDACTED_FLAG, POC.ASA_VALUE, POC.ASA_REDACTED_FLAG,
POC.RT_VALUE, POC.RT_REDACTED_FLAG, POC.RTA_VALUE, POC.RTA_REDACTED_FLAG,
POC.ID_VALUE, POC.ID_REDACTED_FLAG, POC.IDA_VALUE, POC.IDA_REDACTED_FLAG,
POC.IL_VALUE, POC.IL_REDACTED_FLAG, POC.ILA_VALUE, POC.ILA_REDACTED_FLAG,
case
  when coalesce(POC.ID_VALUE, POC.IL_VALUE) is not null
  then nvl(POC.ID_VALUE, 0) + nvl(POC.IL_VALUE, 0)
end as IDT_VALUE,
coalesce(POC.ID_REDACTED_FLAG, POC.IL_REDACTED_FLAG) as IDT_REDACTED_FLAG,
case
  when coalesce(POC.IDA_VALUE, POC.ILA_VALUE) is not null
  then nvl(POC.IDA_VALUE, 0) + nvl(POC.ILA_VALUE, 0)
end as IDTA_VALUE,
coalesce(POC.IDA_REDACTED_FLAG, POC.ILA_REDACTED_FLAG) as IDTA_REDACTED_FLAG,
POC.RD_VALUE, POC.RD_REDACTED_FLAG, POC.RDA_VALUE, POC.RDA_REDACTED_FLAG,
POC.ST_VALUE, POC.ST_REDACTED_FLAG, POC.STA_VALUE, POC.STA_REDACTED_FLAG,
POC.OC_VALUE, POC.OC_REDACTED_FLAG, POC.OCA_VALUE, POC.OCA_REDACTED_FLAG,
case
  when coalesce(POC.RF_VALUE, POC.RL_VALUE, POC.AS_VALUE, POC.RT_VALUE, POC.ID_VALUE,
                POC.IL_VALUE, POC.RD_VALUE, POC.ST_VALUE, POC.OC_VALUE) is not null
  then nvl(POC.RF_VALUE, 0) + nvl(POC.RL_VALUE, 0) + nvl(POC.AS_VALUE, 0) +
    nvl(POC.RT_VALUE, 0) + nvl(POC.ID_VALUE, 0) + nvl(POC.IL_VALUE, 0) +
    nvl(POC.RD_VALUE, 0) + nvl(POC.ST_VALUE, 0) + nvl(POC.OC_VALUE, 0)
end as TPOC_VALUE,
coalesce(POC.RF_REDACTED_FLAG, POC.RL_REDACTED_FLAG, POC.AS_REDACTED_FLAG,
          POC.RT_REDACTED_FLAG, POC.ID_REDACTED_FLAG, POC.IL_REDACTED_FLAG,
          POC.RD_REDACTED_FLAG, POC.ST_REDACTED_FLAG, POC.OC_REDACTED_FLAG)
  as TPOC_REDACTED_FLAG,
case
  when coalesce(POC.RFA_VALUE, POC.RLA_VALUE, POC.ASA_VALUE, POC.RTA_VALUE, POC.IDA_VALUE,
                POC.ILA_VALUE, POC.RDA_VALUE, POC.STA_VALUE, POC.OCA_VALUE) is not null
  then nvl(POC.RFA_VALUE, 0) + nvl(POC.RLA_VALUE, 0) + nvl(POC.ASA_VALUE, 0) +
    nvl(POC.RTA_VALUE, 0) + nvl(POC.IDA_VALUE, 0) + nvl(POC.ILA_VALUE, 0) +
    nvl(POC.RDA_VALUE, 0) + nvl(POC.STA_VALUE, 0) + nvl(POC.OCA_VALUE, 0)
end as TPOCA_VALUE,
coalesce(POC.RFA_REDACTED_FLAG, POC.RLA_REDACTED_FLAG, POC.ASA_REDACTED_FLAG,
          POC.RTA_REDACTED_FLAG, POC.IDA_REDACTED_FLAG, POC.ILA_REDACTED_FLAG,
          POC.RDA_REDACTED_FLAG, POC.STA_REDACTED_FLAG, POC.OCA_REDACTED_FLAG)
  as TPOCA_REDACTED_FLAG
from PSQ_POC_SUMMARY POC ");

    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNSD_MGR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNSD_MGR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNSD_GSR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNSD_GSR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNHCR_BUR",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNHCR_BUR_ITM_ID_RESIDENCE ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMRES", "UNHCR_ROP",
        "left outer join NAMES NAM1 on NAM1.ITM_ID = POC.UNHCR_ROP_ITM_ID_RESIDENCE ",
        String.Empty));

    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNSD_MGR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNSD_MGR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNSD_GSR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNSD_GSR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNHCR_BUR",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNHCR_BUR_ITM_ID_ORIGIN ",
        String.Empty));
    selectStatement.Append(
      ParameterSet.Alternative("SUMOGN", "UNHCR_ROP",
        "left outer join NAMES NAM2 on NAM2.ITM_ID = POC.UNHCR_ROP_ITM_ID_ORIGIN ",
        String.Empty));

    string conjunction = "where ";

    if (ParameterSet.ContainsKey("YEAR"))
    {
      selectStatement.Append(conjunction + "POC.ASR_YEAR in (" +
        ParameterSet.CommaSeparatedList("YEAR") + ") ");
      conjunction = "and ";
    }

    if (ParameterSet.ContainsKey("RES"))
    {
      selectStatement.Append(conjunction + "POC.COU_CODE_RESIDENCE in (" +
        ParameterSet.QuotedCommaSeparatedList("RES") + ") ");
    }

    if (ParameterSet.ContainsKey("OGN"))
    {
      selectStatement.Append(conjunction + "POC.COU_CODE_ORIGIN in (" +
        ParameterSet.QuotedCommaSeparatedList("OGN") + ") ");
    }

    selectStatement.Append(") group by ASR_YEAR, " +
      "COU_NAME_RESIDENCE, ORDER_SEQ_RESIDENCE, COU_NAME_ORIGIN, ORDER_SEQ_ORIGIN) " +
      "where coalesce(RF_VALUE, RL_VALUE, RFT_VALUE, AS_VALUE, RT_VALUE, " +
      "ID_VALUE, IL_VALUE, IDT_VALUE, RD_VALUE, ST_VALUE, OC_VALUE, TPOC_VALUE, " +
      "RFA_VALUE, RLA_VALUE, RFTA_VALUE, ASA_VALUE, RTA_VALUE, IDA_VALUE, " +
      "ILA_VALUE, IDTA_VALUE, RDA_VALUE, STA_VALUE, OCA_VALUE, TPOCA_VALUE, " +
      "RF_REDACTED_FLAG, RL_REDACTED_FLAG, RFT_REDACTED_FLAG, AS_REDACTED_FLAG, " +
      "RT_REDACTED_FLAG, ID_REDACTED_FLAG, IL_REDACTED_FLAG, IDT_REDACTED_FLAG, " +
      "RD_REDACTED_FLAG, ST_REDACTED_FLAG, OC_REDACTED_FLAG, TPOC_REDACTED_FLAG, " +
      "RFA_REDACTED_FLAG, RLA_REDACTED_FLAG, RFTA_REDACTED_FLAG, ASA_REDACTED_FLAG, " +
      "RTA_REDACTED_FLAG, IDA_REDACTED_FLAG, ILA_REDACTED_FLAG, IDTA_REDACTED_FLAG, " +
      "RDA_REDACTED_FLAG, STA_REDACTED_FLAG, OCA_REDACTED_FLAG, TPOCA_REDACTED_FLAG) is not null" +
      " order by ASR_YEAR desc, " +
      "ORDER_SEQ_RESIDENCE, nlssort(COU_NAME_RESIDENCE, 'NLS_SORT=BINARY_AI'), " +
      "ORDER_SEQ_ORIGIN, nlssort(COU_NAME_ORIGIN, 'NLS_SORT=BINARY_AI')");

    dsASR_POC_SUMMARY.SelectCommand = selectStatement.ToString();

    //Label1.Text += selectStatement.ToString() + "<br />" + DateTime.Now;
  }

  private string GetCaption()
  {
    var caption = new StringBuilder("Overview – Persons of concern to UNHCR");
    string conjunction = "";

    int limit = (ParameterSet.ContainsKey("RES") ? ParameterSet["RES"].Count : 0) - 1;

    if (! ParameterSet.ContainsItem("BREAKDOWN", "RES") && limit >= 0 && limit < 5)
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
        caption.Append(ParameterSet["RESNAMES"].ElementAt<string>(i));
        conjunction = " and";
      }
    }

    limit = (ParameterSet.ContainsKey("OGN") ? ParameterSet["OGN"].Count : 0) - 1;

    if (! ParameterSet.ContainsItem("BREAKDOWN", "OGN") && limit >= 0 && limit < 5)
    {
      caption.Append(conjunction);
      conjunction = "";

      if (ParameterSet.ContainsItem("POP_TYPES", "RF") ||
        ParameterSet.ContainsItem("POP_TYPES", "RL") ||
        ParameterSet.ContainsItem("POP_TYPES", "RFT") ||
        ParameterSet.ContainsItem("POP_TYPES", "AS") ||
        ParameterSet.ContainsItem("POP_TYPES", "ID") ||
        ParameterSet.ContainsItem("POP_TYPES", "IL") ||
        ParameterSet.ContainsItem("POP_TYPES", "IDT") ||
        ParameterSet.ContainsItem("POP_TYPES", "ST") ||
        ParameterSet.ContainsItem("POP_TYPES", "OC") ||
        ParameterSet.ContainsItem("POP_TYPES", "TPOC"))
      {
        caption.Append(" originating");
        conjunction = " or";
      }

      if (ParameterSet.ContainsItem("POP_TYPES", "RT") ||
        ParameterSet.ContainsItem("POP_TYPES", "RD") ||
        ParameterSet.ContainsItem("POP_TYPES", "TPOC"))
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
      dpgASR_POC_SUMMARY1.PageSize = 100000000;
      dpgASR_POC_SUMMARY2.PageSize = 100000000;
    }
    else
    {
      dpgASR_POC_SUMMARY1.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
      dpgASR_POC_SUMMARY2.PageSize = Convert.ToInt32(ddlPageRows.SelectedValue);
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

    if (ParameterSet.ContainsItem("INCLUDE", "HCRASS"))
    {
      foreach (ListItem item in ddlPageRows.Items)
      {
        item.Value = smallPageSizes[ddlPageRows.Items.IndexOf(item)].ToString();
      }
    }
    else
    {
      foreach (ListItem item in ddlPageRows.Items)
      {
        item.Value = largePageSizes[ddlPageRows.Items.IndexOf(item)].ToString();
      }
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