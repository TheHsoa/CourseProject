using System;
using System.Threading;

namespace CourseProject
{
    //Честно украл тут: https://github.com/SergeyTeplyakov/CrazyTalk/blob/master/CrazyTalk.Net/Core/CrazyTalk.Core/Utils/ReaderWriterLockSlimExtensions.cs
    public static class ReaderWriterLockSlimExtensions
    {
        public static IDisposable UseReadLock(this ReaderWriterLockSlim locker)
        {
            locker?.EnterReadLock();
            return new DisposeActionWrapper(() => locker?.ExitReadLock());
        }

        public static IDisposable UseWriteLock(this ReaderWriterLockSlim locker)
        {
            locker?.EnterWriteLock();
            return new DisposeActionWrapper(() => locker?.ExitWriteLock());
        }

        public static IDisposable UseUpgratableReadLock(this ReaderWriterLockSlim locker)
        {
            locker?.EnterUpgradeableReadLock();
            return new DisposeActionWrapper(() => locker?.ExitUpgradeableReadLock());
        }
    }

    public class DisposeActionWrapper : IDisposable
    {
        private readonly Action action;

        public DisposeActionWrapper(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}
