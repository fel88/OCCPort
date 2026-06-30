using TKernel;
using TKV3d;

namespace OCCPort
{
    public class AIS_ViewInputBuffer
    {

        public bool IsNewGesture;     //!< transition from one action to another

        public NCollection_Sequence<Aspect_ScrollDelta> ZoomActions = new NCollection_Sequence<Aspect_ScrollDelta>(); //!< the queue with zoom actions


        public struct _selection
        {
            public AIS_ViewSelectionTool Tool;          //!< perform selection
            public AIS_SelectionScheme Scheme;        //!< selection scheme
            public NCollection_Sequence<Graphic3d_Vec2i> Points;        //!< the points for selection
            public bool ToApplyTool;   //!< apply rubber-band selection tool

            public _selection()
            {
                Tool = AIS_ViewSelectionTool.AIS_ViewSelectionTool_Picking;
                Scheme = AIS_SelectionScheme.AIS_SelectionScheme_UNKNOWN;
                ToApplyTool = (false); 
                Points = new NCollection_Sequence<NCollection_Vec2<int>>();
            }
        }

        public _selection Selection = new _selection();

        public struct _panningParams
        {
            public bool ToStart;    //!< start panning
            public Graphic3d_Vec2i PointStart; //!< panning start point
            public bool ToPan;      //!< perform panning
            public Graphic3d_Vec2i Delta;      //!< panning delta

            public _panningParams()
            {
                ToStart = (false);
                ToPan = false;
            }
        }
        public struct _orbitRotation
        {
            public bool ToStart;    //!< start orbit rotation
            public Graphic3d_Vec2d PointStart; //!< orbit rotation start point
            public bool ToRotate;   //!< perform orbit rotation
            public Graphic3d_Vec2d PointTo;    //!< orbit rotation end point

            public _orbitRotation()
            {
                ToStart = (false);
                ToRotate = (false);
            }
        }
        public _orbitRotation OrbitRotation;

        public _panningParams Panning;

        public struct _draggingParams
        {
            public bool ToStart;    //!< start dragging
            public bool ToMove;     //!< perform dragging
            public bool ToStop;     //!< stop  dragging
            public bool ToAbort;    //!< abort dragging (restore previous position)
            public Graphic3d_Vec2i PointStart; //!< drag start point
            public Graphic3d_Vec2i PointTo;    //!< drag end point

            public _draggingParams()
            {
                ToStart = (false);
                ToMove = (false);
                ToStop = (false);
                ToAbort = (false);
            }
        }

        public _draggingParams Dragging;


        public _orientation Orientation;

        //! Reset events buffer.
        public void Reset()
        {
            Orientation.ToFitAll = false;
            Orientation.ToSetViewOrient = false;
            //MoveTo.ToHilight = false;
            //Selection.ToApplyTool = false;
            IsNewGesture = false;
            ZoomActions.Clear();
            Panning.ToStart = false;
            Panning.ToPan = false;
            Dragging.ToStart = false;
            Dragging.ToMove = false;
            Dragging.ToStop = false;
            Dragging.ToAbort = false;
            OrbitRotation.ToStart = false;
            OrbitRotation.ToRotate = false;/*
            ViewRotation.ToStart = false;
            ViewRotation.ToRotate = false;
            ZRotate.ToRotate = false;*/
        }
    }
}