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
    /// A charging period consists of a start timestamp and a list of
    /// possible values that influence this period, for example:
    /// Amount of energy charged this period, maximum current during
    /// this period etc.
    /// </summary>
    public readonly struct SignedData : IEquatable<SignedData>
    {

        #region Properties

        /// <summary>
        /// The name of the encoding used in the SignedData field.
        /// This is the name given to the encoding by a company or group of companies.
        /// </summary>
        [Mandatory]
        public EncodingMethod            EncodingMethod             { get; }

        /// <summary>
        /// Version of the EncodingMethod (when applicable).
        /// </summary>
        [Optional]
        public Int32?                    EncodingMethodVersion      { get; }

        /// <summary>
        /// Public key used to sign the data, base64 encoded.
        /// </summary>
        [Optional]
        public PublicKey?                PublicKey                  { get; }

        /// <summary>
        /// Enumeration of signed values.
        /// </summary>
        [Mandatory]
        public IEnumerable<SignedValue>  SignedValues               { get; }

        /// <summary>
        /// URL that can be shown to an EV driver. This URL gives the EV driver the possibility
        /// to verify the signed data of a charging session.
        /// </summary>
        [Optional]
        public String                    URL                        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A charging period consists of a start timestamp and a
        /// list of possible values that influence this period.
        /// </summary>
        public SignedData(EncodingMethod            EncodingMethod,
                          IEnumerable<SignedValue>  SignedValues,
                          Int32?                    EncodingMethodVersion,
                          PublicKey?                PublicKey,
                          String                    URL)
        {

            #region Initial checks

            if (!SignedValues.SafeAny())
                throw new ArgumentNullException(nameof(SignedValues), "The given enumeration of signed values must not be null or empty!");

            #endregion

            this.EncodingMethod          = EncodingMethod;
            this.SignedValues            = SignedValues.Distinct();
            this.EncodingMethodVersion   = EncodingMethodVersion;
            this.PublicKey               = PublicKey;
            this.URL                     = URL;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignedData1, SignedData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedData1">A charging period.</param>
        /// <param name="SignedData2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignedData SignedData1,
                                           SignedData SignedData2)

            => SignedData1.Equals(SignedData2);

        #endregion

        #region Operator != (SignedData1, SignedData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignedData1">A charging period.</param>
        /// <param name="SignedData2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignedData SignedData1,
                                           SignedData SignedData2)

            => !(SignedData1 == SignedData2);

        #endregion

        #endregion

        #region IEquatable<SignedData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is SignedData signedData &&
                   Equals(signedData);

        #endregion

        #region Equals(SignedData)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="SignedData">A charging period to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SignedData SignedData)

            => EncodingMethod.Equals(SignedData.EncodingMethod)             &&
               SignedValues.Count().Equals(SignedData.SignedValues.Count()) &&
               SignedValues.All(dimension => SignedData.SignedValues.Contains(dimension));

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
                return EncodingMethod.GetHashCode() * 3 ^
                       SignedValues.Aggregate(0, (hashCode, value) => hashCode ^ value.GetHashCode());
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(EncodingMethod,
                             EncodingMethodVersion.HasValue
                                 ? " (v" + EncodingMethodVersion.Value + ")"
                                 : "",
                             " ", SignedValues.Count() + " signed values");

        #endregion

    }

}
