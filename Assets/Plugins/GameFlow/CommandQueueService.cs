using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Dyra.Flow
{
    public class CommandQueueService
    {
        private readonly IObjectResolver _container;
        private Dictionary<Type, CommandQueue> _commandQueues = new();

        public CommandQueueService(IObjectResolver container)
        {
            _container = container;
        }

        protected void AddCommandQueue(CommandQueue queue)
        {
            Debug.Assert(!_commandQueues.ContainsKey(queue.GetType()));
            _commandQueues.Add(queue.GetType(), queue);
        }

        public void AddCommand<TQueue>(Command command) where TQueue : CommandQueue
        {
            _commandQueues[typeof(TQueue)].AddCommand(command);
        }

        public void RemoveCommand<TQueue>(Command command) where TQueue : CommandQueue
        {
            if (_commandQueues.ContainsKey(typeof(TQueue)))
            {
                _commandQueues[typeof(TQueue)].RemoveCommand(command);
            }
        }

        public UniTask ExecuteCommandQueue<TQueue>(CancellationToken token) where TQueue : CommandQueue
        {
            if (_commandQueues.ContainsKey(typeof(TQueue)))
            {
                return _commandQueues[typeof(TQueue)].Execute(token);
            }

            return UniTask.CompletedTask;
        }
    }
}
