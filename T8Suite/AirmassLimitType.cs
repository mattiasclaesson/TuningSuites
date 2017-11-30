using System;
using System.Collections.Generic;

namespace T8SuitePro
{
    public enum AirmassLimitType : int
    {
        None,
        TorqueLimiterEngine,
        TorqueLimiterEngineE85,
        TorqueLimiterGear,
        AirmassLimiter,
        FuelCutLimiter,
        OverBoostLimiter,
        AirTorqueCalibration
    }
}
