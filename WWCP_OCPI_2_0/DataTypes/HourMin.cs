/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0
{

    /// <summary>
    /// This class references business details of EVSE operators.
    /// </summary>
    public struct HourMin : IEquatable<HourMin>, IComparable<HourMin>, IComparable
    {

        #region Properties

        #region Hour

        private readonly UInt16 _Hour;

        /// <summary>
        /// The hour.
        /// </summary>
        public UInt16 Hour
        {
            get
            {
                return _Hour;
            }
        }

        #endregion

        #region Minute

        private readonly UInt16 _Minute;

        /// <summary>
        /// The hour.
        /// </summary>
        public UInt16 Minute
        {
            get
            {
                return _Minute;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new hour/minute.
        /// </summary>
        /// <param name="Hour">The hour.</param>
        /// <param name="Minute">The minute.</param>
        public HourMin(UInt16  Hour,
                       UInt16  Minute)
        {

            #region Initial checks

            if (Hour > 23)
                throw new ArgumentNullException("Hour", "The given parameter is invalid!");

            if (Minute > 59)
                throw new ArgumentNullException("Minute", "The given parameter is invalid!");

            #endregion

            this._Hour    = Hour;
            this._Minute  = Minute;

        }

        #endregion


        #region (static) Parse(Text)

        public static HourMin Parse(String Text)
        {

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException("Text", "The given input is not valid!");

            var Splited = Text.Split(':');

            if (Splited.Length != 2)
                throw new ArgumentException("The given input '" + Text + "' is not valid!");

            return new HourMin(UInt16.Parse(Splited[0]),
                               UInt16.Parse(Splited[1]));

        }

        #endregion

        #region (static) TryParse(Text, out HourMin)

        public static Boolean TryParse(String Text, out HourMin HourMin)
        {

            HourMin = new HourMin(0, 0);

            if (Text.IsNullOrEmpty())
                return false;

            var Splited = Text.Split(':');

            if (Splited.Length != 2)
                return false;

            UInt16 Hour = 0;
            if (!UInt16.TryParse(Splited[0], out Hour))
                return false;

            UInt16 Minute = 0;
            if (!UInt16.TryParse(Splited[1], out Minute))
                return false;

            HourMin = new HourMin(Hour, Minute);

            return true;

        }

        #endregion


        #region Operator overloading

        #region Operator == (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(HourMin HourMin1, HourMin HourMin2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(HourMin1, HourMin2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) HourMin1 == null) || ((Object) HourMin2 == null))
                return false;

            return HourMin1.Equals(HourMin2);

        }

        #endregion

        #region Operator != (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(HourMin HourMin1, HourMin HourMin2)
        {
            return !(HourMin1 == HourMin2);
        }

        #endregion

        #region Operator <  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(HourMin HourMin1, HourMin HourMin2)
        {

            if ((Object) HourMin1 == null)
                throw new ArgumentNullException("The given HourMin1 must not be null!");

            return HourMin1.CompareTo(HourMin2) < 0;

        }

        #endregion

        #region Operator <= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(HourMin HourMin1, HourMin HourMin2)
        {
            return !(HourMin1 > HourMin2);
        }

        #endregion

        #region Operator >  (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(HourMin HourMin1, HourMin HourMin2)
        {

            if ((Object) HourMin1 == null)
                throw new ArgumentNullException("The given HourMin1 must not be null!");

            return HourMin1.CompareTo(HourMin2) > 0;

        }

        #endregion

        #region Operator >= (HourMin1, HourMin2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin1">A HourMin.</param>
        /// <param name="HourMin2">Another HourMin.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(HourMin HourMin1, HourMin HourMin2)
        {
            return !(HourMin1 < HourMin2);
        }

        #endregion

        #endregion

        #region IComparable<HourMin> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an HourMin.
            if (!(Object is HourMin))
                throw new ArgumentNullException("The given object is not a HourMin!");

            return CompareTo((HourMin) Object);

        }

        #endregion

        #region CompareTo(HourMin)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HourMin">An object to compare with.</param>
        public Int32 CompareTo(HourMin HourMin)
        {

            if ((Object) HourMin == null)
                throw new ArgumentNullException("The given HourMin must not be null!");

            // Compare CountryIds
            var _Result = _Hour.CompareTo(HourMin._Hour);

            // If equal: Compare OperatorIds
            if (_Result == 0)
                _Result = _Minute.CompareTo(HourMin._Minute);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<HourMin> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an HourMin.
            if (!(Object is HourMin))
                return false;

            return this.Equals((HourMin) Object);

        }

        #endregion

        #region Equals(HourMin)

        /// <summary>
        /// Compares two HourMins for equality.
        /// </summary>
        /// <param name="HourMin">A HourMin to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(HourMin HourMin)
        {

            if ((Object) HourMin == null)
                return false;

            return _Hour.  Equals(HourMin._Hour) &&
                   _Minute.Equals(HourMin._Minute);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return _Hour.GetHashCode() * 23 ^ _Minute.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return String.Concat(_Hour.ToString("D2"), ":", _Minute.ToString("D2"));
        }

        #endregion

    }

}
