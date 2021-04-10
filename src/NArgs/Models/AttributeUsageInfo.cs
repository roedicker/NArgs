using System;
using System.Collections.Generic;

namespace NArgs.Models
{
  internal abstract class AttributeUsageInfo<T> where T : Attributes.Attribute
  {
    private readonly List<T> _Items;

    public int MaxNameLength
    {
      get;
      protected set;
    }

    public IEnumerable<T> Items
    {
      get
      {
        return _Items;
      }
    }

    protected AttributeUsageInfo()
    {
      _Items = new List<T>();
      MaxNameLength = 0;
    }

    public virtual void AddItem(T item)
    {
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      if (!_Items.Contains(item))
      {
        _Items.Add(item);
      }
    }

    /// <summary>
    /// Gets the usage syntax text for this attribute information.
    /// </summary>
    /// <param name="indention">Indention of multi-line syntaxt text output.</param>
    /// <returns>Usage syntax text for this attribute information.</returns>
    public abstract string GetUsageSyntaxText(int indention);

    /// <summary>
    /// Gets the usage detail text for this attribute information.
    /// </summary>
    /// <returns>Usage detail text for this attribute information.</returns>
    public abstract string GetUsageDetailText();
  }
}
