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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// A patch result.
    /// </summary>
    public readonly struct PatchResult<T> : IEquatable<PatchResult<T>>
    {

        #region Properties

        public Boolean  IsSuccess       { get; }

        public Boolean  IsFailed
            => !IsSuccess;

        public T?       PatchedData     { get; }

        public String?  ErrorResponse   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new patch result.
        /// </summary>
        private PatchResult(Boolean  IsSuccess,
                            T?       PatchedData,
                            String?  ErrorResponse   = null)
        {

            this.IsSuccess      = IsSuccess;
            this.PatchedData    = PatchedData;
            this.ErrorResponse  = ErrorResponse;

            unchecked
            {

                hashCode = this.IsSuccess.     GetHashCode() * 5       ^
                          (this.PatchedData?.  GetHashCode() * 3 ?? 0) ^
                          (this.ErrorResponse?.GetHashCode()     ?? 0);

            }

        }

        #endregion


        public Boolean IsSuccessAndDataNotNull([NotNullWhen(true)] out T? PatchedData)
        {

            PatchedData = IsSuccess
                              ? this.PatchedData
                              : default;

            return IsSuccess && PatchedData is not null;

        }


        #region (static) Success    (PatchedData, ErrorResponse = null)

        public static PatchResult<T> Success(T        PatchedData,
                                             String?  ErrorResponse = null)

            => new (true,
                    PatchedData,
                    ErrorResponse);

        #endregion

        #region (static) NoOperation(PatchedData, ErrorResponse = null)

        public static PatchResult<T> NoOperation(T        PatchedData,
                                                 String?  ErrorResponse = null)

            => new (true,
                    PatchedData,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (PatchedData, ErrorResponse)

        public static PatchResult<T> Failed(T?       PatchedData,
                                            String   ErrorResponse)

            => new (false,
                    PatchedData,
                    ErrorResponse);

        #endregion

        #region (static) Failed     (             ErrorResponse)

        public static PatchResult<T> Failed(String ErrorResponse)

            => new (false,
                    default,
                    ErrorResponse);

        #endregion


        #region Operator overloading

        #region Operator == (PatchResult1, PatchResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PatchResult1">A patch result.</param>
        /// <param name="PatchResult2">Another patch result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PatchResult<T> PatchResult1,
                                           PatchResult<T> PatchResult2)

            => PatchResult1.Equals(PatchResult2);

        #endregion

        #region Operator != (PatchResult1, PatchResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PatchResult1">A patch result.</param>
        /// <param name="PatchResult2">Another patch result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PatchResult<T> PatchResult1,
                                           PatchResult<T> PatchResult2)

            => !(PatchResult1 == PatchResult2);

        #endregion

        #endregion

        #region IEquatable<PatchResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two patch results for equality.
        /// </summary>
        /// <param name="Object">An add result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PatchResult<T> patchResult &&
                   Equals(patchResult);

        #endregion

        #region Equals(PatchResult)

        /// <summary>
        /// Compares two patch results for equality.
        /// </summary>
        /// <param name="PatchResult">An add result to compare with.</param>
        public Boolean Equals(PatchResult<T> PatchResult)

            => IsSuccess.Equals(PatchResult.IsSuccess) &&

             ((PatchedData   is     null && PatchResult.PatchedData   is     null)     ||
              (PatchedData   is not null && PatchResult.PatchedData   is not null && PatchedData.  Equals(PatchResult.PatchedData))) &&

             ((ErrorResponse is     null && PatchResult.ErrorResponse is     null) ||
              (ErrorResponse is not null && PatchResult.ErrorResponse is not null && ErrorResponse.Equals(PatchResult.ErrorResponse)));

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
