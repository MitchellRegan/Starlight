using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    //Bool that determines if we display the transform handles for each spline point
    private static bool showHandles = true;
    //Bool that determines if we display the lines for the spline curve velocity
    private static bool showVelocityLines = false;

    //Array of colors that we use to color control point handles based on their mode
    private Color[] modeColors =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    //The list of bezier splines that we keep rendering
    public static List<BezierSpline> splinesToAlwaysRender;

    //The number of steps along each curve
    private const int stepsPerCurve = 10;

    //Reference to the selected curve
    private BezierSpline spline;
    //The transform of our handle
    private Transform handleTransform;
    //The rotation of our handle
    private Quaternion handleRotation;

    //The index of the currently selected control point of our spline
    private int selectedIndex = -1;



    //Function called when this editor window is initialized
    private void Awake()
    {
        //Initializing our list of splines to always render if it's null
        if (splinesToAlwaysRender == null)
        {
            splinesToAlwaysRender = new List<BezierSpline>();
        }
    }


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
        for(int i = 1; i < this.spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = this.ShowPoint(i);
            Vector3 p2 = this.ShowPoint(i + 1);
            Vector3 p3 = this.ShowPoint(i + 2);

            //If we are supposed to draw handle lines connecting each point
            if (showHandles)
            {
                Handles.color = this.spline.handleLineColor;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);
            }

            //Drawing the bezier curve
            Handles.DrawBezier(p0, p3, p1, p2, this.spline.splineColor, null, this.spline.splineWidth);

            //Setting it so that the last point on this drawn curve is the starting point for the next curve
            p0 = p3;
        }

        //If we show the velocity lines, we draw them
        if (showVelocityLines)
        {
            this.ShowDirections();
        }

        //Rendering all of the other splines that we've designated as "always show"
        this.RenderAllSplines();
    }


    //Function called from the editor window
    public override void OnInspectorGUI()
    {
        //Calling the base function
        base.OnInspectorGUI();

        //Getting the selected spline reference
        this.spline = target as BezierSpline;

        //Checking for any changes with the selected spline's "loop" variable
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop Spline", this.spline.Loop);
        //If the loop variable was changed
        if(EditorGUI.EndChangeCheck())
        {
            //We set the selected spline to dirty so that we can save or undo changes
            Undo.RecordObject(this.spline, "Toggle Loop");
            EditorUtility.SetDirty(this.spline);
            //Telling the selected spline to loop back to the first control point
            this.spline.Loop = loop;
        }

        //If we have a valid selected point index, we can allow the user to edit the selected handle's position with text
        if(this.selectedIndex >= 0 && this.selectedIndex < this.spline.ControlPointCount)
        {
            this.DrawSelectedPointInspector();
        }

        //Creating a GUI toggle for if we should show the transform handles for points
        showHandles = GUILayout.Toggle(showHandles, "Show Handles");
        //Creating a GUI toggle for if we should show the velocity lines for the curves
        showVelocityLines = GUILayout.Toggle(showVelocityLines, "Show Velocity");

        //Creating a GUI button to add a curve to our spline
        if (GUILayout.Button("Add Curve"))
        {
            //Sets the spline to dirty so that we can save changes
            Undo.RecordObject(this.spline, "Add Curve");
            //Adds new points to the end of our spline
            this.spline.AddCurve();
            EditorUtility.SetDirty(this.spline);
        }

        //If this curve isn't in our list of splines to show
        if (!splinesToAlwaysRender.Contains(this.spline))
        {
            //Creating a GUI button to add the selected spline to our list of splines to always show
            if (GUILayout.Button("Always Show"))
            {
                splinesToAlwaysRender.Add(this.spline);
            }
        }
        //If this curve isn already in our list of splines to show
        else
        {
            //Creating a GUI button to remove the selected spline from our list
            if(GUILayout.Button("Stop Showing Spline"))
            {
                splinesToAlwaysRender.Remove(this.spline);
            }
        }
    }


    //Function called from OnSceneGUI to display the transforms for each point
    private Vector3 ShowPoint(int index_)
    {
        float handleSize = 0.06f;
        float pickSize = 0.06f;

        //The position of the point we return
        Vector3 point = this.handleTransform.TransformPoint(this.spline.GetControlPoint(index_));

        //If we show this spline's handles or not
        if (showHandles)
        {
            //Setting our control point handle's color based on the control mode
            Handles.color = this.modeColors[(int)this.spline.GetControlPointMode(index_)];

            float size = HandleUtility.GetHandleSize(point);

            //If the given index is the starting control point in the spline, we increase the size
            if(index_ == 0)
            {
                size *= 2f;
            }

            //Creating a handle button to designate the selected point index
            if(Handles.Button(point, this.handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
            {
                this.selectedIndex = index_;
                this.Repaint();
            }

            //If the selected point index is the one we're clicking, we can move it
            if (this.selectedIndex == index_)
            {
                //Checking to see if we're trying to move the handle for the given point
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, this.handleRotation);

                //If we're done changing the point handle, we apply the change to the curve's point
                if (EditorGUI.EndChangeCheck())
                {
                    //Sets the spline to dirty so that we can save changes
                    Undo.RecordObject(this.spline, "Move Point");
                    EditorUtility.SetDirty(this.spline);
                    //Moves the point at the given index to the handle transform's position
                    this.spline.SetControlPoint(index_, this.handleTransform.InverseTransformPoint(point));
                }
            }
        }

        return point;
    }


    //Function called from OnSceneGUI to draw the velocity lines along the curves
    private void ShowDirections()
    {
        //Constant float to scale the curve
        float directionScale = 0.5f;

        //Drawing the velocity lines for each point
        Handles.color = this.spline.velocityLineColor;
        Vector3 point = this.spline.GetPoint(0f);
        Handles.DrawLine(point, point + this.spline.GetDirection(0f) * directionScale);

        //Looping through each step in the curve
        int steps = stepsPerCurve * this.spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = this.spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + this.spline.GetDirection(i / (float)steps * directionScale));
        }
    }


    //Function called from OnInspectorGUI that lets us edit the selected control point's position with text
    private void DrawSelectedPointInspector()
    {
        //Creating a label in the editor saying which point is selected
        GUILayout.Label("Selected Point");
        //Start checking for input changes to the selected handle
        EditorGUI.BeginChangeCheck();

        //Creating a vector 3 input field showing the selected point's coordinates
        Vector3 point = EditorGUILayout.Vector3Field("Position", this.spline.GetControlPoint(this.selectedIndex));

        //Once we're done checking for changes, we apply them to the selected control point's position
        if(EditorGUI.EndChangeCheck())
        {
            //Setting our spline to dirty so we can save or undo the change
            Undo.RecordObject(this.spline, "Move Point");
            EditorUtility.SetDirty(this.spline);
            //Applying the change in position to the selected control point
            this.spline.SetControlPoint(this.selectedIndex, point);
        }

        //Start checking for input changes to the enum for the control point mode
        EditorGUI.BeginChangeCheck();
        BezierSpline.BezierControlPointMode mode = (BezierSpline.BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", this.spline.GetControlPointMode(this.selectedIndex));
        //Once we're done checking for changes to the enum, we apply them to the selected control point's mode
        if(EditorGUI.EndChangeCheck())
        {
            //Setting our spline to dirty so we can save or undo the change
            Undo.RecordObject(this.spline, "Change Point Mode");
            //Changing the selected control point's mode
            this.spline.SetControlPointMode(this.selectedIndex, mode);
            EditorUtility.SetDirty(this.spline);
        }
    }


    //Function called from OnSceneGUI to render all of the splines that we've marked to always render
    private void RenderAllSplines()
    {
        //Looping through all of the splines in our list
        for(int b = 0; b < splinesToAlwaysRender.Count; ++b)
        {
            BezierSpline bs = splinesToAlwaysRender[b];

            //If the current spline is null, we remove it from the list of splines to render
            if(bs == null)
            {
                splinesToAlwaysRender.RemoveAt(b);
                b -= 1;
            }
            //If the current spline isn't the one we have selected, we render it
            else if(bs != this.spline)
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