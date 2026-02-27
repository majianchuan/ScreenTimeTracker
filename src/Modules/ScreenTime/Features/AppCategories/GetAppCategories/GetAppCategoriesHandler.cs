using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;
using System.Linq.Expressions;
using System.Text.Json;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategories;

public class GetAppCategoriesHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetAppCategoriesQuery, List<Dictionary<string, object?>>>
{
    // 定义响应字段与实体属性的依赖关系
    private static readonly Dictionary<string, string[]> _fieldDependencies = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(GetAppCategoriesResponseItem.Id), [nameof(AppCategory.Id)] },
        { nameof(GetAppCategoriesResponseItem.Name), [nameof(AppCategory.Name)] },
        { nameof(GetAppCategoriesResponseItem.IconPath), [nameof(AppCategory.IconPath)] },
        { nameof(GetAppCategoriesResponseItem.IsSystem), [nameof(AppCategory.IsSystem)] }
    };

    // 各字段的计算方式字典
    private static readonly Dictionary<string, Func<AppCategory, object?>> _fieldCalculations = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(GetAppCategoriesResponseItem.Id)] = e => e.Id,
        [nameof(GetAppCategoriesResponseItem.Name)] = e => e.Name,
        [nameof(GetAppCategoriesResponseItem.IconPath)] = e => e.IconPath,
        [nameof(GetAppCategoriesResponseItem.IsSystem)] = e => e.IsSystem,
    };

    public async ValueTask<List<Dictionary<string, object?>>> Handle(GetAppCategoriesQuery request, CancellationToken cancellationToken)
    {
        var allAvailableFields = _fieldDependencies.Keys.ToList();
        var requestedFields = request.Fields?
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var targetFields = (requestedFields == null || requestedFields.Count == 0)
            ? allAvailableFields
            : [.. allAvailableFields.Where(f => requestedFields.Contains(f))];

        var query = context.AppCategories.AsNoTracking().OrderBy(a => a.Id).AsQueryable();

        // 数据库投影：只查询 AppCategory 的必要字段
        var projectedQuery = query.Select(BuildEntitySelector(targetFields));
        List<AppCategory> dataEntities = await projectedQuery.ToListAsync(cancellationToken);

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

    private static Expression<Func<AppCategory, AppCategory>> BuildEntitySelector(IEnumerable<string> finalFields)
    {
        var parameter = Expression.Parameter(typeof(AppCategory), "x");
        var neededEntityProps = new HashSet<string>();

        // 查找这些 Key 对应 AppCategory 实体的哪些属性
        foreach (var field in finalFields)
        {
            if (_fieldDependencies.TryGetValue(field, out var deps))
                foreach (var d in deps) neededEntityProps.Add(d);
        }

        var bindings = new List<MemberBinding>();
        foreach (var propName in neededEntityProps)
        {
            var property = typeof(AppCategory).GetProperty(propName);
            if (property == null) continue;

            bindings.Add(Expression.Bind(property, Expression.Property(parameter, property)));
        }

        return Expression.Lambda<Func<AppCategory, AppCategory>>(
            Expression.MemberInit(Expression.New(typeof(AppCategory)), bindings),
            parameter
        );
    }
}