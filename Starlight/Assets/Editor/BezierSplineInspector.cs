using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    //Bool that determines if we display the transform handles for each spline point
    private static bool showHandles = true;
    //Bool that determines if we display the rotation handles for each spline point
    private static bool showRotationHandles = false;
    //Bool that determines if we display the time increments for traveling along the spline
    private static bool showTimeIncrements = false;
    //Bool that determines if we display the bounding box along the different paths
    private static bool showBoundingBox = false;

    //Array of colors that we use to color control point handles based on their mode
    private Color[] modeColors =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };

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
    //Bool that determines if we're selecting a normal point on the spline or a rotation point
    private bool isSelectedPointRotation = false;



    //Function called when this inspector is initialized
    private void Awake()
    {
        //If the BezierSplineDisplay.cs' list of splines to show is null, we initialize a new list
        if(BezierSplineDisplay.splinesToAlwaysRender == null)
        {
            BezierSplineDisplay.splinesToAlwaysRender = new List<BezierSpline>();
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

            //If we are supposed to draw the rotation handles for the Up rotation points
            if(showRotationHandles)
            {
                //Drawing the control point to the up direction point
                Quaternion pointOrientation = this.ShowPointOrientation(i - 1);
                //Handles.DoRotationHandle(pointOrientation, p0);
            }

            //Drawing the bezier curve
            Handles.DrawBezier(p0, p3, p1, p2, this.spline.splineColor, null, this.spline.splineWidth);

            //Setting it so that the last point on this drawn curve is the starting point for the next curve
            p0 = p3;
        }

        //If we draw rotation handles, we need to draw the handle for the last point since it gets cut off in the loop above
        if(showRotationHandles && !this.spline.Loop)
        {
            //Drawing the control point to the up direction point
            Quaternion pointOrientation = this.ShowPointOrientation(this.spline.ControlPointCount - 1);
            //Handles.DoRotationHandle(pointOrientation, p0);
        }

        //If we draw time increments, we need to loop through each time increment for the selected spline
        if(showTimeIncrements)
        {
            //Getting the total amount of time it takes to get to the end of the spline
            float totalTime = this.spline.TotalSplineTime;

            Handles.color = this.spline.timeIncrementColor;

            //Starting the increment on the first step since we know where time 0 is
            for(float t = this.spline.timeIncrementDisplay; t <= totalTime; t += this.spline.timeIncrementDisplay)
            {
                //Getting the percent progress that this time increment is along the spline
                float progress = t / totalTime;
                //Getting the point in space relative to our transform handle
                Vector3 point = this.spline.GetPoint(progress);

                //Drawing a button at the point in space
                float size = HandleUtility.GetHandleSize(point);
                float handleSize = this.spline.controlPointHandleSize / 3;
                Handles.Button(point, this.handleRotation, size * handleSize, size * handleSize, Handles.DotCap);

                //Drawing text next to the button so we know which point is at which time
                Handles.Label(point, ("" + t));
            }
        }

        //If we draw the bounding boxes, we need to loop through each box to display along the selected spline
        if(showBoundingBox)
        {
            //Looping through the number of times that our spline designates
            for (int b = 1; b < this.spline.numBoundingBoxToDisplay + 1; ++b)
            {
                //Getting the percent progress this bounding box is along the spline
                float progress = (1f * b) / (1f * this.spline.numBoundingBoxToDisplay);
                //Getting the point in space relative to our transform handle
                Vector3 point = (this.spline.GetPoint(progress));
                //Getting the rotation orientation at the given point
                Quaternion pointOrientation = this.spline.GetOrientationAtPercent(progress);

                //Shorter variables for the x and y values of the bounding box
                float x = this.spline.boundingBoxDisplay.x / 2;
                float y = this.spline.boundingBoxDisplay.y / 2;

                //Creating the array of points for the stretched cube mesh to create and render
                Vector3[] cubeVerts =
                {
                (pointOrientation * new Vector3(-x, -y, 0)),//Bottom left
                (pointOrientation * new Vector3(x, -y, 0)),//Bottom right
                (pointOrientation * new Vector3(x, y, 0)),//Top right
                (pointOrientation * new Vector3(-x, y, 0))//Top left
            };

                //Creating the array of triangles that compose the mesh
                int[] cubeTriangles =
                {
                0,2,1,//Front face
                0,3,2,

                0,1,2,//Back face
                0,2,3
            };

                //Creating a mesh out of the verts and triangles
                Mesh boundingBoxMesh = new Mesh();
                boundingBoxMesh.vertices = cubeVerts;
                boundingBoxMesh.triangles = cubeTriangles;
                boundingBoxMesh.RecalculateNormals();

                //Getting the material from our spline and setting the color
                Material mat = this.spline.boundingBoxMaterial;
                if (mat != null)
                {
                    mat.color = this.spline.boundingBoxColor;
                }

                //Drawing the mesh along the spline
                Graphics.DrawMesh(boundingBoxMesh, point, pointOrientation, mat, 0);
            }
        }
        
        //If we press the F key, we need to override and focus the camera on the selected point
        if (Event.current.keyCode == KeyCode.F)
        {
            //If we have a selected control point
            if (this.selectedIndex > 1)
            {
                Event.current.Use();
                //Making the scene view camera focus on the selected control point
                Vector3 point = this.handleTransform.TransformPoint(this.spline.GetControlPoint(this.selectedIndex));
                SceneView.lastActiveSceneView.LookAt(point);
            }
        }
    }


    //Function called from the editor window
    public override void OnInspectorGUI()
    {
        //Calling the base function
        base.OnInspectorGUI();

        //Adding in a space to separate all of this inspector's settings and the selected spline's settings
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Getting the selected spline reference
        this.spline = target as BezierSpline;

        //Checking for any changes with the spline handle size float
        EditorGUI.BeginChangeCheck();
        float newHandleSize = EditorGUILayout.FloatField("Handle Size", this.spline.controlPointHandleSize);
        //If the handle size was changed
        if(EditorGUI.EndChangeCheck())
        {
            //We set the new handle size
            this.spline.controlPointHandleSize = newHandleSize;
        }

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
            this.Repaint();
        }

        //If we have a valid selected point index, we can allow the user to edit the selected handle's position with text
        if(this.selectedIndex >= 0 && this.selectedIndex < this.spline.ControlPointCount && !this.isSelectedPointRotation)
        {
            this.DrawSelectedPointInspector();
        }

        //Creating a GUI toggle for if we should show the transform handles for points
        EditorGUI.BeginChangeCheck();
        showHandles = GUILayout.Toggle(showHandles, "Show Handles");
        if(EditorGUI.EndChangeCheck())
        {
            this.Repaint();
        }

        //Creating a GUI toggle for if we should show the rotation control handles for points
        EditorGUI.BeginChangeCheck();
        showRotationHandles = GUILayout.Toggle(showRotationHandles, "Show Rotation Handles");
        if(EditorGUI.EndChangeCheck())
        {
            this.Repaint();
        }

        //Creating a GUI toggle for if we should show the time incriments along the spline
        EditorGUI.BeginChangeCheck();
        showTimeIncrements = GUILayout.Toggle(showTimeIncrements, "Show Time Increments");
        if(EditorGUI.EndChangeCheck())
        {
            this.Repaint();
        }

        //Creating a GUI toggle for if we should show the bounding boxes along the spline
        EditorGUI.BeginChangeCheck();
        showBoundingBox = GUILayout.Toggle(showBoundingBox, "Show Bounding Boxes");
        if(EditorGUI.EndChangeCheck())
        {
            this.Repaint();
        }

        //Creating a GUI button to add a curve to our spline
        if (GUILayout.Button("Add Curve At End"))
        {
            //Sets the spline to dirty so that we can save changes
            Undo.RecordObject(this.spline, "Add Curve At End");
            //Adds new points to the end of our spline
            this.spline.AddCurve();
            EditorUtility.SetDirty(this.spline);
            this.Repaint();
        }
        
        //If we're selecting a control point and not a rotation point
        if(!this.isSelectedPointRotation && this.selectedIndex % 3 == 0)
        {
            //If the selected control point isn't the last point
            if(this.selectedIndex < this.spline.ControlPointCount - 1)
            {
                //Creating a GUI to add a new control point between this control point and the next
                if (GUILayout.Button("Add Control Point After This"))
                {
                    //Sets the spline to dirty so that we can save changes
                    Undo.RecordObject(this.spline, "Add Control Point");
                    //Adds the new point between the selected control point and the next
                    this.spline.AddControlPointBetweenPoints(this.selectedIndex);
                    EditorUtility.SetDirty(this.spline);
                    this.Repaint();
                }
            }
            
            //Creating a GUI button to delete a selected control point
            if(GUILayout.Button("Delete Control Point"))
            {
                //Tells our spline to remove the selected control point
                this.spline.RemoveControlPoint(this.selectedIndex);
                //Selecting the spline's starting index
                this.selectedIndex = 0;
                this.Repaint();
            }
        }

        //If this curve isn't in our list of splines to show
        if (!BezierSplineDisplay.splinesToAlwaysRender.Contains(this.spline))
        {
            //Creating a GUI button to add the selected spline to our list of splines to always show
            if (GUILayout.Button("Always Show"))
            {
                //splinesToAlwaysRender.Add(this.spline);
                BezierSplineDisplay.splinesToAlwaysRender.Add(this.spline);
            }
        }
        //If this curve isn already in our list of splines to show
        else
        {
            //Creating a GUI button to remove the selected spline from our list
            if(GUILayout.Button("Stop Showing Spline"))
            {
                BezierSplineDisplay.splinesToAlwaysRender.Remove(this.spline);
            }
        }
    }


    //Function called from OnSceneGUI to display the transforms for each point
    private Vector3 ShowPoint(int index_)
    {
        //The position of the point we return
        Vector3 point = this.handleTransform.TransformPoint(this.spline.GetControlPoint(index_));

        //If we show this spline's handles or not
        if (showHandles)
        {
            //If the point is a control point, we use the handle color
            if (index_ % 3 == 0)
            {
                Handles.color = this.spline.handleLineColor;
            }
            //If the point is a handle, we set the handle's color based on the control mode
            else
            {
                Handles.color = this.modeColors[(int)this.spline.GetControlPointMode(index_)];
            }

            float size = HandleUtility.GetHandleSize(point);

            //If the given index is the starting control point in the spline, we increase the size
            if(index_ == 0)
            {
                size *= 2f;
            }

            //Creating a handle button to designate the selected point index
            float handleSize = this.spline.controlPointHandleSize;
            if(Handles.Button(point, this.handleRotation, size * handleSize, size * handleSize, Handles.DotCap))
            {
                this.selectedIndex = index_;
                this.isSelectedPointRotation = false;
                this.Repaint();
            }

            //If the selected point index is the one we're clicking and we're not selecting a rotation point, we can move it
            if (this.selectedIndex == index_ && !this.isSelectedPointRotation)
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


    //Function called from OnSceneGUI to display the rotation for the given control point
    private Quaternion ShowPointOrientation(int index_)
    {
        //Getting the rotation orientation for the point at the given index
        Quaternion pointOrientation = this.spline.GetPointOrientation(index_);

        //If we show this spline's rotation handles
        if(showRotationHandles)
        {
            //Setting our control point handle's color to the handle line color
            Handles.color = this.spline.rotationLineColor;
            
            //Getting the rotation handle point to toggle the rotation
            Vector3 point = this.handleTransform.TransformPoint(this.spline.GetControlPoint(index_));
            point += pointOrientation * new Vector3(0, this.spline.rotationHandleRadius, 0);

            float size = HandleUtility.GetHandleSize(point);

            //Creating a handle button to designate the selected point index
            float handleSize = this.spline.controlPointHandleSize;
            Handles.DrawLine(point, this.handleTransform.TransformPoint(this.spline.GetControlPoint(index_)));
            if (Handles.Button(point, this.handleRotation, size * handleSize, size * handleSize, Handles.DotCap))
            {
                this.selectedIndex = index_;
                this.isSelectedPointRotation = true;
                this.Repaint();
            }

            //If the selected point index is the one we're clicking and we're clicking a rotation point, we can move it
            if (this.selectedIndex == index_ && this.isSelectedPointRotation)
            {
                //Getting the forward direction of the control point along the spline
                Vector3 forward = pointOrientation * Vector3.forward;
                //Getting the Up direction of the control point based on this up rotation point
                Vector3 up = pointOrientation * Vector3.up;

                //The center point of the disk in local space
                Vector3 localCenterPoint = this.handleTransform.TransformPoint(this.spline.GetControlPoint(index_));

                Quaternion localRot = pointOrientation * this.handleTransform.rotation;

                //Checking to see if we're trying to move the handle for the given point
                EditorGUI.BeginChangeCheck();
                //Quaternion rotationChange = Handles.DoRotationHandle(pointOrientation, localCenterPoint);
                Quaternion rotationChange = Handles.Disc(localRot, localCenterPoint, forward, this.spline.rotationHandleRadius, false, 0);

                //If we're done changing the point handle, we apply the change to the curve's point
                if (EditorGUI.EndChangeCheck())
                {
                    //Sets the spline to dirty so that we can save changes
                    Undo.RecordObject(this.spline, "Rotate Point");
                    EditorUtility.SetDirty(this.spline);
                    this.spline.SetPointOrientation(index_, rotationChange);
                }
            }
        }

        return pointOrientation;
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
}