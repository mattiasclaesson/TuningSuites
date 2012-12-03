using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T8SuitePro
{
    public partial class frmBitmaskViewer : DevExpress.XtraEditors.XtraForm
    {
        public frmBitmaskViewer()
        {
            InitializeComponent();
        }

        private uint _data = 0;

        public uint Data
        {
            get { return _data; }
            set { _data = value; }
        }
        private SymbolCollection _sc = new SymbolCollection();

        public void SetInformation(SymbolCollection sc, uint data)
        {
            _data = data;
            _sc = sc;
            checkEdit1.Text = "";
            checkEdit2.Text = "";
            checkEdit3.Text = "";
            checkEdit4.Text = "";
            checkEdit5.Text = "";
            checkEdit6.Text = "";
            checkEdit7.Text = "";
            checkEdit8.Text = "";
            checkEdit9.Text = "";
            checkEdit10.Text = "";
            checkEdit11.Text = "";
            checkEdit12.Text = "";
            checkEdit13.Text = "";
            checkEdit14.Text = "";
            checkEdit15.Text = "";
            checkEdit16.Text = "";
            checkEdit1.Enabled = false;
            checkEdit2.Enabled = false;
            checkEdit3.Enabled = false;
            checkEdit4.Enabled = false;
            checkEdit5.Enabled = false;
            checkEdit6.Enabled = false;
            checkEdit7.Enabled = false;
            checkEdit8.Enabled = false;
            checkEdit9.Enabled = false;
            checkEdit10.Enabled = false;
            checkEdit11.Enabled = false;
            checkEdit12.Enabled = false;
            checkEdit13.Enabled = false;
            checkEdit14.Enabled = false;
            checkEdit15.Enabled = false;
            checkEdit16.Enabled = false;
            foreach (SymbolHelper sh in sc)
            {
                switch(sh.BitMask)
                {
                    case 0x0001:
                        checkEdit1.Text = sh.Varname;
                        checkEdit1.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit1.Checked = true;
                        break;
                    case 0x0002:
                        checkEdit2.Text = sh.Varname;
                        checkEdit2.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit2.Checked = true;
                        break;
                    case 0x0004:
                        checkEdit3.Text = sh.Varname;
                        checkEdit3.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit3.Checked = true;
                        break;
                    case 0x0008:
                        checkEdit4.Text = sh.Varname;
                        checkEdit4.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit4.Checked = true;
                        break;
                    case 0x0010:
                        checkEdit5.Text = sh.Varname;
                        checkEdit5.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit5.Checked = true;
                        break;
                    case 0x0020:
                        checkEdit6.Text = sh.Varname;
                        checkEdit6.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit6.Checked = true;
                        break;
                    case 0x0040:
                        checkEdit7.Text = sh.Varname;
                        checkEdit7.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit7.Checked = true;
                        break;
                    case 0x0080:
                        checkEdit8.Text = sh.Varname;
                        checkEdit8.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit8.Checked = true;
                        break;
                    case 0x0100:
                        checkEdit9.Text = sh.Varname;
                        checkEdit9.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit9.Checked = true;
                        break;
                    case 0x0200:
                        checkEdit10.Text = sh.Varname;
                        checkEdit10.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit10.Checked = true;
                        break;
                    case 0x0400:
                        checkEdit11.Text = sh.Varname;
                        checkEdit11.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit11.Checked = true;
                        break;
                    case 0x0800:
                        checkEdit12.Text = sh.Varname;
                        checkEdit12.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit12.Checked = true;
                        break;
                    case 0x1000:
                        checkEdit13.Text = sh.Varname;
                        checkEdit13.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit13.Checked = true;
                        break;
                    case 0x2000:
                        checkEdit14.Text = sh.Varname;
                        checkEdit14.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit14.Checked = true;
                        break;
                    case 0x4000:
                        checkEdit15.Text = sh.Varname;
                        checkEdit15.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit15.Checked = true;
                        break;
                    case 0x8000:
                        checkEdit16.Text = sh.Varname;
                        checkEdit16.Enabled = true;
                        if ((data & sh.BitMask) > 0) checkEdit16.Checked = true;
                        break;


                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            _data = 0;
            foreach (SymbolHelper sh in _sc)
            {
                switch (sh.BitMask)
                {
                    case 0x0001:
                        if (checkEdit1.Enabled && checkEdit1.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0002:
                        if (checkEdit2.Enabled && checkEdit2.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0004:
                        if (checkEdit3.Enabled && checkEdit3.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0008:
                        if (checkEdit4.Enabled && checkEdit4.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0010:
                        if (checkEdit5.Enabled && checkEdit5.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0020:
                        if (checkEdit6.Enabled && checkEdit6.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0040:
                        if (checkEdit7.Enabled && checkEdit7.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0080:
                        if (checkEdit8.Enabled && checkEdit8.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0100:
                        if (checkEdit9.Enabled && checkEdit9.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0200:
                        if (checkEdit10.Enabled && checkEdit10.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0400:
                        if (checkEdit11.Enabled && checkEdit11.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x0800:
                        if (checkEdit12.Enabled && checkEdit12.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x1000:
                        if (checkEdit13.Enabled && checkEdit13.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x2000:
                        if (checkEdit14.Enabled && checkEdit14.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x4000:
                        if (checkEdit15.Enabled && checkEdit15.Checked) _data |= (uint)sh.BitMask;
                        break;
                    case 0x8000:
                        if (checkEdit16.Enabled && checkEdit16.Checked) _data |= (uint)sh.BitMask;
                        break;
                }
            }
            this.Close();
        }
    }
}