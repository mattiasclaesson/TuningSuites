using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonSuite
{
    public enum PanelMode : int
    {
        Day,
        Night
    }

    public enum MonitorType : int
    {
        Default,
        Dashboard
    }

    public enum AFRViewType : int
    {
        AFRMode,
        LambdaMode
    }

    public enum SuiteViewType : int
    {
        Hexadecimal = 0,
        Decimal,
        Easy,
        ASCII,
        Decimal3Bar,
        Easy3Bar,
        Decimal35Bar,
        Easy35Bar,
        Decimal4Bar,
        Easy4Bar,
        Decimal5Bar,
        Easy5Bar
    }

    public enum ViewSize : int
    {
        NormalView = 0,
        SmallView,
        ExtraSmallView,
        TouchscreenView
    }

    public enum MapviewerType : int
    {
        Fancy = 0,
        Normal,
        Simple
    }
}
