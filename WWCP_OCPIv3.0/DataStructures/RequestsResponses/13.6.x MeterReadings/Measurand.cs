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

using cloud.charging.open.protocols.WWCP.EVCertificates;
using org.GraphDefined.Vanaheimr.Illias;
using System.ComponentModel;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for measurands.
    /// </summary>
    public static class MeasurandExtensions
    {

        /// <summary>
        /// Indicates whether this measurand is null or empty.
        /// </summary>
        /// <param name="Measurand">A measurand.</param>
        public static Boolean IsNullOrEmpty(this Measurand? Measurand)
            => !Measurand.HasValue || Measurand.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this measurand is NOT null or empty.
        /// </summary>
        /// <param name="Measurand">A measurand.</param>
        public static Boolean IsNotNullOrEmpty(this Measurand? Measurand)
            => Measurand.HasValue && Measurand.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a measurand.
    /// </summary>
    public readonly struct Measurand : IId<Measurand>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this measurand is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this measurand is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the measurand.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new measurand based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a measurand.</param>
        private Measurand(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        public static Measurand Parse(String Text)
        {

            if (TryParse(Text, out var measurand))
                return measurand;

            throw new ArgumentException($"Invalid text representation of a measurand: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        public static Measurand? TryParse(String Text)
        {

            if (TryParse(Text, out var measurand))
                return measurand;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out Measurand)

        /// <summary>
        /// Try to parse the given text as a measurand.
        /// </summary>
        /// <param name="Text">A text representation of a measurand.</param>
        /// <param name="Measurand">The parsed measurand.</param>
        public static Boolean TryParse(String Text, out Measurand Measurand)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    Measurand = new Measurand(Text);
                    return true;
                }
                catch
                { }
            }

            Measurand = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this measurand.
        /// </summary>
        public Measurand Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Electric current, typically reported in Amperes.
        /// </summary>
        public static Measurand  CURRENT      { get; }
            = new ("CURRENT");

        /// <summary>
        /// The electric power currently being consumed, typically reported in Watts.
        /// </summary>
        public static Measurand  POWER        { get; }
            = new ("POWER");

        /// <summary>
        /// The frequency of the AC current alternation.
        /// </summary>
        public static Measurand  FREQUENCY    { get; }
            = new ("FREQUENCY");

        /// <summary>
        /// The state of charge, typically reported as a percentage.
        /// </summary>
        public static Measurand  SOC          { get; }
            = new ("SOC");

        #endregion


        #region Operator overloading

        #region Operator == (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Measurand Measurand1,
                                           Measurand Measurand2)

            => Measurand1.Equals(Measurand2);

        #endregion

        #region Operator != (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Measurand Measurand1,
                                           Measurand Measurand2)

            => !Measurand1.Equals(Measurand2);

        #endregion

        #region Operator <  (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Measurand Measurand1,
                                          Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) < 0;

        #endregion

        #region Operator <= (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Measurand Measurand1,
                                           Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) <= 0;

        #endregion

        #region Operator >  (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Measurand Measurand1,
                                          Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) > 0;

        #endregion

        #region Operator >= (Measurand1, Measurand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Measurand1">A measurand.</param>
        /// <param name="Measurand2">Another measurand.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Measurand Measurand1,
                                           Measurand Measurand2)

            => Measurand1.CompareTo(Measurand2) >= 0;

        #endregion

        #endregion

        #region IComparable<Measurand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two measurands.
        /// </summary>
        /// <param name="Object">A measurand to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Measurand measurand
                   ? CompareTo(measurand)
                   : throw new ArgumentException("The given object is not a measurand!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Measurand)

        /// <summary>
        /// Compares two measurands.
        /// </summary>
        /// <param name="Measurand">A measurand to compare with.</param>
        public Int32 CompareTo(Measurand Measurand)

            => String.Compare(InternalId,
                              Measurand.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Measurand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two measurands for equality.
        /// </summary>
        /// <param name="Object">A measurand to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Measurand measurand &&
                   Equals(measurand);

        #endregion

        #region Equals(Measurand)

        /// <summary>
        /// Compares two measurands for equality.
        /// </summary>
        /// <param name="Measurand">A measurand to compare with.</param>
        public Boolean Equals(Measurand Measurand)

            => String.Equals(InternalId,
                             Measurand.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

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
