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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// An remove result
    /// </summary>
    public readonly struct RemoveResult<T> : IEquatable<RemoveResult<T?>>
    {

        #region Properties

        public Boolean   IsSuccess        { get; }

        public Boolean   IsFailed
            => !IsSuccess;

        public T?        Data             { get; }

        public String?   ErrorResponse    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remove result.
        /// </summary>
        private RemoveResult(Boolean   IsSuccess,
                             T?        Data,
                             String?   ErrorResponse)
        {

            this.IsSuccess      = IsSuccess;
            this.Data           = Data;
            this.ErrorResponse  = ErrorResponse;

            unchecked
            {

                hashCode = this.IsSuccess.     GetHashCode() * 5       ^
                          (this.Data?.         GetHashCode() * 3 ?? 0) ^
                          (this.ErrorResponse?.GetHashCode()     ?? 0);

            }

        }

        #endregion


        #region (static) Success    (Data, ErrorResponse = null)

        public static RemoveResult<T> Success(T        Data,
                                              String?  ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) NoOperation(Data,             ErrorResponse = null)

        public static RemoveResult<T> NoOperation(T        Data,
                                                  String?  ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (Data,             ErrorResponse)

        public static RemoveResult<T> Failed(T?       Data,
                                             String   ErrorResponse)

            => new (false,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (                  ErrorResponse)

        public static RemoveResult<T> Failed(String ErrorResponse)

            => new (false,
                    default,
                    ErrorResponse);

        #endregion


        #region Operator overloading

        #region Operator == (RemoveResult1, RemoveResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoveResult1">An remove result.</param>
        /// <param name="RemoveResult2">Another remove result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RemoveResult<T?> RemoveResult1,
                                           RemoveResult<T?> RemoveResult2)

            => RemoveResult1.Equals(RemoveResult2);

        #endregion

        #region Operator != (RemoveResult1, RemoveResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemoveResult1">An remove result.</param>
        /// <param name="RemoveResult2">Another remove result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RemoveResult<T?> RemoveResult1,
                                           RemoveResult<T?> RemoveResult2)

            => !RemoveResult1.Equals(RemoveResult2);

        #endregion

        #endregion

        #region IEquatable<RemoveResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is RemoveResult<T?> removeResult &&
                   Equals(removeResult);

        #endregion

        #region Equals(RemoveResult)

        /// <summary>
        /// Compares two JSON PATCH results for equality.
        /// </summary>
        /// <param name="RemoveResult">A JSON PATCH result to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(RemoveResult<T?> RemoveResult)

            => IsSuccess.Equals(RemoveResult.IsSuccess) &&

             ((Data          is null     && RemoveResult.Data          is null) ||
              (Data          is not null && RemoveResult.Data          is not null && Data.         Equals(RemoveResult.Data))) &&

             ((ErrorResponse is null     && RemoveResult.ErrorResponse is null) ||
              (ErrorResponse is not null && RemoveResult.ErrorResponse is not null && ErrorResponse.Equals(RemoveResult.ErrorResponse)));

        #endregion

        #endregion

        #region GetHashCode()

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
                       : ""

               );

        #endregion


    }

}
