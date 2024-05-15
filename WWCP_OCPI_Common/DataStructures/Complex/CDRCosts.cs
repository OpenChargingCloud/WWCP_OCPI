/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    public class CDRCosts(Decimal                                    TotalEnergy            = 0,
                          TimeSpan?                                  TotalTime              = null,

                          Decimal                                    TotalCost              = 0,
                          Decimal                                    TotalEnergyCost        = 0,
                          Decimal                                    TotalTimeCost          = 0,

                          Dictionary<String, CDRCosts.EnergyCosts>?  BilledEnergyElements   = null,
                          Dictionary<String, CDRCosts.TimeCosts>?    BilledTimeElements     = null)

    {


        public class EnergyCosts
        {
            public Decimal   Energy          { get; set; }
            public UInt32    StepSize        { get; set; }
            public Decimal   Price           { get; set; }
            public Decimal   BilledEnergy    { get; set; }
            public Decimal   EnergyCost      { get; set; }
        }

        public class TimeCosts
        {
            public TimeSpan  Time            { get; set; }
            public UInt32    StepSize        { get; set; }
            public Decimal   Price           { get; set; }
            public TimeSpan  BilledTime      { get; set; }
            public Decimal   TimeCost        { get; set; }
        }


        public Decimal   TotalEnergy        { get; set; }  = TotalEnergy;
        public TimeSpan  TotalTime          { get; set; }  = TotalTime ?? TimeSpan.Zero;

        public Decimal   BilledEnergy       { get; set; }
        public TimeSpan  BilledTime         { get; set; }

        public Decimal   TotalCost          { get; set; }  = TotalCost;
        public Decimal   TotalEnergyCost    { get; set; }  = TotalEnergyCost;
        public Decimal   TotalTimeCost      { get; set; }  = TotalTimeCost;

        public Dictionary<String, EnergyCosts>  BilledEnergyElements    { get; } = BilledEnergyElements ?? [];

        public Dictionary<String, TimeCosts>    BilledTimeElements      { get; } = BilledTimeElements   ?? [];


        public void BillEnergy(UInt32 StepSize, Decimal  Energy, Decimal Price)
        {

            if (!BilledEnergyElements.TryGetValue($"{StepSize}-{Price}", out var energy))
            {
                energy = new EnergyCosts {
                             StepSize  = StepSize,
                             Price     = Price
                         };
                BilledEnergyElements.Add($"{StepSize}-{Price}", energy);
            }

            energy.Energy += Energy;

        }

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



        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public CDRCosts Clone()

            => new (
                   TotalEnergy,
                   TotalTime,
                   TotalCost,
                   TotalEnergyCost,
                   TotalTimeCost,
                   BilledEnergyElements.ToDictionary(),
                   BilledTimeElements.  ToDictionary()
               );

        #endregion

    }


}
