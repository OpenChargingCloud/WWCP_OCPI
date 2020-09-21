/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A connector is the socket or cable available for the EV to make use of.
    /// </summary>
    public class Connector : IHasId<Connector_Id>,
                             IEquatable<Connector>,
                             IComparable<Connector>,
                             IComparable
    {

        #region Properties

        /// <summary>
        /// Identifier of the connector within the EVSE.
        /// Two connectors may have the same id as long as they do not belong to the same EVSE object.
        /// </summary>
        [Mandatory]
        public Connector_Id            Id                    { get; }

        /// <summary>
        /// The standard of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorTypes          Standard              { get; }

        /// <summary>
        /// The format (socket/cable) of the installed connector.
        /// </summary>
        [Mandatory]
        public ConnectorFormats        Format                { get; }

        /// <summary>
        /// The type of powert at the connector.
        /// </summary>
        [Mandatory]
        public PowerTypes              PowerType             { get; }

        /// <summary>
        /// Voltage of the connector (line to neutral for AC_3_PHASE), in volt [V].
        /// </summary>
        [Mandatory]
        public UInt16                  MaxVoltage            { get; }

        /// <summary>
        /// Maximum amperage of the connector, in ampere [A].
        /// </summary>
        [Mandatory]
        public UInt16                  MaxAmperage           { get; }

        /// <summary>
        /// Maximum electric power that can be delivered by this connector, in Watts (W).
        /// When the maximum electric power is lower than the calculated value from voltage
        /// and amperage, this value should be set.
        /// </summary>
        [Optional]
        public UInt16?                 MaxElectricPower      { get; }

        /// <summary>
        /// Identifiers of the currently valid charging tariffs. Multiple tariffs are possible,
        /// but only one of each Tariff.type can be active at the same time. Tariffs with the
        /// same type are only allowed if they are not active at the same time: start_date_time
        /// and end_date_time period not overlapping.
        /// </summary>
        [Optional]
        public IEnumerable<Tariff_Id>  TariffIds             { get; }

        /// <summary>
        /// Optional URL to the operator's terms and conditions.
        /// </summary>
        [Optional]
        public String                  TermsAndConditions    { get; }

        /// <summary>
        /// Timestamp when this connector was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                LastUpdated           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A connector is the socket or cable available for the EV to make use of.
        /// </summary>
        public Connector(Connector_Id            Id,
                         ConnectorTypes          Standard,
                         ConnectorFormats        Format,
                         PowerTypes              PowerType,
                         UInt16                  MaxVoltage,
                         UInt16                  MaxAmperage,

                         UInt16?                 MaxElectricPower     = null,
                         IEnumerable<Tariff_Id>  TariffIds            = null,
                         String                  TermsAndConditions   = null,

                         DateTime?               LastUpdated          = null)

        {

            this.Id                   = Id;
            this.Standard             = Standard;
            this.Format               = Format;
            this.PowerType            = PowerType;
            this.MaxVoltage           = MaxVoltage;
            this.MaxAmperage          = MaxAmperage;

            this.MaxElectricPower     = MaxElectricPower;
            this.TariffIds            = TariffIds   ?? new Tariff_Id[0];
            this.TermsAndConditions   = TermsAndConditions;

            this.LastUpdated          = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region (static) Parse   (ConnectorJSON, CustomConnectorParser = null)

        /// <summary>
        /// Parse the given JSON representation of a connector.
        /// </summary>
        /// <param name="ConnectorJSON">The JSON to parse.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Connector Parse(JObject                                 ConnectorJSON,
                                      CustomJObjectParserDelegate<Connector>  CustomConnectorParser   = null)
        {

            if (TryParse(ConnectorJSON,
                         out Connector connector,
                         out String    ErrorResponse,
                         CustomConnectorParser))
            {
                return connector;
            }

            throw new ArgumentException("The given JSON representation of a connector is invalid: " + ErrorResponse, nameof(ConnectorJSON));

        }

        #endregion

        #region (static) Parse   (ConnectorText, CustomConnectorParser = null)

        /// <summary>
        /// Parse the given text representation of a connector.
        /// </summary>
        /// <param name="ConnectorText">The text to parse.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Connector Parse(String                                  ConnectorText,
                                      CustomJObjectParserDelegate<Connector>  CustomConnectorParser   = null)
        {

            if (TryParse(ConnectorText,
                         out Connector connector,
                         out String    ErrorResponse,
                         CustomConnectorParser))
            {
                return connector;
            }

            throw new ArgumentException("The given text representation of a connector is invalid: " + ErrorResponse, nameof(ConnectorText));

        }

        #endregion

        #region (static) TryParse(ConnectorJSON, out Connector, out ErrorResponse, CustomConnectorParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a connector.
        /// </summary>
        /// <param name="ConnectorJSON">The JSON to parse.</param>
        /// <param name="Connector">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Boolean TryParse(JObject                                 ConnectorJSON,
                                       out Connector                           Connector,
                                       out String                              ErrorResponse,
                                       CustomJObjectParserDelegate<Connector>  CustomConnectorParser   = null)
        {

            try
            {

                Connector = null;

                if (ConnectorJSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                  [mandatory]

                if (!ConnectorJSON.ParseMandatory("id",
                                                  "connector identification",
                                                  Connector_Id.TryParse,
                                                  out Connector_Id Id,
                                                  out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Standard            [mandatory]

                if (!ConnectorJSON.ParseMandatoryEnum("standard",
                                                      "connector standard",
                                                      out ConnectorTypes Standard,
                                                      out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Format              [mandatory]

                if (!ConnectorJSON.ParseMandatoryEnum("format",
                                                      "connector format",
                                                      out ConnectorFormats Format,
                                                      out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PowerType           [mandatory]

                if (!ConnectorJSON.ParseMandatoryEnum("power_type",
                                                      "power type",
                                                      out PowerTypes PowerType,
                                                      out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxVoltage          [mandatory]

                if (!ConnectorJSON.ParseMandatory("max_voltage",
                                                  "max voltage",
                                                  out UInt16 MaxVoltage,
                                                  out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxAmperage         [mandatory]

                if (!ConnectorJSON.ParseMandatory("max_amperage",
                                                  "max amperage",
                                                  out UInt16 MaxAmperage,
                                                  out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse MaxElectricPower    [optional]

                if (ConnectorJSON.ParseOptional("max_electric_power",
                                                "max voltage",
                                                out UInt16? MaxElectricPower,
                                                out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse MaxElectricPower    [optional]

                if (ConnectorJSON.ParseOptionalHashSet("tariff_ids",
                                                       "tariff identifications",
                                                       Tariff_Id.TryParse,
                                                       out HashSet <Tariff_Id> TariffIds,
                                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var TermsAndConditions = ConnectorJSON.GetString("terms_and_conditions");

                #region Parse LastUpdated         [mandatory]

                if (!ConnectorJSON.ParseMandatory("last_updated",
                                                  "last updated",
                                                  out DateTime LastUpdated,
                                                  out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Connector = new Connector(Id,
                                          Standard,
                                          Format,
                                          PowerType,
                                          MaxVoltage,
                                          MaxAmperage,

                                          MaxElectricPower,
                                          TariffIds,
                                          TermsAndConditions,

                                          LastUpdated);


                if (CustomConnectorParser != null)
                    Connector = CustomConnectorParser(ConnectorJSON,
                                                      Connector);

                return true;

            }
            catch (Exception e)
            {
                Connector = null;
                ErrorResponse = "The given JSON representation of a connector is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(ConnectorText, out Connector, out ErrorResponse, CustomConnectorParser = null)

        /// <summary>
        /// Try to parse the given text representation of a connector.
        /// </summary>
        /// <param name="ConnectorText">The text to parse.</param>
        /// <param name="Connector">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomConnectorParser">A delegate to parse custom connector JSON objects.</param>
        public static Boolean TryParse(String                                  ConnectorText,
                                       out Connector                           Connector,
                                       out String                              ErrorResponse,
                                       CustomJObjectParserDelegate<Connector>  CustomConnectorParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(ConnectorText),
                                out Connector,
                                out ErrorResponse,
                                CustomConnectorParser);

            }
            catch (Exception e)
            {
                Connector      = null;
                ErrorResponse  = "The given text representation of a connector is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomConnectorSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom Connector JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Connector> CustomConnectorSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("id",                          Id.       ToString()),
                           new JProperty("standard",                    Standard. ToString()),
                           new JProperty("format",                      Format.   ToString()),
                           new JProperty("power_type",                  PowerType.ToString()),
                           new JProperty("max_voltage",                 MaxVoltage),
                           new JProperty("max_amperage",                MaxAmperage),

                           MaxElectricPower.HasValue
                               ? new JProperty("max_electric_power",    MaxElectricPower.Value)
                               : null,

                           TariffIds.SafeAny()
                               ? new JProperty("tariff_ids",            new JArray(TariffIds.Select(tarifId => tarifId.ToString())))
                               : null,

                           TermsAndConditions.IsNotNullOrEmpty()
                               ? new JProperty("terms_and_conditions",  TermsAndConditions)
                               : null,

                           new JProperty("last_updated",                LastUpdated.ToIso8601())

                       );

            return CustomConnectorSerializer != null
                       ? CustomConnectorSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region IComparable<Connector> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Connector connector
                   ? CompareTo(connector)
                   : throw new ArgumentException("The given object is not a connector!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Connector)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Connector">An Connector to compare with.</param>
        public Int32 CompareTo(Connector Connector)
        {

            if (Connector is null)
                throw new ArgumentNullException("The given Connector must not be null!");

            return Id.CompareTo(Connector.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Connector> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Connector connector &&
                   Equals(connector);

        #endregion

        #region Equals(Connector)

        /// <summary>
        /// Compares two Connectors for equality.
        /// </summary>
        /// <param name="Connector">An Connector to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Connector Connector)

            => (!(Connector is null)) &&
                   Id.Equals(Connector.Id);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
