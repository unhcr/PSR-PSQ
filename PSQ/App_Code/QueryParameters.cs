using System;
using System.Collections.Generic;
using System.Linq;
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
  public Dictionary<string, SortedSet<string>> Parameters { get; private set; }

  public SortedSet<string> this[string key]
  {
    get { return Parameters[key]; }
  }

  public QueryParameters()
	{
    Parameters = new Dictionary<string, SortedSet<string>>();
  }

  public QueryParameters(string key, SortedSet<string> value)
  {
    Parameters = new Dictionary<string, SortedSet<string>>();
    Parameters.Add(key, value);
  }

  public QueryParameters(string key, string value)
  {
    var set = new SortedSet<string>();
    set.Add(value);
    Parameters = new Dictionary<string, SortedSet<string>>();
    Parameters.Add(key, set);
  }

  public QueryParameters(StateBag viewState)
  {
    Parameters = (Dictionary<string, SortedSet<string>>)viewState["QueryParameters"];
	}
  
  public void AddSet(string key, SortedSet<string> value)
  {
    try
    {
      Parameters.Add(key, value);
    }
    catch (ArgumentException)
    {
      Parameters[key] = value;
    }
  }

  public void RemoveSet(string key)
  {
    Parameters.Remove(key);
  }

  public void AddItem(string key, string value)
  {
    if (Parameters.ContainsKey(key))
    {
      Parameters[key].Add(value);
    }
    else
    {
      var set = new SortedSet<string>();
      set.Add(value);
      Parameters.Add(key, set);
    }
  }

  public void RemoveItem(string key, string value)
  {
    if (Parameters.ContainsKey(key))
    {
      Parameters[key].Remove(value);
    }
  }

  public bool ContainsKey(string key)
  {
    return Parameters.ContainsKey(key) && Parameters[key] != null;
  }

  public void SaveToViewState(StateBag viewState)
  {
    viewState["QueryParameters"] = Parameters;
  }

  public void RestoreFromViewState(StateBag viewState)
  {
    Parameters = (Dictionary<string, SortedSet<string>>)viewState["QueryParameters"];
  }
}

public interface IQueryParameters
{
  QueryParameters Parameters { get; }
}