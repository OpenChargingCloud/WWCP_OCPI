﻿/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A HTTP PATCH result
    /// </summary>
    public readonly struct PatchResult<T> : IEquatable<PatchResult<T>>
    {

        #region Properties

        public Boolean  IsSuccess       { get; }

        public Boolean  IsFailed
            => !IsSuccess;

        public T        PatchedData     { get; }

        public String?  ErrorResponse   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HTTP Patch result.
        /// </summary>
        private PatchResult(Boolean  IsSuccess,
                            T        PatchedData,
                            String?  ErrorResponse   = null)
        {

            this.IsSuccess      = IsSuccess;
            this.PatchedData    = PatchedData;
            this.ErrorResponse  = ErrorResponse;

        }

        #endregion


        public static PatchResult<T> Success(T        PatchedData,
                                             String?  ErrorResponse   = null)

            => new (true,
                    PatchedData,
                    ErrorResponse);

        public static PatchResult<T> Failed(T        PatchedData,
                                            String?  ErrorResponse   = null)

            => new (false,
                    PatchedData,
                    ErrorResponse);


        #region Operator overloading

        #region Operator == (PatchResult1, PatchResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PatchResult1">A JSON PATCH result.</param>
        /// <param name="PatchResult2">Another JSON PATCH result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PatchResult<T> PatchResult1,
                                           PatchResult<T> PatchResult2)

            => PatchResult1.Equals(PatchResult2);

        #endregion

        #region Operator != (PatchResult1, PatchResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PatchResult1">A JSON PATCH result.</param>
        /// <param name="PatchResult2">Another JSON PATCH result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PatchResult<T> PatchResult1,
                                           PatchResult<T> PatchResult2)

            => !(PatchResult1 == PatchResult2);

        #endregion

        #endregion

        #region IEquatable<PatchResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two JSON PATCH results for equality.
        /// </summary>
        /// <param name="Object">A JSON PATCH result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PatchResult<T> patchResult &&
                   Equals(patchResult);

        #endregion

        #region Equals(PatchResult)

        /// <summary>
        /// Compares two JSON PATCH results for equality.
        /// </summary>
        /// <param name="PatchResult">A JSON PATCH result to compare with.</param>
        public Boolean Equals(PatchResult<T> PatchResult)

            => IsSuccess.Equals(PatchResult.IsSuccess) &&

             ((PatchedData   is     null && PatchResult.PatchedData   is     null)     ||
              (PatchedData   is not null && PatchResult.PatchedData   is not null && PatchedData.  Equals(PatchResult.PatchedData))) &&

             ((ErrorResponse is     null && PatchResult.ErrorResponse is     null) ||
              (ErrorResponse is not null && PatchResult.ErrorResponse is not null && ErrorResponse.Equals(PatchResult.ErrorResponse)));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return IsSuccess.     GetHashCode()       * 5 ^
                      (PatchedData?.  GetHashCode() ?? 0) * 3 ^
                      (ErrorResponse?.GetHashCode() ?? 0);

            }
        }

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
