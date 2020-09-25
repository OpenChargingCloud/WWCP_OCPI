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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Specifies one publish token for opening or access hours.
    /// </summary>
    public class PublishTokenType : IEquatable<PublishTokenType>,
                                    IComparable<PublishTokenType>,
                                    IComparable
    {

        #region Properties

        /// <summary>
        /// Unique ID by which this Token can be identified.
        /// </summary>
        /// <remarks>Mandatory within this implementation.</remarks>
        [Mandatory]
        public Token_Id     Id              { get; }

        /// <summary>
        /// Type of the token.
        /// </summary>
        [Optional]
        public TokenTypes?  Type            { get; }

        /// <summary>
        /// Visual readable number/identification as printed on the Token (RFID card).  // 64
        /// </summary>
        [Optional]
        public String       VisualNumber    { get; }

        /// <summary>
        /// Begin of the opening or access hours exception.  // 64
        /// </summary>
        [Optional]
        public String       Issuer          { get; }

        /// <summary>
        /// This ID groups a couple of tokens. This can be used to make two or more tokens work as one.
        /// </summary>
        [Optional]
        public Group_Id?    GroupId         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new publish token for opening or access hours.
        /// </summary>

        public PublishTokenType(Token_Id     Id,
                                TokenTypes?  Type           = null,
                                String       VisualNumber   = null,
                                String       Issuer         = null,
                                Group_Id?    GroupId        = null)
        {

            this.Id            = Id;
            this.Type          = Type;
            this.VisualNumber  = VisualNumber;
            this.Issuer        = Issuer;
            this.GroupId       = GroupId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomPublishTokenTypeParser = null)

        /// <summary>
        /// Parse the given JSON representation of a publish token type.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPublishTokenTypeParser">A delegate to parse custom publish token type JSON objects.</param>
        public static PublishTokenType Parse(JObject                                        JSON,
                                             CustomJObjectParserDelegate<PublishTokenType>  CustomPublishTokenTypeParser   = null)
        {

            if (TryParse(JSON,
                         out PublishTokenType publishTokenType,
                         out String           ErrorResponse,
                         CustomPublishTokenTypeParser))
            {
                return publishTokenType;
            }

            throw new ArgumentException("The given JSON representation of a publish token type is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomPublishTokenTypeParser = null)

        /// <summary>
        /// Parse the given text representation of a publish token type.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomPublishTokenTypeParser">A delegate to parse custom publish token type JSON objects.</param>
        public static PublishTokenType Parse(String                                         Text,
                                             CustomJObjectParserDelegate<PublishTokenType>  CustomPublishTokenTypeParser   = null)
        {

            if (TryParse(Text,
                         out PublishTokenType publishTokenType,
                         out String         ErrorResponse,
                         CustomPublishTokenTypeParser))
            {
                return publishTokenType;
            }

            throw new ArgumentException("The given text representation of a publish token type is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out PublishTokenType, out ErrorResponse, CustomPublishTokenTypeParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a publish token type.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PublishTokenType">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject               JSON,
                                       out PublishTokenType  PublishTokenType,
                                       out String            ErrorResponse)

            => TryParse(JSON,
                        out PublishTokenType,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a publish token type.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PublishTokenType">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublishTokenTypeParser">A delegate to parse custom publish token type JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       out PublishTokenType                           PublishTokenType,
                                       out String                                     ErrorResponse,
                                       CustomJObjectParserDelegate<PublishTokenType>  CustomPublishTokenTypeParser)
        {

            try
            {

                PublishTokenType = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id              [mandatory]

                if (!JSON.ParseMandatory("uid",
                                         "uid",
                                         Token_Id.TryParse,
                                         out Token_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Type            [optional]

                if (JSON.ParseOptionalEnum("type",
                                           "type",
                                           out TokenTypes? Type,
                                           out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                PublishTokenType = new PublishTokenType(Id,
                                                        Type,
                                                        VisualNumber,
                                                        Issuer,
                                                        GroupId);


                if (CustomPublishTokenTypeParser != null)
                    PublishTokenType = CustomPublishTokenTypeParser(JSON,
                                                                PublishTokenType);

                return true;

            }
            catch (Exception e)
            {
                PublishTokenType  = default;
                ErrorResponse     = "The given JSON representation of a publish token type is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out PublishTokenType, out ErrorResponse, CustomPublishTokenTypeParser = null)

        /// <summary>
        /// Try to parse the given text representation of a publish token type.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="PublishTokenType">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublishTokenTypeParser">A delegate to parse custom publish token type JSON objects.</param>
        public static Boolean TryParse(String                                         Text,
                                       out PublishTokenType                           PublishTokenType,
                                       out String                                     ErrorResponse,
                                       CustomJObjectParserDelegate<PublishTokenType>  CustomPublishTokenTypeParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out PublishTokenType,
                                out ErrorResponse,
                                CustomPublishTokenTypeParser);

            }
            catch (Exception e)
            {
                PublishTokenType = default;
                ErrorResponse  = "The given text representation of a publish token type is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublishTokenTypeSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublishTokenTypeSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PublishTokenType> CustomPublishTokenTypeSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("uid",                  Id.     ToString()),

                           Type.HasValue
                               ? new JProperty("type",           Type.   ToString())
                               : null,

                           VisualNumber.IsNotNullOrEmpty()
                               ? new JProperty("visual_number",  VisualNumber)
                               : null,

                           Issuer.IsNotNullOrEmpty()
                               ? new JProperty("issuer",         Issuer)
                               : null,

                           GroupId.HasValue
                               ? new JProperty("group_id",       GroupId.ToString())
                               : null

                       );

            return CustomPublishTokenTypeSerializer != null
                       ? CustomPublishTokenTypeSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">A publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PublishTokenType PublishTokenType1,
                                           PublishTokenType PublishTokenType2)
        {

            if (Object.ReferenceEquals(PublishTokenType1, PublishTokenType2))
                return true;

            if (PublishTokenType1 is null || PublishTokenType2 is null)
                return false;

            return PublishTokenType1.Equals(PublishTokenType2);

        }

        #endregion

        #region Operator != (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">A publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PublishTokenType PublishTokenType1,
                                           PublishTokenType PublishTokenType2)

            => !(PublishTokenType1 == PublishTokenType2);

        #endregion

        #region Operator <  (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">A publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PublishTokenType PublishTokenType1,
                                          PublishTokenType PublishTokenType2)

            => PublishTokenType1 is null
                   ? throw new ArgumentNullException(nameof(PublishTokenType1), "The given publish token must not be null!")
                   : PublishTokenType1.CompareTo(PublishTokenType2) < 0;

        #endregion

        #region Operator <= (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">A publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PublishTokenType PublishTokenType1,
                                           PublishTokenType PublishTokenType2)

            => !(PublishTokenType1 > PublishTokenType2);

        #endregion

        #region Operator >  (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">A publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PublishTokenType PublishTokenType1,
                                          PublishTokenType PublishTokenType2)

            => PublishTokenType1 is null
                   ? throw new ArgumentNullException(nameof(PublishTokenType1), "The given publish token must not be null!")
                   : PublishTokenType1.CompareTo(PublishTokenType2) > 0;

        #endregion

        #region Operator >= (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">A publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PublishTokenType PublishTokenType1,
                                           PublishTokenType PublishTokenType2)

            => !(PublishTokenType1 < PublishTokenType2);

        #endregion

        #endregion

        #region IComparable<PublishTokenType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is PublishTokenType publishTokenType
                   ? CompareTo(publishTokenType)
                   : throw new ArgumentException("The given object is not a publish token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PublishTokenType)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType">An object to compare with.</param>
        public Int32 CompareTo(PublishTokenType PublishTokenType)

            => PublishTokenType is null
                   ? throw new ArgumentNullException(nameof(PublishTokenType), "The given publish token must not be null!")
                   : Id.CompareTo(PublishTokenType.Id);

        #endregion

        #endregion

        #region IEquatable<PublishTokenType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is PublishTokenType publishTokenType &&
                   Equals(publishTokenType);

        #endregion

        #region Equals(PublishTokenType)

        /// <summary>
        /// Compares two PublishTokenTypes for equality.
        /// </summary>
        /// <param name="PublishTokenType">A PublishTokenType to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(PublishTokenType PublishTokenType)

            => !(PublishTokenType is null) &&
                   Id.Equals(PublishTokenType.Id);

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
                return Id.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Id,
                             Type.HasValue
                                 ? " (" + Type.ToString() + ")"
                                 : "",
                             VisualNumber.IsNotNullOrEmpty()
                                 ? ", visual: '" + VisualNumber + "'"
                                 : "",
                             Issuer.IsNotNullOrEmpty()
                                 ? ", issuer: '" + Issuer + "'"
                                 : "",
                             GroupId.HasValue
                                 ? ", group: '"  + GroupId.ToString() + "'"
                                 : "");

        #endregion

    }

}
