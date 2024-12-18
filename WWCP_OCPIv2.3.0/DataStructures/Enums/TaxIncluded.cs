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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Extensions methods for TaxIncluded.
    /// </summary>
    public static class TaxIncludedExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a TaxIncluded.
        /// </summary>
        /// <param name="Text">A text representation of a TaxIncluded.</param>
        public static TaxIncluded Parse(String Text)
        {

            if (TryParse(Text, out var taxIncluded))
                return taxIncluded;

            return TaxIncluded.NA;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a TaxIncluded.
        /// </summary>
        /// <param name="Text">A text representation of a TaxIncluded.</param>
        public static TaxIncluded? TryParse(String Text)
        {

            if (TryParse(Text, out var taxIncluded))
                return taxIncluded;

            return null;

        }

        #endregion

        #region TryParse(Text, out TaxIncluded)

        /// <summary>
        /// Try to parse the given text as a TaxIncluded.
        /// </summary>
        /// <param name="Text">A text representation of a TaxIncluded.</param>
        /// <param name="TaxIncluded">The parsed TaxIncluded.</param>
        public static Boolean TryParse(String Text, out TaxIncluded TaxIncluded)
        {
            switch (Text.Trim().ToUpper())
            {

                case "YES":
                    TaxIncluded = TaxIncluded.Yes;
                    return true;

                case "NO":
                    TaxIncluded = TaxIncluded.No;
                    return true;

                default:
                    TaxIncluded = TaxIncluded.NA;
                    return false;

            }
        }

        #endregion

        #region AsText(this TaxIncluded)

        public static String AsText(this TaxIncluded TaxIncluded)

            => TaxIncluded switch {
                   TaxIncluded.Yes  => "YES",
                   TaxIncluded.No   => "NO",
                   _                => "N/A"
               };

        #endregion

    }


    /// <summary>
    /// Whether taxes are included within a tariff.
    /// </summary>
    public enum TaxIncluded
    {

        /// <summary>
        /// No taxes are applicable to this tariff.
        /// </summary>
        NA,

        /// <summary>
        /// Taxes are included in the prices in this tariff.
        /// </summary>
        Yes,

        /// <summary>
        /// Taxes are not included, and will be added on top of the prices in this tariff.
        /// </summary>
        No

    }

}
