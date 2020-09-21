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


        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(

                           new JProperty("uid",                   Id.ToString()),

                           Type.HasValue
                               ? new JProperty("type",            Type.ToString())
                               : null,

                           VisualNumber.IsNotNullOrEmpty()
                               ? new JProperty("visual_number",   VisualNumber)
                               : null,

                           Issuer.IsNotNullOrEmpty()
                               ? new JProperty("issuer",          Issuer)
                               : null,

                           GroupId.HasValue
                               ? new JProperty("group_id",        GroupId.ToString())
                               : null

                       );

            return JSON;

        }


        #region Operator overloading

        #region Operator == (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">An publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PublishTokenType PublishTokenType1,
                                           PublishTokenType PublishTokenType2)

            => PublishTokenType1.Equals(PublishTokenType2);

        #endregion

        #region Operator != (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">An publish token for opening or access hours.</param>
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
        /// <param name="PublishTokenType1">An publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PublishTokenType PublishTokenType1,
                                          PublishTokenType PublishTokenType2)

            => PublishTokenType1.CompareTo(PublishTokenType2) < 0;

        #endregion

        #region Operator <= (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">An publish token for opening or access hours.</param>
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
        /// <param name="PublishTokenType1">An publish token for opening or access hours.</param>
        /// <param name="PublishTokenType2">Another publish token for opening or access hours.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PublishTokenType PublishTokenType1,
                                          PublishTokenType PublishTokenType2)

            => PublishTokenType1.CompareTo(PublishTokenType2) > 0;

        #endregion

        #region Operator >= (PublishTokenType1, PublishTokenType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublishTokenType1">An publish token for opening or access hours.</param>
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

            => Id.CompareTo(PublishTokenType.Id);

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

            => Id.Equals(PublishTokenType.Id);

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
