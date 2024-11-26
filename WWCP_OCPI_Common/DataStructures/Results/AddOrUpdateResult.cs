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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// An add or update result
    /// </summary>
    public readonly struct AddOrUpdateResult<T> : IEquatable<AddOrUpdateResult<T?>>
    {

        #region Properties

        public Boolean   IsSuccess        { get; }

        public Boolean   IsFailed
            => !IsSuccess;

        public Boolean?  WasCreated       { get; }

        public Boolean?  WasUpdated
            => !WasCreated;

        public T?        Data             { get; }

        public String?   ErrorResponse    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new add or update result.
        /// </summary>
        private AddOrUpdateResult(Boolean   IsSuccess,
                                  T?        Data,
                                  Boolean?  WasCreated,
                                  String?   ErrorResponse)
        {

            this.IsSuccess      = IsSuccess;
            this.Data           = Data;
            this.WasCreated     = WasCreated;
            this.ErrorResponse  = ErrorResponse;

            unchecked
            {

                hashCode = this.IsSuccess.     GetHashCode() * 7       ^
                          (this.Data?.         GetHashCode() * 5 ?? 0) ^
                          (this.WasCreated?.   GetHashCode() * 3 ?? 0) ^
                          (this.ErrorResponse?.GetHashCode()     ?? 0);

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


        #region (static) Success    (Data, WasCreated, ErrorResponse = null)

        public static AddOrUpdateResult<T> Success(T        Data,
                                                   Boolean  WasCreated,
                                                   String?  ErrorResponse = null)

            => new (true,
                    Data,
                    WasCreated,
                    ErrorResponse);

        #endregion

        #region (static) NoOperation(Data,             ErrorResponse = null)

        public static AddOrUpdateResult<T> NoOperation(T        Data,
                                                       String?  ErrorResponse = null)

            => new (true,
                    Data,
                    false,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (Data,             ErrorResponse)

        public static AddOrUpdateResult<T> Failed(T?       Data,
                                                  String   ErrorResponse)

            => new (false,
                    Data,
                    null,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (                  ErrorResponse)

        public static AddOrUpdateResult<T> Failed(String ErrorResponse)

            => new (false,
                    default,
                    null,
                    ErrorResponse);

        #endregion


        #region Operator overloading

        #region Operator == (AddOrUpdateResult1, AddOrUpdateResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AddOrUpdateResult1">An add or update result.</param>
        /// <param name="AddOrUpdateResult2">Another add or update result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AddOrUpdateResult<T?> AddOrUpdateResult1,
                                           AddOrUpdateResult<T?> AddOrUpdateResult2)

            => AddOrUpdateResult1.Equals(AddOrUpdateResult2);

        #endregion

        #region Operator != (AddOrUpdateResult1, AddOrUpdateResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AddOrUpdateResult1">An add or update result.</param>
        /// <param name="AddOrUpdateResult2">Another add or update result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AddOrUpdateResult<T?> AddOrUpdateResult1,
                                           AddOrUpdateResult<T?> AddOrUpdateResult2)

            => !AddOrUpdateResult1.Equals(AddOrUpdateResult2);

        #endregion

        #endregion

        #region IEquatable<AddOrUpdateResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two add or update results for equality.
        /// </summary>
        /// <param name="Object">An add or update result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddOrUpdateResult<T?> addOrUpdateResult &&
                   Equals(addOrUpdateResult);

        #endregion

        #region Equals(AddOrUpdateResult)

        /// <summary>
        /// Compares two add or update results for equality.
        /// </summary>
        /// <param name="AddOrUpdateResult">An add or update result to compare with.</param>
        public Boolean Equals(AddOrUpdateResult<T?> AddOrUpdateResult)

            => IsSuccess.Equals(AddOrUpdateResult.IsSuccess) &&

             ((Data          is null     &&  AddOrUpdateResult.Data          is null) ||
              (Data          is not null &&  AddOrUpdateResult.Data          is not null && Data.            Equals(AddOrUpdateResult.Data)))             &&

            ((!WasCreated.HasValue       && !AddOrUpdateResult.WasCreated.HasValue) ||
              (WasCreated.HasValue       &&  AddOrUpdateResult.WasCreated.HasValue       && WasCreated.Value.Equals(AddOrUpdateResult.WasCreated.Value))) &&

             ((ErrorResponse is null     &&  AddOrUpdateResult.ErrorResponse is null) ||
              (ErrorResponse is not null &&  AddOrUpdateResult.ErrorResponse is not null && ErrorResponse.   Equals(AddOrUpdateResult.ErrorResponse)));

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

                   WasCreated.HasValue
                       ? WasCreated.Value
                             ? ", created"
                             : ", updated"
                       : "",

                   ErrorResponse.IsNotNullOrEmpty()
                       ? ": " + ErrorResponse
                       : ""

               );

        #endregion


    }

}
