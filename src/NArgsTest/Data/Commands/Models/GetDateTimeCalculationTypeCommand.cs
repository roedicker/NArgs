using System.IO;
using NArgs.Attributes;
using NArgsTest.Data.Commands.Enums;

namespace NArgsTest.Data.Commands.Models;

internal sealed class GetDateTimeCalculationTypeCommand
{
    [Parameter(OrdinalNumber = 2, Name = "data-source", Description = "Gets / sets the data source to get and set data")]
    public FileInfo DataSource { get; set; }

    [Parameter(OrdinalNumber = 1, Name = "calculation-type", Description = "Determines what kind of date-time to be calculated")]
    public DateTimeCalculationType CalculationType { get; set; }

    [Option(Name = "utc", LongName = "use-utc", Description = "Indicator whether to use UTC based date-time information")]
    public bool UseUtc { get; set; } = true;

    public GetDateTimeCalculationTypeCommand()
    {
        CalculationType = DateTimeCalculationType.None;
    }
}
