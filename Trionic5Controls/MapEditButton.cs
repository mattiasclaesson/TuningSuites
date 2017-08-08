using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace Trionic5Controls
{
    public partial class MapEditButton : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void NotifyMapOpenRequest(object sender, ShowSymbolEventArgs e);
        public event MapEditButton.NotifyMapOpenRequest onRequestMapDisplay;


        private frmMapSelect mapselect = null;
        private SymbolCollection _relevantsymbols = new SymbolCollection();

        public SymbolCollection Relevantsymbols
        {
            get { return _relevantsymbols; }
            set
            {
                _relevantsymbols = value;
                if (_relevantsymbols == null)
                {
                    _relevantsymbols = new SymbolCollection();
                    this.Enabled = false;
                }
                if (_relevantsymbols.Count == 0)
                {
                    this.Enabled = false;
                }
                else
                {
                    SymbolHelper sh_close = new SymbolHelper();
                    sh_close.Varname = "Close";
                    sh_close.Helptext = "Close";
                    _relevantsymbols.Add(sh_close);
                    this.Enabled = true;
                }
            }
        }

        public MapEditButton()
        {
            InitializeComponent();
        }

        private void btnMapChooser_Click(object sender, EventArgs e)
        {
            // show a little window with maps to choose from

            if (_relevantsymbols.Count > 0)
            {
                mapselect = new frmMapSelect();
                Point p2 = this.Parent.PointToScreen(this.Location);
                //Point p = PointToScreen(this.Location);
                p2.Offset(-mapselect.Width - 6, this.Height - mapselect.Height);
                mapselect.Location = p2;// PointToClient(p);
                mapselect.SetSymbolCollection(_relevantsymbols);
                mapselect.ShowDialog(); 
                //Console.WriteLine("Selected map after dialog: " + mapselect.SelectedMap);
                // let the main application know what map to display!
                if (mapselect.SelectedMap != "Close" && mapselect.SelectedMap != "")
                {
                    CastShowMapEvent(mapselect.SelectedMap);
                }

            }

        }

        private void CastShowMapEvent(string mapname)
        {
            if (onRequestMapDisplay != null)
            {
                onRequestMapDisplay(this, new ShowSymbolEventArgs(mapname));
            }
        }

        public class ShowSymbolEventArgs : System.EventArgs
        {
           
            private string _mapname;


            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }



            public ShowSymbolEventArgs(string mapname)
            {
                this._mapname = mapname;
            }
        }
    }
}
