using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using System.Xml;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class frmDefineMyMaps : DevExpress.XtraEditors.XtraForm
    {
        public frmDefineMyMaps()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            // save all parameters to the given filename
            SaveMyMaps(_filename);
            this.Close();
        }

        private void SaveMyMaps(string filename)
        {
            if (gridControl1.DataSource != null)
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                DataTable dt = (DataTable)gridControl1.DataSource;
                XmlDocument doc = new XmlDocument();// Create the XML Declaration, and append it to XML document
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
                doc.AppendChild(dec);// Create the root element
                XmlElement root = doc.CreateElement("categories");
                doc.AppendChild(root);

                SymbolCollection sc = new SymbolCollection();

                // first get all categories
                foreach (DataRow dr in dt.Rows)
                {
                    string category = dr["category"].ToString();
                    bool found = false;
                    foreach(SymbolHelper sh in sc)
                    {
                        if (sh.Varname == category) found = true;
                    }
                    if (!found)
                    {
                        SymbolHelper newsh = new SymbolHelper();
                        newsh.Varname = category;
                        sc.Add(newsh);
                    }
                }

                foreach (SymbolHelper sh in sc)
                {
                    XmlElement title = doc.CreateElement("category");
                    title.SetAttribute("title", sh.Varname);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["category"].ToString() == sh.Varname)
                        {
                            XmlElement map = doc.CreateElement("map");
                            map.SetAttribute("symbol", dr["symbol"].ToString());
                            map.SetAttribute("title", dr["title"].ToString());
                            title.AppendChild(map);
                        }
                    }
                    root.AppendChild(title);
                }
                /*
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        XmlElement title = doc.CreateElement("category");
                                        title.SetAttribute("title", dr["category"].ToString());
                                        XmlElement map = doc.CreateElement("map");
                                        map.SetAttribute("symbol", dr["symbol"].ToString());
                                        map.SetAttribute("title", dr["title"].ToString());
                                        title.AppendChild(map);
                                        root.AppendChild(title);
                                    }
                 * */
                doc.Save(filename);

            }
        }

        private void LoadMyMaps(string filename)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("symbol");
            dt.Columns.Add("category");
            dt.Columns.Add("title");

            if (File.Exists(filename))
            {
                try
                {
                    System.Xml.XmlDocument mymaps = new System.Xml.XmlDocument();
                    mymaps.Load(filename);

                    foreach (System.Xml.XmlNode category in mymaps.SelectNodes("categories/category"))
                    {
                        foreach (System.Xml.XmlNode map in category.SelectNodes("map"))
                        {
                            dt.Rows.Add(map.Attributes["symbol"].Value, category.Attributes["title"].Value, map.Attributes["title"].Value);
                        }
                        /*DevExpress.XtraBars.Ribbon.RibbonPageGroup rpg = new DevExpress.XtraBars.Ribbon.RibbonPageGroup(category.Attributes["title"].Value);
                        foreach (System.Xml.XmlNode map in category.SelectNodes("map"))
                        {
                            DevExpress.XtraBars.BarButtonItem btn = new BarButtonItem();
                            btn.Caption = map.Attributes["title"].Value;
                            btn.Tag = map.Attributes["symbol"].Value;
                            btn.ItemClick += new ItemClickEventHandler(MyMapItems_ItemClick);
                            rpg.ItemLinks.Add(btn);
                        }

                        page_maps.Groups.Add(rpg);*/

                    }
                }
                catch { }
                //ribbonControl1.Pages.Add(page_maps);
                gridControl1.DataSource = dt;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string _filename = string.Empty;

        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                LoadMyMaps(_filename);
            }
        }

        internal void CreateNewFile(string filename)
        {
            // create a new file
            CreateDefaultFile(filename);
            Filename = filename;
        }

        private void CreateDefaultFile(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<categories>");
                sw.WriteLine("<category title=\"Idle\">");
                sw.WriteLine("<map symbol=\"Idle_rpm_tab!\" title=\"Idle RPM\" />");
                sw.WriteLine("</category>");
                sw.WriteLine("<category title=\"Boost\">");
                sw.WriteLine("<map symbol=\"Tryck_mat!\" title=\"Boost Map\" />");
                sw.WriteLine("<map symbol=\"Reg_kon_mat!\" title=\"Reg Kon Mat\" />");
                sw.WriteLine("</category>");
                sw.WriteLine("</categories>");
            }
        }
    }
}