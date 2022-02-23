using System;
using System.Security;
using NArgs.Attributes;

namespace NArgsTest.Data
{
  public class SimpleValidConfig
  {
    [OptionAttribute(LongName = "help", Name = "h", AlternativeName = "?")]
    public bool ShowHelpOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "verbose", Name = "v")]
    public bool VerboseMessagesOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "string", Name = "s")]
    public string StringValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "char", Name = "c")]
    public char CharValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "double", Name = "dbl")]
    public double DoubleValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "int16", Name = "i16")]
    public short Int16ValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "int32", Name = "i32", AlternativeName = "i")]
    public int Int32ValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "int64", Name = "i64", AlternativeName = "l")]
    public long Int64ValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "date", Name = "dt")]
    public DateTime DateValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "file-info", Name = "file")]
    public System.IO.FileInfo FileInfoValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "directory-info", Name = "dir")]
    public System.IO.DirectoryInfo DirInfoValueOption
    {
      get;
      set;
    }

    [ParameterAttribute(OrdinalNumber = 1, Name = "parameterA", Description = "Example Parameter #1")]
    public string TextValueParameter1
    {
      get;
      set;
    }

    [ParameterAttribute(OrdinalNumber = 2, Name = "parameterB", Description = "Example Parameter #2")]
    public string TextValueParameter2
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "single", Name = "fl")]
    public float SingleValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "uint16", Name = "ui16")]
    public ushort UInt16ValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "uint32", Name = "ui32")]
    public uint UInt32ValueOption
    {
      get;
      set;
    }

    [OptionAttribute(LongName = "uint64", Name = "ui64")]
    public ulong UInt64ValueOption
    {
      get;
      set;
    }

    [OptionAttribute(Name = "uri")]
    public Uri UriOptionValue
    {
      get;
      set;
    }

    [Option(Name = "sstring", LongName = "secure-string")]
    public SecureString SecureStringOption
    {
      get;
      set;
    } = new SecureString();
  }
}

