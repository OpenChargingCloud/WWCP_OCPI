﻿/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The unique identification of an OCPI request.
    /// </summary>
    public struct Request_Id : IId<Request_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        private static readonly Random random = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the request identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HTTP request identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the HTTP request identification.</param>
        private Request_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random  (Length = 30, IsLocal = false)

        /// <summary>
        /// Create a new random request identification.
        /// </summary>
        /// <param name="Length">The expected length of the request identification.</param>
        /// <param name="IsLocal">The request identification was generated locally and not received via network.</param>
        public static Request_Id Random(Byte      Length    = 30,
                                        Boolean?  IsLocal   = false)

            => new Request_Id((IsLocal == true ? "Local:" : "") +
                              random.RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a HTTP request identification.
        /// </summary>
        /// <param name="Text">A text representation of a request identification.</param>
        public static Request_Id Parse(String Text)
        {

            if (TryParse(Text, out Request_Id requestId))
                return requestId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a request identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a request identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a HTTP request identification.
        /// </summary>
        /// <param name="Text">A text representation of a request identification.</param>
        public static Request_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Request_Id requestId))
                return requestId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out RequestId)

        /// <summary>
        /// Try to parse the given text as a HTTP request identification.
        /// </summary>
        /// <param name="Text">A text representation of a request identification.</param>
        /// <param name="RequestId">The parsed request identification.</param>
        public static Boolean TryParse(String Text, out Request_Id RequestId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    RequestId = new Request_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            RequestId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this HTTP request identification.
        /// </summary>
        public Request_Id Clone

            => new Request_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A HTTP request identification.</param>
        /// <param name="RequestId2">Another HTTP request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => RequestId1.Equals(RequestId2);

        #endregion

        #region Operator != (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A HTTP request identification.</param>
        /// <param name="RequestId2">Another HTTP request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => !(RequestId1 == RequestId2);

        #endregion

        #region Operator <  (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A HTTP request identification.</param>
        /// <param name="RequestId2">Another HTTP request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Request_Id RequestId1,
                                          Request_Id RequestId2)

            => RequestId1.CompareTo(RequestId2) < 0;

        #endregion

        #region Operator <= (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A HTTP request identification.</param>
        /// <param name="RequestId2">Another HTTP request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => !(RequestId1 > RequestId2);

        #endregion

        #region Operator >  (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A HTTP request identification.</param>
        /// <param name="RequestId2">Another HTTP request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Request_Id RequestId1,
                                          Request_Id RequestId2)

            => RequestId1.CompareTo(RequestId2) > 0;

        #endregion

        #region Operator >= (RequestId1, RequestId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId1">A HTTP request identification.</param>
        /// <param name="RequestId2">Another HTTP request identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Request_Id RequestId1,
                                           Request_Id RequestId2)

            => !(RequestId1 < RequestId2);

        #endregion

        #endregion

        #region IComparable<RequestId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Request_Id requestId
                   ? CompareTo(requestId)
                   : throw new ArgumentException("The given object is not a request identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RequestId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RequestId">An object to compare with.</param>
        public Int32 CompareTo(Request_Id RequestId)

            => String.Compare(InternalId,
                              RequestId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<RequestId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Request_Id requestId &&
                   Equals(requestId);

        #endregion

        #region Equals(RequestId)

        /// <summary>
        /// Compares two HTTP request identifications for equality.
        /// </summary>
        /// <param name="RequestId">A HTTP request identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Request_Id RequestId)

            => String.Equals(InternalId,
                             RequestId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
