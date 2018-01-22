using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    //The number of steps between each curve segment
    private const int lineSteps = 10;

    //Reference to the selected curve
    private BezierCurve curve;
    //The transform of our handle
    private Transform handleTransform;
    //The rotation of our handle
    private Quaternion handleRotation;


    //Function called when the GUI is displayed
    private void OnSceneGUI()
    {
        //Setting our curve to the selected target's bezier curve
        this.curve = target as BezierCurve;
        //Getting the handle transform and rotation values
        this.handleTransform = this.curve.transform;
        this.handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        //Getting each point's handle position to display
        Vector3 p0 = this.ShowPoint(0);
        Vector3 p1 = this.ShowPoint(1);
        Vector3 p2 = this.ShowPoint(2);

        //Drawing lines connecting each point
        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p2);

        //Drawing the actual bezier curve
        Handles.color = Color.red;
        Vector3 lineStart = this.curve.GetPoint(0f);
        //Looping through each step in the curve segment
        for(int i = 1; i <= lineSteps; i++)
        {
            Vector3 lineEnd = this.curve.GetPoint(i / (float)lineSteps);
            Handles.DrawLine(lineStart, lineEnd);
            Handles.color = Color.green;
            Handles.DrawLine(lineEnd, lineEnd + this.curve.GetVelocity(i / (float)lineSteps));
            lineStart = lineEnd;
        }
    }


    //Function called from OnSceneGUI to display the transforms for each point
    private Vector3 ShowPoint(int index_)
    {
        //The position of the point we return
        Vector3 point = this.handleTransform.TransformPoint(this.curve.points[index_]);

        //Checking to see if we're trying to move the handle for the given point
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        //If we're done changing the point handle, we apply the change to the curve's point
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this.curve, "Move Point");
            EditorUtility.SetDirty(this.curve);
            this.curve.points[index_] = this.handleTransform.InverseTransformPoint(point);
        }

        return point;
    }
}
