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

using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for immediate response to async requests.
    /// </summary>
    public static class ImmediateResponseToAsyncRequestExtensions
    {

        /// <summary>
        /// Indicates whether this immediate response to async request is null or empty.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest">A immediate response to async request.</param>
        public static Boolean IsNullOrEmpty(this ImmediateResponseToAsyncRequest? ImmediateResponseToAsyncRequest)
            => !ImmediateResponseToAsyncRequest.HasValue || ImmediateResponseToAsyncRequest.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this immediate response to async request is NOT null or empty.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest">A immediate response to async request.</param>
        public static Boolean IsNotNullOrEmpty(this ImmediateResponseToAsyncRequest? ImmediateResponseToAsyncRequest)
            => ImmediateResponseToAsyncRequest.HasValue && ImmediateResponseToAsyncRequest.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A immediate response to async request.
    /// </summary>
    public readonly struct ImmediateResponseToAsyncRequest : IId<ImmediateResponseToAsyncRequest>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this immediate response to async request is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this immediate response to async request is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the immediate response to async request.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new immediate response to async request based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a immediate response to async request.</param>
        private ImmediateResponseToAsyncRequest(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a immediate response to async request.
        /// </summary>
        /// <param name="Text">A text representation of a immediate response to async request.</param>
        public static ImmediateResponseToAsyncRequest Parse(String Text)
        {

            if (TryParse(Text, out var immediateResponseToAsyncRequest))
                return immediateResponseToAsyncRequest;

            throw new ArgumentException($"Invalid text representation of a immediate response to async request: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a immediate response to async request.
        /// </summary>
        /// <param name="Text">A text representation of a immediate response to async request.</param>
        public static ImmediateResponseToAsyncRequest? TryParse(String Text)
        {

            if (TryParse(Text, out var immediateResponseToAsyncRequest))
                return immediateResponseToAsyncRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ImmediateResponseToAsyncRequest)

        /// <summary>
        /// Try to parse the given text as a immediate response to async request.
        /// </summary>
        /// <param name="Text">A text representation of a immediate response to async request.</param>
        /// <param name="ImmediateResponseToAsyncRequest">The parsed immediate response to async request.</param>
        public static Boolean TryParse(String Text, out ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ImmediateResponseToAsyncRequest = new ImmediateResponseToAsyncRequest(Text);
                    return true;
                }
                catch
                { }
            }

            ImmediateResponseToAsyncRequest = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this immediate response to async request.
        /// </summary>
        public ImmediateResponseToAsyncRequest Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Remote procedure call accepted by the receiving party.
        /// </summary>
        public static ImmediateResponseToAsyncRequest  ACCEPTED         { get; }
            = new ("ACCEPTED");

        /// <summary>
        /// This remote procedure call is not supported by the receiving party
        /// or by the charging infrastructure device that has to execute it.
        /// </summary>
        public static ImmediateResponseToAsyncRequest  NOT_SUPPORTED    { get; }
            = new ("NOT_SUPPORTED");

        /// <summary>
        /// Remote procedure call was rejected by the receiving party.
        /// </summary>
        public static ImmediateResponseToAsyncRequest  REJECTED         { get; }
            = new ("REJECTED");

        #endregion


        #region Operator overloading

        #region Operator == (ImmediateResponseToAsyncRequest1, ImmediateResponseToAsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest1">A immediate response to async request.</param>
        /// <param name="ImmediateResponseToAsyncRequest2">Another immediate response to async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest1,
                                           ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest2)

            => ImmediateResponseToAsyncRequest1.Equals(ImmediateResponseToAsyncRequest2);

        #endregion

        #region Operator != (ImmediateResponseToAsyncRequest1, ImmediateResponseToAsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest1">A immediate response to async request.</param>
        /// <param name="ImmediateResponseToAsyncRequest2">Another immediate response to async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest1,
                                           ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest2)

            => !ImmediateResponseToAsyncRequest1.Equals(ImmediateResponseToAsyncRequest2);

        #endregion

        #region Operator <  (ImmediateResponseToAsyncRequest1, ImmediateResponseToAsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest1">A immediate response to async request.</param>
        /// <param name="ImmediateResponseToAsyncRequest2">Another immediate response to async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest1,
                                          ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest2)

            => ImmediateResponseToAsyncRequest1.CompareTo(ImmediateResponseToAsyncRequest2) < 0;

        #endregion

        #region Operator <= (ImmediateResponseToAsyncRequest1, ImmediateResponseToAsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest1">A immediate response to async request.</param>
        /// <param name="ImmediateResponseToAsyncRequest2">Another immediate response to async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest1,
                                           ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest2)

            => ImmediateResponseToAsyncRequest1.CompareTo(ImmediateResponseToAsyncRequest2) <= 0;

        #endregion

        #region Operator >  (ImmediateResponseToAsyncRequest1, ImmediateResponseToAsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest1">A immediate response to async request.</param>
        /// <param name="ImmediateResponseToAsyncRequest2">Another immediate response to async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest1,
                                          ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest2)

            => ImmediateResponseToAsyncRequest1.CompareTo(ImmediateResponseToAsyncRequest2) > 0;

        #endregion

        #region Operator >= (ImmediateResponseToAsyncRequest1, ImmediateResponseToAsyncRequest2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest1">A immediate response to async request.</param>
        /// <param name="ImmediateResponseToAsyncRequest2">Another immediate response to async request.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest1,
                                           ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest2)

            => ImmediateResponseToAsyncRequest1.CompareTo(ImmediateResponseToAsyncRequest2) >= 0;

        #endregion

        #endregion

        #region IComparable<ImmediateResponseToAsyncRequest> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two immediate response to async requests.
        /// </summary>
        /// <param name="Object">A immediate response to async request to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ImmediateResponseToAsyncRequest immediateResponseToAsyncRequest
                   ? CompareTo(immediateResponseToAsyncRequest)
                   : throw new ArgumentException("The given object is not a immediate response to async request!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ImmediateResponseToAsyncRequest)

        /// <summary>
        /// Compares two immediate response to async requests.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest">A immediate response to async request to compare with.</param>
        public Int32 CompareTo(ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest)

            => String.Compare(InternalId,
                              ImmediateResponseToAsyncRequest.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ImmediateResponseToAsyncRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two immediate response to async requests for equality.
        /// </summary>
        /// <param name="Object">A immediate response to async request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ImmediateResponseToAsyncRequest immediateResponseToAsyncRequest &&
                   Equals(immediateResponseToAsyncRequest);

        #endregion

        #region Equals(ImmediateResponseToAsyncRequest)

        /// <summary>
        /// Compares two immediate response to async requests for equality.
        /// </summary>
        /// <param name="ImmediateResponseToAsyncRequest">A immediate response to async request to compare with.</param>
        public Boolean Equals(ImmediateResponseToAsyncRequest ImmediateResponseToAsyncRequest)

            => String.Equals(InternalId,
                             ImmediateResponseToAsyncRequest.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToUpper().GetHashCode() ?? 0;

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
