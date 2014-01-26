using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

/// <summary>
/// The QueryParameters class provides a container for all parameters used to control the data
/// returned by queries, and methods to manage these parameters. The parameters are organised as
/// a dictionary of sorted sets of strings, the key of the dictionary identifying the type of
/// parameter information in the associated sorted set.
/// </summary>
[Serializable]
public class QueryParameters
{
  Dictionary<string, SortedSet<string>> parameterSet;

  public SortedSet<string> this[string key]
  {
    get { return parameterSet[key]; }
  }

  public QueryParameters()
	{
    parameterSet = new Dictionary<string, SortedSet<string>>();
  }

  public QueryParameters(string key, SortedSet<string> value)
  {
    parameterSet = new Dictionary<string, SortedSet<string>>();
    parameterSet.Add(key, value);
  }

  public QueryParameters(string key, string value)
  {
    var set = new SortedSet<string>();
    set.Add(value);
    parameterSet = new Dictionary<string, SortedSet<string>>();
    parameterSet.Add(key, set);
  }

  public QueryParameters(StateBag viewState)
  {
    parameterSet = (Dictionary<string, SortedSet<string>>)viewState["QueryParameters"];
	}

  public bool ContainsKey(string key)
  {
    return parameterSet.ContainsKey(key) && parameterSet[key] != null &&
      parameterSet[key].Count() > 0;
  }

  public bool ContainsEmptyKey(string key)
  {
    return parameterSet.ContainsKey(key) &&
      (parameterSet[key] == null || parameterSet[key].Count() == 0);
  }

  public bool ContainsItem(string key, string item)
  {
    return parameterSet.ContainsKey(key) && parameterSet[key] != null &&
      parameterSet[key].Contains(item);
  }

  public void AddSet(string key, SortedSet<string> value)
  {
    try
    {
      parameterSet.Add(key, value);
    }
    catch (ArgumentException)
    {
      parameterSet[key] = value;
    }
  }

  public void RemoveSet(string key)
  {
    parameterSet.Remove(key);
  }

  public void AddItem(string key, string value)
  {
    if (parameterSet.ContainsKey(key))
    {
      parameterSet[key].Add(value);
    }
    else
    {
      var set = new SortedSet<string>();
      set.Add(value);
      parameterSet.Add(key, set);
    }
  }

  public void RemoveItem(string key, string value)
  {
    if (parameterSet.ContainsKey(key))
    {
      parameterSet[key].Remove(value);
    }
  }

  public string CommaSeparatedList(string key)
  {
    if (parameterSet.ContainsKey(key))
    {
      var list = new StringBuilder(1000);
      string sep = "";
      foreach (string item in parameterSet[key])
      {
        list.Append(sep + item);
        sep = ",";
      }
      return list.ToString();
    }
    else
    {
      return "";
    }
  }

  public string QuotedCommaSeparatedList(string key)
  {
    if (parameterSet.ContainsKey(key))
    {
      var list = new StringBuilder(1000);
      string sep = "";
      foreach (string item in parameterSet[key])
      {
        list.Append(sep + "'" + item + "'");
        sep = ",";
      }
      return list.ToString();
    }
    else
    {
      return "";
    }
  }

  public string Alternative(string key, string item, string trueResult, string falseResult)
  {
    return (parameterSet.ContainsKey(key) && parameterSet[key].Contains(item)) ? trueResult : falseResult;
  }

  public void SaveToViewState(StateBag viewState)
  {
    viewState["QueryParameters"] = parameterSet;
  }

  public void RestoreFromViewState(StateBag viewState)
  {
    parameterSet = (Dictionary<string, SortedSet<string>>)viewState["QueryParameters"];
  }

  public override string ToString()
  {
    var result = new StringBuilder(2000);

    if (parameterSet != null)
    {
      foreach (string key in parameterSet.Keys)
      {
        result.Append(key + ": ");

        string sep = "";
        foreach (string item in parameterSet[key])
        {
          result.Append(sep + item);
          sep = ", ";
        }
        result.Append("<br />");
      }
    }
    return result.ToString();
  }
}

public interface IQueryParameters
{
  QueryParameters ParameterSet { get; }
}