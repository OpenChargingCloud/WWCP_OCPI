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

using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// Extensions method for charge detail records.
    /// </summary>
    public static class CDRExtensions
    {

        #region SplittIntoChargingPeriods(this CDR, MeteringValues = null, AlternativeTariffs = null, ExtrapolateEnergy = false)

        public static CDR SplittIntoChargingPeriods(this CDR                             CDR,
                                                    IEnumerable<Timestamped<WattHour>>?  MeteringValues       = null,
                                                    IEnumerable<Tariff>?                 AlternativeTariffs   = null,
                                                    Boolean                              ExtrapolateEnergy    = false)
        {

            try
            {

                if (MeteringValues is null)
                {

                    if (CDR.SignedData?.SignedValues?.Count() >= 2)
                    {

                        MeteringValues = [
                                             new Timestamped<WattHour>(CDR.Start, WattHour.ParseKWh(CDR.SignedData.SignedValues.ElementAt(0).PlainData)),
                                             new Timestamped<WattHour>(CDR.End,   WattHour.ParseKWh(CDR.SignedData.SignedValues.ElementAt(1).PlainData))
                                         ];

                    }

                    else if (CDR.ChargingPeriods.Any())
                    {

                        MeteringValues = [
                                             new Timestamped<WattHour>(CDR.Start, WattHour.Zero),
                                             new Timestamped<WattHour>(CDR.End,   WattHour.ParseKWh(CDR.ChargingPeriods.Last().Dimensions.First(dimension => dimension.Type == CDRDimensionType.ENERGY).Volume))
                                         ];

                    }

                }

                if (MeteringValues is null)
                    throw new Exception("No metering values given!");

                if (MeteringValues.Count() < 2)
                    throw new Exception("The given enumeration of metering values must contain at least two values!");

                if (MeteringValues.First().Timestamp < CDR.Start)
                    throw new Exception("The first metering value must be larger or equal to the start timestamp of the charge detail record!");

                // Can a CDR be longer than the last metering value, e.g. because of parking?
                if (MeteringValues.Last(). Timestamp > CDR.End)
                    throw new Exception("The last metering value must be lower or equal to the stop timestamp of the charge detail record!");


                AlternativeTariffs ??= CDR.Tariffs;

                if (!AlternativeTariffs.Any())
                    throw new Exception("No tariffs given!");

                var timeMarkers     = new HashSet<DateTimeOffset>() { CDR.Start };
                var meteringValues  = MeteringValues.ToArray();
                var tariff          = AlternativeTariffs.First();

                for (var i = 0; i < meteringValues.Length; i++)
                {
                    timeMarkers.Add(meteringValues[i].Timestamp);
                }

                if (tariff.TariffElements.Any(tariffElement => tariffElement.HasRestrictions()))
                {
                    foreach (var tariffElement in tariff.TariffElements)
                    {
                        if (tariffElement.TariffRestrictions.HasRestrictions())
                        {

                            if (tariffElement.TariffRestrictions.StartTime.  HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.EndTime.    HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.StartDate.  HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.EndDate.    HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.MinkWh.     HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.MaxkWh.     HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.MinPower.   HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.MaxPower.   HasValue)
                            { }

                            if (tariffElement.TariffRestrictions.MinDuration.HasValue)
                                timeMarkers.Add(CDR.Start + tariffElement.TariffRestrictions.MinDuration.Value);

                            if (tariffElement.TariffRestrictions.MaxDuration.HasValue)
                                timeMarkers.Add(CDR.Start + tariffElement.TariffRestrictions.MaxDuration.Value);

                            if (tariffElement.TariffRestrictions.DayOfWeek.  Any())
                            { }

                        }
                    }
                }


                // The last time marker is removed, when it is equal to the stop timestamp of the CDR!
                var chargingPeriods = timeMarkers.Where(timeMarker => timeMarker >= CDR.Start && timeMarker < CDR.End).
                                                  Order().Select(tm => ChargingPeriod.Create(tm)).ToList();

                for (var i=0; i<chargingPeriods.Count; i++)
                {

                    chargingPeriods[i].Id = (UInt32) (i + 1);

                    if (i  > 0)
                        chargingPeriods[i].Previous = chargingPeriods[i-1];

                    if (i  < chargingPeriods.Count - 1)
                        chargingPeriods[i].Next     = chargingPeriods[i+1];

                    if (i == chargingPeriods.Count - 1)
                        chargingPeriods[i].SetEndTimestamp(CDR.End);

                    var meteringValuesStart = meteringValues.Where(meteringValue => meteringValue.Timestamp == chargingPeriods[i].StartTimestamp);
                    if (meteringValuesStart.Any())
                        chargingPeriods[i].StartMeteringValue = MeteringValue.Measured(
                                                                    chargingPeriods[i].StartTimestamp,
                                                                    meteringValuesStart.First().Value
                                                                );

                    var meteringValuesStop  = meteringValues.Where(meteringValue => meteringValue.Timestamp == chargingPeriods[i].StopTimestamp);
                    if (meteringValuesStop.Any())
                        chargingPeriods[i].StopMeteringValue  = MeteringValue.Measured(
                                                                    chargingPeriods[i].StopTimestamp!.Value,
                                                                    meteringValuesStop.First().Value
                                                                );

                    if (tariff.IsActive(chargingPeriods[i]))
                    {

                        var activeTariffElements = tariff.TariffElements.Where(tariffElement => tariffElement.IsActive(chargingPeriods[i]));

                        if (activeTariffElements.Any())
                        {
                            // It is always the first matching tariff element (by specification!)
                            foreach (var priceComponent in activeTariffElements.First().PriceComponents)
                            {
                                chargingPeriods[i].PriceComponents.Add(priceComponent.Type, priceComponent);
                            }
                        }

                    }

                }


                for (var i = 1; i < chargingPeriods.Count; i++)
                {
                    if (chargingPeriods[i].StartMeteringValue is null)
                    {

                        var previousMV    = chargingPeriods[i].Previous!.StartMeteringValue!;
                        var nextCP        = chargingPeriods[i].Next;

                        // Skip consecutive null values!
                        while (nextCP is not null && nextCP.StartMeteringValue is null)
                            nextCP = nextCP.Next;

                        var nextMV        = (nextCP?.StartMeteringValue ?? chargingPeriods[i].StopMeteringValue)
                                                ?? throw new Exception($"Could not calculate '{i}.' imputed value!");

                        var imputedValue  = WattHour.ParseWh((nextMV.WattHours - previousMV.Value.WattHours).Value /
                                                             Convert.ToDecimal((nextMV.            Timestamp      - previousMV.Value.Timestamp).TotalSeconds) *
                                                             Convert.ToDecimal((chargingPeriods[i].StartTimestamp - previousMV.Value.Timestamp).TotalSeconds)) + previousMV.Value.WattHours;

                        chargingPeriods[i].StartMeteringValue = MeteringValue.Imputed(
                                                                    chargingPeriods[i].StartTimestamp,
                                                                    imputedValue
                                                                );

                    }
                }


                var cdrCostDetails = new CDRCostDetails();

                foreach (var chargingPeriod in chargingPeriods)
                {

                    var periodStartMeteringValue  = chargingPeriod.StartMeteringValue;
                    var periodStopMeteringValue   = chargingPeriod.StopMeteringValue;

                    if (periodStartMeteringValue is not null &&
                        periodStopMeteringValue  is not null)
                    {

                        chargingPeriod.Energy        = periodStopMeteringValue.Value.WattHours - periodStartMeteringValue.Value.WattHours;
                        chargingPeriod.PowerAverage  = Watt.ParseW(chargingPeriod.Energy.Value / Convert.ToDecimal(chargingPeriod.Duration.TotalHours));

                        cdrCostDetails.      TotalEnergy   = cdrCostDetails.TotalEnergy + chargingPeriod.Energy;

                    }


                    cdrCostDetails.      TotalTime     = cdrCostDetails.TotalTime   + chargingPeriod.Duration;

                    foreach (var priceComponent in chargingPeriod.PriceComponents.Values)
                    {

                        if      (priceComponent.Type == TariffDimension.ENERGY && priceComponent.Price > 0)// && chargingPeriod.Energy.Value > 0) !!!AS we want to be abl to invoice 0 kWhs!
                        {

                            chargingPeriod.Dimensions.Add(
                                CDRDimension.ENERGY(chargingPeriod.Energy)
                            );

                            chargingPeriod.EnergyPrice     = priceComponent.Price;
                            chargingPeriod.EnergyStepSize  = priceComponent.StepSize;

                            cdrCostDetails.BillEnergy(priceComponent.StepSize, chargingPeriod.Energy, chargingPeriod.EnergyPrice);

                        }

                        else if (priceComponent.Type == TariffDimension.TIME   && priceComponent.Price > 0)// && chargingPeriod.Duration.TotalSeconds > 0)!!!AS we want to be abl to invoice 0 sec!
                        {

                            chargingPeriod.Dimensions.Add(
                                CDRDimension.TIME(
                                    chargingPeriod.StopTimestamp!.Value - chargingPeriod.StartTimestamp
                                )
                            );

                            chargingPeriod.TimePrice     = priceComponent.Price;
                            chargingPeriod.TimeStepSize  = priceComponent.StepSize;

                            cdrCostDetails.BillTime(priceComponent.StepSize, chargingPeriod.Duration, chargingPeriod.TimePrice);

                        }

                        else if (priceComponent.Type == TariffDimension.FLAT   && priceComponent.Price > 0)
                        {

                            chargingPeriod.FlatPrice          = priceComponent.Price;

                            cdrCostDetails.BillFlat(chargingPeriod.FlatPrice);

                        }

                        //else if (priceComponent.Type == TariffDimension.PARKING_TIME)
                        //{
                        //    chargingPeriods[i].Dimensions.Add(CDRDimension.ENERGY(0));
                        //}

                    }

                }

                //else if (periodStartMeteringValue is not null &&
                //         periodStopMeteringValue  is     null &&
                //         ExtrapolateEnergy)
                //{
                //    //ToDo: Extrapolate an ongoing charging period!
                //}



                foreach (var energySums in cdrCostDetails.BilledEnergyElements)
                {

                    energySums.Value.BilledEnergy  = WattHour.ParseWh(Math.Ceiling(energySums.Value.Energy.Value / energySums.Value.StepSize) * energySums.Value.StepSize);
                    energySums.Value.EnergyCost    = energySums.Value.BilledEnergy.kWh * energySums.Value.Price;

                    cdrCostDetails.BilledEnergy         += energySums.Value.BilledEnergy;
                    cdrCostDetails.TotalEnergyCost      += energySums.Value.EnergyCost;
                    cdrCostDetails.TotalCost            += energySums.Value.EnergyCost;

                }

                foreach (var timeSums in cdrCostDetails.BilledTimeElements)
                {

                    timeSums.Value.BilledTime      = TimeSpan.FromSeconds(Math.Ceiling(timeSums.Value.Time.TotalSeconds / timeSums.Value.StepSize) * timeSums.Value.StepSize);
                    timeSums.Value.TimeCost        = Convert.ToDecimal(timeSums.Value.BilledTime.TotalHours) * timeSums.Value.Price;

                    cdrCostDetails.BilledTime            = cdrCostDetails.BilledTime.Add(timeSums.Value.BilledTime);
                    cdrCostDetails.TotalTimeCost        += timeSums.Value.TimeCost;
                    cdrCostDetails.TotalCost            += timeSums.Value.TimeCost;

                }

                foreach (var flatSums in cdrCostDetails.BilledFlatElements)
                {

                    cdrCostDetails.TotalFlatCost        += flatSums.Value.Price;
                    cdrCostDetails.TotalCost            += flatSums.Value.Price;

                }

                MeteringValue? startMeteringValue  = null;
                MeteringValue? stopMeteringValue   = null;
                TimeSpan?      totalParkingTime    = new TimeSpan?(TimeSpan.Zero);

                foreach (var cp in chargingPeriods)
                {

                    if (cp.StartMeteringValue is not null && startMeteringValue is null)
                        startMeteringValue = cp.StartMeteringValue;

                    if (cp.StopMeteringValue  is not null)
                        stopMeteringValue  = cp.StopMeteringValue;

                    if (cp.Energy == WattHour.Zero)
                        totalParkingTime   = totalParkingTime.Value.Add(cp.Duration);

                }


                return new CDR(

                           CDR.CountryCode,
                           CDR.PartyId,
                           CDR.Id,
                           CDR.Start,
                           CDR.End,
                           CDR.CDRToken,
                           CDR.AuthMethod,
                           CDR.Location,
                           tariff.Currency,
                           chargingPeriods,
                           new Price(
                               (Double) cdrCostDetails.TotalCost
                           ),
                           cdrCostDetails.TotalEnergy,
                           cdrCostDetails.TotalTime,

                           CDR.SessionId,
                           CDR.AuthorizationReference,
                           CDR.EnergyMeterId,
                           CDR.EnergyMeter,
                           CDR.TransparencySoftware,
                           [ tariff ],

                       //    cdrCostDetails, // Internal only!

                           CDR.SignedData,
                           new Price(0),
                           new Price(0),
                           new Price(0),
                           totalParkingTime,
                           new Price(0),
                           new Price(0),
                           CDR.Remark,
                           CDR.InvoiceReferenceId,
                           CDR.Credit,
                           CDR.CreditReferenceId,
                           CDR.HomeChargingCompensation,

                           CDR.Created,
                           CDR.LastUpdated

                       );

            }
            catch (Exception e)
            {
                throw new Exception("Could not split the given charge detail record into charging periods!", e);
            }

        }

        #endregion

    }


    /// <summary>
    /// The charge detail record describes the charging session and its costs,
    /// how these costs are composed, etc.
    /// </summary>
    public class CDR : IHasId<CDR_Id>,
                       IEquatable<CDR>,
                       IComparable<CDR>,
                       IComparable
    {

        #region Data

        private readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this charging location.
        /// </summary>
        internal CommonAPI?                          CommonAPI                   { get; set; }

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charge detail record.
        /// </summary>
        [Mandatory]
        public   CountryCode                         CountryCode                 { get; }

        /// <summary>
        /// The identification of the charge point operator that 'owns' this charge detail record
        /// (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public   Party_Id                            PartyId                     { get; }

        /// <summary>
        /// The identification of the charge detail record within the charge point operator's platform
        /// (and suboperator platforms).
        /// CiString(39)
        /// </summary>
        [Mandatory]
        public   CDR_Id                              Id                          { get; }

        /// <summary>
        /// The start timestamp of the charging session, or in-case of a reservation
        /// (before the start of a session) the start of the reservation.
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                      Start                       { get; }

        /// <summary>
        /// The timestamp when the session was completed/finished.
        /// Charging might have finished before the session ends, for example:
        /// EV is full, but parking cost also has to be paid.
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                      End                         { get; }

        /// <summary>
        /// The optional unique identification of the charging session.
        /// Is only allowed to be omitted when the CPO has not implemented the sessions module or this
        /// charge detail record is the result of a reservation that never became a charging session.
        /// </summary>
        [Optional]
        public   Session_Id?                         SessionId                   { get; }

        /// <summary>
        /// The token used to start this charging session, includes all relevant information
        /// to identify the unique token.
        /// </summary>
        [Mandatory]
        public   CDRToken                            CDRToken                    { get; }

        /// <summary>
        /// The authentication method used.
        /// </summary>
        [Mandatory]
        public   AuthMethod                          AuthMethod                  { get; }

        /// <summary>
        /// The optional reference to the authorization given by the eMSP.
        /// When the eMSP provided an authorization_reference in either:
        /// real-time authorization or StartSession, this field SHALL contain the same value.
        /// When different authorization_reference values have been given by the eMSP that
        /// are relevant to this session, the last given value SHALL be used here.
        /// </summary>
        [Optional]
        public   AuthorizationReference?             AuthorizationReference      { get; }

        /// <summary>
        /// The location where the charging session took place, including only the relevant
        /// EVSE and connector.
        /// </summary>
        [Mandatory]
        public   CDRLocation                         Location                    { get; }

        /// <summary>
        /// The optional identification of the energy meter.
        /// </summary>
        [Optional]
        public   EnergyMeter_Id?                     EnergyMeterId               { get; }

        /// <summary>
        /// The optional energy meter.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
        public   EnergyMeter<EVSE>?                  EnergyMeter                 { get; }

        /// <summary>
        /// The enumeration of valid transparency software which can be used to validate
        /// the singed charging session and metering data.
        /// </summary>
        [Optional, VendorExtension(VE.GraphDefined, VE.Eichrecht)]
        public   IEnumerable<TransparencySoftware>   TransparencySoftware        { get; }

        /// <summary>
        /// The ISO 4217 code of the currency used for this charge detail record.
        /// </summary>
        [Mandatory]
        public   Currency                            Currency                    { get; }

        /// <summary>
        /// The enumeration of relevant charging tariffs.
        /// </summary>
        [Optional]
        public   IEnumerable<Tariff>                 Tariffs                     { get; }

        /// <summary>
        /// The enumeration of charging periods that make up this charging session.
        /// A session consist of 1 or more periods with, each period has a
        /// different relevant charging tariff.
        /// </summary>
        [Mandatory]
        public   IEnumerable<ChargingPeriod>         ChargingPeriods             { get; }

        /// <summary>
        /// The optional signed metering data that belongs to this charging session.
        /// </summary>
        [Optional]
        public   SignedData?                         SignedData                  { get; }

        /// <summary>
        /// The total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Mandatory]
        public   Price                               TotalCosts                  { get; }

        /// <summary>
        /// The optional total sum of all the costs of this transaction in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalFixedCosts             { get; }

        /// <summary>
        /// The total energy charged (in kWh).
        /// </summary>
        [Mandatory]
        public   WattHour                            TotalEnergy                 { get; }

        /// <summary>
        /// The optional total sum of all the cost of all the energy used, in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalEnergyCost             { get; }

        /// <summary>
        /// The total duration of the charging session, including the duration of charging and not charging.
        /// </summary>
        [Mandatory]
        public   TimeSpan                            TotalTime                   { get; }

        /// <summary>
        /// The optional total sum of all the cost related to duration of charging during this transaction,
        /// in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalTimeCost               { get; }

        /// <summary>
        /// The optional total duration of the charging session where the EV was not charging
        /// (no energy was transferred between EVSE and EV).
        /// </summary>
        [Optional]
        public   TimeSpan?                           TotalParkingTime            { get; }

        /// <summary>
        /// The optional total duration of the charging session where the EV was not charging
        /// (no energy was transferred between EVSE and EV).
        /// </summary>
        [Optional]
        public   TimeSpan                            TotalChargingTime
            => TotalTime - (TotalParkingTime ?? TimeSpan.Zero);

        /// <summary>
        /// The optional total sum of all the cost related to parking of this transaction,
        /// including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalParkingCost            { get; }

        /// <summary>
        /// The optional total sum of all the cost related to a reservation of a charge point,
        /// including fixed price components, in the specified currency.
        /// </summary>
        [Optional]
        public   Price?                              TotalReservationCost        { get; }

        /// <summary>
        /// The optional remark can be used to provide addition human
        /// readable information to the charge detail record, for example a
        /// reason why a transaction was stopped.
        /// </summary>
        [Optional]
        public   String?                             Remark                      { get; }

        /// <summary>
        /// The optional invoice reference identification can be used to reference an invoice,
        /// that will later be send for this charge detail record.
        /// Might even group multiple charge detail records that will be on the same invoice.
        /// </summary>
        [Optional]
        public   InvoiceReference_Id?                InvoiceReferenceId          { get; }

        /// <summary>
        /// The optional indication, that this charge detail record is a "credit CDR".
        /// When set to true the field credit_reference_id needs to be set as well.
        /// </summary>
        [Optional]
        public   Boolean?                            Credit                      { get; }

        /// <summary>
        /// The optional credit reference identification is required to be set for a "credit CDR".
        /// This SHALL contain the identification of the charge detail record for which this is a "credit CDR".
        /// </summary>
        [Optional]
        public   CreditReference_Id?                 CreditReferenceId           { get; }

        /// <summary>
        /// When set to true, this charge detail record is for a charging session using the home charger
        /// of the EV driver for which the energy cost needs to be financial compensated to the EV driver.
        /// </summary>
        [Optional]
        public   Boolean?                            HomeChargingCompensation    { get; }


        /// <summary>
        /// The timestamp when this charge detail record was created.
        /// </summary>
        [Mandatory, VendorExtension(VE.GraphDefined, VE.Pagination)]
        public   DateTimeOffset                      Created                     { get; }

        /// <summary>
        /// The timestamp when this charge detail record was last updated (or created).
        /// </summary>
        [Mandatory]
        public   DateTimeOffset                      LastUpdated                 { get; }

        /// <summary>
        /// The SHA256 hash of the JSON representation of this charge detail record.
        /// </summary>
        public   String                              ETag                        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record.
        /// </summary>
        /// <param name="CountryCode">An ISO-3166 alpha-2 country code of the charge point operator that 'owns' this charge detail record.</param>
        /// <param name="PartyId">An identification of the charge point operator that 'owns' this charge detail record (following the ISO-15118 standard).</param>
        /// <param name="Id">An identification of the charge detail record within the charge point operator's platform (and suboperator platforms).</param>
        /// <param name="Start">The start timestamp of the charging session, or in-case of a reservation (before the start of a session) the start of the reservation.</param>
        /// <param name="End">The timestamp when the session was completed/finished. Charging might have finished before the session ends, for example: EV is full, but parking cost also has to be paid.</param>
        /// <param name="CDRToken">The token used to start this charging session, includes all relevant information to identify the unique token.</param>
        /// <param name="AuthMethod">The authentication method used.</param>
        /// <param name="Location">The location where the charging session took place, including only the relevant EVSE and connector.</param>
        /// <param name="Currency">The ISO 4217 code of the currency used for this charge detail record.</param>
        /// <param name="ChargingPeriods">The enumeration of charging periods that make up this charging session. A session consist of 1 or more periods with, each period has a different relevant charging tariff.</param>
        /// <param name="TotalCosts">The total sum of all the costs of this transaction in the specified currency.</param>
        /// <param name="TotalEnergy">The total energy charged (in kWh).</param>
        /// <param name="TotalTime">The total duration of the charging session, including the duration of charging and not charging.</param>
        /// 
        /// <param name="SessionId">The optional unique identification of the charging session. Is only allowed to be omitted when the CPO has not implemented the sessions module or this charge detail record is the result of a reservation that never became a charging session.</param>
        /// <param name="AuthorizationReference">The optional reference to the authorization given by the eMSP.</param>
        /// <param name="EnergyMeterId">The optional identification of the energy meter.</param>
        /// <param name="EnergyMeter">The optional energy meter.</param>
        /// <param name="TransparencySoftware">The enumeration of valid transparency software which can be used to validate the singed charging session and metering data.</param>
        /// <param name="Tariffs">The enumeration of relevant charging tariffs.</param>
        /// <param name="SignedData">The optional signed metering data that belongs to this charging session.</param>
        /// <param name="TotalFixedCosts">The optional total sum of all the costs of this transaction in the specified currency.</param>
        /// <param name="TotalEnergyCost">The optional total sum of all the cost of all the energy used, in the specified currency.</param>
        /// <param name="TotalTimeCost">The optional total sum of all the cost related to duration of charging during this transaction, in the specified currency.</param>
        /// <param name="TotalParkingTime">The optional total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV).</param>
        /// <param name="TotalParkingCost">The optional total sum of all the cost related to parking of this transaction, including fixed price components, in the specified currency.</param>
        /// <param name="TotalReservationCost">The optional total sum of all the cost related to a reservation of a charge point, including fixed price components, in the specified currency.</param>
        /// <param name="Remark">The optional remark can be used to provide addition human readable information to the charge detail record, for example a reason why a transaction was stopped.</param>
        /// <param name="InvoiceReferenceId">The optional invoice reference identification can be used to reference an invoice, that will later be send for this charge detail record. Might even group multiple charge detail records that will be on the same invoice.</param>
        /// <param name="Credit">The optional indication, that this charge detail record is a "credit CDR". When set to true the field credit_reference_id needs to be set as well.</param>
        /// <param name="CreditReferenceId">The optional credit reference identification is required to be set for a "credit CDR". This SHALL contain the identification of the charge detail record for which this is a "credit CDR".</param>
        /// <param name="HomeChargingCompensation">When set to true, this charge detail record is for a charging session using the home charger of the EV driver for which the energy cost needs to be financial compensated to the EV driver.</param>
        /// 
        /// <param name="Created">An optional timestamp when this charge detail record was created.</param>
        /// <param name="LastUpdated">An optional timestamp when this charge detail record was last updated (or created).</param>
        /// 
        /// <param name="CustomCDRSerializer">A delegate to serialize custom charge detail record JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomCDRLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomSignedDataSerializer">A delegate to serialize custom signed data JSON objects.</param>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public CDR(CountryCode                                             CountryCode,
                   Party_Id                                                PartyId,
                   CDR_Id                                                  Id,
                   DateTimeOffset                                          Start,
                   DateTimeOffset                                          End,
                   CDRToken                                                CDRToken,
                   AuthMethod                                              AuthMethod,
                   CDRLocation                                             Location,
                   Currency                                                Currency,
                   IEnumerable<ChargingPeriod>                             ChargingPeriods,
                   Price                                                   TotalCosts,
                   WattHour                                                TotalEnergy,
                   TimeSpan                                                TotalTime,

                   Session_Id?                                             SessionId                              = null,
                   AuthorizationReference?                                 AuthorizationReference                 = null,
                   EnergyMeter_Id?                                         EnergyMeterId                          = null,
                   EnergyMeter<EVSE>?                                      EnergyMeter                            = null,
                   IEnumerable<TransparencySoftware>?                      TransparencySoftware                  = null,
                   IEnumerable<Tariff>?                                    Tariffs                                = null,
                   SignedData?                                             SignedData                             = null,
                   Price?                                                  TotalFixedCosts                        = null,
                   Price?                                                  TotalEnergyCost                        = null,
                   Price?                                                  TotalTimeCost                          = null,
                   TimeSpan?                                               TotalParkingTime                       = null,
                   Price?                                                  TotalParkingCost                       = null,
                   Price?                                                  TotalReservationCost                   = null,
                   String?                                                 Remark                                 = null,
                   InvoiceReference_Id?                                    InvoiceReferenceId                     = null,
                   Boolean?                                                Credit                                 = null,
                   CreditReference_Id?                                     CreditReferenceId                      = null,
                   Boolean?                                                HomeChargingCompensation               = null,

                   DateTimeOffset?                                         Created                                = null,
                   DateTimeOffset?                                         LastUpdated                            = null,
                   CustomJObjectSerializerDelegate<CDR>?                   CustomCDRSerializer                    = null,
                   CustomJObjectSerializerDelegate<CDRToken>?              CustomCDRTokenSerializer               = null,
                   CustomJObjectSerializerDelegate<CDRLocation>?           CustomCDRLocationSerializer            = null,
                   CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?     CustomEVSEEnergyMeterSerializer        = null,
                   CustomJObjectSerializerDelegate<TransparencySoftware>?  CustomTransparencySoftwareSerializer   = null,
                   CustomJObjectSerializerDelegate<Tariff>?                CustomTariffSerializer                 = null,
                   CustomJObjectSerializerDelegate<DisplayText>?           CustomDisplayTextSerializer            = null,
                   CustomJObjectSerializerDelegate<Price>?                 CustomPriceSerializer                  = null,
                   CustomJObjectSerializerDelegate<TariffElement>?         CustomTariffElementSerializer          = null,
                   CustomJObjectSerializerDelegate<PriceComponent>?        CustomPriceComponentSerializer         = null,
                   CustomJObjectSerializerDelegate<TariffRestrictions>?    CustomTariffRestrictionsSerializer     = null,
                   CustomJObjectSerializerDelegate<EnergyMix>?             CustomEnergyMixSerializer              = null,
                   CustomJObjectSerializerDelegate<EnergySource>?          CustomEnergySourceSerializer           = null,
                   CustomJObjectSerializerDelegate<EnvironmentalImpact>?   CustomEnvironmentalImpactSerializer    = null,
                   CustomJObjectSerializerDelegate<ChargingPeriod>?        CustomChargingPeriodSerializer         = null,
                   CustomJObjectSerializerDelegate<CDRDimension>?          CustomCDRDimensionSerializer           = null,
                   CustomJObjectSerializerDelegate<SignedData>?            CustomSignedDataSerializer             = null,
                   CustomJObjectSerializerDelegate<SignedValue>?           CustomSignedValueSerializer            = null)

        {

            if (!ChargingPeriods.Any())
                throw new ArgumentNullException(nameof(ChargingPeriods),  "The given enumeration of charging periods must not be null or empty!");

            this.CountryCode               = CountryCode;
            this.PartyId                   = PartyId;
            this.Id                        = Id;
            this.Start                     = Start;
            this.End                       = End;
            this.CDRToken                  = CDRToken;
            this.AuthMethod                = AuthMethod;
            this.Location                  = Location;
            this.Currency                  = Currency;
            this.ChargingPeriods           = ChargingPeriods       ?? [];
            this.TotalCosts                = TotalCosts;
            this.TotalEnergy               = TotalEnergy;
            this.TotalTime                 = TotalTime;

            this.SessionId                 = SessionId;
            this.AuthorizationReference    = AuthorizationReference;
            this.EnergyMeterId             = EnergyMeterId;
            this.EnergyMeter               = EnergyMeter;
            this.TransparencySoftware     = TransparencySoftware ?? [];
            this.Tariffs                   = Tariffs               ?? [];
            this.SignedData                = SignedData;
            this.TotalFixedCosts           = TotalFixedCosts;
            this.TotalEnergyCost           = TotalEnergyCost;
            this.TotalTimeCost             = TotalTimeCost;
            this.TotalParkingTime          = TotalParkingTime;
            this.TotalParkingCost          = TotalParkingCost;
            this.TotalReservationCost      = TotalReservationCost;
            this.Remark                    = Remark;
            this.InvoiceReferenceId        = InvoiceReferenceId;
            this.Credit                    = Credit;
            this.CreditReferenceId         = CreditReferenceId;
            this.HomeChargingCompensation  = HomeChargingCompensation;

            this.Created                   = Created               ?? LastUpdated ?? Timestamp.Now;
            this.LastUpdated               = LastUpdated           ?? Created     ?? Timestamp.Now;

            this.ETag                      = SHA256.HashData(
                                                 ToJSON(
                                                     CustomCDRSerializer,
                                                     CustomCDRTokenSerializer,
                                                     CustomCDRLocationSerializer,
                                                     CustomEVSEEnergyMeterSerializer,
                                                     CustomTransparencySoftwareSerializer,
                                                     CustomTariffSerializer,
                                                     CustomDisplayTextSerializer,
                                                     CustomPriceSerializer,
                                                     CustomTariffElementSerializer,
                                                     CustomPriceComponentSerializer,
                                                     CustomTariffRestrictionsSerializer,
                                                     CustomEnergyMixSerializer,
                                                     CustomEnergySourceSerializer,
                                                     CustomEnvironmentalImpactSerializer,
                                                     CustomChargingPeriodSerializer,
                                                     CustomCDRDimensionSerializer,
                                                     CustomSignedDataSerializer,
                                                     CustomSignedValueSerializer
                                                 ).ToUTF8Bytes()
                                             ).ToBase64();

            unchecked
            {

                hashCode = this.CountryCode.              GetHashCode()        * 137 ^
                           this.PartyId.                  GetHashCode()        * 131 ^
                           this.Id.                       GetHashCode()        * 127 ^
                           this.Start.                    GetHashCode()        * 113 ^
                           this.End.                      GetHashCode()        * 109 ^
                           this.CDRToken.                 GetHashCode()        * 107 ^
                           this.AuthMethod.               GetHashCode()        * 103 ^
                           this.Location.                 GetHashCode()        * 101 ^
                           this.Currency.                 GetHashCode()        *  97 ^
                           this.ChargingPeriods.          CalcHashCode()       *  89 ^
                           this.Tariffs.                  CalcHashCode()       *  83 ^
                           this.TotalCosts.               GetHashCode()        *  79 ^
                           this.TotalEnergy.              GetHashCode()        *  73 ^
                           this.TotalTime.                GetHashCode()        *  71 ^
                           this.Created.                  GetHashCode()        *  67 ^
                           this.LastUpdated.              GetHashCode()        *  61 ^

                          (this.SessionId?.               GetHashCode()  ?? 0) *  59 ^
                          (this.AuthorizationReference?.  GetHashCode()  ?? 0) *  53 ^
                          (this.EnergyMeterId?.           GetHashCode()  ?? 0) *  47 ^
                          (this.EnergyMeter?.             GetHashCode()  ?? 0) *  43 ^
                           this.TransparencySoftware.    CalcHashCode()       *  41 ^
                          (this.SignedData?.              GetHashCode()  ?? 0) *  37 ^
                          (this.TotalFixedCosts?.         GetHashCode()  ?? 0) *  31 ^
                          (this.TotalEnergyCost?.         GetHashCode()  ?? 0) *  29 ^
                          (this.TotalTimeCost?.           GetHashCode()  ?? 0) *  23 ^
                          (this.TotalParkingTime?.        GetHashCode()  ?? 0) *  19 ^
                          (this.TotalParkingCost?.        GetHashCode()  ?? 0) *  17 ^
                          (this.TotalReservationCost?.    GetHashCode()  ?? 0) *  13 ^
                          (this.Remark?.                  GetHashCode()  ?? 0) *  11 ^
                          (this.InvoiceReferenceId?.      GetHashCode()  ?? 0) *   7 ^
                          (this.Credit?.                  GetHashCode()  ?? 0) *   5 ^
                          (this.CreditReferenceId?.       GetHashCode()  ?? 0) *   3 ^
                           this.HomeChargingCompensation?.GetHashCode()  ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CountryCodeURL = null, PartyIdURL = null, CDRIdURL = null, CustomCDRParser = null)

        /// <summary>
        /// Parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="CDRIdURL">An optional charge detail record identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom charge detail record JSON objects.</param>
        public static CDR Parse(JObject                            JSON,
                                CountryCode?                       CountryCodeURL    = null,
                                Party_Id?                          PartyIdURL        = null,
                                CDR_Id?                            CDRIdURL          = null,
                                CustomJObjectParserDelegate<CDR>?  CustomCDRParser   = null)
        {

            if (TryParse(JSON,
                         out var CDR,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         CDRIdURL,
                         CustomCDRParser))
            {
                return CDR;
            }

            throw new ArgumentException("The given JSON representation of a charge detail record is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CDR, out ErrorResponse, CDRIdURL = null, CustomCDRParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed CDR.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out CDR?     CDR,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out CDR,
                        out ErrorResponse,
                        null,
                        null,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a CDR.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CDR">The parsed CDR.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CountryCodeURL">An optional country code, e.g. from the HTTP URL.</param>
        /// <param name="PartyIdURL">An optional party identification, e.g. from the HTTP URL.</param>
        /// <param name="CDRIdURL">An optional CDR identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomCDRParser">A delegate to parse custom CDR JSON objects.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out CDR?      CDR,
                                       [NotNullWhen(false)] out String?   ErrorResponse,
                                       CountryCode?                       CountryCodeURL    = null,
                                       Party_Id?                          PartyIdURL        = null,
                                       CDR_Id?                            CDRIdURL          = null,
                                       CustomJObjectParserDelegate<CDR>?  CustomCDRParser   = null)
        {

            try
            {

                CDR = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse CountryCode                 [optional]

                if (JSON.ParseOptional("country_code",
                                       "country code",
                                       CountryCode.TryParse,
                                       out CountryCode? CountryCodeBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CountryCodeURL.HasValue && !CountryCodeBody.HasValue)
                {
                    ErrorResponse = "The country code is missing!";
                    return false;
                }

                if (CountryCodeURL.HasValue && CountryCodeBody.HasValue && CountryCodeURL.Value != CountryCodeBody.Value)
                {
                    ErrorResponse = "The optional country code given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse PartyIdURL                  [optional]

                if (JSON.ParseOptional("party_id",
                                       "party identification",
                                       Party_Id.TryParse,
                                       out Party_Id? PartyIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!PartyIdURL.HasValue && !PartyIdBody.HasValue)
                {
                    ErrorResponse = "The party identification is missing!";
                    return false;
                }

                if (PartyIdURL.HasValue && PartyIdBody.HasValue && PartyIdURL.Value != PartyIdBody.Value)
                {
                    ErrorResponse = "The optional party identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Id                          [optional]

                if (JSON.ParseOptional("id",
                                       "CDR identification",
                                       CDR_Id.TryParse,
                                       out CDR_Id? CDRIdBody,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!CDRIdURL.HasValue && !CDRIdBody.HasValue)
                {
                    ErrorResponse = "The CDR identification is missing!";
                    return false;
                }

                if (CDRIdURL.HasValue && CDRIdBody.HasValue && CDRIdURL.Value != CDRIdBody.Value)
                {
                    ErrorResponse = "The optional CDR identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion

                #region Parse Start                       [mandatory]

                if (!JSON.ParseMandatory("start_date_time",
                                         "start timestamp",
                                         out DateTimeOffset Start,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse End                         [mandatory]

                if (!JSON.ParseMandatory("end_date_time",
                                         "end timestamp",
                                         out DateTimeOffset End,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SessionId                   [optional]

                if (JSON.ParseOptional("session_id",
                                       "session identification",
                                       Session_Id.TryParse,
                                       out Session_Id? SessionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse CDRToken                    [mandatory]

                if (!JSON.ParseMandatoryJSON("cdr_token",
                                             "charge detail record token",
                                             OCPIv2_2_1.CDRToken.TryParse,
                                             out CDRToken CDRToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthMethod                  [mandatory]

                if (!JSON.ParseMandatory("auth_method",
                                         "authentication method",
                                         AuthMethod.TryParse,
                                         out AuthMethod authMethod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference      [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv2_2_1.AuthorizationReference.TryParse,
                                       out AuthorizationReference? AuthorizationReference,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Location                    [mandatory]

                if (!JSON.ParseMandatoryJSON("cdr_location",
                                             "charge detail record location",
                                             CDRLocation.TryParse,
                                             out CDRLocation? Location,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EnergyMeterId               [optional]

                if (JSON.ParseOptional("meter_id",
                                       "meter identification",
                                       EnergyMeter_Id.TryParse,
                                       out EnergyMeter_Id? EnergyMeterId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnergyMeter                 [optional]

                if (JSON.ParseOptionalJSON("energy_meter",
                                           "energy meter",
                                           EnergyMeter<EVSE>.TryParse,
                                           out EnergyMeter<EVSE>? EnergyMeter,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TransparencySoftware       [optional]

                if (JSON.ParseOptionalJSON("transparency_software",
                                           "transparency software",
                                           OCPI.TransparencySoftware.TryParse,
                                           out IEnumerable<TransparencySoftware> TransparencySoftware,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Currency                    [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         Currency.TryParse,
                                         out Currency currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Tariffs                     [optional]

                if (JSON.ParseOptionalJSON("tariffs",
                                           "tariffs",
                                           Tariff.TryParse,
                                           out IEnumerable<Tariff> Tariffs,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ChargingPeriods             [mandatory]

                if (!JSON.ParseMandatoryJSON("charging_periods",
                                             "charging periods",
                                             ChargingPeriod.TryParse,
                                             out IEnumerable<ChargingPeriod> ChargingPeriods,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SignedData                  [optional]

                if (JSON.ParseOptionalJSON("signed_data",
                                           "signed data",
                                           OCPIv2_2_1.SignedData.TryParse,
                                           out SignedData? SignedData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalCosts                  [mandatory]

                if (!JSON.ParseMandatoryJSON("total_cost",
                                             "total costs",
                                             Price.TryParse,
                                             out Price TotalCosts,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalFixedCosts             [optional]

                if (JSON.ParseOptionalJSON("total_fixed_cost",
                                           "total fixed costs",
                                           Price.TryParse,
                                           out Price? TotalFixedCosts,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalEnergy                 [mandatory]

                if (!JSON.ParseMandatory("total_energy",
                                         "total energy",
                                         WattHour.TryParseKWh,
                                         out WattHour TotalEnergy,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalEnergyCost             [optional]

                if (JSON.ParseOptionalJSON("total_energy_cost",
                                           "total energy cost",
                                           Price.TryParse,
                                           out Price? TotalEnergyCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalTime                   [mandatory]

                if (!JSON.ParseMandatory("total_time",
                                         "total time",
                                         out Double totalTimeHours,
                                         out ErrorResponse))
                {
                    return false;
                }

                var TotalTimeHours = TimeSpan.FromHours(totalTimeHours);

                #endregion

                #region Parse TotalTimeCost               [optional]

                if (JSON.ParseOptionalJSON("total_time_cost",
                                           "total time cost",
                                           Price.TryParse,
                                           out Price? TotalTimeCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalParkingTime            [optional]

                if (JSON.ParseOptional("total_parking_time",
                                       "total parking time",
                                       out Double? totalParkingTimeHours,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var TotalParkingTimeHours = totalParkingTimeHours.HasValue
                                                ? new TimeSpan?(TimeSpan.FromHours(totalParkingTimeHours.Value))
                                                : null;

                #endregion

                #region Parse TotalParkingCost            [optional]

                if (JSON.ParseOptionalJSON("total_parking_cost",
                                           "total parking cost",
                                           Price.TryParse,
                                           out Price? TotalParkingCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalReservationCost        [optional]

                if (JSON.ParseOptionalJSON("total_reservation_cost",
                                           "total reservation cost",
                                           Price.TryParse,
                                           out Price? TotalReservationCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Remark                      [optional]

                var Remark = JSON.GetString("remark");

                #endregion

                #region Parse InvoiceReferenceId          [optional]

                if (JSON.ParseOptional("invoice_reference_id",
                                       "invoice reference identification",
                                       InvoiceReference_Id.TryParse,
                                       out InvoiceReference_Id? InvoiceReferenceId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Credit                      [optional]

                if (JSON.ParseOptional("credit",
                                       "credit charge detail record",
                                       out Boolean? Credit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse CreditReferenceId           [optional]

                if (JSON.ParseOptional("credit_reference_id",
                                       "credit reference identification",
                                       CreditReference_Id.TryParse,
                                       out CreditReference_Id? CreditReferenceId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse HomeChargingCompensation    [optional]

                if (JSON.ParseOptional("home_charging_compensation",
                                       "home charging compensation",
                                       out Boolean? HomeChargingCompensation,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Parse Created                     [optional, NonStandard]

                if (JSON.ParseOptional("created",
                                       "created",
                                       out DateTimeOffset? Created,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastUpdated                 [mandatory]

                if (!JSON.ParseMandatory("last_updated",
                                         "last updated",
                                         out DateTimeOffset LastUpdated,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CDR = new CDR(

                          CountryCodeBody ?? CountryCodeURL!.Value,
                          PartyIdBody     ?? PartyIdURL!.    Value,
                          CDRIdBody       ?? CDRIdURL!.      Value,
                          Start,
                          End,
                          CDRToken,
                          authMethod,
                          Location,
                          currency,
                          ChargingPeriods,
                          TotalCosts,
                          TotalEnergy,
                          TotalTimeHours,

                          SessionId,
                          AuthorizationReference,
                          EnergyMeterId,
                          EnergyMeter,
                          TransparencySoftware,
                          Tariffs,
                          SignedData,
                          TotalFixedCosts,
                          TotalEnergyCost,
                          TotalTimeCost,
                          TotalParkingTimeHours,
                          TotalParkingCost,
                          TotalReservationCost,
                          Remark,
                          InvoiceReferenceId,
                          Credit,
                          CreditReferenceId,
                          HomeChargingCompensation,

                          Created,
                          LastUpdated

                      );


                if (CustomCDRParser is not null)
                    CDR = CustomCDRParser(JSON,
                                          CDR);

                return true;

            }
            catch (Exception e)
            {
                CDR            = default;
                ErrorResponse  = "The given JSON representation of a CDR is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCDRSerializer = null, CustomCDRTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCDRSerializer">A delegate to serialize custom charge detail record JSON objects.</param>
        /// <param name="CustomCDRTokenSerializer">A delegate to serialize custom charge detail record token JSON objects.</param>
        /// <param name="CustomCDRLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomEVSEEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom tariff element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom tariff restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging period JSON objects.</param>
        /// <param name="CustomCDRDimensionSerializer">A delegate to serialize custom charge detail record dimension JSON objects.</param>
        /// <param name="CustomSignedDataSerializer">A delegate to serialize custom signed data JSON objects.</param>
        /// <param name="CustomSignedValueSerializer">A delegate to serialize custom signed value JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CDR>?                   CustomCDRSerializer                    = null,
                              CustomJObjectSerializerDelegate<CDRToken>?              CustomCDRTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<CDRLocation>?           CustomCDRLocationSerializer            = null,
                              CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?     CustomEVSEEnergyMeterSerializer        = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?  CustomTransparencySoftwareSerializer   = null,
                              CustomJObjectSerializerDelegate<Tariff>?                CustomTariffSerializer                 = null,
                              CustomJObjectSerializerDelegate<DisplayText>?           CustomDisplayTextSerializer            = null,
                              CustomJObjectSerializerDelegate<Price>?                 CustomPriceSerializer                  = null,
                              CustomJObjectSerializerDelegate<TariffElement>?         CustomTariffElementSerializer          = null,
                              CustomJObjectSerializerDelegate<PriceComponent>?        CustomPriceComponentSerializer         = null,
                              CustomJObjectSerializerDelegate<TariffRestrictions>?    CustomTariffRestrictionsSerializer     = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?             CustomEnergyMixSerializer              = null,
                              CustomJObjectSerializerDelegate<EnergySource>?          CustomEnergySourceSerializer           = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?   CustomEnvironmentalImpactSerializer    = null,
                              CustomJObjectSerializerDelegate<ChargingPeriod>?        CustomChargingPeriodSerializer         = null,
                              CustomJObjectSerializerDelegate<CDRDimension>?          CustomCDRDimensionSerializer           = null,
                              CustomJObjectSerializerDelegate<SignedData>?            CustomSignedDataSerializer             = null,
                              CustomJObjectSerializerDelegate<SignedValue>?           CustomSignedValueSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("country_code",                 CountryCode.                   ToString()),
                                 new JProperty("party_id",                     PartyId.                       ToString()),
                                 new JProperty("id",                           Id.                            ToString()),
                                 new JProperty("start_date_time",              Start.                         ToISO8601()),
                                 new JProperty("end_date_time",                End.                           ToISO8601()),

                           SessionId.HasValue
                               ? new JProperty("session_id",                   SessionId.               Value.ToString())
                               : null,

                                 new JProperty("cdr_token",                    CDRToken.                      ToJSON(CustomCDRTokenSerializer)),
                                 new JProperty("auth_method",                  AuthMethod.                    ToString()),

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",      AuthorizationReference.  Value.ToString())
                               : null,

                                 new JProperty("cdr_location",                 Location.                      ToJSON(CustomCDRLocationSerializer)),

                           EnergyMeterId.HasValue
                               ? new JProperty("meter_id",                     EnergyMeterId.                 Value.ToString())
                               : null,

                           EnergyMeter is not null
                               ? new JProperty("energy_meter",                 EnergyMeter.                   ToJSON(CustomEVSEEnergyMeterSerializer))
                               : null,

                           TransparencySoftware.Any()
                               ? new JProperty("transparency_software",       new JArray(TransparencySoftware.Select(transparencySoftware => transparencySoftware.ToJSON(CustomTransparencySoftwareSerializer))))
                               : null,

                                 new JProperty("currency",                     Currency.                      ISOCode),

                           Tariffs.Any()
                               ? new JProperty("tariffs",                      new JArray(Tariffs.                Select(tariff               => tariff.              ToJSON(true,
                                                                                                                                                                             true,
                                                                                                                                                                             CustomTariffSerializer,
                                                                                                                                                                             CustomDisplayTextSerializer,
                                                                                                                                                                             CustomPriceSerializer,
                                                                                                                                                                             CustomTariffElementSerializer,
                                                                                                                                                                             CustomPriceComponentSerializer,
                                                                                                                                                                             CustomTariffRestrictionsSerializer,
                                                                                                                                                                             CustomEnergyMixSerializer,
                                                                                                                                                                             CustomEnergySourceSerializer,
                                                                                                                                                                             CustomEnvironmentalImpactSerializer))))
                               : null,

                           ChargingPeriods.Any()
                               ? new JProperty("charging_periods",             new JArray(ChargingPeriods.        Select(chargingPeriod       => chargingPeriod.      ToJSON(CustomChargingPeriodSerializer,
                                                                                                                                                                             CustomCDRDimensionSerializer))))
                               : null,

                           SignedData is not null
                               ? new JProperty("signed_data",                  SignedData.                    ToJSON(CustomSignedDataSerializer,
                                                                                                                     CustomSignedValueSerializer))
                               : null,

                                 new JProperty("total_cost",                   TotalCosts.                    ToJSON(CustomPriceSerializer)),

                           TotalFixedCosts.HasValue
                               ? new JProperty("total_fixed_cost",             TotalFixedCosts.         Value.ToJSON(CustomPriceSerializer))
                               : null,

                                 new JProperty("total_energy",                 TotalEnergy.kWh),

                           TotalEnergyCost.HasValue
                               ? new JProperty("total_energy_cost",            TotalEnergyCost.         Value.ToJSON(CustomPriceSerializer))
                               : null,

                                 new JProperty("total_time",                   TotalTime.                     TotalHours),

                           TotalTimeCost.HasValue
                               ? new JProperty("total_time_cost",              TotalTimeCost.           Value.ToJSON(CustomPriceSerializer))
                               : null,

                           TotalParkingTime.HasValue
                               ? new JProperty("total_parking_time",           TotalParkingTime.        Value.TotalHours)
                               : null,

                           TotalParkingCost.HasValue
                               ? new JProperty("total_parking_cost",           TotalParkingCost.        Value.ToJSON(CustomPriceSerializer))
                               : null,

                           TotalReservationCost.HasValue
                               ? new JProperty("total_reservation_cost",       TotalReservationCost.    Value.ToJSON(CustomPriceSerializer))
                               : null,

                           Remark.IsNotNullOrEmpty()
                               ? new JProperty("remark",                       Remark)
                               : null,

                           InvoiceReferenceId.HasValue
                               ? new JProperty("invoice_reference_id",         InvoiceReferenceId.      Value.ToString())
                               : null,

                           Credit.HasValue
                               ? new JProperty("credit",                       Credit.                  Value)
                               : null,

                           CreditReferenceId.HasValue
                               ? new JProperty("credit_reference_id",          CreditReferenceId.       Value.ToString())
                               : null,

                           HomeChargingCompensation.HasValue
                               ? new JProperty("home_charging_compensation",   HomeChargingCompensation.Value)
                               : null,

                                 new JProperty("created",                      Created.                       ToISO8601()),
                                 new JProperty("last_updated",                 LastUpdated.                   ToISO8601())

                       );

            return CustomCDRSerializer is not null
                       ? CustomCDRSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public CDR Clone()

            => new (
                   CountryCode.Clone(),
                   PartyId.    Clone(),
                   Id.         Clone(),
                   Start,
                   End,
                   CDRToken.   Clone(),
                   AuthMethod. Clone(),
                   Location.   Clone(),
                   Currency.   Clone(),
                   ChargingPeriods.      Select(chargingPeriod       => chargingPeriod.      Clone()),
                   TotalCosts. Clone(),
                   TotalEnergy,
                   TotalTime,

                   SessionId?.             Clone(),
                   AuthorizationReference?.Clone(),
                   EnergyMeterId?.         Clone(),
                   EnergyMeter?.           Clone(),
                   TransparencySoftware.Select(transparencySoftware => transparencySoftware.Clone()),
                   Tariffs.              Select(tariff               => tariff.              Clone()),
                   SignedData?.            Clone(),
                   TotalFixedCosts?.       Clone(),
                   TotalEnergyCost?.       Clone(),
                   TotalTimeCost?.         Clone(),
                   TotalParkingTime,
                   TotalParkingCost?.      Clone(),
                   TotalReservationCost?.  Clone(),
                   Remark?.                CloneNullableString(),
                   InvoiceReferenceId?.    Clone(),
                   Credit,
                   CreditReferenceId?.     Clone(),
                   HomeChargingCompensation,

                   Created,
                   LastUpdated
               );

        #endregion


        #region (private) TryPrivatePatch(JSON, Patch)

        private PatchResult<JObject> TryPrivatePatch(JObject           JSON,
                                                     JObject           Patch,
                                                     EventTracking_Id  EventTrackingId)
        {

            foreach (var property in Patch)
            {

                if      (property.Key == "country_code")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'country code' of a charge detail record is not allowed!");

                else if (property.Key == "party_id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'party identification' of a charge detail record is not allowed!");

                else if (property.Key == "id")
                    return PatchResult<JObject>.Failed(EventTrackingId, JSON,
                                                       "Patching the 'identification' of a charge detail record is not allowed!");

                else if (property.Value is null)
                    JSON.Remove(property.Key);

                else if (property.Value is JObject subObject)
                {

                    if (JSON.ContainsKey(property.Key))
                    {

                        if (JSON[property.Key] is JObject oldSubObject)
                        {

                            //ToDo: Perhaps use a more generic JSON patch here!
                            // PatchObject.Apply(ToJSON(), EVSEPatch),
                            var patchResult = TryPrivatePatch(oldSubObject, subObject, EventTrackingId);

                            if (patchResult.IsSuccess)
                                JSON[property.Key] = patchResult.PatchedData;

                        }

                        else
                            JSON[property.Key] = subObject;

                    }

                    else
                        JSON.Add(property.Key, subObject);

                }

                //else if (property.Value is JArray subArray)
                //{
                //}

                else
                    JSON[property.Key] = property.Value;

            }

            return PatchResult<JObject>.Success(EventTrackingId, JSON);

        }

        #endregion

        #region TryPatch(CDRPatch, AllowDowngrades = false)

        /// <summary>
        /// Try to patch the JSON representation of this charge detail record.
        /// </summary>
        /// <param name="CDRPatch">The JSON merge patch.</param>
        /// <param name="AllowDowngrades">Allow to set the 'lastUpdated' timestamp to an earlier value.</param>
        public PatchResult<CDR> TryPatch(JObject           CDRPatch,
                                         Boolean           AllowDowngrades   = false,
                                         EventTracking_Id? EventTrackingId   = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (CDRPatch is null)
                return PatchResult<CDR>.Failed(EventTrackingId, this,
                                                  "The given charge detail record patch must not be null!");

            lock (patchLock)
            {

                if (CDRPatch["last_updated"] is null)
                    CDRPatch["last_updated"] = Timestamp.Now.ToISO8601();

                else if (AllowDowngrades == false &&
                        CDRPatch["last_updated"].Type == JTokenType.Date &&
                       (CDRPatch["last_updated"].Value<DateTime>().ToISO8601().CompareTo(LastUpdated.ToISO8601()) < 1))
                {
                    return PatchResult<CDR>.Failed(EventTrackingId, this,
                                                      "The 'lastUpdated' timestamp of the charge detail record patch must be newer then the timestamp of the existing charge detail record!");
                }


                var patchResult = TryPrivatePatch(ToJSON(), CDRPatch, EventTrackingId);


                if (patchResult.IsFailed)
                    return PatchResult<CDR>.Failed(EventTrackingId, this,
                                                      patchResult.ErrorResponse);

                if (TryParse(patchResult.PatchedData,
                             out var patchedCDR,
                             out var errorResponse))
                {

                    return PatchResult<CDR>.Success(EventTrackingId, patchedCDR,
                                                       errorResponse);

                }

                else
                    return PatchResult<CDR>.Failed(EventTrackingId, this,
                                                      "Invalid JSON merge patch of a charge detail record: " + errorResponse);

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDR? CDR1,
                                           CDR? CDR2)
        {

            if (Object.ReferenceEquals(CDR1, CDR2))
                return true;

            if (CDR1 is null || CDR2 is null)
                return false;

            return CDR1.Equals(CDR2);

        }

        #endregion

        #region Operator != (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 == CDR2);

        #endregion

        #region Operator <  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR? CDR1,
                                          CDR? CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) < 0;

        #endregion

        #region Operator <= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 > CDR2);

        #endregion

        #region Operator >  (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR? CDR1,
                                          CDR? CDR2)

            => CDR1 is null
                   ? throw new ArgumentNullException(nameof(CDR1), "The given charge detail record must not be null!")
                   : CDR1.CompareTo(CDR2) > 0;

        #endregion

        #region Operator >= (CDR1, CDR2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDR1">A charge detail record.</param>
        /// <param name="CDR2">Another charge detail record.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDR? CDR1,
                                           CDR? CDR2)

            => !(CDR1 < CDR2);

        #endregion

        #endregion

        #region IComparable<CDR> Members

        #region CompareTo(Object)

        /// <summary>s
        /// Compares two charge detail records.
        /// </summary>
        /// <param name="Object">A charge detail record to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CDR cdr
                   ? CompareTo(cdr)
                   : throw new ArgumentException("The given object is not a charge detail record!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDR)

        /// <summary>s
        /// Compares two charge detail records.
        /// </summary>
        /// <param name="Object">A charge detail record to compare with.</param>
        public Int32 CompareTo(CDR? CDR)
        {

            if (CDR is null)
                throw new ArgumentNullException(nameof(CDR), "The given charge detail record must not be null!");

            var c = CountryCode.CompareTo(CDR.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(CDR.PartyId);

            if (c == 0)
                c = Id.         CompareTo(CDR.Id);

            if (c == 0)
                c = Start.      CompareTo(CDR.Start);

            if (c == 0)
                c = End.        CompareTo(CDR.End);

            if (c == 0)
                c = CDRToken.   CompareTo(CDR.CDRToken);

            if (c == 0)
                c = AuthMethod. CompareTo(CDR.AuthMethod);

            if (c == 0)
                c = Currency.   CompareTo(CDR.Currency);

            if (c == 0)
                c = TotalCosts. CompareTo(CDR.TotalCosts);

            if (c == 0)
                c = TotalEnergy.CompareTo(CDR.TotalEnergy);

            if (c == 0)
                c = TotalTime.  CompareTo(CDR.TotalTime);

            if (c == 0)
                c = Created.    CompareTo(CDR.Created);

            if (c == 0)
                c = LastUpdated.CompareTo(CDR.LastUpdated);

            // Location,
            // ChargingPeriods,
            // 
            // SessionId
            // AuthorizationReference
            // MeterId
            // EnergyMeter
            // TransparencySoftware
            // Tariffs
            // SignedData
            // TotalFixedCosts
            // TotalEnergyCost
            // TotalTimeCost
            // TotalParkingTime
            // TotalParkingCost
            // TotalReservationCost
            // Remark
            // InvoiceReferenceId
            // Credit
            // CreditReferenceId
            // HomeChargingCompensation

            return c;

        }



        #endregion

        #endregion

        #region IEquatable<CDR> Members

        #region Equals(Object)

        /// <summary>s
        /// Compares two charge detail records for equality.
        /// </summary>
        /// <param name="Object">A charge detail record to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CDR cdr &&
                   Equals(cdr);

        #endregion

        #region Equals(CDR)

        /// <summary>s
        /// Compares two charge detail records for equality.
        /// </summary>
        /// <param name="CDR">A charge detail record to compare with.</param>
        public Boolean Equals(CDR? CDR)

            => CDR is not null &&

               CountryCode.            Equals(CDR.CountryCode)             &&
               PartyId.                Equals(CDR.PartyId)                 &&
               Id.                     Equals(CDR.Id)                      &&
               Start.                  Equals(CDR.Start)                   &&
               End.                    Equals(CDR.End)                     &&
               CDRToken.               Equals(CDR.CDRToken)                &&
               AuthMethod.             Equals(CDR.AuthMethod)              &&
               Location.               Equals(CDR.Location)                &&
               Currency.               Equals(CDR.Currency)                &&
               TotalCosts.             Equals(CDR.TotalCosts)              &&
               TotalEnergy.            Equals(CDR.TotalEnergy)             &&
               TotalTime.              Equals(CDR.TotalTime)               &&
               Created.    ToISO8601().Equals(CDR.Created.    ToISO8601()) &&
               LastUpdated.ToISO8601().Equals(CDR.LastUpdated.ToISO8601()) &&

            ((!SessionId.               HasValue    && !CDR.SessionId.               HasValue)  ||
              (SessionId.               HasValue    &&  CDR.SessionId.               HasValue    && SessionId.               Value.Equals(CDR.SessionId.               Value))) &&

            ((!AuthorizationReference.  HasValue    && !CDR.AuthorizationReference.  HasValue)  ||
              (AuthorizationReference.  HasValue    &&  CDR.AuthorizationReference.  HasValue    && AuthorizationReference.  Value.Equals(CDR.AuthorizationReference.  Value))) &&

            ((!EnergyMeterId.                 HasValue    && !CDR.EnergyMeterId.                 HasValue)  ||
              (EnergyMeterId.                 HasValue    &&  CDR.EnergyMeterId.                 HasValue    && EnergyMeterId.                 Value.Equals(CDR.EnergyMeterId.                 Value))) &&

            ((!TotalFixedCosts.         HasValue    && !CDR.TotalFixedCosts.         HasValue)  ||
              (TotalFixedCosts.         HasValue    &&  CDR.TotalFixedCosts.         HasValue    && TotalFixedCosts.         Value.Equals(CDR.TotalFixedCosts.         Value))) &&

            ((!TotalEnergyCost.         HasValue    && !CDR.TotalEnergyCost.         HasValue)  ||
              (TotalEnergyCost.         HasValue    &&  CDR.TotalEnergyCost.         HasValue    && TotalEnergyCost.         Value.Equals(CDR.TotalEnergyCost.         Value))) &&

            ((!TotalTimeCost.           HasValue    && !CDR.TotalTimeCost.           HasValue)  ||
              (TotalTimeCost.           HasValue    &&  CDR.TotalTimeCost.           HasValue    && TotalTimeCost.           Value.Equals(CDR.TotalTimeCost.           Value))) &&

            ((!TotalParkingTime.        HasValue    && !CDR.TotalParkingTime.        HasValue)  ||
              (TotalParkingTime.        HasValue    &&  CDR.TotalParkingTime.        HasValue    && TotalParkingTime.        Value.Equals(CDR.TotalParkingTime.        Value))) &&

            ((!TotalParkingCost.        HasValue    && !CDR.TotalParkingCost.        HasValue)  ||
              (TotalParkingCost.        HasValue    &&  CDR.TotalParkingCost.        HasValue    && TotalParkingCost.        Value.Equals(CDR.TotalParkingCost.        Value))) &&

            ((!TotalReservationCost.    HasValue    && !CDR.TotalReservationCost.    HasValue)  ||
              (TotalReservationCost.    HasValue    &&  CDR.TotalReservationCost.    HasValue    && TotalReservationCost.    Value.Equals(CDR.TotalReservationCost.    Value))) &&

            ((!InvoiceReferenceId.      HasValue    && !CDR.InvoiceReferenceId.      HasValue)  ||
              (InvoiceReferenceId.      HasValue    &&  CDR.InvoiceReferenceId.      HasValue    && InvoiceReferenceId.      Value.Equals(CDR.InvoiceReferenceId.      Value))) &&

            ((!Credit.                  HasValue    && !CDR.Credit.                  HasValue)  ||
              (Credit.                  HasValue    &&  CDR.Credit.                  HasValue    && Credit.                  Value.Equals(CDR.Credit.                  Value))) &&

            ((!CreditReferenceId.       HasValue    && !CDR.CreditReferenceId.       HasValue)  ||
              (CreditReferenceId.       HasValue    &&  CDR.CreditReferenceId.       HasValue    && CreditReferenceId.       Value.Equals(CDR.CreditReferenceId.       Value))) &&

            ((!HomeChargingCompensation.HasValue    && !CDR.HomeChargingCompensation.HasValue)    ||
              (HomeChargingCompensation.HasValue    &&  CDR.HomeChargingCompensation.HasValue    && HomeChargingCompensation.Value.Equals(CDR.HomeChargingCompensation.Value))) &&


             ((Remark                   is     null &&  CDR.Remark                   is     null) ||
              (Remark                   is not null &&  CDR.Remark                   is not null && Remark.                        Equals(CDR.Remark)))                         &&

             ((EnergyMeter              is     null &&  CDR.EnergyMeter              is     null) ||
              (EnergyMeter              is not null &&  CDR.EnergyMeter              is not null && EnergyMeter.                   Equals(CDR.EnergyMeter)))                    &&

             ((TransparencySoftware    is     null &&  CDR.TransparencySoftware    is     null) ||
              (TransparencySoftware    is not null &&  CDR.TransparencySoftware    is not null && TransparencySoftware.         Equals(CDR.TransparencySoftware)))          &&

             ((SignedData               is     null &&  CDR.SignedData               is     null) ||
              (SignedData               is not null &&  CDR.SignedData               is not null && SignedData.                    Equals(CDR.SignedData)))                     &&

               ChargingPeriods.Count().Equals(CDR.ChargingPeriods.Count()) &&
               ChargingPeriods.Count().Equals(CDR.ChargingPeriods.Count()) &&

               Tariffs.All(data => CDR.Tariffs.Contains(data)) &&
               Tariffs.All(data => CDR.Tariffs.Contains(data));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,                       " (",
                   CountryCode,              "-",
                   PartyId,                  ") ",
                   Start.      ToISO8601(),  ", ",
                   End.        ToISO8601(),  ", ",
                   CDRToken.   ToString(),   ", ",
                   AuthMethod. ToString(),   ", ",
                   Location.   ToString(),   ", ",

                   TotalCosts. ToString(),   " ",
                   Currency.   Name,         ", ",
                   TotalEnergy.ToString(),   " kWh, ",
                   TotalTime.  ToString(),   " h, ",

                   // TotalFixedCosts
                   // TotalEnergyCost
                   // TotalTimeCost
                   // TotalParkingTime
                   // TotalParkingCost
                   // TotalReservationCost
                   // InvoiceReferenceId
                   // Credit
                   // CreditReferenceId
                   // HomeChargingCompensation

                   TotalParkingTime.HasValue
                       ? TotalParkingTime.ToString() + " h parking, "
                       : "",

                   ChargingPeriods.Count(), " charging period(s), ",

                   Tariffs.Any()
                       ? Tariffs.Count() + " tariff(s), "
                       : "",

                   // SessionId
                   // AuthorizationReference

                   EnergyMeterId.HasValue
                       ? "meter id: " + EnergyMeterId.Value.ToString() + ", "
                       : "",

                   // EnergyMeter
                   // TransparencySoftware
                   // SignedData

                   Remark is not null
                       ? "remark: " + Remark + ", "
                       : "",

                   LastUpdated.ToISO8601()

               );

        #endregion


    }

}
