using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ImportData;

public record ImportDataCommand(
    string RawJson
) : IRequest<ImportDataResponse>;