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
using System.Collections.Generic;
using System.Linq;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A signed value consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// </summary>
    public readonly struct SignedValue : IEquatable<SignedValue>
    {

        #region Properties

        /// <summary>
        /// Nature of the value, in other words, the event this value belongs to.
        /// </summary>
        [Mandatory]
        public SignedValueTypes  Nature         { get; }

        /// <summary>
        /// The unencoded string of data. The format of the content depends on the EncodingMethod field.
        /// </summary>
        [Mandatory]
        public String            PlainData      { get; }

        /// <summary>
        /// Blob of signed data, base64 encoded. The format of the content depends on the EncodingMethod field.
        /// </summary>
        [Mandatory]
        public String            SignedData     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A signed value consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        public SignedValue(SignedValueTypes  Nature,
                           String            PlainData,
                           String            SignedData)
        {

            #region Initial checks

            if (PlainData.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(PlainData),   "The given plain data must not be null or empty!");

            if (SignedData.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(SignedData),  "The given signed data must not be null or empty!");

            #endregion

            this.Nature      = Nature;
            this.PlainData   = PlainData;
            this.SignedData  = SignedData;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignedValue1, SignedValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValue1">A signed value.</param>
        /// <param name="SignedValue2">Another signed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignedValue SignedValue1,
                                           SignedValue SignedValue2)

            => SignedValue1.Equals(SignedValue2);

        #endregion

        #region Operator != (SignedValue1, SignedValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedValue1">A signed value.</param>
        /// <param name="SignedValue2">Another signed value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedValue SignedValue1,
                                           SignedValue SignedValue2)

            => !(SignedValue1 == SignedValue2);

        #endregion

        #endregion

        #region IEquatable<SignedValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is SignedValue signedValue &&
                   Equals(signedValue);

        #endregion

        #region Equals(SignedValue)

        /// <summary>
        /// Compares two signed values for equality.
        /// </summary>
        /// <param name="SignedValue">A signed value to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SignedValue SignedValue)

            => Nature.    Equals(SignedValue.Nature)    &&
               PlainData. Equals(SignedValue.PlainData) &&
               SignedData.Equals(SignedValue.SignedData);

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

                return Nature.    GetHashCode() * 5 ^
                       PlainData. GetHashCode() * 3 ^
                       SignedData.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Nature,
                             " -> ",
                             PlainData. SubstringMax(10),
                             SignedData.SubstringMax(10));

        #endregion

    }

}
