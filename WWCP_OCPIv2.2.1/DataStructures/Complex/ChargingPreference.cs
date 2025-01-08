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

using System;
using System.Collections.Generic;
using System.Linq;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The charging preference of an EV driver.
    /// </summary>
    public readonly struct ChargingPreference : IEquatable<ChargingPreference>,
                                                IComparable<ChargingPreference>,
                                                IComparable
    {

        #region Properties

        /// <summary>
        /// Type of Smart Charging Profile selected by the driver.
        /// </summary>
        public ProfileTypes  ProfileType            { get; }

        /// <summary>
        /// Expected departure. The driver has given this timestamp as expected departure moment.
        /// It is only an estimation and not necessarily the timestamp of the actual departure.
        /// </summary>
        public DateTime?     Departure              { get; }

        /// <summary>
        /// Requested amount of energy in kWh.
        /// The EV driver wants to have this amount of energy charged.
        /// </summary>
        public Double?       EnergyNeed             { get; }

        /// <summary>
        /// The driver allows their EV to be discharged when needed, as long as the other
        /// preferences are met: EV is charged with the preferred energy (energy_need) until
        /// the preferred departure moment (departure_time).
        /// </summary>
        public Boolean       DischargeAllowed       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new charging preferences for an EV driver.
        /// </summary>
        /// <param name="ProfileType">Type of Smart Charging Profile selected by the driver.</param>
        /// <param name="Departure">Expected departure. The driver has given this timestamp as expected departure moment. It is only an estimation and not necessarily the timestamp of the actual departure.</param>
        /// <param name="EnergyNeed">Requested amount of energy in kWh. The EV driver wants to have this amount of energy charged.</param>
        /// <param name="DischargeAllowed">The driver allows their EV to be discharged when needed, as long as the other preferences are met.</param>
        public ChargingPreference(ProfileTypes  ProfileType,
                                  DateTime?     Departure,
                                  Double?       EnergyNeed,
                                  Boolean       DischargeAllowed)
        {

            this.ProfileType       = ProfileType;
            this.Departure         = Departure;
            this.EnergyNeed        = EnergyNeed;
            this.DischargeAllowed  = DischargeAllowed;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingPreference1, ChargingPreference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPreference1">A charging preference.</param>
        /// <param name="ChargingPreference2">Another charging preference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingPreference ChargingPreference1,
                                           ChargingPreference ChargingPreference2)

            => ChargingPreference1.Equals(ChargingPreference2);

        #endregion

        #region Operator != (ChargingPreference1, ChargingPreference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPreference1">A charging preference.</param>
        /// <param name="ChargingPreference2">Another charging preference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingPreference ChargingPreference1,
                                           ChargingPreference ChargingPreference2)

            => !(ChargingPreference1 == ChargingPreference2);

        #endregion

        #region Operator <  (ChargingPreference1, ChargingPreference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPreference1">A charging preference.</param>
        /// <param name="ChargingPreference2">Another charging preference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingPreference ChargingPreference1,
                                          ChargingPreference ChargingPreference2)

            => ChargingPreference1.CompareTo(ChargingPreference2) < 0;

        #endregion

        #region Operator <= (ChargingPreference1, ChargingPreference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPreference1">A charging preference.</param>
        /// <param name="ChargingPreference2">Another charging preference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingPreference ChargingPreference1,
                                           ChargingPreference ChargingPreference2)

            => !(ChargingPreference1 > ChargingPreference2);

        #endregion

        #region Operator >  (ChargingPreference1, ChargingPreference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPreference1">A charging preference.</param>
        /// <param name="ChargingPreference2">Another charging preference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingPreference ChargingPreference1,
                                          ChargingPreference ChargingPreference2)

            => ChargingPreference1.CompareTo(ChargingPreference2) > 0;

        #endregion

        #region Operator >= (ChargingPreference1, ChargingPreference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPreference1">A charging preference.</param>
        /// <param name="ChargingPreference2">Another charging preference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingPreference ChargingPreference1,
                                           ChargingPreference ChargingPreference2)

            => !(ChargingPreference1 < ChargingPreference2);

        #endregion

        #endregion

        #region IComparable<ChargingPreference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ChargingPreference chargingPreference
                   ? CompareTo(chargingPreference)
                   : throw new ArgumentException("The given object is not a charging preference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingPreference)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPreference">An object to compare with.</param>
        public Int32 CompareTo(ChargingPreference ChargingPreference)

            => ProfileType.CompareTo(ChargingPreference.ProfileType);

        #endregion

        #endregion

        #region IEquatable<ChargingPreference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ChargingPreference chargingPreference &&
                   Equals(chargingPreference);

        #endregion

        #region Equals(ChargingPreference)

        /// <summary>
        /// Compares two charging preferences for equality.
        /// </summary>
        /// <param name="ChargingPreference">A charging preference to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingPreference ChargingPreference)

            => ProfileType.Equals(ChargingPreference.ProfileType);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return ProfileType.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ProfileType,
                             Departure.HasValue
                                 ? ", " + Departure
                                 : "",
                             EnergyNeed.HasValue
                                 ? ", " + EnergyNeed + " kWh"
                                 : "",
                             DischargeAllowed
                                 ? ", discharge allowed"
                                 : "");

        #endregion

    }

}
