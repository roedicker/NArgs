using System;

using NArgs.Attributes;

namespace NArgsTest.Data
{
  public class SimpleValidConfig
  {
    [Option(LongName = "help", Name = "h", AlternativeName = "?")]
    public bool ShowHelpOption
    {
      get;
      set;
    }

    [Option(LongName = "verbose", Name = "v")]
    public bool VerboseMessagesOption
    {
      get;
      set;
    }

    [Option(LongName = "string", Name = "s")]
    public string StringValueOption
    {
      get;
      set;
    }

    [Option(LongName = "char", Name = "c")]
    public char CharValueOption
    {
      get;
      set;
    }

    [Option(LongName = "double", Name = "dbl")]
    public double DoubleValueOption
    {
      get;
      set;
    }

    [Option(LongName = "int16", Name = "i16")]
    public short Int16ValueOption
    {
      get;
      set;
    }

    [Option(LongName = "int32", Name = "i32", AlternativeName = "i")]
    public int Int32ValueOption
    {
      get;
      set;
    }

    [Option(LongName = "int64", Name = "i64", AlternativeName = "l")]
    public long Int64ValueOption
    {
      get;
      set;
    }

    [Option(LongName = "date", Name = "dt")]
    public DateTime DateValueOption
    {
      get;
      set;
    }

    [Option(LongName = "file-info", Name = "file")]
    public System.IO.FileInfo FileInfoValueOption
    {
      get;
      set;
    }

    [Option(LongName = "directory-info", Name = "dir")]
    public System.IO.DirectoryInfo DirInfoValueOption
    {
      get;
      set;
    }

    [Parameter(OrdinalNumber = 2, Name = "TextParameter B", Description = "Example Parameter #2")]
    public string TextValueParameter2
    {
      get;
      set;
    }

    [Parameter(OrdinalNumber = 1, Name = "TextParameter A", Description = "Example Parameter #1")]
    public string TextValueParameter1
    {
      get;
      set;
    }

    [Option(LongName = "single", Name = "fl")]
    public float SingleValueOption
    {
      get;
      set;
    }

    [Option(LongName = "uint16", Name = "ui16")]
    public ushort UInt16ValueOption
    {
      get;
      set;
    }

    [Option(LongName = "uint32", Name = "ui32")]
    public uint UInt32ValueOption
    {
      get;
      set;
    }

    [Option(LongName = "uint64", Name = "ui64")]
    public ulong UInt64ValueOption
    {
      get;
      set;
    }

    [Option(Name = "uri")]
    public Uri UriOptionValue
    {
      get;
      set;
    }
  }
}

