using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
[CustomEditor(typeof(GameObject))]
public class BezierSplineDisplay : Editor
{
    //The list of bezier splines that we keep rendering
    public static List<BezierSpline> splinesToAlwaysRender;

    

    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    static void RenderCustomGizmo(Transform objectTransform_, GizmoType gizmoType_)
    {
        //If our list of splines to render is null, we initialize a new one
        if(splinesToAlwaysRender == null)
        {
            splinesToAlwaysRender = new List<BezierSpline>();
            return;
        }

        //If we have at least 1 spline in our list, we render them
        if(splinesToAlwaysRender.Count > 0)
        {
            //Looping through all of the splines in our list
            for (int b = 0; b < splinesToAlwaysRender.Count; ++b)
            {
                BezierSpline bs = splinesToAlwaysRender[b];

                //If the current spline is null, we remove it from the list of splines to render
                if (bs == null)
                {
                    splinesToAlwaysRender.RemoveAt(b);
                    b -= 1;
                }
                //If the current spline isn't the one we have selected, we render it
                else
                {
                    //Getting the spline object's transform
                    Transform bsTransformPoint = bs.gameObject.transform;

                    //Getting each point's handle position to display
                    Vector3 p0 = bsTransformPoint.TransformPoint(bs.GetControlPoint(0));

                    //Looping through every curve in our spline to draw them
                    for (int i = 1; i < bs.ControlPointCount; i += 3)
                    {
                        Vector3 p1 = bsTransformPoint.TransformPoint(bs.GetControlPoint(i));
                        Vector3 p2 = bsTransformPoint.TransformPoint(bs.GetControlPoint(i + 1));
                        Vector3 p3 = bsTransformPoint.TransformPoint(bs.GetControlPoint(i + 2));

                        //Drawing the bezier curve
                        Handles.DrawBezier(p0, p3, p1, p2, bs.splineColor, null, bs.splineWidth);

                        //Setting it so that the last point on this drawn curve is the starting point for the next curve
                        p0 = p3;
                    }
                }
            }
        }
    }
}
