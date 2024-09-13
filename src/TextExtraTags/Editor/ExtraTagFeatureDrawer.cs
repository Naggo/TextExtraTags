using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TextExtraTags.Editor {
    [CustomPropertyDrawer(typeof(ExtraTagFeature), useForChildren: true)]
    public class ExtraTagFeatureDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType == SerializedPropertyType.ManagedReference) {
                string typeName = property.managedReferenceFullTypename;
                int space = typeName.IndexOf(' ');
                if (space >= 0) {
                    typeName = typeName.Substring(space + 1);
                }
                label.text = typeName.Replace('/', '.');
            }
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
