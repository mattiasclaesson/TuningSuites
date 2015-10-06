using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MouseGestures {
  [ToolboxBitmap(typeof(MouseGestures), "ComponentIcon")]
  public partial class MouseGestures : Component {
    
    /// <summary>
    /// Maximal ratio of unknown MouseMuveSegments it the gesture
    /// </summary>
    //TODO consider moving the hardcoded value to a property
    private const float maxUnknownSkipRatio = 0.35F;
    
    /// <summary>
    /// Minimal length of MouseMoveSegment
    /// </summary>
    //TODO consider moving the hardcoded value to a property
    private const uint mouseMoveSegmentLength = 8;

    private IMouseMessageFilter mf;
    private List<MouseMoveSegment> mouseMoveSegments;

    private Point lastPoint;
    private Point gestureStartLocation;

    #region Properties

    #region Enabled
    /// <summary>
    /// Gets or sets propery indicating whether component is enabled and
    /// will recognize mouse gestures
    /// </summary>
    [DefaultValue(true)] 
    public bool Enabled {
      get {
        return enabled;
      }
      set {
        enabled = value;
        mf.Enabled = value;
      }
    }
    private bool enabled = true;
    #endregion

    #region EnableComplexGestures
    /// <summary>
    /// Gets or sets propery indicating whether component should recognize
    /// complex gestures
    /// </summary>
    [DefaultValue(true),
    Category("Mouse Gestures")]
    public bool EnableComplexGestures {
      get {
        return enableComplexGestures;
      }
      set {
        enableComplexGestures = value;
      }
    }
    private bool enableComplexGestures = true;
    #endregion

    #region Working
    /// <summary>
    /// Gets value indicating whether component is capturing and recognizing
    /// mouse gesture
    /// </summary>

    [Category("Mouse Gestures")]
    public bool Working {
      get {
        return working;
      }
    }
    private bool working;
    #endregion

    #region MinGestureSize
    /// <summary>
    /// Gets or sets minimal gesture size in pixels
    /// </summary>
    [DefaultValue(30),
    Category("Mouse Gestures")]
    public int MinGestureSize {
      get {
        return minGestureSize;
      }
      set {
        if(value >0)
        minGestureSize = value;
      }
    }
    private int minGestureSize = 30;
    #endregion

    #endregion

    #region Constructors
    /// <summary>
    /// Creates MouseGestures component
    /// </summary>
    /// <param name="useLLMessageFilter">Specifies whether use LLMessageFilter</param>
    public MouseGestures(bool useLLMessageFilter) {
      InitializeComponent();

      mouseMoveSegments = new List<MouseMoveSegment>();
      InitializeMessageFilter(useLLMessageFilter);
    }
    

    /// <summary>
    /// Generic constructor used by Visual Studio
    /// </summary>
    /// <param name="container"></param>
    public MouseGestures(IContainer container) {
      container.Add(this);

      InitializeComponent();

      mouseMoveSegments = new List<MouseMoveSegment>();

      //for standart forms alway use ManagedMouseFilter
      InitializeMessageFilter(false);   
    }
    
    #endregion

    /// <summary>
    /// Installs MouseMessageFilter and hooks it's events
    /// </summary>
    private void InitializeMessageFilter(bool useLLMessageFilter) {
      if ( useLLMessageFilter ) {
        mf = new LLMouseFilter();
      }
      else {
        mf = new ManagedMouseFilter();
      }

      mf.Enabled = enabled;
      mf.RightButtonDown += new MouseFilterEventHandler(BeginGesture);
      mf.MouseMove += new MouseFilterEventHandler(AddToGesture);
      mf.RightButtonUp += new MouseFilterEventHandler(EndGesture);
    }

    #region Destructors
    ~MouseGestures() {
      
    }
    #endregion

    #region Helper Functions
    /// <summary>
    /// Calculates distance between 2 points
    /// </summary>
    /// <param name="p1">First point</param>
    /// <param name="p2">Second point</param>
    /// <returns>Distance between two points</returns>
    private static double GetDistance(Point p1, Point p2) {
      int dx = p1.X - p2.X;
      int dy = p1.Y - p2.Y;

      return Math.Sqrt(dx * dx + dy * dy);
    }
    
    /// <summary>
    /// Counts segments in row with SegmentDirection. 
    /// Counting started at the index of mouseMoveSegments array.
    /// </summary>
    /// <param name="index">
    /// Index to start at. Index to start next search is passed to
    /// this var.
    /// </param>
    /// <param name="segmentDirection">The direction of segments to count.</param>
    /// <returns>Returns the number of segments with direction in the row.</returns>
    private int CountMouseMoveSegments(ref int index, MouseMoveSegment.SegmentDirection segmentDirection) {
      int count = 0;

      while (index < mouseMoveSegments.Count &&
             mouseMoveSegments[index].Direction == segmentDirection) {
        index++;
        count++;
      }

      return count;
    }

    /// <summary>
    /// Counts segments with the same direction in mouseMoveSegments.
    /// Counting started at the index of mouseMoveSegments array and
    /// the direction of segments is passed to the segmentDirection
    /// </summary>
    /// <param name="index">
    /// Index to start at. Index to start next search is passed to
    /// this var.
    /// </param>
    /// <param name="segmentDirection">
    /// The direction of the segments is passed
    /// to this var.
    /// </param>
    /// <returns>Returns the number of segments with the same direction in the row.</returns>
    private int CountMouseMoveSegments(ref int index, out MouseMoveSegment.SegmentDirection segmentDirection) {
      int count = 0;
      segmentDirection = MouseMoveSegment.SegmentDirection.Unknown;

      if ( index < mouseMoveSegments.Count ) {
        segmentDirection = mouseMoveSegments[index].Direction;
      }
      else
        return 0;

      while ( index < mouseMoveSegments.Count && 
        mouseMoveSegments[index].Direction == segmentDirection) {
        index++;
        count++;
      }

      return count;
    }
    #endregion

    #region Recognition Functions
    /// <summary>
    /// Tries to recognize simple mouse gesture
    /// </summary>
    /// <param name="unknownBefore">The number of segments with SegmentDirection.Unknown before the gesture</param>
    /// <param name="length">The number of segments of the gesture.</param>
    /// <param name="unknownAfter">The number of segments with SegmentDirection.Unknown after the gesture</param>
    /// <param name="direction">The direction of the gesture.</param>
    /// <returns>Returns the simple gesture or MouseGesture.Unknown if no gesture is recognized.</returns>
    protected MouseGesture RecognizeSimpleGesture(int unknownBefore, int length, int unknownAfter, MouseMoveSegment.SegmentDirection direction) {
      // max length of unknown segments before and after gesture
      double lengthTolerance = length * maxUnknownSkipRatio;
      // check unknown segments
      if ( (unknownBefore < lengthTolerance) && (unknownAfter < lengthTolerance) ) {
        //according to the direction of the segment choose simple MouseGesture
        switch ( direction ) {
          case MouseMoveSegment.SegmentDirection.Up:
            return MouseGesture.Up;
          case MouseMoveSegment.SegmentDirection.Right:
            return MouseGesture.Right;
          case MouseMoveSegment.SegmentDirection.Down:
            return MouseGesture.Down;
          case MouseMoveSegment.SegmentDirection.Left:
            return MouseGesture.Left;
        }
      }

      return MouseGesture.Unknown;
    }

    /// <summary>
    /// Recognizes complex MouseGesture from two simple gestures
    /// </summary>
    /// <param name="firstGesture">First simple gesture.</param>
    /// <param name="secondGesture">Second simple gesture</param>
    /// <returns>Returns complex MouseGesture or MouseGesture.Unknown if no gesture is recognized.</returns>
    protected MouseGesture RecognizeComplexGeasture(MouseGesture firstGesture, MouseGesture secondGesture) {
      if ( firstGesture == MouseGesture.Unknown || secondGesture == MouseGesture.Unknown )
        return MouseGesture.Unknown;

      //treats two simple gesture with the same direction with some unknown
      //segments between them as valid simple gesture
      //TODO consider disabling this
      if ( firstGesture == secondGesture )
        return firstGesture;

      //see MouseGesture.cs for referecne how to compute complex gesture
      return (firstGesture | (MouseGesture)(( int )secondGesture * 2));
    }
    
    /// <summary>
    /// Recognize gesture from the recorded data
    /// </summary>
    /// <returns>Returns MouseGesture or MouseGesture.Unknown if no gesture is recognized.</returns>
    /// <remarks>
    /// Funtion counts the number of unknown segments before the gestures,
    /// the number of segments in the gestures and the number of unknown segments
    /// after the gestures. These counts are keystone for the gesture recognition.
    /// </remarks>
    private MouseGesture RecognizeGesture() {
      int index = 0;


      int unknownSegmentsBefore = CountMouseMoveSegments(ref index, MouseMoveSegment.SegmentDirection.Unknown);

      MouseMoveSegment.SegmentDirection firstSegmentDirection;
      int firstGestureLenght = CountMouseMoveSegments(ref index, out firstSegmentDirection);


      int unknownSegmentsMiddle = CountMouseMoveSegments(ref index, MouseMoveSegment.SegmentDirection.Unknown);

      MouseMoveSegment.SegmentDirection secondSegmentDirection = MouseMoveSegment.SegmentDirection.Unknown;
      int secondGestureLength = 0;
      int unknownSegmentAfter = 0;
 
      //if complex gesture are enabled count segments for the second gesture
      if ( enableComplexGestures ) {  
        secondGestureLength = CountMouseMoveSegments(ref index, out secondSegmentDirection);

        unknownSegmentAfter = CountMouseMoveSegments(ref index, MouseMoveSegment.SegmentDirection.Unknown);
      }

      //if there are some segments left, the recorded data does not contain valid mouse gesture
      MouseMoveSegment.SegmentDirection nextSegment;
      if ( CountMouseMoveSegments(ref index, out nextSegment) > 0 ) {
        return MouseGesture.Unknown;
      }

      //recognize firs gesture
      MouseGesture firstGesture =
        RecognizeSimpleGesture(unknownSegmentsBefore, firstGestureLenght,
                               unknownSegmentsMiddle, firstSegmentDirection);

      //if complex gesture are enabled continue with second gesture
      MouseGesture secondGesture;
      if ( (enableComplexGestures) && (secondGestureLength > 0) ) {
        secondGesture = RecognizeSimpleGesture(unknownSegmentsMiddle, secondGestureLength,
                                               unknownSegmentAfter, secondSegmentDirection);

        return RecognizeComplexGeasture(firstGesture, secondGesture);
      }
      else
        return firstGesture;
    }
    #endregion

    #region Mouse Events Handling
    /// <summary>
    /// Starts new mouse gesture
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Mouse event data</param>
    /// <remarks>
    /// Functions is called on the RightButtonDown event of MouseMessageFilter.
    /// </remarks>
    public void BeginGesture(object sender, EventArgs e) {
      mouseMoveSegments.Clear();
      
      working = true;

      gestureStartLocation = Cursor.Position;
      lastPoint = Cursor.Position;
    }

    /// <summary>
    /// Adds MouseMoveSegment to the current gesture.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Mouse event data</param>
    /// <remarks>
    /// Function is called on the MouseMoveSegment of MouseMessageFilter
    /// The segment is added only when segment length is greater then 
    /// mouseMoveSegmentLength
    /// </remarks>
    public void AddToGesture(object sender, EventArgs e) {
      if ( working ) {
        if ( GetDistance(lastPoint, Cursor.Position) >= mouseMoveSegmentLength ) {
          MouseMoveSegment segment = new MouseMoveSegment(lastPoint, Cursor.Position);
          mouseMoveSegments.Add(segment);
          lastPoint = Cursor.Position;
        }
      }
    }

    /// <summary>
    /// Stops mouse gesture recording and tries to recognize the gesture
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Mouse event data</param>
    public void EndGesture(object sender,  EventArgs e) {
      working = false;

      //check minimal length
      //TODO change minimal length checking  - does not work for gesture LeftRight, etc...
      if ( mouseMoveSegments.Count * mouseMoveSegmentLength < minGestureSize ) {
        //too short for mouse gesture - send regular right mouse click
        mf.Enabled = false;
        WinAPI.MouseInputEmulation.SendRightMouseClick();
        Application.DoEvents();
        mf.Enabled = true;

        return;
      }
      
      //try recognize mouse gesture
      MouseGesture gesture = RecognizeGesture();
      if(gesture != MouseGesture.Unknown) {
        RaiseGestureEvents(gesture);
      }
    }
    #endregion

    #region MouseGesture Evetns

    /// <summary>
    /// Raises proper events
    /// </summary>
    /// <param name="gesture">Gesture performed.</param>
    private void RaiseGestureEvents(MouseGesture gesture) {
      if ( gesture != MouseGesture.Unknown ) {
        MouseGestureEventArgs eventArgs = new MouseGestureEventArgs(gesture, gestureStartLocation);
        
        //always raise general event
        RaiseGestureEvent(eventArgs);

        switch ( gesture ) {
          case MouseGesture.Up:
            RaiseGestureUpEvent(eventArgs);
            break;
          case MouseGesture.Right:
            RaiseGestureRightEvent(eventArgs);
            break;
          case MouseGesture.Down:
            RaiseGestureDownEvent(eventArgs);
            break;
          case MouseGesture.Left:
            RaiseGestureLeftEvent(eventArgs);
            break;
        }

        if ( enableComplexGestures ) {
          switch ( gesture ) {
            case MouseGesture.UpDown:
              RaiseGestureUpDownEvent(eventArgs);
              break;
            case MouseGesture.UpRight:
              RaiseGestureUpRightEvent(eventArgs);
              break;
            case MouseGesture.UpLeft:
              RaiseGestureUpLeftEvent(eventArgs);
              break;

            case MouseGesture.RightUp:
              RaiseGestureRightUpEvent(eventArgs);
              break;
            case MouseGesture.RightDown:
              RaiseGestureRightDownEvent(eventArgs);
              break;
            case MouseGesture.RightLeft:
              RaiseGestureRightLeftEvent(eventArgs);
              break;

            case MouseGesture.DownUp:
              RaiseGestureDownUpEvent(eventArgs);
              break;
            case MouseGesture.DownRight:
              RaiseGestureDownRightEvent(eventArgs);
              break;
            case MouseGesture.DownLeft:
              RaiseGestureDownLeftEvent(eventArgs);
              break;

            case MouseGesture.LeftUp:
              RaiseGestureLeftUpEvent(eventArgs);
              break;
            case MouseGesture.LeftRight:
              RaiseGestureLeftRightEvent(eventArgs);
              break;
            case MouseGesture.LeftDown:
              RaiseGestureLeftDownEvent(eventArgs);
              break;
          }
        }
      }
    }
    
    /// <summary>
    /// Represents the method that will handle MouseGesture events.
    /// </summary>
    /// <param name="sender">The source of event.</param>
    /// <param name="start">A MouseGestureEventArgs that contains event data.</param>
    public delegate void GestureHandler(object sender, MouseGestureEventArgs e);

    /// <summary>
    /// Occurs whether valid mouse gesture is performed.
    /// </summary>
    public event GestureHandler Gesture;
    private void RaiseGestureEvent(MouseGestureEventArgs e) {
      GestureHandler temp = Gesture;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    #region Simple Gesture Events

    /// <summary>
    /// Occurs whether GestureUp is performed.
    /// </summary>
    [Category("SimpleGestures")]
    public event GestureHandler GestureUp;
    private void RaiseGestureUpEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureUp;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureRight is performed.
    /// </summary>
    [Category("SimpleGestures")]
    public event GestureHandler GestureRight;
    private void RaiseGestureRightEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureRight;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureDown is performed.
    /// </summary>
    [Category("SimpleGestures")]
    public event GestureHandler GestureDown;
    private void RaiseGestureDownEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureDown;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureLeft is performed.
    /// </summary>
    [Category("SimpleGestures")]
    public event GestureHandler GestureLeft;
    private void RaiseGestureLeftEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureLeft;
      if ( temp != null ) {
        temp(this, e);
      }
    }
    #endregion

    #region Complex Gesture Events

    /// <summary>
    /// Occurs whether GestureUpRight is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureUpRight;
    private void RaiseGestureUpRightEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureUpRight;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureUpDown is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureUpDown;
    private void RaiseGestureUpDownEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureUpDown;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureUpLeft is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureUpLeft;
    private void RaiseGestureUpLeftEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureUpLeft;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureRightUp is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureRightUp;
    private void RaiseGestureRightUpEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureRightUp;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureRightDown is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureRightDown;
    private void RaiseGestureRightDownEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureRightDown;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureRightLeft is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureRightLeft;
    private void RaiseGestureRightLeftEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureRightLeft;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureDownUp is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureDownUp;
    private void RaiseGestureDownUpEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureDownUp;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureDownRight is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureDownRight;
    private void RaiseGestureDownRightEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureDownRight;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureDownLeft is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureDownLeft;
    private void RaiseGestureDownLeftEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureDownLeft;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureLeftUp is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureLeftUp;
    private void RaiseGestureLeftUpEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureLeftUp;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureLeftRight is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureLeftRight;
    private void RaiseGestureLeftRightEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureLeftRight;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    /// <summary>
    /// Occurs whether GestureLeftDown is performed.
    /// </summary>
    [Category("ComplexGestures")]
    public event GestureHandler GestureLeftDown;
    private void RaiseGestureLeftDownEvent(MouseGestureEventArgs e) {
      GestureHandler temp = GestureLeftDown;
      if ( temp != null ) {
        temp(this, e);
      }
    }

    #endregion

    #endregion
  }
}
