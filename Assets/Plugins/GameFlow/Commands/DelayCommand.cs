using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dyra.Flow
{
    public class DelayCommand : Command
    {
        private readonly float _delay;

        public DelayCommand(float delay)
        {
            _delay = delay;
        }

        public override async UniTask Execute(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: token);
        }
    }
}
