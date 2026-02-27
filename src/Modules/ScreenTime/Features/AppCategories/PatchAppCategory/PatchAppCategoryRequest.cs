using ScreenTimeTracker.BuildingBlocks.Common.Types;
using ScreenTimeTracker.BuildingBlocks.Infrastructure.Serialization;
using System.Text.Json.Serialization;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.PatchAppCategory;

public record PatchAppCategoryRequest(
    Guid Id,
    [property: JsonConverter(typeof(OptionalJsonConverter<string>))] Optional<string> Name,
    [property: JsonConverter(typeof(OptionalJsonConverter<string?>))] Optional<string?> IconPath
);