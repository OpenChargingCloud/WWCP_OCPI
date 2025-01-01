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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A OCPI party
    /// </summary>
    public readonly struct PartyRole : IEquatable<PartyRole>,
                                       IComparable<PartyRole>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/partyRole");

        #endregion

        #region Properties

        /// <summary>
        /// The module that this platform serves for this party.
        /// </summary>
        [Mandatory]
        public Module_Id       Module    { get; }

        /// <summary>
        /// The side (sender or receiver) this platform serves for the module and party.
        /// </summary>
        [Mandatory]
        public InterfaceRoles  Side      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPI party role.
        /// </summary>
        /// <param name="Module">An unique identification of the module that this platform serves for this party.</param>
        /// <param name="Side">The side (sender or receiver) this platform serves for the module and party.</param>
        public PartyRole(Module_Id       Module,
                         InterfaceRoles  Side)

        {

            this.Module  = Module;
            this.Side    = Side;

            unchecked
            {

                hashCode = Module.GetHashCode() * 3 ^
                           Side.  GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a party role.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPartyRoleParser">A delegate to parse custom party role JSON objects.</param>
        public static PartyRole Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<PartyRole>?  CustomPartyRoleParser   = null)
        {

            if (TryParse(JSON,
                         out var partyRole,
                         out var errorResponse,
                         CustomPartyRoleParser))
            {
                return partyRole;
            }

            throw new ArgumentException("The given JSON representation of a party role is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PartyRole, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a party role.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PartyRole">The parsed party role.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out PartyRole  PartyRole,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out PartyRole,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a party role.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PartyRole">The parsed party role.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPartyRoleParser">A delegate to parse custom party role JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out PartyRole       PartyRole,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<PartyRole>?  CustomPartyRoleParser)
        {

            try
            {

                PartyRole = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Module    [mandatory]

                if (!JSON.ParseMandatory("module",
                                         "OCPI module",
                                         Module_Id.TryParse,
                                         out Module_Id Module,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Side      [mandatory]

                if (!JSON.ParseMandatory("side",
                                         "party interface role",
                                         InterfaceRolesExtensions.TryParse,
                                         out InterfaceRoles Side,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PartyRole = new PartyRole(
                                Module,
                                Side
                            );


                if (CustomPartyRoleParser is not null)
                    PartyRole = CustomPartyRoleParser(JSON,
                                                      PartyRole);

                return true;

            }
            catch (Exception e)
            {
                PartyRole      = default;
                ErrorResponse  = "The given JSON representation of a party role is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPartyRoleSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPartyRoleSerializer">A delegate to serialize custom party role JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PartyRole>?  CustomPartyRoleSerializer   = null)
        {

            var json = JSONObject.Create(

                           new JProperty("module",  Module.ToString()),
                           new JProperty("side",    Side.  AsText())

                       );

            return CustomPartyRoleSerializer is not null
                       ? CustomPartyRoleSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this party role.
        /// </summary>
        public PartyRole Clone()

            => new (
                   Module.Clone(),
                   Side
               );

        #endregion


        #region Operator overloading

        #region Operator == (PartyRole1, PartyRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyRole1">A party role.</param>
        /// <param name="PartyRole2">Another party role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PartyRole PartyRole1,
                                           PartyRole PartyRole2)

            => PartyRole1.Equals(PartyRole2);

        #endregion

        #region Operator != (PartyRole1, PartyRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyRole1">A party role.</param>
        /// <param name="PartyRole2">Another party role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PartyRole PartyRole1,
                                           PartyRole PartyRole2)

            => !PartyRole1.Equals(PartyRole2);

        #endregion

        #region Operator <  (PartyRole1, PartyRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyRole1">A party role.</param>
        /// <param name="PartyRole2">Another party role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PartyRole PartyRole1,
                                          PartyRole PartyRole2)

            => PartyRole1.CompareTo(PartyRole2) < 0;

        #endregion

        #region Operator <= (PartyRole1, PartyRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyRole1">A party role.</param>
        /// <param name="PartyRole2">Another party role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PartyRole PartyRole1,
                                           PartyRole PartyRole2)

            => PartyRole1.CompareTo(PartyRole2) <= 0;

        #endregion

        #region Operator >  (PartyRole1, PartyRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyRole1">A party role.</param>
        /// <param name="PartyRole2">Another party role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PartyRole PartyRole1,
                                          PartyRole PartyRole2)

            => PartyRole1.CompareTo(PartyRole2) > 0;

        #endregion

        #region Operator >= (PartyRole1, PartyRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyRole1">A party role.</param>
        /// <param name="PartyRole2">Another party role.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PartyRole PartyRole1,
                                           PartyRole PartyRole2)

            => PartyRole1.CompareTo(PartyRole2) >= 0;

        #endregion

        #endregion

        #region IComparable<PartyRole> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two party roles.
        /// </summary>
        /// <param name="Object">A party role to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PartyRole partyRole
                   ? CompareTo(partyRole)
                   : throw new ArgumentException("The given object is not a party role!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PartyRole)

        /// <summary>
        /// Compares two party roles.
        /// </summary>
        /// <param name="PartyRole">A party role to compare with.</param>
        public Int32 CompareTo(PartyRole PartyRole)
        {

            var c = Module.CompareTo(PartyRole.Module);

            if (c == 0)
                c = Side.  CompareTo(PartyRole.Side);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PartyRole> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two party roles for equality.
        /// </summary>
        /// <param name="Object">A party role to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PartyRole partyRole &&
                   Equals(partyRole);

        #endregion

        #region Equals(PartyRole)

        /// <summary>
        /// Compares two party roles for equality.
        /// </summary>
        /// <param name="PartyRole">A party role to compare with.</param>
        public Boolean Equals(PartyRole PartyRole)

            => Module.Equals(PartyRole.Module) &&
               Side.  Equals(PartyRole.Side);

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

            => $"{Module} / {Side.AsText()}";

        #endregion

    }

}
