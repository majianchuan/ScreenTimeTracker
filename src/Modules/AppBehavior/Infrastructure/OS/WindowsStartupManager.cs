using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace ScreenTimeTracker.Modules.AppBehavior.Infrastructure.OS;

[SupportedOSPlatform("windows")]
public class WindowsStartupManager : IStartupManager
{
    private const string RegistryRunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string RegistryAppName = "ScreenTimeTracker";
    private static bool IsRunningAsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public bool IsEnabled()
    {
        return IsTaskSchedulerStartupEnabled(RegistryAppName) || IsRegistryStartupEnabled(RegistryAppName);
    }

    public void Enable()
    {
        string? currentPath = Environment.ProcessPath ?? throw new InvalidOperationException("Current process path is null.");
        if (IsRunningAsAdmin())
            EnableTaskSchedulerStartup(RegistryAppName, currentPath);
        else
            EnableRegistryStartup(RegistryAppName, currentPath);
    }

    public void Disable()
    {
        if (IsRunningAsAdmin() && IsTaskSchedulerStartupEnabled(RegistryAppName))
            DisableTaskSchedulerStartup(RegistryAppName);
        if (IsRegistryStartupEnabled(RegistryAppName))
            DisableRegistryStartup(RegistryAppName);
    }

    private static bool IsRegistryStartupEnabled(string RegistryAppName)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, false);
        return key?.GetValue(RegistryAppName) != null;
    }

    private static void EnableRegistryStartup(string RegistryAppName, string filePath)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, true);
        key?.SetValue(RegistryAppName, $"\"{filePath}\"");
    }

    private static void DisableRegistryStartup(string RegistryAppName)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, true);
        key?.DeleteValue(RegistryAppName, false); // false表示如果值不存在也不会抛出异常
    }

    private static bool IsTaskSchedulerStartupEnabled(string RegistryAppName)
    {
        using var ts = new TaskService();
        return ts.FindTask(RegistryAppName) != null;
    }

    private static void EnableTaskSchedulerStartup(string RegistryAppName, string filePath)
    {
        using var ts = new TaskService();
        var td = ts.NewTask();
        td.RegistrationInfo.Description = $"Starts {RegistryAppName} on user logon.";
        td.RegistrationInfo.Author = typeof(WindowsStartupManager).FullName;
        // 以最高权限运行
        td.Principal.RunLevel = TaskRunLevel.Highest;
        // 创建一个在用户登录时触发的触发器
        td.Triggers.Add(new LogonTrigger());
        // 创建启动应用程序的操作
        td.Actions.Add(new ExecAction(filePath));
        // 不限制电池情况下启动
        td.Settings.DisallowStartIfOnBatteries = false;
        // 切换到电池时不停止任务  
        td.Settings.StopIfGoingOnBatteries = false;
        // 关闭“如果运行时间超过…，停止任务”
        td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
        // 注册任务
        ts.RootFolder.RegisterTaskDefinition(RegistryAppName, td);
    }

    private static void DisableTaskSchedulerStartup(string RegistryAppName)
    {
        using var ts = new TaskService();
        ts.RootFolder.DeleteTask(RegistryAppName, false);
    }
}
