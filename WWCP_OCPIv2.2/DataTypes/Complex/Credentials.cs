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
        /// Case Sensitive, ASCII only. The credentials token for the other party to
        /// authenticate in your system. Not encoded in Base64 or any other encoding.
        /// </summary>
        [Mandatory]
        public AccessToken                   Token     { get; }

        /// <summary>
        /// The URL to your API versions endpoint.
        /// </summary>
        [Mandatory]
        public String                        URL       { get; }

        /// <summary>
        /// The enumeration of roles a party provides.
        /// </summary>
        [Optional]
        public IEnumerable<CredentialsRole>  Roles     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new credentials.
        /// </summary>
        public Credentials(AccessToken                   Token,
                           String                        URL,
                           IEnumerable<CredentialsRole>  Roles)
        {

            this.Token  = Token;
            this.URL    = URL;
            this.Roles  = Roles;

        }

        #endregion


        #region (static) Parse   (JSON, CustomCredentialsParser = null)

        /// <summary>
        /// Parse the given JSON representation of a credentials.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCredentialsParser">A delegate to parse custom credentials JSON objects.</param>
        public static Credentials Parse(JObject                                   JSON,
                                        CustomJObjectParserDelegate<Credentials>  CustomCredentialsParser   = null)
        {

            if (TryParse(JSON,
                         out Credentials credentials,
                         out String      ErrorResponse,
                         CustomCredentialsParser))
            {
                return credentials;
            }

            throw new ArgumentException("The given JSON representation of a credentials is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomCredentialsParser = null)

        /// <summary>
        /// Parse the given text representation of a credentials.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomCredentialsParser">A delegate to parse custom credentials JSON objects.</param>
        public static Credentials Parse(String                                    Text,
                                        CustomJObjectParserDelegate<Credentials>  CustomCredentialsParser   = null)
        {

            if (TryParse(Text,
                         out Credentials credentials,
                         out String      ErrorResponse,
                         CustomCredentialsParser))
            {
                return credentials;
            }

            throw new ArgumentException("The given text representation of a credentials is invalid: " + ErrorResponse, nameof(Text));

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
        public static Boolean TryParse(JObject          JSON,
                                       out Credentials  Credentials,
                                       out String       ErrorResponse)

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
        public static Boolean TryParse(JObject                                   JSON,
                                       out Credentials                           Credentials,
                                       out String                                ErrorResponse,
                                       CustomJObjectParserDelegate<Credentials>  CustomCredentialsParser)
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

                if (!JSON.ParseMandatoryText("url",
                                             "url",
                                             out String URL,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Roles     [optional]

                if (JSON.ParseOptionalJSON("roles",
                                           "roles",
                                           CredentialsRole.TryParse,
                                           out IEnumerable<CredentialsRole> Roles,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                Credentials = new Credentials(Token,
                                              URL,
                                              Roles);


                if (CustomCredentialsParser != null)
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

        #region (static) TryParse(Text, out Credentials, out ErrorResponse, CustomCredentialsParser = null)

        /// <summary>
        /// Try to parse the given text representation of a credentials.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Credentials">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCredentialsParser">A delegate to parse custom credentials JSON objects.</param>
        public static Boolean TryParse(String                                    Text,
                                       out Credentials                           Credentials,
                                       out String                                ErrorResponse,
                                       CustomJObjectParserDelegate<Credentials>  CustomCredentialsParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Credentials,
                                out ErrorResponse,
                                CustomCredentialsParser);

            }
            catch (Exception e)
            {
                Credentials = default;
                ErrorResponse  = "The given text representation of a credentials is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCredentialsSerializer = null, CustomCredentialsRoleSerializer = null, CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCredentialsSerializer">A delegate to serialize custom credentials JSON objects.</param>
        /// <param name="CustomCredentialsRoleSerializer">A delegate to serialize custom credentials roles JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Credentials>      CustomCredentialsSerializer       = null,
                              CustomJObjectSerializerDelegate<CredentialsRole>  CustomCredentialsRoleSerializer   = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>  CustomBusinessDetailsSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("token",  Token.ToString()),
                           new JProperty("url",    URL),

                           new JProperty("roles",  new JArray(Roles.SafeSelect(role => role.ToJSON(CustomCredentialsRoleSerializer,
                                                                                                   CustomBusinessDetailsSerializer))))

                       );

            return CustomCredentialsSerializer != null
                       ? CustomCredentialsSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">A credential.</param>
        /// <param name="Credentials2">Another credential.</param>
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
        /// <param name="Credentials1">A credential.</param>
        /// <param name="Credentials2">Another credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Credentials Credentials1,
                                           Credentials Credentials2)

            => !(Credentials1 == Credentials2);

        #endregion

        #region Operator <  (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">A credential.</param>
        /// <param name="Credentials2">Another credential.</param>
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
        /// <param name="Credentials1">A credential.</param>
        /// <param name="Credentials2">Another credential.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Credentials Credentials1,
                                           Credentials Credentials2)

            => !(Credentials1 > Credentials2);

        #endregion

        #region Operator >  (Credentials1, Credentials2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials1">A credential.</param>
        /// <param name="Credentials2">Another credential.</param>
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
        /// <param name="Credentials1">A credential.</param>
        /// <param name="Credentials2">Another credential.</param>
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
        public Int32 CompareTo(Object Object)

            => Object is Credentials credentials
                   ? CompareTo(credentials)
                   : throw new ArgumentException("The given object is not a credential!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Credentials)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Credentials">An object to compare with.</param>
        public Int32 CompareTo(Credentials Credentials)
        {

            if (Credentials == null)
                throw new ArgumentNullException(nameof(Credentials), "The given credential must not be null!");

            return Token.CompareTo(Credentials.Token);

        }

        #endregion

        #endregion

        #region IEquatable<Credentials> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Credentials Credentials &&
                   Equals(Credentials);

        #endregion

        #region Equals(Credentials)

        /// <summary>
        /// Compares two Credentialss for equality.
        /// </summary>
        /// <param name="Credentials">A Credentials to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Credentials Credentials)

            => !(Credentials is null) &&

               Token.Equals(Credentials.Token) &&
               URL.  Equals(Credentials.URL)   &&

               Roles.Count().Equals(Credentials.Roles.Count()) &&
               Roles.All(role => Credentials.Roles.Contains(role));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Token.GetHashCode() * 5 ^
                       URL.  GetHashCode() * 3 ^

                       Roles.Aggregate(0, (hashCode, role) => hashCode ^ role.GetHashCode());

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Token.ToString().SubstringMax(5), " : ", URL, " => ", Roles.AggregateWith(", "));

        #endregion

    }

}
