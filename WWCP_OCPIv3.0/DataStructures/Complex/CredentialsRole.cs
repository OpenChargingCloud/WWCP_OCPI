/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using cloud.charging.open.protocols.OCPIv3_0.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Opening and access credentials roles.
    /// </summary>
    public readonly struct CredentialsRole : IEquatable<CredentialsRole>,
                                             IComparable<CredentialsRole>,
                                             IComparable
    {

        #region Properties

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Idv3       PartyId             { get; }

        /// <summary>
        /// The type of the role.
        /// </summary>
        [Mandatory]
        public Role             Role                { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails  BusinessDetails     { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI does not define any behaviour for this.
        /// This is optional in order not to confuse standard OCPI implementations.
        /// </summary>
        [Optional]
        public Boolean?         AllowDowngrades     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new credentials role.
        /// </summary>
        /// <param name="PartyId">CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).</param>
        /// <param name="Role">The type of the role.</param>
        /// <param name="BusinessDetails">Business details of this party.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CredentialsRole(Party_Idv3       PartyId,
                               Role             Role,
                               BusinessDetails  BusinessDetails,
                               Boolean?         AllowDowngrades   = null)
        {

            this.PartyId          = PartyId;
            this.Role             = Role;
            this.BusinessDetails  = BusinessDetails;
            this.AllowDowngrades  = AllowDowngrades;

            unchecked
            {

                hashCode = this.PartyId.         GetHashCode() * 7 ^
                           this.Role.            GetHashCode() * 5 ^
                           this.BusinessDetails. GetHashCode() * 3 ^
                           this.AllowDowngrades?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomCredentialsRoleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a credentials role.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCredentialsRoleParser">A delegate to parse custom credentials role JSON objects.</param>
        public static CredentialsRole Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<CredentialsRole>?  CustomCredentialsRoleParser   = null)
        {

            if (TryParse(JSON,
                         out var credentialsRole,
                         out var errorResponse,
                         CustomCredentialsRoleParser))
            {
                return credentialsRole;
            }

            throw new ArgumentException("The given JSON representation of a credentials role is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CredentialsRole, out ErrorResponse, CustomCredentialsRoleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a credentials role.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CredentialsRole">The parsed credentials role.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out CredentialsRole  CredentialsRole,
                                       [NotNullWhen(false)] out String?          ErrorResponse)

            => TryParse(JSON,
                        out CredentialsRole,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a credentials role.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CredentialsRole">The parsed credentials role.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCredentialsRoleParser">A delegate to parse custom credentials role JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out CredentialsRole       CredentialsRole,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<CredentialsRole>?  CustomCredentialsRoleParser)
        {

            try
            {

                CredentialsRole = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse PartyId             [mandatory]

                if (!JSON.ParseMandatory("party_id",
                                         "party identification",
                                         Party_Idv3.TryParse,
                                         out Party_Idv3 PartyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Role                [mandatory]

                if (!JSON.ParseMandatory("role",
                                         "role",
                                         OCPI.Role.TryParse,
                                         out Role Role,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Business details    [mandatory]

                if (!JSON.ParseMandatoryJSON("business_details",
                                             "business details",
                                             OCPIv3_0.BusinessDetails.TryParse,
                                             out BusinessDetails? BusinessDetails,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (BusinessDetails is null)
                    return false;

                #endregion

                #region Parse AllowDowngrades     [optional]

                if (JSON.ParseOptional("allowDowngrades",
                                       "allow downgrades",
                                       out Boolean? AllowDowngrades,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                CredentialsRole = new CredentialsRole(
                                      PartyId,
                                      Role,
                                      BusinessDetails,
                                      AllowDowngrades
                                  );


                if (CustomCredentialsRoleParser is not null)
                    CredentialsRole = CustomCredentialsRoleParser(JSON,
                                                                  CredentialsRole);

                return true;

            }
            catch (Exception e)
            {
                CredentialsRole  = default;
                ErrorResponse    = "The given JSON representation of a credentials role is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCredentialsRoleSerializer = null, CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials role JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CredentialsRole>?  CustomCredentialsRoleSerializer   = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("party_id",           PartyId.        ToString()),
                                 new JProperty("role",               Role.           ToString()),
                                 new JProperty("business_details",   BusinessDetails.ToJSON(CustomBusinessDetailsSerializer,
                                                                                            CustomImageSerializer)),

                           AllowDowngrades.HasValue
                               ? new JProperty("allow_downgrades",   AllowDowngrades.Value)
                               : null

                       );

            return CustomCredentialsRoleSerializer is not null
                       ? CustomCredentialsRoleSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this credentials role.
        /// </summary>
        public CredentialsRole Clone()

            => new (
                   PartyId.        Clone(),
                   Role.           Clone(),
                   BusinessDetails.Clone(),
                   AllowDowngrades
               );

        #endregion


        public static CredentialsRole From(PartyData PartyData)

            => new (
                   PartyData.Id,
                   PartyData.Role,
                   PartyData.BusinessDetails,
                   PartyData.AllowDowngrades
               );


        #region Operator overloading

        #region Operator == (CredentialsRole1, CredentialsRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CredentialsRole1">A credentials role.</param>
        /// <param name="CredentialsRole2">Another credentials role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CredentialsRole CredentialsRole1,
                                           CredentialsRole CredentialsRole2)

            => CredentialsRole1.Equals(CredentialsRole2);

        #endregion

        #region Operator != (CredentialsRole1, CredentialsRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CredentialsRole1">A credentials role.</param>
        /// <param name="CredentialsRole2">Another credentials role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CredentialsRole CredentialsRole1,
                                           CredentialsRole CredentialsRole2)

            => !CredentialsRole1.Equals(CredentialsRole2);

        #endregion

        #region Operator <  (CredentialsRole1, CredentialsRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CredentialsRole1">A credentials role.</param>
        /// <param name="CredentialsRole2">Another credentials role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CredentialsRole CredentialsRole1,
                                          CredentialsRole CredentialsRole2)

            => CredentialsRole1.CompareTo(CredentialsRole2) < 0;

        #endregion

        #region Operator <= (CredentialsRole1, CredentialsRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CredentialsRole1">A credentials role.</param>
        /// <param name="CredentialsRole2">Another credentials role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CredentialsRole CredentialsRole1,
                                           CredentialsRole CredentialsRole2)

            => CredentialsRole1.CompareTo(CredentialsRole2) <= 0;

        #endregion

        #region Operator >  (CredentialsRole1, CredentialsRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CredentialsRole1">A credentials role.</param>
        /// <param name="CredentialsRole2">Another credentials role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CredentialsRole CredentialsRole1,
                                          CredentialsRole CredentialsRole2)

            => CredentialsRole1.CompareTo(CredentialsRole2) > 0;

        #endregion

        #region Operator >= (CredentialsRole1, CredentialsRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CredentialsRole1">A credentials role.</param>
        /// <param name="CredentialsRole2">Another credentials role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CredentialsRole CredentialsRole1,
                                           CredentialsRole CredentialsRole2)

            => CredentialsRole1.CompareTo(CredentialsRole2) >= 0;

        #endregion

        #endregion

        #region IComparable<CredentialsRole> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two credentials roles.
        /// </summary>
        /// <param name="Object">A credentials role to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CredentialsRole credentialsRole
                   ? CompareTo(credentialsRole)
                   : throw new ArgumentException("The given object is not a credentials role!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CredentialsRole)

        /// <summary>
        /// Compares two credentials roles.
        /// </summary>
        /// <param name="CredentialsRole">A credentials role to compare with.</param>
        public Int32 CompareTo(CredentialsRole CredentialsRole)
        {

            var c = PartyId.              CompareTo(CredentialsRole.PartyId);

            if (c == 0)
                c = Role.                 CompareTo(CredentialsRole.Role);

            if (c == 0 && AllowDowngrades.HasValue && CredentialsRole.AllowDowngrades.HasValue)
                c = AllowDowngrades.Value.CompareTo(CredentialsRole.AllowDowngrades.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CredentialsRole> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two credentials roles for equality.
        /// </summary>
        /// <param name="Object">A credentials role to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CredentialsRole credentialsRole &&
                   Equals(credentialsRole);

        #endregion

        #region Equals(CredentialsRole)

        /// <summary>
        /// Compares two credentials roles for equality.
        /// </summary>
        /// <param name="CredentialsRole">A credentials role to compare with.</param>
        public Boolean Equals(CredentialsRole CredentialsRole)

            => PartyId.        Equals(CredentialsRole.PartyId)         &&
               Role.           Equals(CredentialsRole.Role)            &&
               BusinessDetails.Equals(CredentialsRole.BusinessDetails) &&
               AllowDowngrades.Equals(CredentialsRole.AllowDowngrades);

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

            => $"{BusinessDetails.Name} ({PartyId.ToString(Role)} {Role}) {(AllowDowngrades.HasValue ? AllowDowngrades.Value ? "[Downgrades allowed]" : "[No downgrades]" : "")}";

        #endregion

    }

}
