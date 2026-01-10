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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Platform parties (for roaming hubs).
    /// </summary>
    public class PlatformParties : IEquatable<PlatformParties>,
                                   IComparable<PlatformParties>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/platformParties");

        #endregion

        #region Properties

        /// <summary>
        /// The optional unique party identification of this hub party.
        /// </summary>
        [Optional]
        public Party_Idv3?                 HubPartyId    { get; }

        /// <summary>
        /// The enumeration of parties that the platform sending this PlatformParties object
        /// serves to the Platform receiving this PlatformParties object.
        /// </summary>
        [Mandatory]
        public IEnumerable<PlatformParty>  Parties       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new platform parties.
        /// </summary>
        /// <param name="HubPartyId">An optional unique party identification of this hub party.</param>
        /// <param name="Parties">An enumeration of parties that the platform sending this PlatformParties object serves to the Platform receiving this PlatformParties object.</param>
        public PlatformParties(Party_Idv3?                    HubPartyId   = null,
                               IEnumerable<PlatformParty>?  Parties      = null)

        {

            this.HubPartyId  = HubPartyId;
            this.Parties     = Parties?.Distinct() ?? [];

            unchecked
            {

                hashCode = HubPartyId.GetHashCode() * 3 ^
                           Parties?.  CalcHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of platform parties.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPlatformPartiesParser">A delegate to parse custom platform parties JSON objects.</param>
        public static PlatformParties Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<PlatformParties>?  CustomPlatformPartiesParser   = null)
        {

            if (TryParse(JSON,
                         out var platformParties,
                         out var errorResponse,
                         CustomPlatformPartiesParser))
            {
                return platformParties;
            }

            throw new ArgumentException("The given JSON representation of platform parties is invalid: " + errorResponse, nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PlatformParties, out ErrorResponse, ...)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of platform parties.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PlatformParties">The parsed platform parties.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out PlatformParties?  PlatformParties,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out PlatformParties,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of platform parties.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PlatformParties">The parsed platform parties.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPlatformPartiesParser">A delegate to parse custom platform parties JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out PlatformParties?      PlatformParties,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<PlatformParties>?  CustomPlatformPartiesParser)
        {

            try
            {

                PlatformParties = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse HubPartyId    [optional]

                if (!JSON.ParseOptional("hub_party_id",
                                        "hub party identification",
                                        Party_Idv3.TryParse,
                                        out Party_Idv3 HubPartyId,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Parties       [optional]

                if (!JSON.ParseOptionalHashSet("parties",
                                               "platform parties",
                                               PlatformParty.TryParse,
                                               out HashSet<PlatformParty> Parties,
                                               out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PlatformParties = new PlatformParties(
                                      HubPartyId,
                                      Parties
                                  );


                if (CustomPlatformPartiesParser is not null)
                    PlatformParties = CustomPlatformPartiesParser(JSON,
                                                                  PlatformParties);

                return true;

            }
            catch (Exception e)
            {
                PlatformParties  = default;
                ErrorResponse    = "The given JSON representation of platform parties is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPlatformPartiesSerializer = null, CustomPlatformPartySerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPlatformPartiesSerializer">A delegate to serialize custom platform parties JSON objects.</param>
        /// <param name="CustomPlatformPartySerializer">A delegate to serialize custom platform party JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom images.</param>
        /// <param name="CustomPointOfContactSerializer">A delegate to serialize custom point of contacts.</param>
        /// <param name="CustomPartyRoleSerializer">A delegate to serialize custom party role JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PlatformParties>?  CustomPlatformPartiesSerializer   = null,
                              CustomJObjectSerializerDelegate<PlatformParty>?    CustomPlatformPartySerializer     = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null,
                              CustomJObjectSerializerDelegate<PointOfContact>?   CustomPointOfContactSerializer    = null,
                              CustomJObjectSerializerDelegate<PartyRole>?        CustomPartyRoleSerializer         = null)
        {

            var json = JSONObject.Create(

                           HubPartyId.HasValue
                               ? new JProperty("hub_party_id",  HubPartyId.Value.ToString())
                               : null,

                           Parties.Any()
                               ? new JProperty("parties",       new JArray(Parties.Select(party => party.ToJSON(CustomPlatformPartySerializer,
                                                                                                                CustomBusinessDetailsSerializer,
                                                                                                                CustomImageSerializer,
                                                                                                                CustomPointOfContactSerializer,
                                                                                                                CustomPartyRoleSerializer))))
                               : null

                       );

            return CustomPlatformPartiesSerializer is not null
                       ? CustomPlatformPartiesSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this platform parties.
        /// </summary>
        public PlatformParties Clone()

            => new (
                   HubPartyId?.Clone(),
                   Parties.Any() ? Parties.Select(role => role.Clone()) : null
               );

        #endregion


        #region Operator overloading

        #region Operator == (PlatformParties1, PlatformParties2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParties1">A platform parties.</param>
        /// <param name="PlatformParties2">Another platform parties.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PlatformParties PlatformParties1,
                                           PlatformParties PlatformParties2)

            => PlatformParties1.Equals(PlatformParties2);

        #endregion

        #region Operator != (PlatformParties1, PlatformParties2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParties1">A platform parties.</param>
        /// <param name="PlatformParties2">Another platform parties.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PlatformParties PlatformParties1,
                                           PlatformParties PlatformParties2)

            => !PlatformParties1.Equals(PlatformParties2);

        #endregion

        #region Operator <  (PlatformParties1, PlatformParties2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParties1">A platform parties.</param>
        /// <param name="PlatformParties2">Another platform parties.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PlatformParties PlatformParties1,
                                          PlatformParties PlatformParties2)

            => PlatformParties1.CompareTo(PlatformParties2) < 0;

        #endregion

        #region Operator <= (PlatformParties1, PlatformParties2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParties1">A platform parties.</param>
        /// <param name="PlatformParties2">Another platform parties.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PlatformParties PlatformParties1,
                                           PlatformParties PlatformParties2)

            => PlatformParties1.CompareTo(PlatformParties2) <= 0;

        #endregion

        #region Operator >  (PlatformParties1, PlatformParties2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParties1">A platform parties.</param>
        /// <param name="PlatformParties2">Another platform parties.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PlatformParties PlatformParties1,
                                          PlatformParties PlatformParties2)

            => PlatformParties1.CompareTo(PlatformParties2) > 0;

        #endregion

        #region Operator >= (PlatformParties1, PlatformParties2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PlatformParties1">A platform parties.</param>
        /// <param name="PlatformParties2">Another platform parties.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PlatformParties PlatformParties1,
                                           PlatformParties PlatformParties2)

            => PlatformParties1.CompareTo(PlatformParties2) >= 0;

        #endregion

        #endregion

        #region IComparable<PlatformParties> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two platform partiess.
        /// </summary>
        /// <param name="Object">A platform parties to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PlatformParties platformParties
                   ? CompareTo(platformParties)
                   : throw new ArgumentException("The given object is not a platform parties object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PlatformParties)

        /// <summary>
        /// Compares two platform partiess.
        /// </summary>
        /// <param name="PlatformParties">A platform parties to compare with.</param>
        public Int32 CompareTo(PlatformParties? PlatformParties)
        {

            if (PlatformParties is null)
                throw new ArgumentNullException(nameof(PlatformParties), "The given platform parties object must not be null!");

            var c = HubPartyId?.CompareTo(PlatformParties.HubPartyId) ?? (PlatformParties.HubPartyId is null ? 0 : -1);

            if (c == 0)
                c = Parties.OrderBy(party => party).SequenceEqual(PlatformParties.Parties.OrderBy(party => party)) ? 0 : 1;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PlatformParties> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two platform partiess for equality.
        /// </summary>
        /// <param name="Object">A platform parties to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PlatformParties platformParties &&
                   Equals(platformParties);

        #endregion

        #region Equals(PlatformParties)

        /// <summary>
        /// Compares two platform partiess for equality.
        /// </summary>
        /// <param name="PlatformParties">A platform parties to compare with.</param>
        public Boolean Equals(PlatformParties? PlatformParties)

            => PlatformParties is not null &&
               HubPartyId.Equals(PlatformParties.HubPartyId) &&
               Parties.OrderBy(party => party).SequenceEqual(PlatformParties.Parties.OrderBy(party => party));

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

            => $"{HubPartyId?.ToString() ?? "<none>"}: {Parties.Select(party => party.PartyId.ToString()).AggregateWith(", ")}";

        #endregion

    }

}
