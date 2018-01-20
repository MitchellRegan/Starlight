using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    //Function called when the GUI is displayed
    private void OnSceneGUI()
    {
        //The selected object's Line component
        Line line = target as Line;

        //Setting our line's point positions relative to its relative space
        Transform handleTransform = line.transform;
        Vector3 p0 = handleTransform.TransformPoint(line.p0);
        Vector3 p1 = handleTransform.TransformPoint(line.p1);

        //Drawing a white line between the two points
        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);

        //Getting the rotation for the handles of each point. This can either be set in local or global space
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;
        Handles.DoPositionHandle(p0, handleRotation);
        Handles.DoPositionHandle(p1, handleRotation);

        //Checking to see if we're trying to move the p0 handle
        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        //If we're done changing the p0 handle, we apply the change to the line's p0 point
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p0 = handleTransform.InverseTransformPoint(p0);
        }

        //Checking to see if we're trying to move the p1 handle
        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        //If we're done changing the p1 handle, we apply the change to the line's p1 point
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p1 = handleTransform.InverseTransformPoint(p1);
        }
    }
}
