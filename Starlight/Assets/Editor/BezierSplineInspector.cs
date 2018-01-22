using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    //The number of steps between each curve segment
    private const int lineSteps = 10;

    //Reference to the selected curve
    private BezierSpline spline;
    //The transform of our handle
    private Transform handleTransform;
    //The rotation of our handle
    private Quaternion handleRotation;

    //Constant float to scale the curve
    private const float directionScale = 0.5f;



    //Function called when the GUI is displayed
    private void OnSceneGUI()
    {
        //Setting our curve to the selected target's bezier curve
        this.spline = target as BezierSpline;
        //Getting the handle transform and rotation values
        this.handleTransform = this.spline.transform;
        this.handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        //Getting each point's handle position to display
        Vector3 p0 = this.ShowPoint(0);

        //Looping through every curve in our spline to draw them
        for(int i = 1; i < this.spline.points.Count; i += 3)
        {
            Vector3 p1 = this.ShowPoint(i);
            Vector3 p2 = this.ShowPoint(i + 1);
            Vector3 p3 = this.ShowPoint(i + 2);

            //Drawing lines connecting each point
            Handles.color = this.spline.handleLineColor;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            //Drawing the bezier curve
            Handles.DrawBezier(p0, p3, p1, p2, this.spline.splineColor, null, 2f);

            //Setting it so that the last point on this drawn curve is the starting point for the next curve
            p0 = p3;
        }

        this.ShowDirections();
    }


    //Function called from the editor window
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        this.spline = target as BezierSpline;
        //Creating a GUI button to add a curve to our spline
        if(GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(this.spline, "Add Curve");
            this.spline.AddCurve();
            EditorUtility.SetDirty(this.spline);
        }
    }


    //Function called from OnSceneGUI to display the transforms for each point
    private Vector3 ShowPoint(int index_)
    {
        //The position of the point we return
        Vector3 point = this.handleTransform.TransformPoint(this.spline.points[index_]);

        //Checking to see if we're trying to move the handle for the given point
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        //If we're done changing the point handle, we apply the change to the curve's point
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this.spline, "Move Point");
            EditorUtility.SetDirty(this.spline);
            this.spline.points[index_] = this.handleTransform.InverseTransformPoint(point);
        }

        return point;
    }


    //Function called from OnSceneGUI
    private void ShowDirections()
    {
        Handles.color = this.spline.velocityLineColor;
        Vector3 point = this.spline.GetPoint(0f);
        Handles.DrawLine(point, point + this.spline.GetDirection(0f) * directionScale);
        for (int i = 1; i <= lineSteps; i++)
        {
            point = this.spline.GetPoint(i / (float)lineSteps);
            Handles.DrawLine(point, point + this.spline.GetDirection(i / (float)lineSteps * directionScale));
        }
    }
}