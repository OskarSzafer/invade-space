using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhysicsProperty))]
public class PhysicsPropertyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PhysicsProperty physicsProperty = (PhysicsProperty)target;

        GUILayout.Label("body type:");
        physicsProperty.bodyTypeIndex = EditorGUILayout.Popup(physicsProperty.bodyTypeIndex, physicsProperty.optionList);
        physicsProperty.bodyType = physicsProperty.optionList[physicsProperty.bodyTypeIndex];
    }
}
