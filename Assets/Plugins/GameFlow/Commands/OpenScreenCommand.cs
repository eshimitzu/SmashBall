using System.Threading;
using Cysharp.Threading.Tasks;
using UIFramework;
using UIFramework.Runtime;

namespace Dyra.Flow
{
    public class OpenScreenCommand<T> : Command where T : UIScreenBase
    {
        private readonly UIFrame _uiFrame;
        private readonly IScreenProperties _properties;

        public OpenScreenCommand(UIFrame uiFrame, IScreenProperties properties = null)
        {
            _uiFrame = uiFrame;
            _properties = properties;
        }

        public override UniTask Execute(CancellationToken token)
        {
            UniTaskCompletionSource tcs = new UniTaskCompletionSource();

            _uiFrame.Open<T>(_properties);
            _uiFrame.AddEventForScreen<T>(OnScreenEvent.Opened, () =>
            {
                tcs.TrySetResult();
                _uiFrame.RemoveAllEventsForScreen<T>(OnScreenEvent.Opened);
            });


            return tcs.Task;
        }
    }
}
