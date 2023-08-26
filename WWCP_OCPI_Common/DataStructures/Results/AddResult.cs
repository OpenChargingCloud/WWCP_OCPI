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
    /// An add result
    /// </summary>
    public readonly struct AddResult<T> : IEquatable<AddResult<T?>>
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
        /// Create a new add result.
        /// </summary>
        private AddResult(Boolean  IsSuccess,
                          T?       Data,
                          String?  ErrorResponse)
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

        public static AddResult<T> Success(T        Data,
                                           String?  ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) NoOperation(Data, ErrorResponse = null)

        public static AddResult<T> NoOperation(T        Data,
                                               String?  ErrorResponse = null)

            => new (true,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (Data, ErrorResponse)

        public static AddResult<T> Failed(T?       Data,
                                          String   ErrorResponse)

            => new (false,
                    Data,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (      ErrorResponse)

        public static AddResult<T> Failed(String ErrorResponse)

            => new (false,
                    default,
                    ErrorResponse);

        #endregion


        #region Operator overloading

        #region Operator == (AddResult1, AddResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AddResult1">An add result.</param>
        /// <param name="AddResult2">Another add result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AddResult<T?> AddResult1,
                                           AddResult<T?> AddResult2)

            => AddResult1.Equals(AddResult2);

        #endregion

        #region Operator != (AddResult1, AddResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AddResult1">An add result.</param>
        /// <param name="AddResult2">Another add result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AddResult<T?> AddResult1,
                                           AddResult<T?> AddResult2)

            => !AddResult1.Equals(AddResult2);

        #endregion

        #endregion

        #region IEquatable<AddResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two add results for equality.
        /// </summary>
        /// <param name="Object">An add result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddResult<T?> addResult &&
                   Equals(addResult);

        #endregion

        #region Equals(AddResult)

        /// <summary>
        /// Compares two add results for equality.
        /// </summary>
        /// <param name="AddResult">An add result to compare with.</param>
        public Boolean Equals(AddResult<T?> AddResult)

            => IsSuccess.Equals(AddResult.IsSuccess) &&

             ((Data          is null     && AddResult.Data          is null) ||
              (Data          is not null && AddResult.Data          is not null && Data.         Equals(AddResult.Data))) &&

             ((ErrorResponse is null     && AddResult.ErrorResponse is null) ||
              (ErrorResponse is not null && AddResult.ErrorResponse is not null && ErrorResponse.Equals(AddResult.ErrorResponse)));

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
                       : ""

               );

        #endregion


    }

}
