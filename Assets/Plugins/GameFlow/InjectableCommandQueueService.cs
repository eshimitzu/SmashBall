using System.Collections.Generic;
using VContainer;
using VContainer.Unity;

namespace Dyra.Flow
{
    public class InjectableCommandQueueService : CommandQueueService, IStartable
    {
        private readonly IReadOnlyList<CommandQueue> _commandQueues;

        public InjectableCommandQueueService(IReadOnlyList<CommandQueue> commandQueues, IObjectResolver container) : base(container)
        {
            _commandQueues = commandQueues;
        }

        public void Start()
        {
            foreach (var commandQueue in _commandQueues)
            {
                AddCommandQueue(commandQueue);
            }
        }
    }
}
