using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trionic5Tools;
using System.Data;
using CommonSuite;

namespace Trionic5Controls
{
    abstract public class IMapViewer : DevExpress.XtraEditors.XtraUserControl
    {

        public abstract bool AutoUpdateChecksum
        {
            get;
            set;
        }

        public abstract bool ClearData
        {
            get;
            set;
        }

        public abstract bool OnlineMode
        {
            get;
            set;
        }


        public abstract bool DirectSRAMWriteOnSymbolChange
        {
            get;
            set;
        }

        public abstract Trionic5Tools.SymbolCollection mapSymbolCollection
        {
            get;
            set;
        }

        public abstract SuiteViewType Viewtype
        {
            get;
            set;
        }

        public abstract int MaxValueInTable
        {
            get;
            set;
        }

        public abstract bool AutoSizeColumns
        {
            set;
        }

        public abstract bool DisableColors
        {
            get;
            set;
        }

        public abstract int SliderPosition
        {
            get;
            set;
        }

        public abstract bool TableVisible
        {
            get;
            set;
        }

        public abstract int LockMode
        {
            get;
            set;
        }

        public abstract double Max_y_axis_value
        {
            get;
            set;
        }

        public abstract bool IsRAMViewer
        {
            get;
            set;
        }

        public abstract bool IsUpsideDown
        {
            get;
            set;
        }

        public abstract double Correction_factor
        {
            get;
            set;
        }

        public abstract double Correction_offset
        {
            get;
            set;
        }

        public abstract bool GraphVisible
        {
            get;
            set;
        }

        public abstract bool IsRedWhite
        {
            get;
            set;
        }

        public abstract string Filename
        {
            get;
            set;
        }

        public abstract bool DatasourceMutated
        {
            get;
            set;
        }

        public abstract bool UseNewCompare
        {
            get;
            set;
        }

        public abstract bool SaveChanges
        {
            get;
            set;
        }

        public abstract byte[] Map_content
        {
            get;
            set;
        }

        public abstract byte[] Map_compare_content
        {
            get;
            set;
        }

        public abstract byte[] Map_original_content
        {
            get;
            set;
        }

        public abstract byte[] Values_changed_highlight_user
        {
            get;
            set;
        }

        public abstract byte[] Values_changed_highlight_ecu
        {
            get;
            set;
        }

        public abstract Int32 Map_address
        {
            get;
            set;
        }

        public abstract Int32 Map_sramaddress
        {
            get;
            set;
        }

        public abstract Int32 Map_length
        {
            get;
            set;
        }
        public abstract string Map_name
        {
            get;
            set;
        }

        public abstract string Map_descr
        {
            get;
            set;
        }

        public abstract XDFCategories Map_cat
        {
            get;
            set;
        }

        public abstract string X_axis_name
        {
            get;
            set;
        }
        public abstract string Y_axis_name
        {
            get;
            set;
        }
        public abstract string Z_axis_name
        {
            get;
            set;
        }

        public abstract int[] X_axisvalues
        {
            get;
            set;
        }
        public abstract int[] Y_axisvalues
        {
            get;
            set;
        }

        public abstract bool IsCompareViewer
        {
            get;
            set;
        }

        public abstract int[] Afr_counter
        {
            get;
            set;
        }

        public abstract int[] Turbo_press_tab
        {
            get;
            set;
        }

        public abstract int[] Ignition_lock_map
        {
            get;
            set;
        }

        public abstract int[] AFR_lock_map
        {
            get;
            set;
        }

        public abstract int[] IdleAFR_lock_map
        {
            get;
            set;
        }

        public abstract byte[] Open_loop_knock
        {
            get;
            set;
        }

        public abstract byte[] Open_loop
        {
            get;
            set;
        }

        public abstract int BoostAdaptionRpmFrom
        {
            get;
            set;
        }

        public abstract int BoostAdaptionRpmUpto
        {
            get;
            set;
        }

        public abstract int KnockAdaptionRpmFrom
        {
            get;
            set;
        }

