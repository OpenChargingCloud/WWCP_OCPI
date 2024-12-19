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
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Point of contact.
    /// </summary>
    public class PointOfContact : IEquatable<PointOfContact>,
                                  IComparable<PointOfContact>,
                                  IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/pointOfContact");

        #endregion

        #region Properties

        /// <summary>
        /// The name of the point of contact. This can be the name of a person, but it can also
        /// be the name of a department or team like "Corporate Clients Desk".
        /// </summary>
        [Mandatory]
        public String              Name         { get; }

        /// <summary>
        /// The email address of the point of contact.
        /// </summary>
        [Mandatory]
        public SimpleEMailAddress  EMail        { get; }

        /// <summary>
        /// The telephone number at which the point of contact can be reached.
        /// </summary>
        [Mandatory]
        public PhoneNumber         Telephone    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new point of contacts.
        /// </summary>
        /// <param name="Name">The name of the point of contact. This can be the name of a person, but it can also be the name of a department or team like "Corporate Clients Desk".</param>
        /// <param name="EMail">The email address of the point of contact.</param>
        /// <param name="Telephone">The telephone number at which the point of contact can be reached.</param>
        public PointOfContact(String              Name,
                              SimpleEMailAddress  EMail,
                              PhoneNumber         Telephone)
        {

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

            this.Name       = Name.Trim();
            this.EMail      = EMail;
            this.Telephone  = Telephone;

            unchecked
            {

                hashCode = this.Name.     GetHashCode() * 5 ^
                           this.EMail.    GetHashCode() * 3 ^
                           this.Telephone.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomPointOfContactParser = null)

        /// <summary>
        /// Parse the given JSON representation of point of contacts.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPointOfContactParser">A delegate to parse custom point of contacts.</param>
        public static PointOfContact Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<PointOfContact>?  CustomPointOfContactParser   = null)
        {

            if (TryParse(JSON,
                         out var pointOfContact,
                         out var errorResponse,
                         CustomPointOfContactParser))
            {
                return pointOfContact;
            }

            throw new ArgumentException("The given JSON representation of a point of contact is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PointOfContact, out ErrorResponse, CustomPointOfContactParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of point of contacts.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PointOfContact">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out PointOfContact?  PointOfContact,
                                       [NotNullWhen(false)] out String?          ErrorResponse)

            => TryParse(JSON,
                        out PointOfContact,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of point of contacts.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="PointOfContact">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPointOfContactParser">A delegate to parse custom point of contacts.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out PointOfContact?      PointOfContact,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       CustomJObjectParserDelegate<PointOfContact>?  CustomPointOfContactParser)
        {

            try
            {

                PointOfContact = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Name         [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "name",
                                             out String? Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EMail        [mandatory]

                if (!JSON.ParseMandatory("email",
                                         "email address",
                                         SimpleEMailAddress.TryParse,
                                         out SimpleEMailAddress EMail,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Telephone    [mandatory]

                if (!JSON.ParseMandatory("telephone",
                                         "telephone",
                                         PhoneNumber.TryParse,
                                         out PhoneNumber Telephone,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PointOfContact = new PointOfContact(
                                     Name,
                                     EMail,
                                     Telephone
                                 );


                if (CustomPointOfContactParser is not null)
                    PointOfContact = CustomPointOfContactParser(JSON,
                                                                PointOfContact);

                return true;

            }
            catch (Exception e)
            {
                PointOfContact  = default;
                ErrorResponse   = "The given JSON representation of point of contacts is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPointOfContactSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPointOfContactSerializer">A delegate to serialize custom point of contacts.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PointOfContact>?  CustomPointOfContactSerializer   = null)
        {

            var json = JSONObject.Create(

                           new JProperty("name",   Name),
                           new JProperty("email",  EMail.    ToString()),
                           new JProperty("logo",   Telephone.ToString())

                       );

            return CustomPointOfContactSerializer is not null
                       ? CustomPointOfContactSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public PointOfContact Clone()

            => new (
                   Name.     CloneString(),
                   EMail.    Clone(),
                   Telephone.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (PointOfContact1, PointOfContact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PointOfContact1">Point of contacts.</param>
        /// <param name="PointOfContact2">Other point of contacts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PointOfContact PointOfContact1,
                                           PointOfContact PointOfContact2)
        {

            if (Object.ReferenceEquals(PointOfContact1, PointOfContact2))
                return true;

            if (PointOfContact1 is null || PointOfContact2 is null)
                return false;

            return PointOfContact1.Equals(PointOfContact2);

        }

        #endregion

        #region Operator != (PointOfContact1, PointOfContact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PointOfContact1">Point of contacts.</param>
        /// <param name="PointOfContact2">Other point of contacts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PointOfContact PointOfContact1,
                                           PointOfContact PointOfContact2)

            => !(PointOfContact1 == PointOfContact2);

        #endregion

        #region Operator <  (PointOfContact1, PointOfContact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PointOfContact1">Point of contacts.</param>
        /// <param name="PointOfContact2">Other point of contacts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PointOfContact PointOfContact1,
                                          PointOfContact PointOfContact2)

            => PointOfContact1 is null
                   ? throw new ArgumentNullException(nameof(PointOfContact1), "The given point of contacts must not be null!")
                   : PointOfContact1.CompareTo(PointOfContact2) < 0;

        #endregion

        #region Operator <= (PointOfContact1, PointOfContact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PointOfContact1">Point of contacts.</param>
        /// <param name="PointOfContact2">Other point of contacts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PointOfContact PointOfContact1,
                                           PointOfContact PointOfContact2)

            => !(PointOfContact1 > PointOfContact2);

        #endregion

        #region Operator >  (PointOfContact1, PointOfContact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PointOfContact1">Point of contacts.</param>
        /// <param name="PointOfContact2">Other point of contacts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PointOfContact PointOfContact1,
                                          PointOfContact PointOfContact2)

            => PointOfContact1 is null
                   ? throw new ArgumentNullException(nameof(PointOfContact1), "The given point of contacts must not be null!")
                   : PointOfContact1.CompareTo(PointOfContact2) > 0;

        #endregion

        #region Operator >= (PointOfContact1, PointOfContact2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PointOfContact1">Point of contacts.</param>
        /// <param name="PointOfContact2">Other point of contacts.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PointOfContact PointOfContact1,
                                           PointOfContact PointOfContact2)

            => !(PointOfContact1 < PointOfContact2);

        #endregion

        #endregion

        #region IComparable<PointOfContact> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two point of contacts.
        /// </summary>
        /// <param name="Object">Point of contacts to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PointOfContact pointOfContact
                   ? CompareTo(pointOfContact)
                   : throw new ArgumentException("The given object is not a point of contact!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PointOfContact)

        /// <summary>
        /// Compares two point of contacts.
        /// </summary>
        /// <param name="PointOfContact">Point of contacts to compare with.</param>
        public Int32 CompareTo(PointOfContact? PointOfContact)
        {

            if (PointOfContact is null)
                throw new ArgumentNullException(nameof(PointOfContact), "The given point of contacts must not be null!");

            var c = Name.     CompareTo(PointOfContact.Name);

            if (c == 0)
                c = EMail.    CompareTo(PointOfContact.EMail);

            if (c == 0)
                c = Telephone.CompareTo(PointOfContact.Telephone);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PointOfContact> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two point of contacts for equality.
        /// </summary>
        /// <param name="Object">Point of contacts to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PointOfContact pointOfContact &&
                   Equals(pointOfContact);

        #endregion

        #region Equals(PointOfContact)

        /// <summary>
        /// Compares two point of contacts for equality.
        /// </summary>
        /// <param name="PointOfContact">Point of contacts to compare with.</param>
        public Boolean Equals(PointOfContact? PointOfContact)

            => PointOfContact is not null &&

               Name.     Equals(PointOfContact.Name)  &&
               EMail.    Equals(PointOfContact.EMail) &&
               Telephone.Equals(PointOfContact.Telephone);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Name} <{EMail}>, {Telephone}";

        #endregion

    }

}
