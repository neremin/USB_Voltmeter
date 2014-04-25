using System;
using System.Linq;
using System.Reflection;
using Voltmeter.Resources;

namespace Voltmeter.Model
{
    public static class Units
    {
        static Unit[] GetAllUnits(Type containerType)
        {
            return
                containerType.GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(field => field.GetValue(null))
                    .OfType<Unit>()
                    .OrderByDescending(unit => unit.Scale + unit.Shift)
                    .ToArray();
        }

        public static class Power
        {
            public static readonly string Type      = UnitNames.Power_Type;

            /// <summary>
            /// System units
            /// </summary>
            public static readonly Unit Watts       = new Unit(Type, UnitNames.Power_Watt,      UnitNames.Power_Watt_Abbr, 1.0d);
            public static readonly Unit kiloWatts   = new Unit(Type, UnitNames.Power_kiloWatt,  UnitNames.Power_kiloWatt_Abbr, 0.001d);
            public static readonly Unit milliWatts  = new Unit(Type, UnitNames.Power_milliWatt, UnitNames.Power_milliWatt_Abbr, 1000.0d);

            public static readonly Unit[] All       = GetAllUnits(typeof(Power));

        }

        public static class Current
        {
            public static readonly string Type      = UnitNames.Current_Type;

            /// <summary>
            /// System units
            /// </summary>
            public static readonly Unit Ampers      = new Unit(Type, UnitNames.Current_Ampere,      UnitNames.Current_Ampere_Abbr, 1.0d);
            public static readonly Unit milliAmpers = new Unit(Type, UnitNames.Current_milliAmpere, UnitNames.Current_milliAmpere_Abbr, 1000.0d);
            public static readonly Unit microAmpers = new Unit(Type, UnitNames.Current_microAmpere, UnitNames.Current_microAmpere_Abbr, 1000000.0d);

            public static readonly Unit[] All       = GetAllUnits(typeof(Current));

        }

        public static class Voltage
        {
            public static readonly string Type      = UnitNames.Voltage_Type;

            /// <summary>
            /// System units
            /// </summary>
            public static readonly Unit Volts       = new Unit(Type, UnitNames.Voltage_Volt, UnitNames.Voltage_Volt_Abbr, 1.0d);
            public static readonly Unit kiloVolts   = new Unit(Type, UnitNames.Voltage_kiloVolt, UnitNames.Voltage_kiloVolt_Abbr, 0.001d);
            public static readonly Unit milliVolts  = new Unit(Type, UnitNames.Voltage_milliVolt, UnitNames.Voltage_milliVolt_Abbr, 1000.0d);

            public static readonly Unit[] All       = GetAllUnits(typeof (Voltage));
        }
        
        public static class Resistance
        {
            public static readonly string Type      = UnitNames.Resistance_Type;

            /// <summary>
            /// System units
            /// </summary>
            public static readonly Unit Ohm         = new Unit(Type, UnitNames.Resistance_Ohm, UnitNames.Resistance_Ohm_Abbr, 1.0d);
            public static readonly Unit kiloOhm     = new Unit(Type, UnitNames.Resistance_kiloOhm, UnitNames.Resistance_kiloOhm_Abbr, 0.001d);
            public static readonly Unit milliОhm    = new Unit(Type, UnitNames.Resistance_milliOhm, UnitNames.Resistance_milliOhm_Abbr, 1000.0d);

            public static readonly Unit[] All       = GetAllUnits(typeof(Resistance));
        }

        public static class Energy
        {
            public static readonly string Type          = UnitNames.Energy_Type;

            /// <summary>
            /// System units
            /// </summary>
            public static readonly Unit WattSecond      = new Unit(Type, UnitNames.Energy_WattSec, UnitNames.Energy_WattSec_Abbr, 1.0d);
            public static readonly Unit WattHour        = new Unit(Type, UnitNames.Energy_WattHour, UnitNames.Energy_WattHour_Abbr, 1.0d / (60 * 60));
            public static readonly Unit kiloWattHour    = new Unit(Type, UnitNames.Energy_kiloWattHour, UnitNames.Energy_kiloWattHour_Abbr, 0.001d / (60 * 60));
            public static readonly Unit milliWattSecond = new Unit(Type, UnitNames.Energy_milliWattSec, UnitNames.Energy_milliWattSec_Abbr, 1000.0d);

            public static readonly Unit[] All           = GetAllUnits(typeof (Energy));
        }

    }
}
