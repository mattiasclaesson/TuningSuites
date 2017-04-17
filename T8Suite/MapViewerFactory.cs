using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonSuite;

namespace T8SuitePro
{
    static class MapViewerFactory
    {
        public static IMapViewer Get(AppSettings m_appSettings)
        {
            IMapViewer mapviewer;
            if (m_appSettings.UseNewMapViewer)
            {
                mapviewer = new MapViewerEx();
            }
            else
            {
                mapviewer = new MapViewer();
            }
            mapviewer.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
            mapviewer.AutoUpdateIfSRAM = m_appSettings.AutoUpdateSRAMViewers;
            mapviewer.AutoUpdateInterval = m_appSettings.AutoUpdateInterval;
            mapviewer.DisableColors = m_appSettings.DisableMapviewerColors;
            mapviewer.GraphVisible = m_appSettings.ShowGraphs;
            mapviewer.IsRedWhite = m_appSettings.ShowRedWhite;
            mapviewer.SetViewSize(m_appSettings.DefaultViewSize);
            mapviewer.Viewtype = m_appSettings.DefaultViewType;

            return mapviewer;
        }
    }
}
