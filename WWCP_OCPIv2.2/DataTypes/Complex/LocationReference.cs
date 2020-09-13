/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Reference to location details.
    /// </summary>
    public readonly struct LocationReference : IEquatable<LocationReference>,
                                               IComparable<LocationReference>,
                                               IComparable
    {

        #region Properties

        /// <summary>
        /// Unique identifier for the location.
        /// </summary>
        [Mandatory]
        public Location_Id            LocationId    { get; }

        /// <summary>
        /// Optional enumeration of EVSE identifiers within the CPO’s platform within the EVSE within the given location.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_UId>  EVSEUIds      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new references to location details.
        /// </summary>
        /// <param name="LocationId">Unique identifier for the location..</param>
        /// <param name="EVSEUIds">Optional enumeration of EVSE identifiers within the CPO’s platform within the given location.</param>
        public LocationReference(Location_Id            LocationId,
                                 IEnumerable<EVSE_UId>  EVSEUIds)
        {

            this.LocationId  = LocationId;
            this.EVSEUIds    = EVSEUIds?.Distinct() ?? new EVSE_UId[0];

        }

        #endregion


        #region Operator overloading

        #region Operator == (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => LocationReference1.Equals(LocationReference2);

        #endregion

        #region Operator != (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => !(LocationReference1 == LocationReference2);

        #endregion

        #region Operator <  (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LocationReference LocationReference1,
                                          LocationReference LocationReference2)

            => LocationReference1.CompareTo(LocationReference2) < 0;

        #endregion

        #region Operator <= (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => !(LocationReference1 > LocationReference2);

        #endregion

        #region Operator >  (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LocationReference LocationReference1,
                                          LocationReference LocationReference2)

            => LocationReference1.CompareTo(LocationReference2) > 0;

        #endregion

        #region Operator >= (LocationReference1, LocationReference2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference1">A location reference.</param>
        /// <param name="LocationReference2">Another location reference.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LocationReference LocationReference1,
                                           LocationReference LocationReference2)

            => !(LocationReference1 < LocationReference2);

        #endregion

        #endregion

        #region IComparable<LocationReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is LocationReference locationReference
                   ? CompareTo(locationReference)
                   : throw new ArgumentException("The given object is not a location reference!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationReference)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationReference">An object to compare with.</param>
        public Int32 CompareTo(LocationReference LocationReference)

            => LocationId.CompareTo(LocationReference.LocationId);

        #endregion

        #endregion

        #region IEquatable<LocationReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is LocationReference locationReference &&
                   Equals(locationReference);

        #endregion

        #region Equals(LocationReference)

        /// <summary>
        /// Compares two location references for equality.
        /// </summary>
        /// <param name="LocationReference">A location reference to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(LocationReference LocationReference)

            => LocationId.Equals(LocationReference.LocationId);

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
                return LocationId.GetHashCode();
            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(LocationId,
                             " -> ",
                             EVSEUIds.OrderBy(evse_uid => evse_uid).
                                      AggregateWith(", "));

        #endregion

    }

}
