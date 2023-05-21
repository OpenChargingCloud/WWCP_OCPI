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
    /// An update result
    /// </summary>
    public readonly struct UpdateResult<T> : IEquatable<UpdateResult<T?>>
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
        /// Create a new update result.
        /// </summary>
        private UpdateResult(Boolean   IsSuccess,
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

        public static UpdateResult<T> Success(T        Data,
                                              String?  ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) NoOperation(Data,             ErrorResponse = null)

        public static UpdateResult<T> NoOperation(T        Data,
                                                  String?  ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (Data,             ErrorResponse)

        public static UpdateResult<T> Failed(T?       Data,
                                             String   ErrorResponse)

            => new (false,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (                  ErrorResponse)

        public static UpdateResult<T> Failed(String ErrorResponse)

            => new (false,
                    default,
                    ErrorResponse);

        #endregion


        #region Operator overloading

        #region Operator == (UpdateResult1, UpdateResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UpdateResult1">An update result.</param>
        /// <param name="UpdateResult2">Another update result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UpdateResult<T?> UpdateResult1,
                                           UpdateResult<T?> UpdateResult2)

            => UpdateResult1.Equals(UpdateResult2);

        #endregion

        #region Operator != (UpdateResult1, UpdateResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UpdateResult1">An update result.</param>
        /// <param name="UpdateResult2">Another update result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UpdateResult<T?> UpdateResult1,
                                           UpdateResult<T?> UpdateResult2)

            => !UpdateResult1.Equals(UpdateResult2);

        #endregion

        #endregion

        #region IEquatable<UpdateResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is UpdateResult<T?> updateResult &&
                   Equals(updateResult);

        #endregion

        #region Equals(UpdateResult)

        /// <summary>
        /// Compares two JSON PATCH results for equality.
        /// </summary>
        /// <param name="UpdateResult">A JSON PATCH result to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(UpdateResult<T?> UpdateResult)

            => IsSuccess.Equals(UpdateResult.IsSuccess) &&

             ((Data          is null     && UpdateResult.Data          is null) ||
              (Data          is not null && UpdateResult.Data          is not null && Data.         Equals(UpdateResult.Data))) &&

             ((ErrorResponse is null     && UpdateResult.ErrorResponse is null) ||
              (ErrorResponse is not null && UpdateResult.ErrorResponse is not null && ErrorResponse.Equals(UpdateResult.ErrorResponse)));

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
