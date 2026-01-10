/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// CDR Cost details.
    /// </summary>
    /// <param name="TotalEnergy">The total amount of energy charged.</param>
    /// <param name="TotalTime">The total amount of time.</param>
    /// 
    /// <param name="TotalFlatCost">The total flat cost.</param>
    /// <param name="TotalEnergyCost">The total energy cost.</param>
    /// <param name="TotalTimeCost">The total time cost.</param>
    /// <param name="TotalCost">The total cost.</param>
    /// 
    /// <param name="BilledFlatElements">The billed flat elements.</param>
    /// <param name="BilledEnergyElements">The billed energy elements.</param>
    /// <param name="BilledTimeElements">The billed time elements.</param>
    public class CDRCostDetails(WattHour?                                        TotalEnergy            = null,
                                TimeSpan?                                        TotalTime              = null,

                                Decimal                                          TotalFlatCost          = 0,
                                Decimal                                          TotalEnergyCost        = 0,
                                Decimal                                          TotalTimeCost          = 0,
                                Decimal                                          TotalCost              = 0,

                                Dictionary<String, CDRCostDetails.FlatCosts>?    BilledFlatElements     = null,
                                Dictionary<String, CDRCostDetails.EnergyCosts>?  BilledEnergyElements   = null,
                                Dictionary<String, CDRCostDetails.TimeCosts>?    BilledTimeElements     = null)
    {


        #region (class) FlatCosts

        public class FlatCosts
        {

            public Decimal   Price           { get; set; }


            #region ToJSON(CostDigits = 3)

            /// <summary>
            /// Return a JSON representation of this object.
            /// </summary>
            /// <param name="CostDigits">The number of digits after the comma.</param>
            public JObject ToJSON(Byte CostDigits = 3)

                => JSONObject.Create(
                       new JProperty("price",   Math.Round(Price, CostDigits))
                   );

            #endregion

        }

        #endregion

        #region (class) EnergyCosts

        public class EnergyCosts
        {

            public WattHour  Energy          { get; set; }
            public UInt32    StepSize        { get; set; }
            public Decimal   Price           { get; set; }
            public WattHour  BilledEnergy    { get; set; }
            public Decimal   EnergyCost      { get; set; }


            #region ToJSON(CostDigits = 3)

            /// <summary>
            /// Return a JSON representation of this object.
            /// </summary>
            /// <param name="CostDigits">The number of digits after the comma.</param>
            public JObject ToJSON(Byte CostDigits = 3)

                => JSONObject.Create(
                       new JProperty("energy",          Energy.      kWh),
                       new JProperty("step_size",       StepSize),
                       new JProperty("price",           Price),
                       new JProperty("billed_energy",   BilledEnergy.kWh),
                       new JProperty("energy_cost",     Math.Round(EnergyCost, CostDigits))
                   );

            #endregion

        }

        #endregion

        #region (class) TimeCosts

        public class TimeCosts
        {

            public TimeSpan  Time            { get; set; }
            public UInt32    StepSize        { get; set; }
            public Decimal   Price           { get; set; }
            public TimeSpan  BilledTime      { get; set; }
            public Decimal   TimeCost        { get; set; }


            #region ToJSON(CostDigits = 3)

            /// <summary>
            /// Return a JSON representation of this object.
            /// </summary>
            /// <param name="CostDigits">The number of digits after the comma.</param>
            public JObject ToJSON(Byte CostDigits = 3)

                => JSONObject.Create(
                       new JProperty("time",          Time),
                       new JProperty("step_size",     StepSize),
                       new JProperty("price",         Price),
                       new JProperty("billed_time",   BilledTime),
                       new JProperty("time_cost",     Math.Round(TimeCost, CostDigits))
                   );

            #endregion

        }

        #endregion


        #region Properties

        public WattHour  TotalEnergy        { get; set; }  = TotalEnergy ?? WattHour.Zero;
        public TimeSpan  TotalTime          { get; set; }  = TotalTime   ?? TimeSpan.Zero;

        public WattHour  BilledEnergy       { get; set; }
        public TimeSpan  BilledTime         { get; set; }

        public Decimal   TotalCost          { get; set; }  = TotalCost;
        public Decimal   TotalEnergyCost    { get; set; }  = TotalEnergyCost;
        public Decimal   TotalTimeCost      { get; set; }  = TotalTimeCost;
        public Decimal   TotalFlatCost      { get; set; }  = TotalFlatCost;


        public Dictionary<String, EnergyCosts>  BilledEnergyElements    { get; } = BilledEnergyElements ?? [];

        public Dictionary<String, TimeCosts>    BilledTimeElements      { get; } = BilledTimeElements   ?? [];

        public Dictionary<String, FlatCosts>    BilledFlatElements      { get; } = BilledFlatElements   ?? [];

        #endregion


        #region BillEnergy (StepSize, Energy, Price)

        public void BillEnergy(UInt32 StepSize, WattHour Energy, Decimal Price)
        {

            if (!BilledEnergyElements.TryGetValue($"{StepSize}-{Price}", out var energy))
            {
                energy = new EnergyCosts {
                             StepSize  = StepSize,
                             Price     = Price
                         };
                BilledEnergyElements.Add($"{StepSize}-{Price}", energy);
            }

            energy.Energy = energy.Energy + Energy;

        }

        #endregion

        #region BillTime   (StepSize, Time,   Price)

        public void BillTime  (UInt32 StepSize, TimeSpan Time,   Decimal Price)
        {

            if (!BilledTimeElements.TryGetValue($"{StepSize}-{Price}", out var time))
            {
                time = new TimeCosts {
                           StepSize  = StepSize,
                           Price     = Price
                       };
                BilledTimeElements.Add($"{StepSize}-{Price}", time);
            }

            time.Time += Time;

        }

        #endregion

        #region BillFlat   (                  Price)

        public void BillFlat  (Decimal Price)
        {

            // A FLAT cost is a cost that is not dependent on the amount of energy
            // or time used and thus will only be billed once!
            // When there are multiple different flat costs, they will be summed up!
            if (!BilledFlatElements.TryGetValue($"{Price}", out var flat))
            {
                flat = new FlatCosts {
                           Price     = Price
                       };
                BilledFlatElements.Add($"{Price}", flat);
            }

        }

        #endregion


        #region ToJSON(CustomCDRCostDetailsSerializer = null, CostDigits = 3, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRCostDetailsSerializer">A delegate to serialize custom CDRCostDetails JSON objects.</param>
        /// <param name="CostDigits">The number of digits after the comma.</param>
        public JObject ToJSON(Byte                                              CostDigits                       = 3,
                              CustomJObjectSerializerDelegate<CDRCostDetails>?  CustomCDRCostDetailsSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("total_energy",             TotalEnergy.kWh),
                                 new JProperty("total_time",               TotalTime.  TotalHours),

                                 new JProperty("total_flat_cost",          Math.Round(TotalFlatCost,   CostDigits)),
                                 new JProperty("total_energy_cost",        Math.Round(TotalEnergyCost, CostDigits)),
                                 new JProperty("total_time_cost",          Math.Round(TotalTimeCost,   CostDigits)),
                                 new JProperty("total_cost",               Math.Round(TotalCost,       CostDigits)),

                           BilledFlatElements.  Count != 0
                               ? new JProperty("billed_flat_elements",     new JArray(BilledFlatElements.  Select(billedFlatElement   => billedFlatElement.  Value.ToJSON(CostDigits))))
                               : null,

                           BilledEnergyElements.Count != 0
                               ? new JProperty("billed_energy_elements",   new JArray(BilledEnergyElements.Select(billedEnergyElement => billedEnergyElement.Value.ToJSON(CostDigits))))
                               : null,

                           BilledTimeElements.Count   != 0
                               ? new JProperty("billed_time_elements",     new JArray(BilledTimeElements.  Select(billedTimeElement   => billedTimeElement.  Value.ToJSON(CostDigits))))
                               : null

                       );

            return CustomCDRCostDetailsSerializer is not null
                       ? CustomCDRCostDetailsSerializer(this, json)
                       : json;

        }

        #endregion


        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public CDRCostDetails Clone()

            => new (

                   TotalEnergy,
                   TotalTime,

                   TotalFlatCost,
                   TotalEnergyCost,
                   TotalTimeCost,
                   TotalCost,

                   BilledFlatElements.  ToDictionary(),
                   BilledEnergyElements.ToDictionary(),
                   BilledTimeElements.  ToDictionary()

               );

        #endregion


    }

}