        public abstract int KnockAdaptionRpmUpto
        {
            get;
            set;
        }

        public abstract int KnockAdaptionLoadFrom
        {
            get;
            set;
        }

        public abstract int KnockAdaptionLoadUpto
        {
            get;
            set;
        }

        public abstract double Rpm
        {
            get;
            set;
        }

        public abstract double Boost
        {
            get;
            set;
        }
        public abstract double BoostTarget
        {
            get;
            set;
        }

        public abstract double Coolant
        {
            get;
            set;
        }

        public abstract double Iat
        {
            get;
            set;
        }

        public abstract double Tps
        {
            get;
            set;
        }

        abstract public int DetermineWidth();
        abstract public void SetDataTable(DataTable dt);
        abstract public void LoadSymbol(string symbolname, IECUFile trionic_file);
        abstract public void LoadSymbol(string symbolname, IECUFile trionic_file, string sramfile);
        abstract public void SetSelectedTabPageIndex(int tabpageindex);
        abstract public void SetSplitter(int panel1height, int panel2height, int splitdistance, bool panel1collapsed, bool panel2collapsed);
        abstract public void SelectCell(int rowhandle, int colindex);
        abstract public void ShowTable(int tablewidth, bool issixteenbits);
        abstract public bool SaveData();
        abstract public void InitEditValues();
        abstract public void SetViewSize(ViewSize vs);
        abstract public void SetSurfaceGraphView(int pov_x, int pov_y, int pov_z, int pan_x, int pan_y, double pov_d);
        abstract public void SetSurfaceGraphViewEx(float depthx, float depthy, float zoom, float rotation, float elevation);

        public delegate void ViewerClose(object sender, EventArgs e);
        abstract public event ViewerClose onClose;

        public delegate void AxisEditorRequested(object sender, AxisEditorRequestedEventArgs e);
        abstract public event MapViewerEx.AxisEditorRequested onAxisEditorRequested;

        public delegate void ReadDataFromSRAM(object sender, ReadFromSRAMEventArgs e);
        abstract public event MapViewerEx.ReadDataFromSRAM onReadFromSRAM;
        public delegate void WriteDataToSRAM(object sender, WriteToSRAMEventArgs e);
        abstract public event MapViewerEx.WriteDataToSRAM onWriteToSRAM;

        public delegate void ViewTypeChanged(object sender, ViewTypeChangedEventArgs e);
        abstract public event MapViewerEx.ViewTypeChanged onViewTypeChanged;


        public delegate void GraphSelectionChanged(object sender, GraphSelectionChangedEventArgs e);
        abstract public event MapViewerEx.GraphSelectionChanged onGraphSelectionChanged;

        public delegate void SurfaceGraphViewChanged(object sender, SurfaceGraphViewChangedEventArgs e);
        abstract public event MapViewerEx.SurfaceGraphViewChanged onSurfaceGraphViewChanged;

        public delegate void SurfaceGraphViewChangedEx(object sender, SurfaceGraphViewChangedEventArgsEx e);
        abstract public event MapViewerEx.SurfaceGraphViewChangedEx onSurfaceGraphViewChangedEx;


        public delegate void NotifySaveSymbol(object sender, SaveSymbolEventArgs e);
        abstract public event MapViewerEx.NotifySaveSymbol onSymbolSave;

        public delegate void SplitterMoved(object sender, SplitterMovedEventArgs e);
        abstract public event MapViewerEx.SplitterMoved onSplitterMoved;

        public delegate void SelectionChanged(object sender, CellSelectionChangedEventArgs e);
        abstract public event MapViewerEx.SelectionChanged onSelectionChanged;

        public delegate void NotifyAxisLock(object sender, AxisLockEventArgs e);
        abstract public event MapViewerEx.NotifyAxisLock onAxisLock;

        public delegate void NotifySliderMove(object sender, SliderMoveEventArgs e);
        abstract public event MapViewerEx.NotifySliderMove onSliderMove;


        public delegate void CellLocked(object sender, CellLockedEventArgs e);
        abstract public event MapViewerEx.CellLocked onCellLocked;

