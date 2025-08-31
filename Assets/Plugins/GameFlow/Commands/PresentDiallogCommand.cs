using System.Threading;
using Cysharp.Threading.Tasks;
using UIFramework;
using UIFramework.Runtime;

namespace Dyra.Flow
{
    public class PresentDialogCommand<T> : Command where T : UIScreenBase
    {
        private readonly UIFrame _uiFrame;

        public PresentDialogCommand(UIFrame uiFrame, IScreenProperties properties = null)
        {
            _uiFrame = uiFrame;
        }

        public override UniTask Execute(CancellationToken token)
        {
            UniTaskCompletionSource tcs = new UniTaskCompletionSource();

            _uiFrame.Open<T>();
            _uiFrame.AddEventForScreen<T>(OnScreenEvent.Closed, () =>
            {
                tcs.TrySetResult();
                _uiFrame.RemoveAllEventsForScreen<T>(OnScreenEvent.Closed);
            });


            return tcs.Task;
        }
    }
}
