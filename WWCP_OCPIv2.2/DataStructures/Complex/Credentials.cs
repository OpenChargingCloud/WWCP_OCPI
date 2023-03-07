/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
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
        public AccessToken                   Token     { get; }

        /// <summary>
        /// The URL to your API versions endpoint.
        /// </summary>
        [Mandatory]
        public URL                           URL       { get; }

        /// <summary>
        /// The enumeration of roles a party provides.
        /// </summary>
        [Mandatory]
        public IEnumerable<CredentialsRole>  Roles     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new credentials.
        /// </summary>
        /// <param name="Token">The credentials token for the other party to authenticate in your system.</param>
        /// <param name="URL">The URL to your API versions endpoint.</param>
        /// <param name="Roles">The enumeration of roles a party provides.</param>
        public Credentials(AccessToken                   Token,
                           URL                           URL,
                           IEnumerable<CredentialsRole>  Roles)
        {

            if (!Roles.SafeAny())
                throw new ArgumentNullException(nameof(Roles),  "The given enumeration of roles must not be null or empty!");

            this.Token  = Token;
            this.URL    = URL;
            this.Roles  = Roles?.Distinct() ?? Array.Empty<CredentialsRole>();

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
                return credentials!;
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
        public static Boolean TryParse(JObject           JSON,
                                       out Credentials?  Credentials,
                                       out String?       ErrorResponse)

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
                                       out Credentials?                           Credentials,
                                       out String?                                ErrorResponse,
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

                #region Parse Token     [mandatory]

                if (!JSON.ParseMandatory("token",
                                         "access token",
                                         AccessToken.TryParse,
                                         out AccessToken Token,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URL       [mandatory]

                if (!JSON.ParseMandatory("url",
                                         "url",
                                         org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Roles     [mandatory]

                if (JSON.ParseMandatoryHashSet("roles",
                                               "roles",
                                               CredentialsRole.TryParse,
                                               out HashSet<CredentialsRole> Roles,
                                               out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                Credentials = new Credentials(Token,
                                              URL,
                                              Roles);


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

        #region ToJSON(CustomCredentialsSerializer = null, CustomCredentialsRoleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCredentialsSerializer">A delegate to serialize custom credentials JSON objects.</param>
        /// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials roles JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Credentials>?      CustomCredentialsSerializer       = null,
                              CustomJObjectSerializerDelegate<CredentialsRole>?  CustomCredentialsRoleSerializer   = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("token",  Token.ToString()),
                           new JProperty("url",    URL.  ToString()),

                           new JProperty("roles",  new JArray(Roles.Select(role => role.ToJSON(CustomCredentialsRoleSerializer,
                                                                                               CustomBusinessDetailsSerializer))))

                       );

            return CustomCredentialsSerializer is not null
                       ? CustomCredentialsSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Credentials Clone()

            => new (Token.Clone,
                    URL.  Clone,
                    Roles.Select(role => role.Clone()).ToArray());

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
        /// Compares two credentials.
        /// </summary>
        /// <param name="Object">Credentials to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Credentials credentials
                   ? CompareTo(credentials)
                   : throw new ArgumentException("The given object is not a credentials object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Credentials)

        /// <summary>
        /// Compares two credentials.
        /// </summary>
        /// <param name="Credentials">Credentials to compare with.</param>
        public Int32 CompareTo(Credentials? Credentials)
        {

            if (Credentials is null)
                throw new ArgumentNullException(nameof(Credentials), "The given credential must not be null!");

            var c = Token.CompareTo(Credentials.Token);

            if (c == 0)
                c = URL.  CompareTo(Credentials.URL);

            //ToDo: Compare credentials roles!

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

               Token.Equals(Credentials.Token) &&
               URL.  Equals(Credentials.URL)   &&

               Roles.Count().Equals(Credentials.Roles.Count()) &&
               Roles.All(role => Credentials.Roles.Contains(role));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Token.GetHashCode() * 5 ^
                       URL.  GetHashCode() * 3 ^
                       Roles.CalcHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Token.ToString().SubstringMax(5),
                   " : ",
                   URL,
                   " => ",
                   Roles.AggregateWith(", ")

               );

        #endregion

    }

}
