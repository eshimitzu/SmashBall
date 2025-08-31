using System;
using UIFramework;
using UnityEditor;
using UnityEngine;

namespace Magero.UIFramework.Editor
{
    [CustomPropertyDrawer(typeof(ScreenInfo))]
    public class ScreenInfoPropertyDrawer : PropertyDrawer
    {
        private const float Height = 25f;
        private const float ButtonWidth = 30f;
        private const float Space = 5f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get layer type belonging to this property
            var path = property.propertyPath.Split(']')[0] + "]";
            var layerProperty = property.serializedObject.FindProperty(path);
            var layerTypeProperty = layerProperty.FindPropertyRelative("LayerType");
            var layerTypeEnumString = layerTypeProperty.enumNames[layerTypeProperty.enumValueIndex];
            Enum.TryParse(layerTypeEnumString, out LayerType layerType);
            
            //
            var posX = position.x;
            var totalAvailableWidth = position.width;
            var buttonCount = layerType == LayerType.Panel ? 2 : 4;
            var prefabFieldWidth = totalAvailableWidth - (ButtonWidth + Space) * buttonCount;
            
            // Prefab
            EditorGUI.PropertyField(new Rect(posX, position.y, prefabFieldWidth, Height), 
                property.FindPropertyRelative("Prefab"),
                new GUIContent("", "A prefab which has UIScreen implemented component on it"));
            
            // Load on demand
            posX += prefabFieldWidth + Space;
            var rect = new Rect(posX, position.y, ButtonWidth, Height);
            var loadOnDemandProperty = property.FindPropertyRelative("LoadOnDemand");
            var selectedIcon = EditorGUIUtility.IconContent(
                loadOnDemandProperty.boolValue ? "NetworkMigrationManager Icon" : "TestNormal");
            selectedIcon.tooltip = "Load On Demand \n\n If selected, screen will be instantiated on first opening call";
            if (GUI.Button(rect, selectedIcon))
            {
                loadOnDemandProperty.boolValue = !loadOnDemandProperty.boolValue;
            }
            
            // Destroy on close
            posX += ButtonWidth + Space;
            rect = new Rect(posX, position.y, ButtonWidth, Height);
            var destroyOnCloseProperty = property.FindPropertyRelative("DestroyOnClose");
            selectedIcon = EditorGUIUtility.IconContent(
                destroyOnCloseProperty.boolValue ? "RaycastCollider Icon" : "TestNormal");
            selectedIcon.tooltip = "Destroy On Close \n\n If selected, screen will be destroyed after closing";
            if (GUI.Button(rect, selectedIcon))
            {
                destroyOnCloseProperty.boolValue = !destroyOnCloseProperty.boolValue;
            }
            
            if(layerType == LayerType.Panel)
                return;
            
            // Close with escape key
            posX += ButtonWidth + Space;
            rect = new Rect(posX, position.y, ButtonWidth, Height);
            var closeWithEscapeProperty = property.FindPropertyRelative("CloseWithEscape");
            selectedIcon = EditorGUIUtility.IconContent(
                closeWithEscapeProperty.boolValue ? "AnimatorStateTransition Icon" : "TestNormal");
            selectedIcon.tooltip = "Close with Escape \n\n If selected, back button on Android devices or escape button on Editor will close the popup.";
            if (GUI.Button(rect, selectedIcon))
            {
                closeWithEscapeProperty.boolValue = !closeWithEscapeProperty.boolValue;
            }
            
            // Close with background click 
            posX += ButtonWidth + Space;
            rect = new Rect(posX, position.y, ButtonWidth, Height);
            var closeWithBgClickProperty = property.FindPropertyRelative("CloseWithBgClick");
            selectedIcon = EditorGUIUtility.IconContent(
                closeWithBgClickProperty.boolValue ? "Grid.BoxTool@2x" : "TestNormal");
            selectedIcon.tooltip = "Close with background click \n\n If selected, touching to background blocker will close the popup.";
            if (GUI.Button(rect, selectedIcon))
            {
                closeWithBgClickProperty.boolValue = !closeWithBgClickProperty.boolValue;
            }
        }
    }
}