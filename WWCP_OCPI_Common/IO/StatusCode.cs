/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for status codes.
    /// </summary>
    public static class StatusCodesExtensions
    {

        /// <summary>
        /// Indicates whether this status code is null or empty.
        /// </summary>
        /// <param name="StatusCodes">A status code.</param>
        public static Boolean IsNullOrEmpty(this StatusCode? StatusCodes)
            => !StatusCodes.HasValue || StatusCodes.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this status code is null or empty.
        /// </summary>
        /// <param name="StatusCodes">A status code.</param>
        public static Boolean IsNotNullOrEmpty(this StatusCode? StatusCodes)
            => StatusCodes.HasValue && StatusCodes.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An OCPI status code.
    /// </summary>
    public readonly struct StatusCode : IId,
                                        IEquatable<StatusCode>,
                                        IComparable<StatusCode>
    {

        #region Data

        /// <summary>
        /// The numeric value of the status code.
        /// </summary>
        public readonly Int32 Value;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => false;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => true;

        /// <summary>
        /// The length of the status code.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) Value.ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new status code based on the given number.
        /// </summary>
        /// <param name="Number">A numeric representation of a status code.</param>
        private StatusCode(Int32 Number)
        {
            this.Value = Number;
        }

        #endregion


        #region (static) Parse    (Number)

        /// <summary>
        /// Parse the given number as a status code.
        /// </summary>
        /// <param name="Number">A numeric representation of a status code.</param>
        public static StatusCode Parse(Int32 Number)

            => new (Number);

        #endregion

        #region (static) TryParse (Number)

        /// <summary>
        /// Try to parse the given number as a status code.
        /// </summary>
        /// <param name="Number">A numeric representation of a status code.</param>
        public static StatusCode? TryParse(Int32 Number)
        {

            if (TryParse(Number, out var statusCodes))
                return statusCodes;

            return null;

        }


        /// <summary>
        /// Try to parse the given number as a status code.
        /// </summary>
        /// <param name="Number">A numeric representation of a status code.</param>
        public static StatusCode? TryParse(Int32? Number)
        {

            if (TryParse(Number, out var statusCodes))
                return statusCodes;

            return null;

        }

        #endregion

        #region (static) TryParse (Number, out StatusCodes)

        /// <summary>
        /// Try to parse the given number as a status code.
        /// </summary>
        /// <param name="Number">A numeric representation of a status code.</param>
        /// <param name="StatusCodes">The parsed status code.</param>
        public static Boolean TryParse(Int32 Number, out StatusCode StatusCodes)
        {

            StatusCodes = new StatusCode(Number);

            return true;

        }


        /// <summary>
        /// Try to parse the given number as a status code.
        /// </summary>
        /// <param name="Number">A numeric representation of a status code.</param>
        /// <param name="StatusCodes">The parsed status code.</param>
        public static Boolean TryParse(Int32?                               Number,
                                       [NotNullWhen(true)] out StatusCode?  StatusCodes)
        {

            if (!Number.HasValue)
            {
                StatusCodes = null;
                return false;
            }

            StatusCodes = new StatusCode(Number.Value);
            return true;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this status code.
        /// </summary>
        public StatusCode Clone()

            => new (Value);

        #endregion



        /// <summary>
        /// Generic error (-1)
        /// </summary>
        public static StatusCode  GenericError                  { get; }
            = new StatusCode(-1);


        /// <summary>
        /// Success (1000)
        /// </summary>
        public static StatusCode  Success                       { get; }
            = new StatusCode(1000);

        // 19xx – Reserved for custom success codes (1900–1999) must not be used in the standard OCPI modules to avoid conflicts!




        /// <summary>
        /// Client errors (2xxx)
        /// 29xx is reserved for custom client errors and must not be used in the standard OCPI modules to avoid conflicts!
        /// </summary>
        public static class ClientErrors
        {

            /// <summary>
            /// Generic client error (2000)
            /// </summary>
            public static StatusCode  GenericClientError            { get; }
                = new StatusCode(2000);

            /// <summary>
            /// Invalid or missing parameters (2001), e.g. missing last_updated field in a PATCH request.
            /// </summary>
            public static StatusCode  InvalidOrMissingParameters    { get; }
                = new StatusCode(2001);

            /// <summary>
            /// Not enough information (2002), e.g. too little information in an authorization request.
            /// </summary>
            public static StatusCode  NotEnoughInformation          { get; }
                = new StatusCode(2002);

            /// <summary>
            /// Unknown location (2003), e.g. START_SESSION with an unknown location.
            /// </summary>
            public static StatusCode  UnknownLocation               { get; }
                = new StatusCode(2003);

            /// <summary>
            /// Unknown token (2004), e.g. real-time authorization of an unknown token.
            /// </summary>
            public static StatusCode  UnknownToken                  { get; }
                = new StatusCode(2004);



            /// <summary>
            /// Unknown tariff (2900)
            /// </summary>
            public static StatusCode  UnknownTariff                 { get; }
                = new StatusCode(2900);

            /// <summary>
            /// Unknown tariff (2901)
            /// </summary>
            public static StatusCode  UnknownSession                { get; }
                = new StatusCode(2901);

            /// <summary>
            /// Unknown tariff (2902)
            /// </summary>
            public static StatusCode  UnknownCDR                    { get; }
                = new StatusCode(2902);

            /// <summary>
            /// Unknown tariff (2903)
            /// </summary>
            public static StatusCode  UnknownTerminal               { get; }
                = new StatusCode(2903);

        }



        /// <summary>
        /// Server errors (3xxx)
        /// 39xx is reserved for custom server errors and must not be used in the standard OCPI modules to avoid conflicts!
        /// </summary>
        public static class ServerErrors
        {

            /// <summary>
            /// Generic server error (3000)
            /// </summary>
            public static StatusCode  GenericServerError            { get; }
                = new StatusCode(3000);

            /// <summary>
            /// Unable to use the clients API (3001), e.g. during credentials registration, if a GET request fails.
            /// </summary>
            public static StatusCode  UnableToUseTheClientsAPI      { get; }
                = new StatusCode(3001);

            /// <summary>
            /// Unsupported version (3002)
            /// </summary>
            public static StatusCode  UnsupportedVersion            { get; }
                = new StatusCode(3002);

            /// <summary>
            /// No matching endpoints (3003), e.g. during registration, if no common modules are available.
            /// </summary>
            public static StatusCode  NoMatchingEndpoints           { get; }
                = new StatusCode(3003);

        }



        /// <summary>
        /// Hub errors (3xxx)
        /// 49xx is reserved for custom server errors and must not be used in the standard OCPI modules to avoid conflicts!
        /// </summary>
        public static class HubErrors
        {

            /// <summary>
            /// Generic hub error (4000)
            /// </summary>
            public static StatusCode  GenericHubError               { get; }
                = new StatusCode(4000);

            /// <summary>
            /// Unknown receiver (4001), e.g. TO-Adress unknown.
            /// </summary>
            public static StatusCode  UnknownReceiver               { get; }
                = new StatusCode(4001);

            /// <summary>
            /// Timeout on forwarded request (4002)
            /// </summary>
            public static StatusCode  TimeoutOnForwardedRequest     { get; }
                = new StatusCode(4002);

            /// <summary>
            /// Connection problem (4003), e.g. receiver not connected.
            /// </summary>
            public static StatusCode  ConnectionProblem             { get; }
                = new StatusCode(4003);

        }



        #region Operator overloading

        #region Operator == (StatusCodes1, StatusCodes2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusCodes1">A status code.</param>
        /// <param name="StatusCodes2">Another status code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StatusCode StatusCodes1,
                                           StatusCode StatusCodes2)

            => StatusCodes1.Equals(StatusCodes2);

        #endregion

        #region Operator != (StatusCodes1, StatusCodes2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusCodes1">A status code.</param>
        /// <param name="StatusCodes2">Another status code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StatusCode StatusCodes1,
                                           StatusCode StatusCodes2)

            => !StatusCodes1.Equals(StatusCodes2);

        #endregion

        #region Operator <  (StatusCodes1, StatusCodes2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusCodes1">A status code.</param>
        /// <param name="StatusCodes2">Another status code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StatusCode StatusCodes1,
                                          StatusCode StatusCodes2)

            => StatusCodes1.CompareTo(StatusCodes2) < 0;

        #endregion

        #region Operator <= (StatusCodes1, StatusCodes2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusCodes1">A status code.</param>
        /// <param name="StatusCodes2">Another status code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StatusCode StatusCodes1,
                                           StatusCode StatusCodes2)

            => StatusCodes1.CompareTo(StatusCodes2) <= 0;

        #endregion

        #region Operator >  (StatusCodes1, StatusCodes2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusCodes1">A status code.</param>
        /// <param name="StatusCodes2">Another status code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StatusCode StatusCodes1,
                                          StatusCode StatusCodes2)

            => StatusCodes1.CompareTo(StatusCodes2) > 0;

        #endregion

        #region Operator >= (StatusCodes1, StatusCodes2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusCodes1">A status code.</param>
        /// <param name="StatusCodes2">Another status code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StatusCode StatusCodes1,
                                           StatusCode StatusCodes2)

            => StatusCodes1.CompareTo(StatusCodes2) >= 0;

        #endregion

        #endregion

        #region IComparable<StatusCodes> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two status codes.
        /// </summary>
        /// <param name="Object">A status code to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is StatusCode statusCodes
                   ? CompareTo(statusCodes)
                   : throw new ArgumentException("The given object is not a status code!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StatusCodes)

        /// <summary>
        /// Compares two status codes.
        /// </summary>
        /// <param name="StatusCodes">A status code to compare with.</param>
        public Int32 CompareTo(StatusCode StatusCodes)

            => Value.CompareTo(StatusCodes.Value);

        #endregion

        #endregion

        #region IEquatable<StatusCodes> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two status codes for equality.
        /// </summary>
        /// <param name="Object">A status code to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusCode statusCodes &&
                   Equals(statusCodes);

        #endregion

        #region Equals(StatusCodes)

        /// <summary>
        /// Compares two status codes for equality.
        /// </summary>
        /// <param name="StatusCodes">A status code to compare with.</param>
        public Boolean Equals(StatusCode StatusCodes)

            => Value.Equals(StatusCodes.Value);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Value.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Value.ToString();

        #endregion

    }

}
