using System;
using System.Collections.Generic;
using System.Linq;
using Trionic5Tools;
using CommonSuite;

namespace Trionic5Controls
{
    public static class MapViewerFactory
    {
        public static IMapViewer Get(T5AppSettings m_appSettings)
        {
            IMapViewer mapviewer;
            if (m_appSettings.MapViewerType == MapviewerType.Fancy)
            {
                mapviewer = new MapViewerEx();
            }
            else if (m_appSettings.MapViewerType == MapviewerType.Normal)
            {
                mapviewer = new MapViewer();
            }
            else
            {
                mapviewer = new SimpleMapViewer();
            }
            mapviewer.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
            mapviewer.DisableColors = m_appSettings.DisableMapviewerColors;
            mapviewer.GraphVisible = m_appSettings.ShowGraphs;
            mapviewer.IsRedWhite = m_appSettings.ShowRedWhite;
            mapviewer.SetViewSize(m_appSettings.DefaultViewSize);
            mapviewer.Viewtype = m_appSettings.DefaultViewType;
            mapviewer.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            mapviewer.GraphVisible = m_appSettings.ShowGraphs;

            return mapviewer;
        }

        public static IMapViewer Get(T5AppSettings m_appSettings, IECUFile file)
        {
            IMapViewer mapviewer = Get(m_appSettings);

            if (file.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
            {
                if (m_appSettings.DefaultViewType == SuiteViewType.Decimal)
                {
                    mapviewer.Viewtype = SuiteViewType.Decimal;
                }
                else
                {
                    mapviewer.Viewtype = SuiteViewType.Easy;
                }
            }
            else if (file.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
            {
                if (m_appSettings.DefaultViewType == SuiteViewType.Decimal)
                {
                    mapviewer.Viewtype = SuiteViewType.Decimal3Bar;
                }
                else
                {
                    mapviewer.Viewtype = SuiteViewType.Easy3Bar;
                }
            }
            else if (file.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
            {
                if (m_appSettings.DefaultViewType == SuiteViewType.Decimal)
                {
                    mapviewer.Viewtype = SuiteViewType.Decimal35Bar;
                }
                else
                {
                    mapviewer.Viewtype = SuiteViewType.Easy35Bar;
                }
            }
            else if (file.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
            {
                if (m_appSettings.DefaultViewType == SuiteViewType.Decimal)
                {
                    mapviewer.Viewtype = SuiteViewType.Decimal4Bar;
                }
                else
                {
                    mapviewer.Viewtype = SuiteViewType.Easy4Bar;
                }
            }
            else if (file.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
            {
                if (m_appSettings.DefaultViewType == SuiteViewType.Decimal)
                {
                    mapviewer.Viewtype = SuiteViewType.Decimal5Bar;
                }
                else
                {
                    mapviewer.Viewtype = SuiteViewType.Easy5Bar;
                }
            }

            return mapviewer;
        }
    }
}
