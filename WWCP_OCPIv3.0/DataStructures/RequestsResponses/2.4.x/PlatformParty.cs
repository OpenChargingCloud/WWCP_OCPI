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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A platform party
    /// </summary>
    public class PlatformParty : IEquatable<PlatformParty>,
                                 IComparable<PlatformParty>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/platformParty");

        #endregion

        #region Properties

        /// <summary>
        /// The unique party identification of this party.
        /// </summary>
        [Mandatory]
        public Party_Idv3                PartyId            { get; }

        /// <summary>
        /// The business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails         BusinessDetails    { get; }

        /// <summary>
        /// The roles that this platform serves for this party.
        /// </summary>
        [Mandatory]
        public IEnumerable<PartyRole>  Roles              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new platform party.
        /// </summary>
        /// <param name="PartyId">An unique party identification of this party.</param>
        /// <param name="BusinessDetails">The business details of this party.</param>
        /// <param name="Roles">An enumeration of roles that this platform serves for this party.</param>
        public PlatformParty(Party_Idv3                PartyId,
                             BusinessDetails         BusinessDetails,
                             IEnumerable<PartyRole>  Roles)

        {

            if (!Roles.Any())
                throw new ArgumentNullException(nameof(Roles), "The enumeration of roles must not be empty!");

            this.PartyId          = PartyId;
            this.BusinessDetails  = BusinessDetails;
            this.Roles            = Roles.Distinct();

            unchecked
            {

                hashCode = PartyId.        GetHashCode() * 5 ^
                           BusinessDetails.GetHashCode() * 3 ^
                           Roles.          CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a platform party.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPlatformPartyParser">A delegate to parse custom platform party JSON objects.</param>
        public static PlatformParty Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<PlatformParty>?  CustomPlatformPartyParser   = null)
        {

            if (TryParse(JSON,
                         out var platformParty,
                         out var errorResponse,
                         CustomPlatformPartyParser))
            {
                return platformParty;
            }

            throw new ArgumentException("The given JSON representation of a platform party is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PlatformParty, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a platform party.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PlatformParty">The parsed platform party.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out PlatformParty?  PlatformParty,
                                       [NotNullWhen(false)] out String?         ErrorResponse)

            => TryParse(JSON,
                        out PlatformParty,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a platform party.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PlatformParty">The parsed platform party.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPlatformPartyParser">A delegate to parse custom platform party JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out PlatformParty?      PlatformParty,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       CustomJObjectParserDelegate<PlatformParty>?  CustomPlatformPartyParser)
        {

            try
            {

                PlatformParty = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PartyId            [mandatory]

                if (!JSON.ParseMandatory("party_id",
                                         "party identification",
                                         Party_Idv3.TryParse,
                                         out Party_Idv3 PartyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse BusinessDetails    [mandatory]

                if (!JSON.ParseMandatoryJSON("business_details",
                                             "business details",
                                             OCPIv3_0.BusinessDetails.TryParse,
                                             out BusinessDetails? BusinessDetails,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Roles              [mandatory]

                if (!JSON.ParseMandatoryHashSet("roles",
                                                "party roles",
                                                PartyRole.TryParse,
                                                out HashSet<PartyRole> Roles,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PlatformParty = new PlatformParty(
                                    PartyId,
                                    BusinessDetails,
                                    Roles
                                );


                if (CustomPlatformPartyParser is not null)
                    PlatformParty = CustomPlatformPartyParser(JSON,
                                                              PlatformParty);

                return true;

            }
            catch (Exception e)
            {
                PlatformParty  = default;
                ErrorResponse  = "The given JSON representation of a platform party is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPlatformPartySerializer = null, CustomBusinessDetailsSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPlatformPartySerializer">A delegate to serialize custom platform party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom images.</param>
        /// <param name="CustomPointOfContactSerializer">A delegate to serialize custom point of contacts.</param>
        /// <param name="CustomPartyRoleSerializer">A delegate to serialize custom party role JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PlatformParty>?    CustomPlatformPartySerializer     = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null,
                              CustomJObjectSerializerDelegate<PointOfContact>?   CustomPointOfContactSerializer    = null,
                              CustomJObjectSerializerDelegate<PartyRole>?        CustomPartyRoleSerializer         = null)
        {

            var json = JSONObject.Create(

                           new JProperty("party_id",           PartyId.        ToString()),

                           new JProperty("business_details",   BusinessDetails.ToJSON(CustomBusinessDetailsSerializer,
                                                                                      CustomImageSerializer,
                                                                                      CustomPointOfContactSerializer)),

                           new JProperty("roles",              new JArray(Roles.Select(role => role.ToJSON(CustomPartyRoleSerializer))))

                       );

            return CustomPlatformPartySerializer is not null
                       ? CustomPlatformPartySerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this platform party.
        /// </summary>
        public PlatformParty Clone()

            => new (
                   PartyId.        Clone(),
                   BusinessDetails.Clone(),
                   Roles.Select(role => role.Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (PlatformParty1, PlatformParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParty1">A platform party.</param>
        /// <param name="PlatformParty2">Another platform party.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PlatformParty PlatformParty1,
                                           PlatformParty PlatformParty2)

            => PlatformParty1.Equals(PlatformParty2);

        #endregion

        #region Operator != (PlatformParty1, PlatformParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParty1">A platform party.</param>
        /// <param name="PlatformParty2">Another platform party.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PlatformParty PlatformParty1,
                                           PlatformParty PlatformParty2)

            => !PlatformParty1.Equals(PlatformParty2);

        #endregion

        #region Operator <  (PlatformParty1, PlatformParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParty1">A platform party.</param>
        /// <param name="PlatformParty2">Another platform party.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PlatformParty PlatformParty1,
                                          PlatformParty PlatformParty2)

            => PlatformParty1.CompareTo(PlatformParty2) < 0;

        #endregion

        #region Operator <= (PlatformParty1, PlatformParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParty1">A platform party.</param>
        /// <param name="PlatformParty2">Another platform party.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PlatformParty PlatformParty1,
                                           PlatformParty PlatformParty2)

            => PlatformParty1.CompareTo(PlatformParty2) <= 0;

        #endregion

        #region Operator >  (PlatformParty1, PlatformParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParty1">A platform party.</param>
        /// <param name="PlatformParty2">Another platform party.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PlatformParty PlatformParty1,
                                          PlatformParty PlatformParty2)

            => PlatformParty1.CompareTo(PlatformParty2) > 0;

        #endregion

        #region Operator >= (PlatformParty1, PlatformParty2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParty1">A platform party.</param>
        /// <param name="PlatformParty2">Another platform party.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PlatformParty PlatformParty1,
                                           PlatformParty PlatformParty2)

            => PlatformParty1.CompareTo(PlatformParty2) >= 0;

        #endregion

        #endregion

        #region IComparable<PlatformParty> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two platform partys.
        /// </summary>
        /// <param name="Object">A platform party to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PlatformParty platformParty
                   ? CompareTo(platformParty)
                   : throw new ArgumentException("The given object is not a platform party!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PlatformParty)

        /// <summary>
        /// Compares two platform partys.
        /// </summary>
        /// <param name="PlatformParty">A platform party to compare with.</param>
        public Int32 CompareTo(PlatformParty? PlatformParty)
        {

            if (PlatformParty is null)
                throw new ArgumentNullException(nameof(PlatformParty), "The given platform party must not be null!");

            var c = PartyId.CompareTo(PlatformParty.PartyId);

            if (c == 0)
                c = BusinessDetails.CompareTo(PlatformParty.BusinessDetails);

            if (c == 0)
                c = Roles.OrderBy(role => role).SequenceEqual(PlatformParty.Roles.OrderBy(role => role)) ? 0 : 1;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PlatformParty> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two platform partys for equality.
        /// </summary>
        /// <param name="Object">A platform party to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PlatformParty platformParty &&
                   Equals(platformParty);

        #endregion

        #region Equals(PlatformParty)

        /// <summary>
        /// Compares two platform partys for equality.
        /// </summary>
        /// <param name="PlatformParty">A platform party to compare with.</param>
        public Boolean Equals(PlatformParty? PlatformParty)

            => PlatformParty is not null &&
               PartyId.        Equals(PlatformParty.PartyId)         &&
               BusinessDetails.Equals(PlatformParty.BusinessDetails) &&
               Roles.OrderBy(role => role).SequenceEqual(PlatformParty.Roles.OrderBy(role => role));

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

            => $"{BusinessDetails.Name} ({PartyId}): {Roles.Select(role => role.ToString()).AggregateWith(", ")}";

        #endregion

    }

}
