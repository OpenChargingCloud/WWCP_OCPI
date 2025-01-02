/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A SetChargingProfileOnSession request.
    /// </summary>
    public class SetChargingProfileOnSessionRequest
    {

        #region Properties

        /// <summary>
        /// The charging profile to apply to or remove from the given charging session.
        /// If this field is unset, it indicates that a previously set charging profile is to be removed.
        /// </summary>
        public ChargingProfile?  ChargingProfile    { get; }

        /// <summary>
        /// The charging session identification where to apply or remove the charging profile.
        /// </summary>
        public Session_Id        SessionId          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetChargingProfileOnSession request.
        /// </summary>
        /// <param name="ChargingProfile">The Charging Profile to apply to the group of EVSEs. If this field is unset, it indicates that a previously set Charging Profile is to be removed.</param>
        /// <param name="SessionId">The charging session identification where to apply or remove the charging profile.</param>
        public SetChargingProfileOnSessionRequest(ChargingProfile?  ChargingProfile,
                                                  Session_Id        SessionId)
        {

            this.ChargingProfile  = ChargingProfile;
            this.SessionId        = SessionId;

            unchecked
            {

                hashCode = (this.ChargingProfile?.GetHashCode() ?? 0) * 3 ^
                            this.SessionId.       GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON as a SetChargingProfileOnSession request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSetChargingProfileOnSessionRequestParser">A delegate to parse custom SetChargingProfileOnSession request JSON objects.</param>
        public static SetChargingProfileOnSessionRequest Parse(JObject                                                           JSON,
                                                               CustomJObjectParserDelegate<SetChargingProfileOnSessionRequest>?  CustomSetChargingProfileOnSessionRequestParser   = null)
        {

            if (TryParse(JSON,
                         out var setChargingProfileOnSessionRequest,
                         out var errorResponse,
                         CustomSetChargingProfileOnSessionRequestParser))
            {
                return setChargingProfileOnSessionRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetChargingProfileOnSession request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SetChargingProfileOnSessionRequest, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a SetChargingProfileOnSession request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfileOnSessionRequest">The parsed SetChargingProfileOnSession request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       [NotNullWhen(true)]  out SetChargingProfileOnSessionRequest?  SetChargingProfileOnSessionRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse)

            => TryParse(JSON,
                        out SetChargingProfileOnSessionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a SetChargingProfileOnSession request.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfileOnSessionRequest">The parsed SetChargingProfileOnSession request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileOnSessionRequestParser">A delegate to parse custom SetChargingProfileOnSession request JSON objects.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       [NotNullWhen(true)]  out SetChargingProfileOnSessionRequest?      SetChargingProfileOnSessionRequest,
                                       [NotNullWhen(false)] out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<SetChargingProfileOnSessionRequest>?  CustomSetChargingProfileOnSessionRequestParser)
        {

            try
            {

                SetChargingProfileOnSessionRequest = null;

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

                #region Parse SessionId          [mandatory]

                if (!JSON.ParseMandatory("session_id",
                                         "session identification",
                                         Session_Id.TryParse,
                                         out Session_Id SessionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SetChargingProfileOnSessionRequest = new SetChargingProfileOnSessionRequest(
                                                         ChargingProfile,
                                                         SessionId
                                                     );


                if (CustomSetChargingProfileOnSessionRequestParser is not null)
                    SetChargingProfileOnSessionRequest = CustomSetChargingProfileOnSessionRequestParser(JSON,
                                                                                                    SetChargingProfileOnSessionRequest);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileOnSessionRequest  = null;
                ErrorResponse                       = "The given JSON representation of a SetChargingProfileOnSession request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetChargingProfileOnSessionRequestSerializer = null, CustomChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileOnSessionRequestSerializer">A delegate to serialize custom subscription cancellation requests.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profile JSON objects.</param>
        /// <param name="CustomChargingProfilePeriodSerializer">A delegate to serialize custom charging profile period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfileOnSessionRequest>?  CustomSetChargingProfileOnSessionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?                     CustomChargingProfileSerializer                      = null,
                              CustomJObjectSerializerDelegate<ChargingProfilePeriod>?               CustomChargingProfilePeriodSerializer                = null)
        {

            var json = JSONObject.Create(

                           ChargingProfile is not null
                               ? new JProperty("charging_profile",   ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                            CustomChargingProfilePeriodSerializer))
                               : null,

                                 new JProperty("session_id",         SessionId.ToString())

                       );

            return CustomSetChargingProfileOnSessionRequestSerializer is not null
                       ? CustomSetChargingProfileOnSessionRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this SetChargingProfileOnSession request.
        /// </summary>
        public SetChargingProfileOnSessionRequest Clone()

            => new (
                   ChargingProfile?.Clone(),
                   SessionId.       Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileOnSessionRequest1, SetChargingProfileOnSessionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest1">A SetChargingProfileOnSession request.</param>
        /// <param name="SetChargingProfileOnSessionRequest2">Another SetChargingProfileOnSession request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest1,
                                           SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest2)

            => SetChargingProfileOnSessionRequest1.Equals(SetChargingProfileOnSessionRequest2);

        #endregion

        #region Operator != (SetChargingProfileOnSessionRequest1, SetChargingProfileOnSessionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest1">A SetChargingProfileOnSession request.</param>
        /// <param name="SetChargingProfileOnSessionRequest2">Another SetChargingProfileOnSession request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest1,
                                           SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest2)

            => !SetChargingProfileOnSessionRequest1.Equals(SetChargingProfileOnSessionRequest2);

        #endregion

        #region Operator <  (SetChargingProfileOnSessionRequest1, SetChargingProfileOnSessionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest1">A SetChargingProfileOnSession request.</param>
        /// <param name="SetChargingProfileOnSessionRequest2">Another SetChargingProfileOnSession request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest1,
                                          SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest2)

            => SetChargingProfileOnSessionRequest1.CompareTo(SetChargingProfileOnSessionRequest2) < 0;

        #endregion

        #region Operator <= (SetChargingProfileOnSessionRequest1, SetChargingProfileOnSessionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest1">A SetChargingProfileOnSession request.</param>
        /// <param name="SetChargingProfileOnSessionRequest2">Another SetChargingProfileOnSession request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest1,
                                           SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest2)

            => SetChargingProfileOnSessionRequest1.CompareTo(SetChargingProfileOnSessionRequest2) <= 0;

        #endregion

        #region Operator >  (SetChargingProfileOnSessionRequest1, SetChargingProfileOnSessionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest1">A SetChargingProfileOnSession request.</param>
        /// <param name="SetChargingProfileOnSessionRequest2">Another SetChargingProfileOnSession request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest1,
                                          SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest2)

            => SetChargingProfileOnSessionRequest1.CompareTo(SetChargingProfileOnSessionRequest2) > 0;

        #endregion

        #region Operator >= (SetChargingProfileOnSessionRequest1, SetChargingProfileOnSessionRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest1">A SetChargingProfileOnSession request.</param>
        /// <param name="SetChargingProfileOnSessionRequest2">Another SetChargingProfileOnSession request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest1,
                                           SetChargingProfileOnSessionRequest SetChargingProfileOnSessionRequest2)

            => SetChargingProfileOnSessionRequest1.CompareTo(SetChargingProfileOnSessionRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<SetChargingProfileOnSessionRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two SetChargingProfileOnSession requests.
        /// </summary>
        /// <param name="Object">A SetChargingProfileOnSession request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SetChargingProfileOnSessionRequest setChargingProfileOnSessionRequest
                   ? CompareTo(setChargingProfileOnSessionRequest)
                   : throw new ArgumentException("The given object is not a SetChargingProfileOnSession request!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SetChargingProfileOnSessionRequest)

        /// <summary>
        /// Compares two SetChargingProfileOnSession requests.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest">A SetChargingProfileOnSession request to compare with.</param>
        public Int32 CompareTo(SetChargingProfileOnSessionRequest? SetChargingProfileOnSessionRequest)
        {

            if (SetChargingProfileOnSessionRequest is null)
                throw new ArgumentNullException(nameof(SetChargingProfileOnSessionRequest), "The given SetChargingProfileOnSession request must not be null!");

            var c = SessionId.CompareTo(SetChargingProfileOnSessionRequest.SessionId);
            if (c != 0)
                return c;

            return ChargingProfile?.CompareTo(SetChargingProfileOnSessionRequest.ChargingProfile) ?? (SetChargingProfileOnSessionRequest.ChargingProfile is null ? 0 : -1);

        }

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileOnSessionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetChargingProfileOnSession requests for equality.
        /// </summary>
        /// <param name="Object">A SetChargingProfileOnSession request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetChargingProfileOnSessionRequest setChargingProfileOnSessionRequest &&
                   Equals(setChargingProfileOnSessionRequest);

        #endregion

        #region Equals(SetChargingProfileOnSessionRequest)

        /// <summary>
        /// Compares two SetChargingProfileOnSession requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileOnSessionRequest">A SetChargingProfileOnSession request to compare with.</param>
        public Boolean Equals(SetChargingProfileOnSessionRequest? SetChargingProfileOnSessionRequest)

            => SetChargingProfileOnSessionRequest is not null &&

               SessionId.Equals(SetChargingProfileOnSessionRequest.SessionId) &&

             ((ChargingProfile is     null && SetChargingProfileOnSessionRequest.ChargingProfile is null) ||
              (ChargingProfile is not null && ChargingProfile.Equals(SetChargingProfileOnSessionRequest.ChargingProfile)));

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
                   ? $"Removing the charging profile from session: {SessionId}"
                   : $"Apply charging profile '{ChargingProfile.Id}' to session: {SessionId}";

        #endregion

    }

}
