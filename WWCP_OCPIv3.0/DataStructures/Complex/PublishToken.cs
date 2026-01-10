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
    /// The publish token specifies a token for opening or access hours.
    /// </summary>
    public class PublishToken : IEquatable<PublishToken>,
                                IComparable<PublishToken>,
                                IComparable
    {

        #region Properties

        /// <summary>
        /// The publish token.
        /// </summary>
        [Optional]
        public Token_Id?   Id              { get; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        [Optional]
        public TokenType?  Type            { get; }

        /// <summary>
        /// The optional visual or readable number/identification as printed on the token (RFID card).
        /// Might be equal to the contract identification.
        /// string(64)
        /// </summary>
        [Optional]
        public String?     VisualNumber    { get; }

        /// <summary>
        /// The optional issuing company.
        /// This is normally the name of the company printed on the token (RFID card), but not necessarily the eMSP.
        /// string(64)
        /// </summary>
        [Optional]
        public String?     Issuer          { get; }

        /// <summary>
        /// The optional group identification. This identification groups a couple of tokens to make two or more tokens work as one.
        /// So a session can be started with one token and stopped with another, handy when a card
        /// and key-fob are given to the EV-driver.
        /// Beware that OCPP 1.5/1.6 only support group_ids (it is called parentId in OCPP 1.5/1.6) with
        /// a maximum length of 20.
        /// </summary>
        [Optional]
        public Group_Id?   GroupId         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new publish token for opening or access hours.
        /// At least one of the following fields SHALL be set: Id, VisualNumber, or GroupId:
        ///   When Id is set           => Type SHALL also be set.
        ///   When VisualNumber is set => Issuer SHALL also be set.
        /// </summary>
        /// <param name="Id">A publish token.</param>
        /// <param name="Type">The type of the token.</param>
        /// <param name="VisualNumber">An optional visual or readable number/identification as printed on the token (RFID card). Might be equal to the contract identification.</param>
        /// <param name="Issuer">An optional issuing company. This is normally the name of the company printed on the token (RFID card), but not necessarily the eMSP.</param>
        /// <param name="GroupId">An optional group identification. This identification groups a couple of tokens to make two or more tokens work as one.</param>
        public PublishToken(Token_Id?   Id             = null,
                            TokenType?  Type           = null,
                            String?     VisualNumber   = null,
                            String?     Issuer         = null,
                            Group_Id?   GroupId        = null)
        {

            this.Id            = Id;
            this.Type          = Type;
            this.VisualNumber  = VisualNumber;
            this.Issuer        = Issuer;
            this.GroupId       = GroupId;

            if (!GroupId.HasValue && VisualNumber is null && !Id.HasValue)
                throw new ArgumentException("At least one of the following fields SHALL be set: Id, VisualNumber, or GroupId!");

        }

        #endregion


        #region (static) Parse   (JSON, CustomPublishTokenParser = null)

        /// <summary>
        /// Parse the given JSON representation of a publish token type.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPublishTokenParser">A delegate to parse custom publish token type JSON objects.</param>
        public static PublishToken? Parse(JObject                                     JSON,
                                          CustomJObjectParserDelegate<PublishToken>?  CustomPublishTokenParser   = null)
        {

            if (TryParse(JSON,
                         out var publishToken,
                         out var errorResponse,
                         CustomPublishTokenParser))
            {
                return publishToken;
            }

            throw new ArgumentException("The given JSON representation of a publish token type is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PublishToken, out ErrorResponse, CustomPublishTokenParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a publish token type.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PublishToken">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                                            out PublishToken?  PublishToken,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out PublishToken,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a publish token type.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PublishToken">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublishTokenParser">A delegate to parse custom publish token type JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                                            out PublishToken?      PublishToken,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<PublishToken>?  CustomPublishTokenParser)
        {

            try
            {

                PublishToken = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = null;
                    return true;
                }

                #region Parse Id              [optional]

                if (JSON.ParseOptional("uid",
                                       "uid",
                                       Token_Id.TryParse,
                                       out Token_Id? Id,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Type            [optional]

                if (JSON.ParseOptional("type",
                                       "type",
                                       TokenType.TryParse,
                                       out TokenType? Type,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse VisualNumber    [optional]

                var VisualNumber = JSON.GetString("visual_number");

                #endregion

                #region Parse Issuer          [optional]

                var Issuer = JSON.GetString("issuer");

                #endregion

                #region Parse GroupId         [optional]

                if (JSON.ParseOptional("group_id",
                                       "group id",
                                       Group_Id.TryParse,
                                       out Group_Id? GroupId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PublishToken = (Id.          HasValue           ||
                                Type.        HasValue           ||
                                VisualNumber.IsNotNullOrEmpty() ||
                                Issuer.      IsNotNullOrEmpty() ||
                                GroupId.     HasValue)

                                    ? new PublishToken(
                                          Id,
                                          Type,
                                          VisualNumber,
                                          Issuer,
                                          GroupId
                                      )

                                    : null;


                if (PublishToken             is not null &&
                    CustomPublishTokenParser is not null)
                {

                    PublishToken = CustomPublishTokenParser(JSON,
                                                            PublishToken);

                }

                return true;

            }
            catch (Exception e)
            {
                PublishToken   = default;
                ErrorResponse  = "The given JSON representation of a publish token type is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublishTokenSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublishTokenSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PublishToken>? CustomPublishTokenSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("uid",            Id.           ToString()),

                           Type.HasValue
                               ? new JProperty("type",           Type.   Value.ToString())
                               : null,

                           VisualNumber.IsNotNullOrEmpty()
                               ? new JProperty("visual_number",  VisualNumber)
                               : null,

                           Issuer.IsNotNullOrEmpty()
                               ? new JProperty("issuer",         Issuer)
                               : null,

                           GroupId.HasValue
                               ? new JProperty("group_id",       GroupId.Value.ToString())
                               : null

                       );

            return CustomPublishTokenSerializer is not null
                       ? CustomPublishTokenSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public PublishToken Clone()

            => new (
                   Id?.         Clone(),
                   Type?.       Clone(),
                   VisualNumber.CloneNullableString(),
                   Issuer.      CloneNullableString(),
                   GroupId?.    Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (PublishToken1, PublishToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishToken1">A publish token for opening or access hours.</param>
        /// <param name="PublishToken2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PublishToken PublishToken1,
                                           PublishToken PublishToken2)
        {

            if (Object.ReferenceEquals(PublishToken1, PublishToken2))
                return true;

            if (PublishToken1 is null || PublishToken2 is null)
                return false;

            return PublishToken1.Equals(PublishToken2);

        }

        #endregion

        #region Operator != (PublishToken1, PublishToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishToken1">A publish token for opening or access hours.</param>
        /// <param name="PublishToken2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PublishToken PublishToken1,
                                           PublishToken PublishToken2)

            => !(PublishToken1 == PublishToken2);

        #endregion

        #region Operator <  (PublishToken1, PublishToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishToken1">A publish token for opening or access hours.</param>
        /// <param name="PublishToken2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PublishToken PublishToken1,
                                          PublishToken PublishToken2)

            => PublishToken1 is null
                   ? throw new ArgumentNullException(nameof(PublishToken1), "The given publish token must not be null!")
                   : PublishToken1.CompareTo(PublishToken2) < 0;

        #endregion

        #region Operator <= (PublishToken1, PublishToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishToken1">A publish token for opening or access hours.</param>
        /// <param name="PublishToken2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PublishToken PublishToken1,
                                           PublishToken PublishToken2)

            => !(PublishToken1 > PublishToken2);

        #endregion

        #region Operator >  (PublishToken1, PublishToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishToken1">A publish token for opening or access hours.</param>
        /// <param name="PublishToken2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PublishToken PublishToken1,
                                          PublishToken PublishToken2)

            => PublishToken1 is null
                   ? throw new ArgumentNullException(nameof(PublishToken1), "The given publish token must not be null!")
                   : PublishToken1.CompareTo(PublishToken2) > 0;

        #endregion

        #region Operator >= (PublishToken1, PublishToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishToken1">A publish token for opening or access hours.</param>
        /// <param name="PublishToken2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PublishToken PublishToken1,
                                           PublishToken PublishToken2)

            => !(PublishToken1 < PublishToken2);

        #endregion

        #endregion

        #region IComparable<PublishToken> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two publish tokens.
        /// </summary>
        /// <param name="Object">A publish token to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PublishToken publishToken
                   ? CompareTo(publishToken)
                   : throw new ArgumentException("The given object is not a publish token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PublishToken)

        /// <summary>
        /// Compares two publish tokens.
        /// </summary>
        /// <param name="PublishToken">A publish token to compare with.</param>
        public Int32 CompareTo(PublishToken? PublishToken)
        {

            if (PublishToken is null)
                throw new ArgumentNullException(nameof(PublishToken), "The given publish token must not be null!");

            var c = Id.HasValue &&
                    PublishToken.Id.HasValue
                        ? Id.Value.CompareTo(PublishToken.Id.Value)
                        : 0;

            if (c == 0        &&
                Type.HasValue &&
                PublishToken.Type.HasValue)
            {
                c = Type.Value.CompareTo(PublishToken.Type.Value);
            }

            if (c == 0                                &&
                VisualNumber is not null              &&
                VisualNumber.IsNotNullOrEmpty()       &&
                PublishToken.VisualNumber is not null &&
                PublishToken.VisualNumber.IsNotNullOrEmpty())
            {
                c = VisualNumber.CompareTo(PublishToken.VisualNumber);
            }

            if (c == 0                          &&
                Issuer is not null              &&
                Issuer.IsNotNullOrEmpty()       &&
                PublishToken.Issuer is not null &&
                PublishToken.Issuer.IsNotNullOrEmpty())
            {
                c = Issuer.CompareTo(PublishToken.Issuer);
            }

            if (c == 0           &&
                GroupId.HasValue &&
                PublishToken.GroupId.HasValue)
            {
                c = GroupId.Value.CompareTo(PublishToken.GroupId.Value);
            }

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PublishToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two publish tokens for equality.
        /// </summary>
        /// <param name="Object">A publish token to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublishToken publishToken &&
                   Equals(publishToken);

        #endregion

        #region Equals(PublishToken)

        /// <summary>
        /// Compares two publish tokens for equality.
        /// </summary>
        /// <param name="PublishToken">A publish token to compare with.</param>
        public Boolean Equals(PublishToken? PublishToken)

            => PublishToken is not null &&

            ((!Id.          HasValue    && !PublishToken.Id.          HasValue)    ||
              (Id.          HasValue    &&  PublishToken.Id.          HasValue    && Id.     Value.Equals(PublishToken.Id.     Value))) &&

            ((!Type.        HasValue    && !PublishToken.Type.        HasValue)    ||
              (Type.        HasValue    &&  PublishToken.Type.        HasValue    && Type.   Value.Equals(PublishToken.Type.   Value))) &&

            ((!GroupId.     HasValue    && !PublishToken.GroupId.     HasValue)    ||
              (GroupId.     HasValue    &&  PublishToken.GroupId.     HasValue    && GroupId.Value.Equals(PublishToken.GroupId.Value))) &&

             ((VisualNumber is     null &&  PublishToken.VisualNumber is     null) ||
              (VisualNumber is not null &&  PublishToken.VisualNumber is not null && VisualNumber. Equals(PublishToken.VisualNumber)))  &&

             ((Issuer       is     null &&  PublishToken.Issuer       is     null) ||
              (Issuer       is not null &&  PublishToken.Issuer       is not null && Issuer.       Equals(PublishToken.Issuer)));

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

                return (Id?.          GetHashCode() ?? 0) * 11 ^
                       (Type?.        GetHashCode() ?? 0) *  7 ^
                       (VisualNumber?.GetHashCode() ?? 0) *  5 ^
                       (Issuer?.      GetHashCode() ?? 0) *  3 ^
                       (GroupId?.     GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new String?[] {

                   Id.HasValue
                       ? "id: '"            + Id.     Value.ToString() + "'"
                       : null,

                   Type.HasValue
                       ? "type: '"          + Type.   Value.ToString() + "'"
                       : null,

                   VisualNumber.IsNotNullOrEmpty()
                       ? "visual number: '" + VisualNumber + "'"
                       : null,

                   Issuer.IsNotNullOrEmpty()
                       ? "issuer: '"        + Issuer + "'"
                       : null,

                   GroupId.HasValue
                       ? "groupId: '"       + GroupId.Value.ToString() + "'"
                       : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
