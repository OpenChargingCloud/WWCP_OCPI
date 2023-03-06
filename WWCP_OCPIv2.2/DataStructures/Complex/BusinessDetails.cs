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
    /// The business details.
    /// </summary>
    public class BusinessDetails : IEquatable<BusinessDetails>,
                                   IComparable<BusinessDetails>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// Name of the operator.
        /// [max 100]
        /// </summary>
        [Mandatory]
        public String  Name       { get; }

        /// <summary>
        /// Optinal link to the operator's website.
        /// </summary>
        [Optional]
        public URL?    Website    { get; }

        /// <summary>
        /// Optinal image link to the operator's logo.
        /// </summary>
        [Optional]
        public Image?  Logo       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new business details.
        /// </summary>
        /// <param name="Name">Name of the operator.</param>
        /// <param name="Website">Optinal link to the operator's website.</param>
        /// <param name="Logo">Optinal image link to the operator's logo.</param>
        public BusinessDetails(String  Name,
                               URL?    Website   = null,
                               Image?  Logo      = null)
        {

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

            this.Name     = Name.Trim();
            this.Website  = Website;
            this.Logo     = Logo;

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
                return businessDetails!;
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
        public static Boolean TryParse(JObject               JSON,
                                       out BusinessDetails?  BusinessDetails,
                                       out String?           ErrorResponse)

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
                                       out BusinessDetails?                           BusinessDetails,
                                       out String?                                    ErrorResponse,
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

                #region Parse Name        [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "name",
                                             out String Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Website     [optional]

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

                #region Parse Logo        [optional]

                if (JSON.ParseOptionalJSON("logo",
                                           "logo",
                                           Image.TryParse,
                                           out Image Logo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BusinessDetails = new BusinessDetails(Name,
                                                      Website,
                                                      Logo);


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

        #region ToJSON(CustomBusinessDetailsSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BusinessDetails>?  CustomBusinessDetailsSerializer   = null,
                              CustomJObjectSerializerDelegate<Image>?            CustomImageSerializer             = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("name",            Name),

                           Website.HasValue
                               ? new JProperty("website",   Website.ToString())
                               : null,

                           Logo is not null
                               ? new JProperty("logo",      Logo.   ToJSON(CustomImageSerializer))
                               : null

                       );

            return CustomBusinessDetailsSerializer is not null
                       ? CustomBusinessDetailsSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public BusinessDetails Clone()

            => new (new String(Name.ToCharArray()),
                    Website.HasValue    ? Website.Value.Clone   : null,
                    Logo    is not null ? Logo.         Clone() : null);

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

            var c = Name.   CompareTo(BusinessDetails.Name);

            if (c == 0)
                c = Website.HasValue && BusinessDetails.Website.HasValue
                        ? Website.Value.CompareTo(BusinessDetails.Website.Value)
                        : 0;

            if (c == 0)
                c = Logo is not null && BusinessDetails.Logo is not null
                        ? Logo.CompareTo(BusinessDetails.Logo)
                        : 0;

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

            ((!Website.HasValue    && !BusinessDetails.Website.HasValue)    ||
              (Website.HasValue    &&  BusinessDetails.Website.HasValue    && Website.Value.Equals(BusinessDetails.Website.Value))) &&

             ((Logo    is     null &&  BusinessDetails.Logo    is     null) ||
              (Logo    is not null &&  BusinessDetails.Logo    is not null && Logo.         Equals(BusinessDetails.Logo)));

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

                return  Name.    GetHashCode()       * 5 ^
                       (Website?.GetHashCode() ?? 0) * 3 ^
                       (Logo?.   GetHashCode() ?? 0);

            }
        }

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
                       : ""

               );

        #endregion

    }

}
