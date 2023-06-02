using NArgs.Attributes;

namespace NArgsTest.Data.Commands.Models;

internal sealed class ComplexCommandConfig
{
    [Option(Name = "h", AlternativeName = "?", LongName = "help")]
    public bool ShowHelp { get; set; }

    [Option(Name = "v", LongName = "verbose", Description = "Indicator whether output should be verbose")]
    public bool VerboseOutput { get; set; }

    [Command(Name = "g",
             LongName = "get",
             Description = "Gets information about date and time")]
    public GetDateTimeCalculationTypeCommand Get { get; } = new GetDateTimeCalculationTypeCommand();

    [Command(Name = "s",
         LongName = "set",
         Description = "Sets information about date and time")]
    public GetDateTimeCalculationTypeCommand Set { get; } = new GetDateTimeCalculationTypeCommand();
}
