using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyTweens
{
    public class DropFromHierarchyAndCreateNewTween : PointerManipulator
    {
        private TweenAnimationEditor _mainAnimationEditor;
        private StyleColor _originalBackgroundColor;
        
        public DropFromHierarchyAndCreateNewTween(VisualElement t, TweenAnimationEditor mainAnimationEditor)
        {
            _mainAnimationEditor = mainAnimationEditor;
            target = t;
            _originalBackgroundColor = target.style.backgroundColor;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragEnterEvent>(DragEnter);
            target.RegisterCallback<DragLeaveEvent>(DragLeave);
            target.RegisterCallback<DragUpdatedEvent>(DragUpdated);
            target.RegisterCallback<DragPerformEvent>(DragPerform);
            target.RegisterCallback<DragExitedEvent>(DragExited);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragEnterEvent>(DragEnter);
            target.UnregisterCallback<DragLeaveEvent>(DragLeave);
            target.UnregisterCallback<DragUpdatedEvent>(DragUpdated);
            target.UnregisterCallback<DragPerformEvent>(DragPerform);
            target.UnregisterCallback<DragExitedEvent>(DragExited);
        }

        private void DragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();
        
            ResetColor();

            var objectReference = DragAndDrop.objectReferences[0];

            var gameObject = (GameObject) objectReference;
            if (gameObject == null) return;

            var components = gameObject.GetComponents<Component>();

            List<Type> tweenTypes = new List<Type>();
            List<string> names = new List<string>();

            foreach (var type in TweenAnimationEditor.tweenTypes)   
            {
                foreach (var component in components)
                {
                    var fieldInfo = type.GetField("target");
                    if (fieldInfo != null && fieldInfo.FieldType.IsInstanceOfType(component))
                    {
                        tweenTypes.Add(type);
                        names.Add(TweenAnimationEditor._availableTweenNames[TweenAnimationEditor.tweenTypes.IndexOf(type)]);
                    }
                }
            }
            
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < tweenTypes.Count; i++)
            {
                var type = tweenTypes[i];
                var name = names[i];
                menu.AddItem(new GUIContent(name), false, () =>
                {
                    Undo.RecordObject(_mainAnimationEditor, "Add tween");
                    var newTween = _mainAnimationEditor.AddTween(type);
                    var fieldInfo = type.GetField("target");
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(newTween, gameObject.GetComponent(fieldInfo.FieldType));
                    }
                    EditorUtility.SetDirty(_mainAnimationEditor);
                });
            }
            menu.ShowAsContext();
        }

        private void DragUpdated(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;   
        }

        private void DragLeave(DragLeaveEvent evt)
        {
            ResetColor();
        }

        private void DragExited(DragExitedEvent evt)
        {
            ResetColor();
        }

        public void ResetColor()
        {
            target.style.backgroundColor = _originalBackgroundColor;
        }

        private void DragEnter(DragEnterEvent evt)
        {
            target.style.backgroundColor = new StyleColor(new Color(0.52f,0.912f,0.23f, 0.3f));
        }
    }
}