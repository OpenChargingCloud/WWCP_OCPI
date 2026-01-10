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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A SetChargingProfileOnEVSEs request.
    /// </summary>
    public class SetChargingProfileOnEVSEsRequest
    {

        #region Properties

        /// <summary>
        /// The charging profile to apply to or remove from the given group of EVSEs.
        /// If this field is unset, it indicates that a previously set charging profile is to be removed.
        /// </summary>
        public ChargingProfile?       ChargingProfile    { get; }

        /// <summary>
        /// The enumeration of EVSE UIds where to apply or remove the charging profile.
        /// </summary>
        public IEnumerable<EVSE_UId>  EVSEUIds           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetChargingProfileOnEVSEs request.
        /// </summary>
        /// <param name="ChargingProfile">The Charging Profile to apply to the group of EVSEs. If this field is unset, it indicates that a previously set Charging Profile is to be removed.</param>
        /// <param name="EVSEUIds">The enumeration of EVSE UIds where to apply or remove the charging profile.</param>
        public SetChargingProfileOnEVSEsRequest(ChargingProfile?              ChargingProfile,
                                                params IEnumerable<EVSE_UId>  EVSEUIds)
        {

            if (!EVSEUIds.Any())
                throw new ArgumentNullException(nameof(EVSEUIds), "The given enumeration of EVSE UIds must not be empty!");

            this.ChargingProfile  = ChargingProfile;
            this.EVSEUIds         = EVSEUIds;

            unchecked
            {

                hashCode = (this.ChargingProfile?.GetHashCode() ?? 0) * 3 ^
                            this.EVSEUIds.        CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a SetChargingProfileOnEVSEs request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSetChargingProfileOnEVSEsRequestParser">A delegate to parse custom SetChargingProfileOnEVSEs request JSON objects.</param>
        public static SetChargingProfileOnEVSEsRequest Parse(JObject                                                         JSON,
                                                             CustomJObjectParserDelegate<SetChargingProfileOnEVSEsRequest>?  CustomSetChargingProfileOnEVSEsRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var setChargingProfileOnEVSEsRequest,
                         out var errorResponse,
                         CustomSetChargingProfileOnEVSEsRequestParser))
            {
                return setChargingProfileOnEVSEsRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetChargingProfileOnEVSEs request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SetChargingProfileOnEVSEsRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SetChargingProfileOnEVSEs request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest">The parsed SetChargingProfileOnEVSEs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       [NotNullWhen(true)]  out SetChargingProfileOnEVSEsRequest?  SetChargingProfileOnEVSEsRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse)

            => TryParse(JSON,
                        out SetChargingProfileOnEVSEsRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a SetChargingProfileOnEVSEs request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest">The parsed SetChargingProfileOnEVSEs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileOnEVSEsRequestParser">A delegate to parse custom SetChargingProfileOnEVSEs request JSON objects.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       [NotNullWhen(true)]  out SetChargingProfileOnEVSEsRequest?      SetChargingProfileOnEVSEsRequest,
                                       [NotNullWhen(false)] out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<SetChargingProfileOnEVSEsRequest>?  CustomSetChargingProfileOnEVSEsRequestParser)
        {

            try
            {

                SetChargingProfileOnEVSEsRequest = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ChargingProfile    [optional]

                if (JSON.ParseOptionalJSON("charging_profile",
                                           "charging profile",
                                           OCPIv3_0.ChargingProfile.TryParse,
                                           out ChargingProfile? ChargingProfile,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EVSEUIds           [mandatory]

                if (!JSON.ParseMandatoryHashSet("evses",
                                                "EVSE unique identifications",
                                                EVSE_UId.TryParse,
                                                out HashSet<EVSE_UId> EVSEUIds,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SetChargingProfileOnEVSEsRequest = new SetChargingProfileOnEVSEsRequest(
                                                       ChargingProfile,
                                                       EVSEUIds
                                                   );


                if (CustomSetChargingProfileOnEVSEsRequestParser is not null)
                    SetChargingProfileOnEVSEsRequest = CustomSetChargingProfileOnEVSEsRequestParser(JSON,
                                                                                                    SetChargingProfileOnEVSEsRequest);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileOnEVSEsRequest  = null;
                ErrorResponse                     = "The given JSON representation of a SetChargingProfileOnEVSEs request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetChargingProfileOnEVSEsRequestSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileOnEVSEsRequestSerializer">A delegate to serialize custom subscription cancellation requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profile JSON objects.</param>
        /// <param name="CustomChargingProfilePeriodSerializer">A delegate to serialize custom charging profile period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfileOnEVSEsRequest>?  CustomSetChargingProfileOnEVSEsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?                   CustomChargingProfileSerializer                    = null,
                              CustomJObjectSerializerDelegate<ChargingProfilePeriod>?             CustomChargingProfilePeriodSerializer              = null)
        {

            var json = JSONObject.Create(

                           ChargingProfile is not null
                               ? new JProperty("charging_profile",   ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                            CustomChargingProfilePeriodSerializer))
                               : null,

                                 new JProperty("evses",              new JArray(EVSEUIds.Select(evseUId => evseUId.ToString())))

                       );

            return CustomSetChargingProfileOnEVSEsRequestSerializer is not null
                       ? CustomSetChargingProfileOnEVSEsRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this SetChargingProfileOnEVSEs request.
        /// </summary>
        public SetChargingProfileOnEVSEsRequest Clone()

            => new (
                   ChargingProfile?.Clone(),
                   EVSEUIds.Select(evseUId => evseUId.Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileOnEVSEsRequest1, SetChargingProfileOnEVSEsRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest1">A SetChargingProfileOnEVSEs request.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest2">Another SetChargingProfileOnEVSEs request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest1,
                                           SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest2)

            => SetChargingProfileOnEVSEsRequest1.Equals(SetChargingProfileOnEVSEsRequest2);

        #endregion

        #region Operator != (SetChargingProfileOnEVSEsRequest1, SetChargingProfileOnEVSEsRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest1">A SetChargingProfileOnEVSEs request.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest2">Another SetChargingProfileOnEVSEs request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest1,
                                           SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest2)

            => !SetChargingProfileOnEVSEsRequest1.Equals(SetChargingProfileOnEVSEsRequest2);

        #endregion

        #region Operator <  (SetChargingProfileOnEVSEsRequest1, SetChargingProfileOnEVSEsRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest1">A SetChargingProfileOnEVSEs request.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest2">Another SetChargingProfileOnEVSEs request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest1,
                                          SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest2)

            => SetChargingProfileOnEVSEsRequest1.CompareTo(SetChargingProfileOnEVSEsRequest2) < 0;

        #endregion

        #region Operator <= (SetChargingProfileOnEVSEsRequest1, SetChargingProfileOnEVSEsRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest1">A SetChargingProfileOnEVSEs request.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest2">Another SetChargingProfileOnEVSEs request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest1,
                                           SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest2)

            => SetChargingProfileOnEVSEsRequest1.CompareTo(SetChargingProfileOnEVSEsRequest2) <= 0;

        #endregion

        #region Operator >  (SetChargingProfileOnEVSEsRequest1, SetChargingProfileOnEVSEsRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest1">A SetChargingProfileOnEVSEs request.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest2">Another SetChargingProfileOnEVSEs request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest1,
                                          SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest2)

            => SetChargingProfileOnEVSEsRequest1.CompareTo(SetChargingProfileOnEVSEsRequest2) > 0;

        #endregion

        #region Operator >= (SetChargingProfileOnEVSEsRequest1, SetChargingProfileOnEVSEsRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest1">A SetChargingProfileOnEVSEs request.</param>
        /// <param name="SetChargingProfileOnEVSEsRequest2">Another SetChargingProfileOnEVSEs request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest1,
                                           SetChargingProfileOnEVSEsRequest SetChargingProfileOnEVSEsRequest2)

            => SetChargingProfileOnEVSEsRequest1.CompareTo(SetChargingProfileOnEVSEsRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<SetChargingProfileOnEVSEsRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two SetChargingProfileOnEVSEs requests.
        /// </summary>
        /// <param name="Object">A SetChargingProfileOnEVSEs request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SetChargingProfileOnEVSEsRequest setChargingProfileOnEVSEsRequest
                   ? CompareTo(setChargingProfileOnEVSEsRequest)
                   : throw new ArgumentException("The given object is not a SetChargingProfileOnEVSEs request!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SetChargingProfileOnEVSEsRequest)

        /// <summary>
        /// Compares two SetChargingProfileOnEVSEs requests.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest">A SetChargingProfileOnEVSEs request to compare with.</param>
        public Int32 CompareTo(SetChargingProfileOnEVSEsRequest? SetChargingProfileOnEVSEsRequest)
        {

            if (SetChargingProfileOnEVSEsRequest is null)
                throw new ArgumentNullException(nameof(SetChargingProfileOnEVSEsRequest), "The given SetChargingProfileOnEVSEs request must not be null!");

            var c = ChargingProfile?.CompareTo(SetChargingProfileOnEVSEsRequest.ChargingProfile) ?? (SetChargingProfileOnEVSEsRequest.ChargingProfile is null ? 0 : -1);
            if (c != 0)
                return c;

            c = EVSEUIds.Count().CompareTo(SetChargingProfileOnEVSEsRequest.EVSEUIds.Count());
            if (c != 0)
                return c;

            return EVSEUIds.Zip(SetChargingProfileOnEVSEsRequest.EVSEUIds, (evse1, evse2) => evse1.CompareTo(evse2)).FirstOrDefault(result => result != 0);


        }

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileOnEVSEsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetChargingProfileOnEVSEs requests for equality.
        /// </summary>
        /// <param name="Object">A SetChargingProfileOnEVSEs request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetChargingProfileOnEVSEsRequest setChargingProfileOnEVSEsRequest &&
                   Equals(setChargingProfileOnEVSEsRequest);

        #endregion

        #region Equals(SetChargingProfileOnEVSEsRequest)

        /// <summary>
        /// Compares two SetChargingProfileOnEVSEs requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileOnEVSEsRequest">A SetChargingProfileOnEVSEs request to compare with.</param>
        public Boolean Equals(SetChargingProfileOnEVSEsRequest? SetChargingProfileOnEVSEsRequest)

            => SetChargingProfileOnEVSEsRequest is not null &&

             ((ChargingProfile is     null && SetChargingProfileOnEVSEsRequest.ChargingProfile is null) ||
              (ChargingProfile is not null && ChargingProfile.Equals(SetChargingProfileOnEVSEsRequest.ChargingProfile))) &&

               EVSEUIds.SequenceEqual(SetChargingProfileOnEVSEsRequest.EVSEUIds);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ChargingProfile is null
                   ? $"Removing the charging profile from EVSEs: {EVSEUIds.AggregateWith(", ")}"
                   : $"Apply charging profile '{ChargingProfile.Id}' to EVSEs: {EVSEUIds.AggregateWith(", ")}";

        #endregion

    }

}
