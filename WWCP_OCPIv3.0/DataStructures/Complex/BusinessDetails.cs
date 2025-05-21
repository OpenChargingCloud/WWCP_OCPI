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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The business details.
    /// </summary>
    public class BusinessDetails : IEquatable<BusinessDetails>,
                                   IComparable<BusinessDetails>,
                                   IComparable
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of this object.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/contexts/OCPI/3.0/businessDetails");

        #endregion

        #region Properties

        /// <summary>
        /// The name of the company.
        /// </summary>
        [Mandatory]
        public String           Name                { get; }

        /// <summary>
        /// The optional URL of the company’s website.
        /// </summary>
        [Optional]
        public URL?             Website             { get; }

        /// <summary>
        /// The optional image link to the operator's logo.
        /// </summary>
        [Optional]
        public Image?           Logo                { get; }

        /// <summary>
        /// The optional contact point of the company for technical matters.
        /// </summary>
        [Optional]
        public PointOfContact?  TechnicalContact    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new business details.
        /// </summary>
        /// <param name="Name">The name of the company.</param>
        /// <param name="Website">An optional URL of the company’s website.</param>
        /// <param name="Logo">An optional image link to the operator's logo.</param>
        /// <param name="TechnicalContact">An optional contact point of the company for technical matters.</param>
        public BusinessDetails(String          Name,
                               URL?            Website            = null,
                               Image?          Logo               = null,
                               PointOfContact? TechnicalContact   = null)
        {

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

            this.Name              = Name.Trim();
            this.Website           = Website;
            this.Logo              = Logo;
            this.TechnicalContact  = TechnicalContact;

            unchecked
            {

                hashCode = this.Name.             GetHashCode()       * 7 ^
                          (this.Website?.         GetHashCode() ?? 0) * 5 ^
                          (this.Logo?.            GetHashCode() ?? 0) * 3 ^
                           this.TechnicalContact?.GetHashCode() ?? 0;

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomBusinessDetailsParser = null)

        /// <summary>
        /// Parse the given JSON representation of business details.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomBusinessDetailsParser">A delegate to parse custom business details.</param>
        public static BusinessDetails Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<BusinessDetails>?  CustomBusinessDetailsParser   = null)
        {

            if (TryParse(JSON,
                         out var businessDetails,
                         out var errorResponse,
                         CustomBusinessDetailsParser))
            {
                return businessDetails;
            }

            throw new ArgumentException("The given JSON representation of a business detail is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BusinessDetails, out ErrorResponse, CustomBusinessDetailsParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of business details.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BusinessDetails">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out BusinessDetails?  BusinessDetails,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out BusinessDetails,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of business details.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="BusinessDetails">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBusinessDetailsParser">A delegate to parse custom business details.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out BusinessDetails?      BusinessDetails,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<BusinessDetails>?  CustomBusinessDetailsParser)
        {

            try
            {

                BusinessDetails = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Name                [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "name",
                                             out String? Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Website             [optional]

                if (JSON.ParseOptional("website",
                                       "website",
                                       URL.TryParse,
                                       out URL? Website,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Logo                [optional]

                if (JSON.ParseOptionalJSON("logo",
                                           "logo",
                                           Image.TryParse,
                                           out Image? Logo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TechnicalContact    [optional]

                if (JSON.ParseOptionalJSON("technical_contact",
                                           "technical contact",
                                           PointOfContact.TryParse,
                                           out PointOfContact? TechnicalContact,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BusinessDetails = new BusinessDetails(
                                      Name,
                                      Website,
                                      Logo,
                                      TechnicalContact
                                  );


                if (CustomBusinessDetailsParser is not null)
                    BusinessDetails = CustomBusinessDetailsParser(JSON,
                                                                  BusinessDetails);

                return true;

            }
            catch (Exception e)
            {
                BusinessDetails  = default;
                ErrorResponse    = "The given JSON representation of business details is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBusinessDetailsSerializer = null, CustomImageSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom images.</param>
        /// <param name="CustomPointOfContactSerializer">A delegate to serialize custom point of contacts.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null,
                              CustomJObjectSerializerDelegate<PointOfContact>?   CustomPointOfContactSerializer    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("name",               Name),

                           Website.HasValue
                               ? new JProperty("website",            Website.         ToString())
                               : null,

                           Logo is not null
                               ? new JProperty("logo",               Logo.            ToJSON(CustomImageSerializer))
                               : null,

                           TechnicalContact is not null
                               ? new JProperty("technical_contact",  TechnicalContact.ToJSON())
                               : null

                       );

            return CustomBusinessDetailsSerializer is not null
                       ? CustomBusinessDetailsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public BusinessDetails Clone()

            => new (
                   Name.             CloneString(),
                   Website?.         Clone(),
                   Logo?.            Clone(),
                   TechnicalContact?.Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">Business details.</param>
        /// <param name="BusinessDetails2">Other business details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)
        {

            if (Object.ReferenceEquals(BusinessDetails1, BusinessDetails2))
                return true;

            if (BusinessDetails1 is null || BusinessDetails2 is null)
                return false;

            return BusinessDetails1.Equals(BusinessDetails2);

        }

        #endregion

        #region Operator != (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">Business details.</param>
        /// <param name="BusinessDetails2">Other business details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)

            => !(BusinessDetails1 == BusinessDetails2);

        #endregion

        #region Operator <  (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">Business details.</param>
        /// <param name="BusinessDetails2">Other business details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BusinessDetails BusinessDetails1,
                                          BusinessDetails BusinessDetails2)

            => BusinessDetails1 is null
                   ? throw new ArgumentNullException(nameof(BusinessDetails1), "The given business details must not be null!")
                   : BusinessDetails1.CompareTo(BusinessDetails2) < 0;

        #endregion

        #region Operator <= (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">Business details.</param>
        /// <param name="BusinessDetails2">Other business details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)

            => !(BusinessDetails1 > BusinessDetails2);

        #endregion

        #region Operator >  (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">Business details.</param>
        /// <param name="BusinessDetails2">Other business details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BusinessDetails BusinessDetails1,
                                          BusinessDetails BusinessDetails2)

            => BusinessDetails1 is null
                   ? throw new ArgumentNullException(nameof(BusinessDetails1), "The given business details must not be null!")
                   : BusinessDetails1.CompareTo(BusinessDetails2) > 0;

        #endregion

        #region Operator >= (BusinessDetails1, BusinessDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BusinessDetails1">Business details.</param>
        /// <param name="BusinessDetails2">Other business details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BusinessDetails BusinessDetails1,
                                           BusinessDetails BusinessDetails2)

            => !(BusinessDetails1 < BusinessDetails2);

        #endregion

        #endregion

        #region IComparable<BusinessDetails> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two business details.
        /// </summary>
        /// <param name="Object">Business details to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BusinessDetails businessDetails
                   ? CompareTo(businessDetails)
                   : throw new ArgumentException("The given object is not a business detail!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BusinessDetails)

        /// <summary>
        /// Compares two business details.
        /// </summary>
        /// <param name="BusinessDetails">Business details to compare with.</param>
        public Int32 CompareTo(BusinessDetails? BusinessDetails)
        {

            if (BusinessDetails is null)
                throw new ArgumentNullException(nameof(BusinessDetails), "The given business details must not be null!");

            var c = Name.CompareTo(BusinessDetails.Name);

            if (c == 0)
                c = Website.HasValue && BusinessDetails.Website.HasValue
                        ? Website.Value.CompareTo(BusinessDetails.Website.Value)
                        : Website.HasValue ? 1 : BusinessDetails.Website.HasValue ? -1 : 0;

            if (c == 0)
                c = Logo is not null && BusinessDetails.Logo is not null
                        ? Logo.CompareTo(BusinessDetails.Logo)
                        : Logo is not null ? 1 : BusinessDetails.Logo is not null ? -1 : 0;

            if (c == 0)
                c = TechnicalContact is not null && BusinessDetails.TechnicalContact is not null
                        ? TechnicalContact.CompareTo(BusinessDetails.TechnicalContact)
                        : TechnicalContact is not null ? 1 : BusinessDetails.TechnicalContact is not null ? -1 : 0;

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<BusinessDetails> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two business details for equality.
        /// </summary>
        /// <param name="Object">Business details to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BusinessDetails businessDetails &&
                   Equals(businessDetails);

        #endregion

        #region Equals(BusinessDetails)

        /// <summary>
        /// Compares two business details for equality.
        /// </summary>
        /// <param name="BusinessDetails">Business details to compare with.</param>
        public Boolean Equals(BusinessDetails? BusinessDetails)

             => BusinessDetails is not null &&

                Name.Equals(BusinessDetails.Name) &&

             ((!Website.         HasValue    && !BusinessDetails.Website.         HasValue) ||
               (Website.         HasValue    &&  BusinessDetails.Website.         HasValue    && Website.Value.   Equals(BusinessDetails.Website.Value))) &&

              ((Logo             is null     &&  BusinessDetails.Logo             is null) ||
               (Logo             is not null &&  BusinessDetails.Logo             is not null && Logo.            Equals(BusinessDetails.Logo)))          &&

              ((TechnicalContact is null     &&  BusinessDetails.TechnicalContact is null) ||
               (TechnicalContact is not null &&  BusinessDetails.TechnicalContact is not null && TechnicalContact.Equals(BusinessDetails.TechnicalContact)));

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

            => String.Concat(

                   Name,

                   Website.HasValue
                       ? "; " + Website
                       : "",

                   Logo is not null
                       ? "; " + Logo
                       : "",

                   TechnicalContact is not null
                       ? "; " + TechnicalContact
                       : ""

               );

        #endregion

    }

}
