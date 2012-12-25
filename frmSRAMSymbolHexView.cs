using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Be.Windows.Forms;

namespace T7
{
    public partial class frmSRAMSymbolHexView : DevExpress.XtraEditors.XtraForm
    {
        private Be.Windows.Forms.DynamicByteProvider m_dynamicByteProvider;

        public frmSRAMSymbolHexView()
        {
            InitializeComponent();

            m_dynamicByteProvider = new DynamicByteProvider(new byte[0]);
            hexBox1.ByteProvider = m_dynamicByteProvider;

        }

        public void SetData(byte[] data)
        {
            m_dynamicByteProvider.DeleteBytes(0, m_dynamicByteProvider.Length);
            m_dynamicByteProvider.Bytes.InsertRange(0, data);
            m_dynamicByteProvider.ApplyChanges();
            hexBox1.Refresh();
            
        }
    }
}