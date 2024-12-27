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

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{


    /// <summary>
    /// Extension methods for location services.
    /// </summary>
    public static class LocationServiceExtensions
    {

        /// <summary>
        /// Indicates whether this location service is null or empty.
        /// </summary>
        /// <param name="LocationService">A location service.</param>
        public static Boolean IsNullOrEmpty(this LocationService? LocationService)
            => !LocationService.HasValue || LocationService.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this location service is NOT null or empty.
        /// </summary>
        /// <param name="LocationService">A location service.</param>
        public static Boolean IsNotNullOrEmpty(this LocationService? LocationService)
            => LocationService.HasValue && LocationService.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a location service.
    /// </summary>
    public readonly struct LocationService : IId<LocationService>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this location service is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this location service is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the location service.
        /// </summary>
        public UInt64 Length
            => (UInt64)InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new location service based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a location service.</param>
        private LocationService(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a location service.
        /// </summary>
        /// <param name="Text">A text representation of a location service.</param>
        public static LocationService Parse(String Text)
        {

            if (TryParse(Text, out var locationService))
                return locationService;

            throw new ArgumentException($"Invalid text representation of a location service: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a location service.
        /// </summary>
        /// <param name="Text">A text representation of a location service.</param>
        public static LocationService? TryParse(String Text)
        {

            if (TryParse(Text, out var locationService))
                return locationService;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LocationService)

        /// <summary>
        /// Try to parse the given text as a location service.
        /// </summary>
        /// <param name="Text">A text representation of a location service.</param>
        /// <param name="LocationService">The parsed location service.</param>
        public static Boolean TryParse(String Text, out LocationService LocationService)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    LocationService = new LocationService(Text);
                    return true;
                }
                catch
                { }
            }

            LocationService = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this location service.
        /// </summary>
        public LocationService Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// One or more EVSEs have accessibility modifications in place to allow use by people with disabilities.
        /// Note that more information on accessibility modifications can be provided using the various fields
        /// for images and in the parking field of the EVSE object.
        /// </summary>
        public static LocationService  ACCESSIBLE_CHARGING    { get; }
            = new ("ACCESSIBLE_CHARGING");

        /// <summary>
        /// Assistance from on-site staff is available to help a Driver charge at the Location.
        /// </summary>
        public static LocationService  FLASSISTANCEAT         { get; }
            = new ("ASSISTANCE");

        /// <summary>
        /// Security monitoring with video cameras is in place at the location.
        /// </summary>
        public static LocationService  CAMERA_SURVEILLANCE    { get; }
            = new ("CAMERA_SURVEILLANCE");

        /// <summary>
        /// A voice communication channel is available for the Driver to contact security staff from the location.
        /// </summary>
        public static LocationService  EMERGENCY_CALL         { get; }
            = new ("EMERGENCY_CALL");

        /// <summary>
        /// Time charging: defined in hours, step_size multiplier: 1 second.
        /// Can also be used in combination with a RESERVATION restriction to describe
        /// the price of the reservation time.
        /// </summary>
        public static LocationService  TIME            { get; }
            = new ("TIME");

        #endregion


        #region Operator overloading

        #region Operator == (LocationService1, LocationService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationService1">A location service.</param>
        /// <param name="LocationService2">Another location service.</param>
        /// <returns>true|false</returns>
        public static Boolean operator ==(LocationService LocationService1,
                                           LocationService LocationService2)

            => LocationService1.Equals(LocationService2);

        #endregion

        #region Operator != (LocationService1, LocationService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationService1">A location service.</param>
        /// <param name="LocationService2">Another location service.</param>
        /// <returns>true|false</returns>
        public static Boolean operator !=(LocationService LocationService1,
                                           LocationService LocationService2)

            => !LocationService1.Equals(LocationService2);

        #endregion

        #region Operator <  (LocationService1, LocationService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationService1">A location service.</param>
        /// <param name="LocationService2">Another location service.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(LocationService LocationService1,
                                          LocationService LocationService2)

            => LocationService1.CompareTo(LocationService2) < 0;

        #endregion

        #region Operator <= (LocationService1, LocationService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationService1">A location service.</param>
        /// <param name="LocationService2">Another location service.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(LocationService LocationService1,
                                           LocationService LocationService2)

            => LocationService1.CompareTo(LocationService2) <= 0;

        #endregion

        #region Operator >  (LocationService1, LocationService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationService1">A location service.</param>
        /// <param name="LocationService2">Another location service.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(LocationService LocationService1,
                                          LocationService LocationService2)

            => LocationService1.CompareTo(LocationService2) > 0;

        #endregion

        #region Operator >= (LocationService1, LocationService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationService1">A location service.</param>
        /// <param name="LocationService2">Another location service.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(LocationService LocationService1,
                                           LocationService LocationService2)

            => LocationService1.CompareTo(LocationService2) >= 0;

        #endregion

        #endregion

        #region IComparable<LocationService> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two location services.
        /// </summary>
        /// <param name="Object">A location service to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LocationService locationService
                   ? CompareTo(locationService)
                   : throw new ArgumentException("The given object is not a location service!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationService)

        /// <summary>
        /// Compares two location services.
        /// </summary>
        /// <param name="LocationService">A location service to compare with.</param>
        public Int32 CompareTo(LocationService LocationService)

            => String.Compare(InternalId,
                              LocationService.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LocationService> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two location services for equality.
        /// </summary>
        /// <param name="Object">A location service to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LocationService locationService &&
                   Equals(locationService);

        #endregion

        #region Equals(LocationService)

        /// <summary>
        /// Compares two location services for equality.
        /// </summary>
        /// <param name="LocationService">A location service to compare with.</param>
        public Boolean Equals(LocationService LocationService)

            => String.Equals(InternalId,
                             LocationService.InternalId,
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
