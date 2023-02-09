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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Version detail information.
    /// </summary>
    public class VersionDetail : IEquatable<VersionDetail>,
                                 IComparable<VersionDetail>,
                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The unique version identification.
        /// </summary>
        [Mandatory]
        public Version_Id                    VersionId    { get; }

        /// <summary>
        /// The enumeration of endpoints for the given version
        /// </summary>
        [Mandatory]
        public IEnumerable<VersionEndpoint>  Endpoints    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new version detail information.
        /// </summary>
        /// <param name="VersionId">An unique version identification.</param>
        /// <param name="Endpoints">An enumeration of endpoints for the given version.</param>
        public VersionDetail(Version_Id                    VersionId,
                             IEnumerable<VersionEndpoint>  Endpoints)
        {

            if (!Endpoints.Any())
                throw new ArgumentNullException(nameof(Endpoints),  "The given version endpoints must not be null or empty!");

            this.VersionId  = VersionId;
            this.Endpoints  = Endpoints?.Distinct() ?? Array.Empty<VersionEndpoint>();

        }

        #endregion


        #region (static) Parse   (JSON, CustomVersionDetailParser = null)

        /// <summary>
        /// Parse the given JSON representation of a version detail.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomVersionDetailParser">A delegate to parse custom version details.</param>
        public static VersionDetail Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<VersionDetail>?  CustomVersionDetailParser   = null)
        {

            if (TryParse(JSON,
                         out var versionDetail,
                         out var errorResponse,
                         CustomVersionDetailParser))
            {
                return versionDetail!;
            }

            throw new ArgumentException("The given JSON representation of a version detail is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VersionDetail, out ErrorResponse, CustomVersionDetailParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a version detail.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="VersionDetail">The parsed version detail.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out VersionDetail?  VersionDetail,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out VersionDetail,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a version detail.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="VersionDetail">The parsed version detail.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVersionDetailParser">A delegate to parse custom version JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out VersionDetail?                           VersionDetail,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<VersionDetail>?  CustomVersionDetailParser)
        {

            try
            {

                VersionDetail = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse VersionId    [mandatory]

                if (!JSON.ParseMandatory("version",
                                         "version identification",
                                         Version_Id.TryParse,
                                         out Version_Id VersionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Endpoints    [mandatory]

                if (!JSON.ParseMandatoryHashSet("endpoints",
                                                "version endpoints",
                                                VersionEndpoint.TryParse,
                                                out HashSet<VersionEndpoint> Endpoints,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                VersionDetail = new VersionDetail(VersionId,
                                                  Endpoints);


                if (CustomVersionDetailParser is not null)
                    VersionDetail = CustomVersionDetailParser(JSON,
                                                              VersionDetail);

                return true;

            }
            catch (Exception e)
            {
                VersionDetail  = default;
                ErrorResponse  = "The given JSON representation of a version detail is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVersionDetailSerializer = null, CustomVersionEndpointSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVersionDetailSerializer">A delegate to serialize custom version detail JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VersionDetail>?    CustomVersionDetailSerializer     = null,
                              CustomJObjectSerializerDelegate<VersionEndpoint>?  CustomVersionEndpointSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("version",    VersionId.ToString()),
                           new JProperty("endpoints",  new JArray(Endpoints.Select(endpoint => endpoint.ToJSON(CustomVersionEndpointSerializer))))
                       );

            return CustomVersionDetailSerializer is not null
                       ? CustomVersionDetailSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VersionDetail1, VersionDetail2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionDetail1">A version detail.</param>
        /// <param name="VersionDetail2">Another version detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VersionDetail? VersionDetail1,
                                           VersionDetail? VersionDetail2)
        {

            if (Object.ReferenceEquals(VersionDetail1, VersionDetail2))
                return true;

            if (VersionDetail1 is null || VersionDetail2 is null)
                return false;

            return VersionDetail1.Equals(VersionDetail2);

        }

        #endregion

        #region Operator != (VersionDetail1, VersionDetail2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionDetail1">A version detail.</param>
        /// <param name="VersionDetail2">Another version detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VersionDetail? VersionDetail1,
                                           VersionDetail? VersionDetail2)

            => !(VersionDetail1 == VersionDetail2);

        #endregion

        #region Operator <  (VersionDetail1, VersionDetail2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionDetail1">A version detail.</param>
        /// <param name="VersionDetail2">Another version detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (VersionDetail? VersionDetail1,
                                          VersionDetail? VersionDetail2)

            => VersionDetail1 is null
                   ? throw new ArgumentNullException(nameof(VersionDetail1), "The given version detail must not be null!")
                   : VersionDetail1.CompareTo(VersionDetail2) < 0;

        #endregion

        #region Operator <= (VersionDetail1, VersionDetail2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionDetail1">A version detail.</param>
        /// <param name="VersionDetail2">Another version detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (VersionDetail? VersionDetail1,
                                           VersionDetail? VersionDetail2)

            => !(VersionDetail1 > VersionDetail2);

        #endregion

        #region Operator >  (VersionDetail1, VersionDetail2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionDetail1">A version detail.</param>
        /// <param name="VersionDetail2">Another version detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (VersionDetail? VersionDetail1,
                                          VersionDetail? VersionDetail2)

            => VersionDetail1 is null
                   ? throw new ArgumentNullException(nameof(VersionDetail1), "The given version detail must not be null!")
                   : VersionDetail1.CompareTo(VersionDetail2) > 0;

        #endregion

        #region Operator >= (VersionDetail1, VersionDetail2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VersionDetail1">A version detail.</param>
        /// <param name="VersionDetail2">Another version detail.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (VersionDetail? VersionDetail1,
                                           VersionDetail? VersionDetail2)

            => !(VersionDetail1 < VersionDetail2);

        #endregion

        #endregion

        #region IComparable<VersionDetail> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two version details.
        /// </summary>
        /// <param name="Object">A version detail to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is VersionDetail versionDetail
                   ? CompareTo(versionDetail)
                   : throw new ArgumentException("The given object is not a version detail!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(VersionDetail)

        /// <summary>
        /// Compares two version details.
        /// </summary>
        /// <param name="VersionDetail">A version detail to compare with.</param>
        public Int32 CompareTo(VersionDetail? VersionDetail)
        {

            if (VersionDetail is null)
                throw new ArgumentNullException(nameof(VersionDetail), "The given version detail must not be null!");

            var c = VersionId.        CompareTo(VersionDetail.VersionId);

            if (c == 0)
                c = Endpoints.Count().CompareTo(VersionDetail.Endpoints.Count());

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<VersionDetail> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two version details for equality.
        /// </summary>
        /// <param name="Object">A version detail to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VersionDetail versionDetail &&
                   Equals(versionDetail);

        #endregion

        #region Equals(VersionDetail)

        /// <summary>
        /// Compares two version details for equality.
        /// </summary>
        /// <param name="VersionDetail">A version detail to compare with.</param>
        public Boolean Equals(VersionDetail? VersionDetail)

            => VersionDetail is not null &&

               VersionId.Equals(VersionDetail.VersionId) &&

               Endpoints.Count().Equals(VersionDetail.Endpoints.Count()) &&
               Endpoints.All(versionEndpoint => VersionDetail.Endpoints.Contains(versionEndpoint));

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

                return VersionId.GetHashCode() * 3 ^
                       Endpoints.CalcHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(VersionId,
                             " -> ",
                             Endpoints.Select(endpoint => endpoint.Identifier.ToString()).AggregateWith(", "));

        #endregion

    }

}
