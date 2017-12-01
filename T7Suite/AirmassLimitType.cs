using System;
using System.Collections.Generic;

namespace T7
{
    public enum AirmassLimitType : int
    {
        None,
        TorqueLimiterEngine,
        TorqueLimiterEngineE85,
        TorqueLimiterGear,
        AirmassLimiter,
        TurboSpeedLimiter,
        FuelCutLimiter,
        OverBoostLimiter,
        AirTorqueCalibration,
        TorqueLimiterEngineE85Auto
    }
}
