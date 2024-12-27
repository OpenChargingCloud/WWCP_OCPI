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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The status of an Electric Vehicle Supply Equipment (EVSE).
    /// </summary>
    public class EVSEStatus : IEquatable<EVSEStatus>,
                              IComparable<EVSEStatus>,
                              IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of locations.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/evse/status");

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of the EVSE within the CPOs platform.
        /// </summary>
        [Mandatory]
        public EVSE_UId    Id           { get; }

        /// <summary>
        /// The current status of the EVSE.
        /// </summary>
        [Mandatory]
        public StatusType  Status       { get; }

        /// <summary>
        /// The optional timestamp of the status update.
        /// </summary>
        [Mandatory]
        public DateTime    Timestamp    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EVSE status.
        /// </summary>
        /// <param name="Id">An unique identification of the EVSE within the CPOs platform.</param>
        /// <param name="Status">A current status of the EVSE.</param>
        /// <param name="Timestamp">An optional timestamp of the status update.</param>
        public EVSEStatus(EVSE_UId    Id,
                          StatusType  Status,
                          DateTime?   Timestamp   = null)
        {

            this.Id         = Id;
            this.Status     = Status;
            this.Timestamp  = Timestamp ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            unchecked
            {

                hashCode = this.Id.       GetHashCode() * 5 ^
                           this.Status.   GetHashCode() * 3 ^
                           this.Timestamp.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomEVSEStatusParser = null)

        /// <summary>
        /// Parse the given JSON representation of an EVSE status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEVSEStatusParser">A delegate to parse custom EVSE status JSON objects.</param>
        public static EVSEStatus Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<EVSEStatus>?  CustomEVSEStatusParser   = null)
        {

            if (TryParse(JSON,
                         out var evseStatus,
                         out var errorResponse,
                         CustomEVSEStatusParser))
            {
                return evseStatus;
            }

            throw new ArgumentException("The given JSON representation of an EVSE status is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVSEStatus, out ErrorResponse, CustomEVSEStatusParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an EVSE status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSEStatus">The parsed EVSE status.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out EVSEStatus?  EVSEStatus,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

            => TryParse(JSON,
                        out EVSEStatus,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an EVSE status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EVSEStatus">The parsed EVSE status.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVSEParser">A delegate to parse custom EVSE status JSON objects.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out EVSEStatus?      EVSEStatus,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<EVSEStatus>?  CustomEVSEStatusParser   = null)
        {

            try
            {

                EVSEStatus = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id           [mandatory]

                if (!JSON.ParseMandatory("uid",
                                         "internal EVSE identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId UId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Status       [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "EVSE status",
                                         StatusType.TryParse,
                                         out StatusType Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Timestamp    [optional]

                if (!JSON.ParseOptional("timestamp",
                                        "timestamp",
                                        out DateTime? Timestamp,
                                        out ErrorResponse))
                {
                    return false;
                }

                #endregion


                EVSEStatus = new EVSEStatus(
                                 UId,
                                 Status,
                                 Timestamp
                             );


                if (CustomEVSEStatusParser is not null)
                    EVSEStatus = CustomEVSEStatusParser(JSON,
                                                        EVSEStatus);

                return true;

            }
            catch (Exception e)
            {
                EVSEStatus     = default;
                ErrorResponse  = "The given JSON representation of an EVSE status is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEVSEStatusSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVSEStatusSerializer">A delegate to serialize custom EVSE status JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVSEStatus>?  CustomEVSEStatusSerializer   = null)
        {

            var json = JSONObject.Create(

                           new JProperty("id",         Id.       ToString()),
                           new JProperty("status",     Status.   ToString()),
                           new JProperty("timestamp",  Timestamp.ToIso8601())

                       );

            return CustomEVSEStatusSerializer is not null
                       ? CustomEVSEStatusSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this EVSE status.
        /// </summary>
        public EVSEStatus Clone()

            => new (
                   Id.    Clone(),
                   Status.Clone(),
                   Timestamp
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSEStatus? EVSEStatus1,
                                           EVSEStatus? EVSEStatus2)
        {

            if (Object.ReferenceEquals(EVSEStatus1, EVSEStatus2))
                return true;

            if (EVSEStatus1 is null || EVSEStatus2 is null)
                return false;

            return EVSEStatus1.Equals(EVSEStatus2);

        }

        #endregion

        #region Operator != (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSEStatus? EVSEStatus1,
                                           EVSEStatus? EVSEStatus2)

            => !(EVSEStatus1 == EVSEStatus2);

        #endregion

        #region Operator <  (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EVSEStatus? EVSEStatus1,
                                          EVSEStatus? EVSEStatus2)

            => EVSEStatus1 is null
                   ? throw new ArgumentNullException(nameof(EVSEStatus1), "The given EVSE must not be null!")
                   : EVSEStatus1.CompareTo(EVSEStatus2) < 0;

        #endregion

        #region Operator <= (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EVSEStatus? EVSEStatus1,
                                           EVSEStatus? EVSEStatus2)

            => !(EVSEStatus1 > EVSEStatus2);

        #endregion

        #region Operator >  (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EVSEStatus? EVSEStatus1,
                                          EVSEStatus? EVSEStatus2)

            => EVSEStatus1 is null
                   ? throw new ArgumentNullException(nameof(EVSEStatus1), "The given EVSE must not be null!")
                   : EVSEStatus1.CompareTo(EVSEStatus2) > 0;

        #endregion

        #region Operator >= (EVSEStatus1, EVSEStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatus1">An EVSE status.</param>
        /// <param name="EVSEStatus2">Another EVSE status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EVSEStatus? EVSEStatus1,
                                           EVSEStatus? EVSEStatus2)

            => !(EVSEStatus1 < EVSEStatus2);

        #endregion

        #endregion

        #region IComparable<EVSEStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EVSE status.
        /// </summary>
        /// <param name="Object">An EVSE status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EVSEStatus evseStatus
                   ? CompareTo(evseStatus)
                   : throw new ArgumentException("The given object is not an EVSE status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEStatus)

        /// <summary>
        /// Compares two EVSE status.
        /// </summary>
        /// <param name="EVSEStatus">An EVSE status to compare with.</param>
        public Int32 CompareTo(EVSEStatus? EVSEStatus)
        {

            if (EVSEStatus is null)
                throw new ArgumentNullException(nameof(EVSEStatus), "The given EVSE status must not be null!");

            var c = Id.                   CompareTo(EVSEStatus.Id);

            if (c == 0)
                c = Status.               CompareTo(EVSEStatus.Status);

            if (c == 0)
                c = Timestamp.ToIso8601().CompareTo(EVSEStatus.Timestamp.ToIso8601());

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EVSEStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EVSE status for equality.
        /// </summary>
        /// <param name="Object">An EVSE status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSEStatus evseStatus &&
                   Equals(evseStatus);

        #endregion

        #region Equals(EVSEStatus)

        /// <summary>
        /// Compares two EVSE status for equality.
        /// </summary>
        /// <param name="EVSE">An EVSE status to compare with.</param>
        public Boolean Equals(EVSEStatus? EVSEStatus)

            => EVSEStatus is not null &&

               Id.                   Equals(EVSEStatus.Id)     &&
               Status.               Equals(EVSEStatus.Status) &&
               Timestamp.ToIso8601().Equals(EVSEStatus.Timestamp.ToIso8601());

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

            => $"{Id} => {Status} @ {Timestamp.ToIso8601()}";

        #endregion


    }

}
