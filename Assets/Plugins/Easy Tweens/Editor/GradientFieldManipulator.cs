using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace EasyTweens
{
    public class GradientFieldManipulator : PointerManipulator
    {
        private readonly Action callback;

        public GradientFieldManipulator(GradientField t, Action callback)
        {
            this.callback = callback;
            target = t;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(MouseDown);
            target.RegisterCallback<MouseUpEvent>(MouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseUpEvent>(MouseUp);
            target.UnregisterCallback<MouseDownEvent>(MouseDown);
        }

        private void MouseDown(MouseDownEvent evt)
        {
            evt.PreventDefault();
        }

        private void MouseUp(MouseUpEvent evt)
        {
            callback?.Invoke();
        }
    }
}