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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for async result types.
    /// </summary>
    public static class AsyncResultTypeExtensions
    {

        /// <summary>
        /// Indicates whether this async result type is null or empty.
        /// </summary>
        /// <param name="AsyncResultType">A async result type.</param>
        public static Boolean IsNullOrEmpty(this AsyncResultType? AsyncResultType)
            => !AsyncResultType.HasValue || AsyncResultType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this async result type is NOT null or empty.
        /// </summary>
        /// <param name="AsyncResultType">A async result type.</param>
        public static Boolean IsNotNullOrEmpty(this AsyncResultType? AsyncResultType)
            => AsyncResultType.HasValue && AsyncResultType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A async result type.
    /// </summary>
    public readonly struct AsyncResultType : IId<AsyncResultType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this async result type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this async result type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the async result type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new async result type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a async result type.</param>
        private AsyncResultType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a async result type.
        /// </summary>
        /// <param name="Text">A text representation of a async result type.</param>
        public static AsyncResultType Parse(String Text)
        {

            if (TryParse(Text, out var immediateResponseToAsyncRequest))
                return immediateResponseToAsyncRequest;

            throw new ArgumentException($"Invalid text representation of a async result type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a async result type.
        /// </summary>
        /// <param name="Text">A text representation of a async result type.</param>
        public static AsyncResultType? TryParse(String Text)
        {

            if (TryParse(Text, out var immediateResponseToAsyncRequest))
                return immediateResponseToAsyncRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AsyncResultType)

        /// <summary>
        /// Try to parse the given text as a async result type.
        /// </summary>
        /// <param name="Text">A text representation of a async result type.</param>
        /// <param name="AsyncResultType">The parsed async result type.</param>
        public static Boolean TryParse(String Text, out AsyncResultType AsyncResultType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AsyncResultType = new AsyncResultType(Text);
                    return true;
                }
                catch
                { }
            }

            AsyncResultType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this async result type.
        /// </summary>
        public AsyncResultType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Remote procedure call executed successfully.
        /// </summary>
        public static AsyncResultType  SUCCESS          { get; }
            = new ("SUCCESS");

        /// <summary>
        /// Remote procedure call execution failed.
        /// </summary>
        public static AsyncResultType  FAILED           { get; }
            = new ("FAILED");

        /// <summary>
        /// The requested operation is not supported by the device that has to execute it.
        /// </summary>
        public static AsyncResultType  NOT_SUPPORTED    { get; }
            = new ("NOT_SUPPORTED");

        /// <summary>
        /// The requested operation was rejected by the device that has to execute it.
        /// </summary>
        public static AsyncResultType  REJECTED         { get; }
            = new ("REJECTED");

        /// <summary>
        /// No result was received from the device that had to execute the requested operation within a reasonable time.
        /// </summary>
        public static AsyncResultType  TIMEOUT          { get; }
            = new ("TIMEOUT");

        #endregion


        #region Operator overloading

        #region Operator == (AsyncResultType1, AsyncResultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResultType1">A async result type.</param>
        /// <param name="AsyncResultType2">Another async result type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AsyncResultType AsyncResultType1,
                                           AsyncResultType AsyncResultType2)

            => AsyncResultType1.Equals(AsyncResultType2);

        #endregion

        #region Operator != (AsyncResultType1, AsyncResultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResultType1">A async result type.</param>
        /// <param name="AsyncResultType2">Another async result type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AsyncResultType AsyncResultType1,
                                           AsyncResultType AsyncResultType2)

            => !AsyncResultType1.Equals(AsyncResultType2);

        #endregion

        #region Operator <  (AsyncResultType1, AsyncResultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResultType1">A async result type.</param>
        /// <param name="AsyncResultType2">Another async result type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AsyncResultType AsyncResultType1,
                                          AsyncResultType AsyncResultType2)

            => AsyncResultType1.CompareTo(AsyncResultType2) < 0;

        #endregion

        #region Operator <= (AsyncResultType1, AsyncResultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResultType1">A async result type.</param>
        /// <param name="AsyncResultType2">Another async result type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AsyncResultType AsyncResultType1,
                                           AsyncResultType AsyncResultType2)

            => AsyncResultType1.CompareTo(AsyncResultType2) <= 0;

        #endregion

        #region Operator >  (AsyncResultType1, AsyncResultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResultType1">A async result type.</param>
        /// <param name="AsyncResultType2">Another async result type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AsyncResultType AsyncResultType1,
                                          AsyncResultType AsyncResultType2)

            => AsyncResultType1.CompareTo(AsyncResultType2) > 0;

        #endregion

        #region Operator >= (AsyncResultType1, AsyncResultType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AsyncResultType1">A async result type.</param>
        /// <param name="AsyncResultType2">Another async result type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AsyncResultType AsyncResultType1,
                                           AsyncResultType AsyncResultType2)

            => AsyncResultType1.CompareTo(AsyncResultType2) >= 0;

        #endregion

        #endregion

        #region IComparable<AsyncResultType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two async result types.
        /// </summary>
        /// <param name="Object">A async result type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is AsyncResultType immediateResponseToAsyncRequest
                   ? CompareTo(immediateResponseToAsyncRequest)
                   : throw new ArgumentException("The given object is not a async result type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AsyncResultType)

        /// <summary>
        /// Compares two async result types.
        /// </summary>
        /// <param name="AsyncResultType">A async result type to compare with.</param>
        public Int32 CompareTo(AsyncResultType AsyncResultType)

            => String.Compare(InternalId,
                              AsyncResultType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AsyncResultType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two async result types for equality.
        /// </summary>
        /// <param name="Object">A async result type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AsyncResultType immediateResponseToAsyncRequest &&
                   Equals(immediateResponseToAsyncRequest);

        #endregion

        #region Equals(AsyncResultType)

        /// <summary>
        /// Compares two async result types for equality.
        /// </summary>
        /// <param name="AsyncResultType">A async result type to compare with.</param>
        public Boolean Equals(AsyncResultType AsyncResultType)

            => String.Equals(InternalId,
                             AsyncResultType.InternalId,
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
