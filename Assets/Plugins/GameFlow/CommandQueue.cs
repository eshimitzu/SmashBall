using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dyra.Flow
{
    [Serializable]
    public abstract class CommandQueue : Command
    {
        private List<Command> _commands = new();
        public IReadOnlyList<Command> Commands => _commands;

        public void AddCommand(Command command)
        {
            int index = 0;
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                if (_commands[i].Priority >= command.Priority)
                {
                    index = i + 1;
                    break;
                }
            }

            _commands.Insert(index, command);
        }

        public void RemoveCommand<T>() where T : Command
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                if (_commands[i] is T)
                {
                    _commands.RemoveAt(i);
                }
            }
        }

        public void RemoveCommand(Command command)
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                if (_commands[i] == command)
                {
                    _commands.RemoveAt(i);
                }
            }
        }

        public void Clear()
        {
            _commands.Clear();
        }

        public override async UniTask Execute(CancellationToken token)
        {
            foreach (var action in _commands)
            {
                if (token.IsCancellationRequested)
                    return;

                await action.Execute(token);
            }

            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                if (_commands[i].RemoveAfterExecute)
                    _commands.RemoveAt(i);
            }
        }
    }
}