        public class CellLockedEventArgs : System.EventArgs
        {
            private int _rowindex;

            public int Rowindex
            {
                get { return _rowindex; }
                set { _rowindex = value; }
            }
            private int _columnindex;

            public int Columnindex
            {
                get { return _columnindex; }
                set { _columnindex = value; }
            }
            private bool _locked;

            public bool Locked
            {
                get { return _locked; }
                set { _locked = value; }
            }


            public CellLockedEventArgs(int rowindex, int columnindex, bool locked)
            {
                this._rowindex = rowindex;
                this._columnindex = columnindex;
                this._locked = locked;
            }
        }

        public class AxisEditorRequestedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private string _filename;

            public string Filename
            {
                get { return _filename; }
                set { _filename = value; }
            }

            private AxisIdent _axisident;

            public AxisIdent Axisident
            {
                get { return _axisident; }
                set { _axisident = value; }
            }

            public AxisEditorRequestedEventArgs(AxisIdent ident, string mapname, string filename)
            {
                this._axisident = ident;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class ViewTypeChangedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private SuiteViewType _view;

            public SuiteViewType View
            {
                get { return _view; }
                set { _view = value; }
            }


            public ViewTypeChangedEventArgs(SuiteViewType view, string mapname)
            {
                this._view = view;
                this._mapname = mapname;
            }
        }

        public class AxisLockEventArgs : System.EventArgs
        {
            private int _y_axis_max_value;
            private string _mapname;
            private string _filename;
            private int _lock_mode;

            public int AxisMaxValue
            {
                get
                {
                    return _y_axis_max_value;
                }
            }

            public int LockMode
            {
                get
                {
                    return _lock_mode;
                }
            }

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }


            public string Filename
            {
                get
                {
                    return _filename;
                }
            }

            public AxisLockEventArgs(int max_value, int lockmode, string mapname, string filename)
            {
                this._y_axis_max_value = max_value;
                this._lock_mode = lockmode;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class SliderMoveEventArgs : System.EventArgs
        {
            private int _slider_position;
            private string _mapname;
            private string _filename;

            public int SliderPosition
            {
                get
                {
                    return _slider_position;
                }
            }

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }


            public string Filename
            {
                get
                {
                    return _filename;
                }
            }

            public SliderMoveEventArgs(int slider_position, string mapname, string filename)
            {
                this._slider_position = slider_position;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class SplitterMovedEventArgs : System.EventArgs
        {
            private int _splitdistance;

            public int Splitdistance
            {
                get { return _splitdistance; }
                set { _splitdistance = value; }
            }


            private int _panel1height;

            public int Panel1height
            {
                get { return _panel1height; }
                set { _panel1height = value; }
            }
            private int _panel2height;

            public int Panel2height
            {
                get { return _panel2height; }
                set { _panel2height = value; }
            }
            private bool _panel1collapsed;

            public bool Panel1collapsed
            {
                get { return _panel1collapsed; }
                set { _panel1collapsed = value; }
            }
            private bool _panel2collapsed;

            public bool Panel2collapsed
            {
                get { return _panel2collapsed; }
                set { _panel2collapsed = value; }
            }

            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public SplitterMovedEventArgs(int panel1height, int panel2height, int splitdistance, bool panel1collapsed, bool panel2collapsed, string mapname)
            {
                this._splitdistance = splitdistance;
                this._panel1collapsed = panel1collapsed;
                this._panel1height = panel1height;
                this._panel2collapsed = panel2collapsed;
                this._panel2height = panel2height;
                this._mapname = mapname;
            }
        }

        public class CellSelectionChangedEventArgs : System.EventArgs
        {
            private int _rowhandle;

            public int Rowhandle
            {
                get { return _rowhandle; }
                set { _rowhandle = value; }
            }
            private int _colindex;

            public int Colindex
            {
                get { return _colindex; }
                set { _colindex = value; }
            }

            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public CellSelectionChangedEventArgs(int rowhandle, int colindex, string mapname)
            {
                this._rowhandle = rowhandle;
                this._colindex = colindex;
                this._mapname = mapname;
            }

        }

        public enum AxisIdent : int
        {
            X_Axis = 0,
            Y_Axis = 1,
            Z_Axis = 2
        }

        public class ReadFromSRAMEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public ReadFromSRAMEventArgs(string mapname)
            {
                this._mapname = mapname;
            }
        }

        public class WriteToSRAMEventArgs : System.EventArgs
        {
            private byte[] _data;

            public byte[] Data
            {
                get { return _data; }
                set { _data = value; }
            }

            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public WriteToSRAMEventArgs(string mapname, byte[] data)
            {
                this._mapname = mapname;
                this._data = data;
            }
        }

        public class SaveSymbolEventArgs : System.EventArgs
        {
            private int _address;
            private int _length;
            private byte[] _mapdata;
            private string _mapname;
            private string _filename;

            public int SymbolAddress
            {
                get
                {
                    return _address;
                }
            }

            public int SymbolLength
            {
                get
                {
                    return _length;
                }
            }
            public byte[] SymbolDate
            {
                get
                {
                    return _mapdata;
                }
            }
            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }


            public string Filename
            {
                get
                {
                    return _filename;
                }
            }
            public SaveSymbolEventArgs(int address, int length, byte[] mapdata, string mapname, string filename)
            {
                this._address = address;
                this._length = length;
                this._mapdata = mapdata;
                this._mapname = mapname;
                this._filename = filename;
            }
        }

