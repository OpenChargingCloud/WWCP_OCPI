/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.WWCP.EVCertificates;
using org.GraphDefined.Vanaheimr.Illias;
using System.ComponentModel;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for component locations.
    /// </summary>
    public static class ComponentLocationExtensions
    {

        /// <summary>
        /// Indicates whether this component location is null or empty.
        /// </summary>
        /// <param name="ComponentLocation">A component location.</param>
        public static Boolean IsNullOrEmpty(this ComponentLocation? ComponentLocation)
            => !ComponentLocation.HasValue || ComponentLocation.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this component location is NOT null or empty.
        /// </summary>
        /// <param name="ComponentLocation">A component location.</param>
        public static Boolean IsNotNullOrEmpty(this ComponentLocation? ComponentLocation)
            => ComponentLocation.HasValue && ComponentLocation.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a component location.
    /// </summary>
    public readonly struct ComponentLocation : IId<ComponentLocation>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this component location is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this component location is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the component location.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new component location based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a component location.</param>
        private ComponentLocation(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a component location.
        /// </summary>
        /// <param name="Text">A text representation of a component location.</param>
        public static ComponentLocation Parse(String Text)
        {

            if (TryParse(Text, out var componentLocation))
                return componentLocation;

            throw new ArgumentException($"Invalid text representation of a component location: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a component location.
        /// </summary>
        /// <param name="Text">A text representation of a component location.</param>
        public static ComponentLocation? TryParse(String Text)
        {

            if (TryParse(Text, out var componentLocation))
                return componentLocation;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ComponentLocation)

        /// <summary>
        /// Try to parse the given text as a component location.
        /// </summary>
        /// <param name="Text">A text representation of a component location.</param>
        /// <param name="ComponentLocation">The parsed component location.</param>
        public static Boolean TryParse(String Text, out ComponentLocation ComponentLocation)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ComponentLocation = new ComponentLocation(Text);
                    return true;
                }
                catch
                { }
            }

            ComponentLocation = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this component location.
        /// </summary>
        public ComponentLocation Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The meter reading was obtained at the inlet, that is, the meter reading describes
        /// the power that is taken from the power supply by the component.
        /// </summary>
        public static ComponentLocation  INLET     { get; }
            = new ("INLET");

        /// <summary>
        /// The meter reading was obtained at the outlet, that is, the meter reading describes
        /// the power being delivered by the component.
        /// </summary>
        public static ComponentLocation  OUTLET    { get; }
            = new ("OUTLET");

        #endregion


        #region Operator overloading

        #region Operator == (ComponentLocation1, ComponentLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLocation1">A component location.</param>
        /// <param name="ComponentLocation2">Another component location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ComponentLocation ComponentLocation1,
                                           ComponentLocation ComponentLocation2)

            => ComponentLocation1.Equals(ComponentLocation2);

        #endregion

        #region Operator != (ComponentLocation1, ComponentLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLocation1">A component location.</param>
        /// <param name="ComponentLocation2">Another component location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ComponentLocation ComponentLocation1,
                                           ComponentLocation ComponentLocation2)

            => !ComponentLocation1.Equals(ComponentLocation2);

        #endregion

        #region Operator <  (ComponentLocation1, ComponentLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLocation1">A component location.</param>
        /// <param name="ComponentLocation2">Another component location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ComponentLocation ComponentLocation1,
                                          ComponentLocation ComponentLocation2)

            => ComponentLocation1.CompareTo(ComponentLocation2) < 0;

        #endregion

        #region Operator <= (ComponentLocation1, ComponentLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLocation1">A component location.</param>
        /// <param name="ComponentLocation2">Another component location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ComponentLocation ComponentLocation1,
                                           ComponentLocation ComponentLocation2)

            => ComponentLocation1.CompareTo(ComponentLocation2) <= 0;

        #endregion

        #region Operator >  (ComponentLocation1, ComponentLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLocation1">A component location.</param>
        /// <param name="ComponentLocation2">Another component location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ComponentLocation ComponentLocation1,
                                          ComponentLocation ComponentLocation2)

            => ComponentLocation1.CompareTo(ComponentLocation2) > 0;

        #endregion

        #region Operator >= (ComponentLocation1, ComponentLocation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLocation1">A component location.</param>
        /// <param name="ComponentLocation2">Another component location.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ComponentLocation ComponentLocation1,
                                           ComponentLocation ComponentLocation2)

            => ComponentLocation1.CompareTo(ComponentLocation2) >= 0;

        #endregion

        #endregion

        #region IComparable<ComponentLocation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two component locations.
        /// </summary>
        /// <param name="Object">A component location to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ComponentLocation componentLocation
                   ? CompareTo(componentLocation)
                   : throw new ArgumentException("The given object is not a component location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ComponentLocation)

        /// <summary>
        /// Compares two component locations.
        /// </summary>
        /// <param name="ComponentLocation">A component location to compare with.</param>
        public Int32 CompareTo(ComponentLocation ComponentLocation)

            => String.Compare(InternalId,
                              ComponentLocation.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ComponentLocation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two component locations for equality.
        /// </summary>
        /// <param name="Object">A component location to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ComponentLocation componentLocation &&
                   Equals(componentLocation);

        #endregion

        #region Equals(ComponentLocation)

        /// <summary>
        /// Compares two component locations for equality.
        /// </summary>
        /// <param name="ComponentLocation">A component location to compare with.</param>
        public Boolean Equals(ComponentLocation ComponentLocation)

            => String.Equals(InternalId,
                             ComponentLocation.InternalId,
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
