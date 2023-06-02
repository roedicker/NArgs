using NArgs.Attributes;

namespace NArgsTest.Data.Commands.Models;

internal sealed class CommandOnlyConfig
{
    [Command(Name = "g",
             LongName = "get",
             Description = "Gets information about date and time")]
    public GetDateTimeCalculationTypeCommand Get { get; } = new GetDateTimeCalculationTypeCommand();
}
