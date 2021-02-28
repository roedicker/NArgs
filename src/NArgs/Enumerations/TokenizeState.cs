using System;

namespace NArgs.Enumerations
{
  internal enum TokenizeState
  {
    ScanName,
    ScanBeginValue,
    ScanValue,
    ScanQuotedName,
    ScanQuotedValue
  }
}