        public class GraphSelectionChangedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private int _tabpageindex;

            public int Tabpageindex
            {
                get { return _tabpageindex; }
                set { _tabpageindex = value; }
            }


            public GraphSelectionChangedEventArgs(int tabpageindex, string mapname)
            {
                this._tabpageindex = tabpageindex;
                this._mapname = mapname;
            }
        }


        public class SurfaceGraphViewChangedEventArgs : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private int _pov_x;

            public int Pov_x
            {
                get { return _pov_x; }
                set { _pov_x = value; }
            }
            private int _pov_y;

            public int Pov_y
            {
                get { return _pov_y; }
                set { _pov_y = value; }
            }
            private int _pov_z;

            public int Pov_z
            {
                get { return _pov_z; }
                set { _pov_z = value; }
            }
            private int _pan_x;

            public int Pan_x
            {
                get { return _pan_x; }
                set { _pan_x = value; }
            }
            private int _pan_y;

            public int Pan_y
            {
                get { return _pan_y; }
                set { _pan_y = value; }
            }
            private double _pov_d;

            public double Pov_d
            {
                get { return _pov_d; }
                set { _pov_d = value; }
            }

            public SurfaceGraphViewChangedEventArgs(int povx, int povy, int povz, int panx, int pany, double povd, string mapname)
            {
                this._pan_x = panx;
                this._pan_y = pany;
                this._pov_d = povd;
                this._pov_x = povx;
                this._pov_y = povy;
                this._pov_z = povz;
                this._mapname = mapname;
            }
        }

        public class SurfaceGraphViewChangedEventArgsEx : System.EventArgs
        {
            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            private float _depthx;

            public float DepthX
            {
                get { return _depthx; }
                set { _depthx = value; }
            }
            private float _depthy;

            public float DepthY
            {
                get { return _depthy; }
                set { _depthy = value; }
            }
            private float _zoom;

            public float Zoom
            {
                get { return _zoom; }
                set { _zoom = value; }
            }
            private float _rotation;

            public float Rotation
            {
                get { return _rotation; }
                set { _rotation = value; }
            }
            private float _elevation;

            public float Elevation
            {
                get { return _elevation; }
                set { _elevation = value; }
            }
            

            public SurfaceGraphViewChangedEventArgsEx(float depthx, float depthy, float zoom, float rotation, float elevation, string mapname)
            {
                this._depthx = depthx;
                this._depthy = depthy;
                this._zoom = zoom;
                this._rotation = rotation;
                this._elevation = elevation;
                this._mapname = mapname;
            }
        }

    }
}
