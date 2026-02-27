using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;
using System.Linq.Expressions;
using System.Text.Json;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetApps;

public class GetAppsHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetAppsQuery, List<Dictionary<string, object?>>>
{
    // 定义响应字段与实体属性的依赖关系
    private static readonly Dictionary<string, string[]> _fieldDependencies = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(GetAppsResponseItem.Id), [nameof(App.Id)] },
        { nameof(GetAppsResponseItem.Name), [nameof(App.Name)] },
        { nameof(GetAppsResponseItem.ProcessName), [nameof(App.ProcessName)] },
        { nameof(GetAppsResponseItem.IsAutoUpdateEnabled), [nameof(App.IsAutoUpdateEnabled)] },
        { nameof(GetAppsResponseItem.LastAutoUpdated), [nameof(App.LastAutoUpdated)] },
        { nameof(GetAppsResponseItem.AppCategoryId), [nameof(App.AppCategoryId)] },
        { nameof(GetAppsResponseItem.ExecutablePath), [nameof(App.ExecutablePath)] },
        { nameof(GetAppsResponseItem.IconPath), [nameof(App.IconPath)] },
        { nameof(GetAppsResponseItem.Description), [nameof(App.Description)] }
    };

    // 各字段的计算方式字典
    private static readonly Dictionary<string, Func<App, object?>> _fieldCalculations = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(GetAppsResponseItem.Id)] = e => e.Id,
        [nameof(GetAppsResponseItem.Name)] = e => e.Name,
        [nameof(GetAppsResponseItem.ProcessName)] = e => e.ProcessName,
        [nameof(GetAppsResponseItem.IsAutoUpdateEnabled)] = e => e.IsAutoUpdateEnabled,
        [nameof(GetAppsResponseItem.LastAutoUpdated)] = e => e.LastAutoUpdated.ToString("o"),
        [nameof(GetAppsResponseItem.AppCategoryId)] = e => e.AppCategoryId,
        [nameof(GetAppsResponseItem.IconPath)] = e => e.IconPath,
        [nameof(GetAppsResponseItem.ExecutablePath)] = e => e.ExecutablePath,
        [nameof(GetAppsResponseItem.Description)] = e => e.Description,
    };

    public async ValueTask<List<Dictionary<string, object?>>> Handle(GetAppsQuery request, CancellationToken cancellationToken)
    {
        var allAvailableFields = _fieldDependencies.Keys.ToList();
        var requestedFields = request.Fields?
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var targetFields = (requestedFields == null || requestedFields.Count == 0)
            ? allAvailableFields
            : [.. allAvailableFields.Where(f => requestedFields.Contains(f))];

        var query = context.Apps.AsNoTracking().OrderBy(a => a.Id).AsQueryable();

        // 数据库投影：只查询 App 的必要字段
        var projectedQuery = query.Select(BuildEntitySelector(targetFields));
        List<App> dataEntities = await projectedQuery.ToListAsync(cancellationToken);

        // 组装结果字典
        var result = new List<Dictionary<string, object?>>(dataEntities.Count);
        foreach (var entity in dataEntities)
        {
            var dict = new Dictionary<string, object?>(targetFields.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var field in targetFields)
            {
                var camelKey = JsonNamingPolicy.CamelCase.ConvertName(field);
                if (_fieldCalculations.TryGetValue(field, out var mapper))
                    dict[camelKey] = mapper(entity);
                else
                    dict[camelKey] = null;
            }
            result.Add(dict);
        }

        return result;
    }

    private static Expression<Func<App, App>> BuildEntitySelector(IEnumerable<string> finalFields)
    {
        var parameter = Expression.Parameter(typeof(App), "x");
        var requiredEntityProps = new HashSet<string>();

        // 查找这些 Key 对应 App 实体的哪些属性
        foreach (var field in finalFields)
        {
            if (_fieldDependencies.TryGetValue(field, out var deps))
                foreach (var d in deps) requiredEntityProps.Add(d);
        }

        var bindings = new List<MemberBinding>();
        foreach (var propName in requiredEntityProps)
        {
            var property = typeof(App).GetProperty(propName);
            if (property == null) continue;

            bindings.Add(Expression.Bind(property, Expression.Property(parameter, property)));
        }

        return Expression.Lambda<Func<App, App>>(
            Expression.MemberInit(Expression.New(typeof(App)), bindings),
            parameter
        );
    }
}