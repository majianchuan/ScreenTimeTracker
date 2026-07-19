using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Hosts.Desktop.Hosting;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ScreenTimeTracker.Hosts.Desktop.Platforms;

[SupportedOSPlatform("windows")]
public class WindowsSingleInstanceLock(ILogger<WindowsSingleInstanceLock> logger) : ISingleInstanceLock
{

    private readonly string _mutexName = @"Global\ScreenTimeTrackerDesktopUniqueMutexName";
    private Mutex? _mutex;
    private bool _hasMutexOwnership;
    private bool _disposed;

    public bool TryAcquire()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        try
        {
            var mutexSecurity = new MutexSecurity();

            // 允许所有用户读取/修改 Mutex 状态
            mutexSecurity.AddAccessRule(new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow
            ));

            // 创建临时 Mutex
            var localMutex = MutexAcl.Create(false, _mutexName, out _, mutexSecurity);

            try
            {
                _hasMutexOwnership = localMutex.WaitOne(0);
            }
            catch (AbandonedMutexException)
            {
                // 如果上一个实例崩溃退出，系统会废弃Mutex。
                // 此时当前实例会自动接管 Mutex 的所有权，并抛出此异常。
                logger.LogWarning("Previous instance crashed. Acquired abandoned mutex.");
                _hasMutexOwnership = true;
            }

            if (_hasMutexOwnership)
            {
                _mutex = localMutex; // 只有在成功获取所有权后，才赋值给类成员
                return true;
            }
            else
            {
                localMutex.Dispose();
                return false;
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            // 可能是已经有一个更高权限的实例在运行
            logger.LogError(ex, "Access denied when creating mutex. Likely another instance is running with higher privileges.");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred when trying to acquire the mutex.");
            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // DI 异步关闭会导致 Dispose 线程与获取锁的线程不一致
            // 调用 ReleaseMutex 时几乎必定会抛出 ApplicationException 异常，
            // 所以只依靠 _mutex?.Dispose() 释放句柄
            /*
            if (_hasMutexOwnership && _mutex is not null)
            {
                try
                {
                    _mutex?.ReleaseMutex();
                }
                catch (ApplicationException ex)
                {
                    logger.LogError(ex, "ReleaseMutex failed because Dispose was called from a thread that does not own the mutex.");
                    // 如果 Dispose 在非获取锁的线程中被调用，ReleaseMutex 会抛出 ApplicationException，
                    // 但 Windows 会在进程结束时强制释放，所以这里不需要处理。
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred when trying to release the mutex.");
                }
            }
            */

            _mutex?.Dispose();
            _mutex = null;
            _hasMutexOwnership = false;
        }

        _disposed = true;
    }
}