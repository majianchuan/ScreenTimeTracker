using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ScreenTimeTracker.Hosts.Desktop;

public class WindowPlacementStore(ILogger<WindowPlacementStore> logger) : IWindowPlacementStore
{
    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WindowSettings.json");
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    public WindowPlacement Load()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                string json = File.ReadAllText(ConfigPath);
                // 反序列化，如果为空则返回默认的新对象
                return JsonSerializer.Deserialize<WindowPlacement>(json) ?? new WindowPlacement();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load window placement configuration.");
                return new WindowPlacement();
            }
        }
        return new WindowPlacement();
    }

    public void Save(WindowPlacement settings)
    {
        try
        {
            string json = JsonSerializer.Serialize(settings, jsonSerializerOptions);
            File.WriteAllText(ConfigPath, json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save window placement configuration.");
        }
    }
}