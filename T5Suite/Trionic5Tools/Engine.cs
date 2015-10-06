using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public class Engine
    {
        private float m_rpm = 0;
        private float m_coolant_temperature = 70;
        private float m_oil_temperature = 85;
        private float m_intake_air_temperature = 0;
        private float m_outside_air_temperature = 15;
        private float m_speed = 0;
        private float m_turbo_pressure = 0;
        private float m_wastegate_position = 0;
        private bool m_knocking = false;
        private float m_injection_time = 0;
        private float m_fuel_pressure = 0;
        private float m_throttleposition = 0; // 0 - 100%

        public float Throttleposition
        {
            get { return m_throttleposition; }
            set { m_throttleposition = value; }
        }
        private bool m_ignition = false;

        public bool Ignition
        {
            get { return m_ignition; }
            set { m_ignition = value; }
        }
        //private float m_engine_resistance = 0;
        //private float m_road_resistance = 0;
        //private float m_wind_resistance = 0;
        private float m_afr = 0;
        private bool m_fan_active = false;
        private float m_engine_load = 0; // 0 - 100%

        public delegate void NotifyEngineState(object sender, EngineStateEventArgs e);
        public event Engine.NotifyEngineState onEngineRunning;


        private System.Timers.Timer m_enginestate_timer = new System.Timers.Timer(100);

        public Engine()
        {
            m_enginestate_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_enginestate_timer_Elapsed);
            m_enginestate_timer.Enabled = false;
        }

        public void StartEngine()
        {
            m_ignition = true;
            m_fuel_pressure = 8; // init at 8 bar
        }

        public void StopEngine()
        {
            m_ignition = false;
            m_fuel_pressure = 0;
        }

        public void InitializeEngine()
        {
            m_enginestate_timer.Enabled = true;
        }


        public void TerminateEngine()
        {
            m_ignition = false;
            m_fuel_pressure = 0;
            m_enginestate_timer.Enabled = false;
        }

        private int m_updatecount = 0;
        private Random m_rand = new Random(DateTime.Now.Millisecond);

        private void RunEngine()
        {
            InterpolateCoolantTemperature();
            InterpolateOilTemperature();
            InterpolateIntakeAirTemperature();
            InterpolateRPMByThrottlePosition();
            InterpolateEngineLoad();
            InterpolateBoostPressure();

            m_afr = /*7F + ((float)m_rand.NextDouble() * 14.7F)*/ 15F;
            //m_throttleposition = 65;// DateTime.Now.Second;
            //Console.WriteLine("RPM: " + m_rpm.ToString("F0") + " COOLANT: " + m_coolant_temperature.ToString("F2") + " OIL: " + m_oil_temperature.ToString("F2") + " INTAKE: " + m_intake_air_temperature.ToString("F2") + " LOAD: "  + m_engine_load.ToString("F2"));

            /*if (onEngineRunning != null)
            {
                onEngineRunning(this, new EngineStateEventArgs(m_rpm, m_coolant_temperature, m_oil_temperature, m_engine_load, m_intake_air_temperature, m_speed, m_turbo_pressure, m_wastegate_position, m_knocking, m_injection_time, m_fuel_pressure, m_throttleposition, m_ignition, m_afr, m_fan_active));
            }*/
            
            m_updatecount++;
            if (m_updatecount == 2)
            {
                m_updatecount = 0;
                if (onEngineRunning != null)
                {
                    m_throttleposition++;
                    if (m_throttleposition == 90) m_throttleposition = 0;
                    onEngineRunning(this, new EngineStateEventArgs(m_rpm, m_coolant_temperature, m_oil_temperature, m_engine_load, m_intake_air_temperature, m_speed, m_turbo_pressure, m_wastegate_position, m_knocking, m_injection_time, m_fuel_pressure, m_throttleposition, m_ignition, m_afr, m_fan_active));
                }
            }
        }

        private void InterpolateBoostPressure()
        {
            m_turbo_pressure = m_throttleposition / 100;
            if (m_throttleposition < 70) m_turbo_pressure -= 0.5F;
        }

        private void InterpolateEngineLoad()
        {
            m_engine_load = m_throttleposition;
        }

        /// <summary>
        /// Estimate intake air temp by engine load, environment temp etc
        /// </summary>
        private void InterpolateIntakeAirTemperature()
        {
            m_intake_air_temperature = m_outside_air_temperature + (m_oil_temperature * (m_engine_load/200));
        }

        /// <summary>
        /// calculate estimate RPM by current engineload, throttleposition, engine temp etc
        /// </summary>
        private void InterpolateRPMByThrottlePosition()
        {
            if(!m_ignition) m_rpm = 0;
            else
            {
                float rpm_delta = 0;
                float throttle = m_throttleposition - 5;
                rpm_delta = m_throttleposition * (m_engine_load/200) * 50;
                m_rpm = 900 + rpm_delta;
            }
        }

        private void InterpolateOilTemperature()
        {
            if (m_ignition)
            {
                if (m_oil_temperature < 100)
                {
                    m_oil_temperature += 0.005F * (m_rpm / 1000);
                }
            }
            else
            {
                if (m_oil_temperature > m_outside_air_temperature)
                {
                    m_oil_temperature -= 0.005F;
                }
            }
        }

        private void InterpolateCoolantTemperature()
        {
            if (m_ignition)
            {
                // engine running
                if (!m_fan_active)
                {
                    if (m_coolant_temperature < 90)
                    {
                        m_coolant_temperature += 0.01F * ((m_oil_temperature / 100) + 1);
                    }
                    else
                    {
                        m_coolant_temperature += 0.001F;
                    }
                    if (m_coolant_temperature > 95) m_fan_active = true;
                }
                else
                {
                    m_coolant_temperature -= 0.01F;
                    if (m_coolant_temperature < 80) m_fan_active = false;
                }
            }
            else
            {
                if (m_coolant_temperature > m_outside_air_temperature)
                {
                    m_coolant_temperature -= 0.01F;
                }
            }
        }

        void m_enginestate_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Do engine state machine!
            m_enginestate_timer.Enabled = false;
            try
            {
                RunEngine();
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to run the engine: " + E.Message);
            }
            m_enginestate_timer.Enabled = true;
        }

        public class EngineStateEventArgs : System.EventArgs
        {
            private float _rpm;
            public float RPM
            {
                get
                {
                    return _rpm;
                }
            }
            private float _coolant_temp;
            public float CoolantTemperature
            {
                get
                {
                    return _coolant_temp;
                }
            }

            private float _oil_temp;
            public float OilTemperature
            {
                get
                {
                    return _oil_temp;
                }
            }


            private float _engine_load;
            public float EngineLoad
            {
                get
                {
                    return _engine_load;
                }
            }

            private float _intake_temp;
            public float IntakeAitTemperature
            {
                get
                {
                    return _intake_temp;
                }
            }

            private float _speed;
            public float Speed
            {
                get
                {
                    return _speed;
                }
            }

            private float _turbo_pressure ;
            public float TurboPressure
            {
                get{
                    return _turbo_pressure;
                }
            }

            private float _wastegate_position;
            public float WastegatePosition
            {
                get
                {
                    return _wastegate_position;
                }
            }

            private bool _knocking ;
            public bool Knocking
            {
                get
                {
                    return _knocking;
                }
            }

            private float _injection_time;
            public float InjectionTime
            {
                get
                {
                    return _injection_time;
                }
            }

            private float _fuel_pressure;
            public float FuelPressure
            {
                get
                {
                    return _fuel_pressure;
                }
            }

            private float _throttleposition;
            public float ThrottlePosition
            {
                get
                {
                    return _throttleposition;
                }
            }

            private bool _ignition;
            public bool IgnitionOn
            {
                get
                {
                    return _ignition;
                }
            }
            
            private float _afr;
            public float AirFuelRatio
            {
                get
                {
                    return _afr;
                }
            }
            private bool _fan_active ;
            public bool FanActive
            {
                get
                {
                    return _fan_active;
                }
            }


            public EngineStateEventArgs(float rpm, float coolanttemp, float oiltemp, float engineload, float intakeairtemp, float speed, float turbopressure, float wastegateposition, bool knocking, float injectiontime, float fuelpressure, float throttleposition, bool ignition, float afr, bool fanactive)
            {
                this._afr = afr;
                this._coolant_temp = coolanttemp;
                this._engine_load= engineload;
                this._fan_active = fanactive;
                this._fuel_pressure = fuelpressure;
                this._ignition = ignition;
                this._injection_time = injectiontime;
                this._intake_temp = intakeairtemp;
                this._knocking = knocking;
                this._oil_temp = oiltemp;
                this._rpm = rpm;
                this._speed = speed;
                this._throttleposition = throttleposition;
                this._turbo_pressure = turbopressure;
                this._wastegate_position = wastegateposition;
            }
        }

    }
}
