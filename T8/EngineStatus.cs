using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonSuite
{
    class EngineStatus
    {
        private bool _HeadlightsOn = false;

        public bool HeadlightsOn
        {
            get { return _HeadlightsOn; }
            set { _HeadlightsOn = value; }
        }
        
        
        private float _currentRPM = 0;

        public float CurrentRPM
        {
            get { return _currentRPM; }
            set { _currentRPM = value; }
        }
        private float _currentEngineTemp = 0;

        public float CurrentEngineTemp
        {
            get { return _currentEngineTemp; }
            set { _currentEngineTemp = value; }
        }
        private float _currentIAT = 0;

        public float CurrentIAT
        {
            get { return _currentIAT; }
            set { _currentIAT = value; }
        }
        private int _currentAirmassLimiterID = 0;

        public int CurrentAirmassLimiterID
        {
            get { return _currentAirmassLimiterID; }
            set { _currentAirmassLimiterID = value; }
        }
        private int _currentLambdaStatus = 0;

        public int CurrentLambdaStatus
        {
            get { return _currentLambdaStatus; }
            set { _currentLambdaStatus = value; }
        }


        private int _currentFuelcutStatus = 0;

        public int CurrentFuelcutStatus
        {
            get { return _currentFuelcutStatus; }
            set { _currentFuelcutStatus = value; }
        }

        private float _currentIgnitionOffset = 0;

        public float CurrentIgnitionOffset
        {
            get { return _currentIgnitionOffset; }
            set { _currentIgnitionOffset = value; }
        }
        private float _currentAirmassRequest = 0;

        public float CurrentAirmassRequest
        {
            get { return _currentAirmassRequest; }
            set { _currentAirmassRequest = value; }
        }
        private float _currentEngineTorque = 0;

        public float CurrentEngineTorque
        {
            get { return _currentEngineTorque; }
            set { _currentEngineTorque = value; }
        }
        private float _currentEnginePower = 0;

        public float CurrentEnginePower
        {
            get { return _currentEnginePower; }
            set { _currentEnginePower = value; }
        }
        private float _currentBoostPressure = 0;

        public float CurrentBoostPressure
        {
            get { return _currentBoostPressure; }
            set { _currentBoostPressure = value; }
        }
        private float _currentBoostValvePWM = 0;

        public float CurrentBoostValvePWM
        {
            get { return _currentBoostValvePWM; }
            set { _currentBoostValvePWM = value; }
        }
        private float _currentIgnitionAdvance = 0;

        public float CurrentIgnitionAdvance
        {
            get { return _currentIgnitionAdvance; }
            set { _currentIgnitionAdvance = value; }
        }
        private float _currentThrottlePosition = 0;

        public float CurrentThrottlePosition
        {
            get { return _currentThrottlePosition; }
            set { _currentThrottlePosition = value; }
        }

        private float _currentLambda = 0;

        public float CurrentLambda
        {
            get { return _currentLambda; }
            set { _currentLambda = value; }
        }
        private float _currentAFR = 0;

        public float CurrentAFR
        {
            get { return _currentAFR; }
            set { _currentAFR = value; }
        }

        private float _currentAirmassPerCombustion = 0;

        public float CurrentAirmassPerCombustion
        {
            get { return _currentAirmassPerCombustion; }
            set { _currentAirmassPerCombustion = value; }
        }
        private float _currentVehicleSpeed = 0;

        public float CurrentVehicleSpeed
        {
            get { return _currentVehicleSpeed; }
            set { _currentVehicleSpeed = value; }
        }
        private float _currentEGT = 0;

        public float CurrentEGT
        {
            get { return _currentEGT; }
            set { _currentEGT = value; }
        }
        private float _currentConsumption = 0;

        public float CurrentConsumption
        {
            get { return _currentConsumption; }
            set { _currentConsumption = value; }
        }

    }
}
