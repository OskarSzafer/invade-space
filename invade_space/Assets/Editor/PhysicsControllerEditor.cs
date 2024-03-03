using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhysicsController))]
public class PhysicsControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PhysicsSystem.gravitationalConstant = EditorGUILayout.FloatField("Gravitational Constant", PhysicsSystem.gravitationalConstant);
        PhysicsSystem.atmosphericDragConstant = EditorGUILayout.FloatField("Atmospheric Drag Constant", PhysicsSystem.atmosphericDragConstant);

        PhysicsController physicsController = (PhysicsController)target;
        physicsController.temporalGravitationalConstant = PhysicsSystem.gravitationalConstant;
        physicsController.temporalAtmosphericDragConstant = PhysicsSystem.atmosphericDragConstant;
    
    }
}
