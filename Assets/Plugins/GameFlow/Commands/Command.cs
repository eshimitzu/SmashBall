
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dyra.Flow
{
    [Serializable]
    public abstract class Command
    {
        public static readonly int DefaultPriority = 0;
        public static readonly int HighPriority = 100;
        public static readonly int HighestPriority = 200;
        public static readonly int LowPriority = -100;
        public static readonly int LowestPriority = -200;

        private readonly int _priority;

        public bool RemoveAfterExecute { get; set; }

        public int Priority => _priority;

        public abstract UniTask Execute(CancellationToken token);

        protected Command()
        {
            _priority = DefaultPriority;
        }

        protected Command(int priority)
        {
            _priority = priority;
        }
    }
}
