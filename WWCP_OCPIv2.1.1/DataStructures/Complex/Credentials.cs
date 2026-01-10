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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Credentials.
    /// </summary>
    public class Credentials : IEquatable<Credentials>,
                               IComparable<Credentials>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The credentials token for the other party to authenticate in your system.
        /// </summary>
        [Mandatory]
        public AccessToken     Token               { get; }

        /// <summary>
        /// The URL to your API versions endpoint.
        /// </summary>
        [Mandatory]
        public URL              URL                { get; }

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails  BusinessDetails    { get; }

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode      CountryCode        { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id         PartyId            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new credentials.
        /// </summary>
        /// <param name="Token">The credentials token for the other party to authenticate in your system.</param>
        /// <param name="URL">The URL to your API versions endpoint.</param>
        /// <param name="BusinessDetails">Business details of this party.</param>
        /// <param name="CountryCode">ISO-3166 alpha-2 country code of the country this party is operating in.</param>
        /// <param name="PartyId">CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).</param>
        public Credentials(AccessToken      Token,
                           URL              URL,
                           BusinessDetails  BusinessDetails,
                           CountryCode      CountryCode,
                           Party_Id         PartyId)
        {

            this.Token            = Token;
            this.URL              = URL;
            this.BusinessDetails  = BusinessDetails;
            this.CountryCode      = CountryCode;
            this.PartyId          = PartyId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomCredentialsParser = null)

        /// <summary>
        /// Parse the given JSON representation of a credentials.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCredentialsParser">A delegate to parse custom credentials JSON objects.</param>
        public static Credentials Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<Credentials>?  CustomCredentialsParser   = null)
        {

            if (TryParse(JSON,
                         out var credentials,
                         out var errorResponse,
                         CustomCredentialsParser))
            {
                return credentials;
            }

            throw new ArgumentException("The given JSON representation of a credentials is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Credentials, out ErrorResponse, CustomCredentialsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a credentials.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Credentials">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Credentials?  Credentials,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out Credentials,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a credentials.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Credentials">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCredentialsParser">A delegate to parse custom credentials JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out Credentials?      Credentials,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<Credentials>?  CustomCredentialsParser)
        {

            try
            {

                Credentials = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Token               [mandatory]

                if (!JSON.ParseMandatory("token",
                                         "access token",
                                         AccessToken.TryParse,
                                         out AccessToken Token,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL                 [mandatory]

                if (!JSON.ParseMandatory("url",
                                         "url",
                                         org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Business details    [mandatory]

                if (!JSON.ParseMandatoryJSON("business_details",
                                             "business details",
                                             OCPI.BusinessDetails.TryParse,
                                             out BusinessDetails? BusinessDetails,
                                             out ErrorResponse) ||
                    BusinessDetails is null)
                {
                    return false;
                }

                #endregion

                #region Parse CountryCode         [mandatory]

                if (!JSON.ParseMandatory("country_code",
                                         "country code",
                                         OCPI.CountryCode.TryParse,
                                         out CountryCode CountryCode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PartyId             [mandatory]

                if (!JSON.ParseMandatory("party_id",
                                         "party identification",
                                         Party_Id.TryParse,
                                         out Party_Id PartyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                Credentials = new Credentials(
                                  Token,
                                  URL,
                                  BusinessDetails,
                                  CountryCode,
                                  PartyId
                              );


                if (CustomCredentialsParser is not null)
                    Credentials = CustomCredentialsParser(JSON,
                                                          Credentials);

                return true;

            }
            catch (Exception e)
            {
                Credentials    = default;
                ErrorResponse  = "The given JSON representation of a credentials is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCredentialsSerializer = null, CustomCredentialsRoleSerializer = null, CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCredentialsSerializer">A delegate to serialize custom credentials JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Credentials>?      CustomCredentialsSerializer       = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null)
        {

            var json = JSONObject.Create(

                           new JProperty("token",             Token.          ToString()),
                           new JProperty("url",               URL.            ToString()),
                           new JProperty("business_details",  BusinessDetails.ToJSON(CustomBusinessDetailsSerializer)),
                           new JProperty("country_code",      CountryCode.    ToString()),
                           new JProperty("party_id",          PartyId.        ToString())

                       );

            return CustomCredentialsSerializer is not null
                       ? CustomCredentialsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Credentials Clone()

            => new (
                   Token.          Clone(),
                   URL.            Clone(),
                   BusinessDetails.Clone(),
                   CountryCode.    Clone(),
                   PartyId.        Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">Credentials.</param>
        /// <param name="Credentials2">Other credentials.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Credentials Credentials1,
                                           Credentials Credentials2)
        {

            if (Object.ReferenceEquals(Credentials1, Credentials2))
                return true;

            if (Credentials1 is null || Credentials2 is null)
                return false;

            return Credentials1.Equals(Credentials2);

        }

        #endregion

        #region Operator != (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">Credentials.</param>
        /// <param name="Credentials2">Other credentials.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Credentials Credentials1,
                                           Credentials Credentials2)

            => !(Credentials1 == Credentials2);

        #endregion

        #region Operator <  (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">Credentials.</param>
        /// <param name="Credentials2">Other credentials.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Credentials Credentials1,
                                          Credentials Credentials2)

            => Credentials1 is null
                   ? throw new ArgumentNullException(nameof(Credentials1), "The given credentials must not be null!")
                   : Credentials1.CompareTo(Credentials2) < 0;

        #endregion

        #region Operator <= (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">Credentials.</param>
        /// <param name="Credentials2">Other credentials.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Credentials Credentials1,
                                           Credentials Credentials2)

            => !(Credentials1 > Credentials2);

        #endregion

        #region Operator >  (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">Credentials.</param>
        /// <param name="Credentials2">Other credentials.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Credentials Credentials1,
                                          Credentials Credentials2)

            => Credentials1 is null
                   ? throw new ArgumentNullException(nameof(Credentials1), "The given credentials must not be null!")
                   : Credentials1.CompareTo(Credentials2) > 0;

        #endregion

        #region Operator >= (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">Credentials.</param>
        /// <param name="Credentials2">Other credentials.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Credentials Credentials1,
                                           Credentials Credentials2)

            => !(Credentials1 < Credentials2);

        #endregion

        #endregion

        #region IComparable<Credentials> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Credentials credentials
                   ? CompareTo(credentials)
                   : throw new ArgumentException("The given object is not a credentials object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Credentials)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials">An object to compare with.</param>
        public Int32 CompareTo(Credentials? Credentials)
        {

            if (Credentials is null)
                throw new ArgumentNullException(nameof(Credentials), "The given credential must not be null!");

            var c = CountryCode.    CompareTo(Credentials.CountryCode);

            if (c == 0)
                c = PartyId.        CompareTo(Credentials.PartyId);

            if (c == 0)
                c = BusinessDetails.CompareTo(Credentials.BusinessDetails);

            if (c == 0)
                c = URL.            CompareTo(Credentials.URL);

            if (c == 0)
                c = Token.          CompareTo(Credentials.Token);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Credentials> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two credentials for equality.
        /// </summary>
        /// <param name="Object">Credentials to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Credentials credentials &&
                   Equals(credentials);

        #endregion

        #region Equals(Credentials)

        /// <summary>
        /// Compares two credentials for equality.
        /// </summary>
        /// <param name="Credentials">Credentials to compare with.</param>
        public Boolean Equals(Credentials? Credentials)

            => Credentials is not null &&

               Token.          Equals(Credentials.Token)           &&
               URL.            Equals(Credentials.URL)             &&
               BusinessDetails.Equals(Credentials.BusinessDetails) &&
               CountryCode.    Equals(Credentials.CountryCode)     &&
               PartyId.        Equals(Credentials.PartyId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Token.          GetHashCode() * 11 ^
                       URL.            GetHashCode() *  7 ^
                       BusinessDetails.GetHashCode() *  5 ^
                       CountryCode.    GetHashCode() *  3 ^
                       PartyId.        GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   BusinessDetails,
                   " (",
                   CountryCode,
                   "-",
                   PartyId,
                   ", ",
                   Token,
                   ") => ",
                   URL

               );

        #endregion

    }

}
