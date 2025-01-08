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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// A remove result
    /// </summary>
    public readonly struct RemoveResult<T> : IEquatable<RemoveResult<T>>
        //where T : IEquatable<T>
    {

        #region Properties

        public Boolean           IsSuccess          { get; }

        public Boolean           IsFailed
            => !IsSuccess;

        public T?                Data               { get; }

        public String?           ErrorResponse      { get; }

        public EventTracking_Id  EventTrackingId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remove result.
        /// </summary>
        /// <param name="IsSuccess">Whether the operation was successful or not.</param>
        /// <param name="Data">The data of the operation.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        private RemoveResult(Boolean           IsSuccess,
                             T?                Data,
                             String?           ErrorResponse,
                             EventTracking_Id  EventTrackingId)
        {

            this.IsSuccess        = IsSuccess;
            this.Data             = Data;
            this.ErrorResponse    = ErrorResponse;
            this.EventTrackingId  = EventTrackingId;

            unchecked
            {

                hashCode = this.IsSuccess.      GetHashCode()       * 7 ^
                          (this.Data?.          GetHashCode() ?? 0) * 5 ^
                          (this.ErrorResponse?. GetHashCode() ?? 0) * 3 ^
                           this.EventTrackingId.GetHashCode();

            }

        }

        #endregion


        public Boolean IsSuccessAndDataNotNull([NotNullWhen(true)] out T? Data)
        {

            Data = IsSuccess
                       ? this.Data
                       : default;

            return IsSuccess && Data is not null;

        }


        #region (static) Success     (EventTrackingId, Data, ErrorResponse = null)

        public static RemoveResult<T> Success(EventTracking_Id  EventTrackingId,
                                              T                 Data,
                                              String?           ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse,
                    EventTrackingId);

        #endregion

        #region (static) NoOperation (EventTrackingId, Data, ErrorResponse = null)

        public static RemoveResult<T> NoOperation(EventTracking_Id  EventTrackingId,
                                                  T                 Data,
                                                  String?           ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse,
                    EventTrackingId);

        #endregion

        #region (static) Failed      (EventTrackingId, Data, ErrorResponse)

        public static RemoveResult<T> Failed(EventTracking_Id  EventTrackingId,
                                             T?                Data,
                                             String            ErrorResponse)

            => new (false,
                    Data,
                    ErrorResponse,
                    EventTrackingId);

        #endregion

        #region (static) Failed      (EventTrackingId,       ErrorResponse)

        public static RemoveResult<T> Failed(EventTracking_Id  EventTrackingId,
                                             String            ErrorResponse)

            => new (false,
                    default,
                    ErrorResponse,
                    EventTrackingId);

        #endregion


        #region Operator overloading

        #region Operator == (RemoveResult1, RemoveResult2)

        /// <summary>
        /// Compares two remove results for equality.
        /// </summary>
        /// <param name="RemoveResult1">A remove result.</param>
        /// <param name="RemoveResult2">Another remove result.</param>
        public static Boolean operator == (RemoveResult<T> RemoveResult1,
                                           RemoveResult<T> RemoveResult2)

            => RemoveResult1.Equals(RemoveResult2);

        #endregion

        #region Operator != (RemoveResult1, RemoveResult2)

        /// <summary>
        /// Compares two add results for inequality.
        /// </summary>
        /// <param name="RemoveResult1">A remove result.</param>
        /// <param name="RemoveResult2">Another remove result.</param>
        public static Boolean operator != (RemoveResult<T> RemoveResult1,
                                           RemoveResult<T> RemoveResult2)

            => !RemoveResult1.Equals(RemoveResult2);

        #endregion

        #endregion

        #region IEquatable<RemoveResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remove results for equality.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoveResult<T> removeResult &&
                   Equals(removeResult);

        #endregion

        #region Equals(RemoveResult)

        /// <summary>
        /// Compares two remove results for equality.
        /// </summary>
        /// <param name="RemoveResult">A remove result to compare with.</param>
        public Boolean Equals(RemoveResult<T> RemoveResult)

            => IsSuccess.      Equals(RemoveResult.IsSuccess)       &&
               EventTrackingId.Equals(RemoveResult.EventTrackingId) &&

             ((Data          is null                 && RemoveResult.Data          is null) ||
              (Data          is IEnumerable<T> dataT && RemoveResult.Data          is IEnumerable<T> removeDataT && dataT.SequenceEqual (removeDataT)) ||
              (Data          is not null             && RemoveResult.Data          is not null                   && Data.         Equals(RemoveResult.Data))) &&

             ((ErrorResponse is null                 && RemoveResult.ErrorResponse is null) ||
              (ErrorResponse is not null             && RemoveResult.ErrorResponse is not null                   && ErrorResponse.Equals(RemoveResult.ErrorResponse)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   IsSuccess
                       ? "success"
                       : "failed",

                   ErrorResponse.IsNotNullOrEmpty()
                       ? ": " + ErrorResponse
                       : "",

                   $", {EventTrackingId}"

               );

        #endregion


    }

}
