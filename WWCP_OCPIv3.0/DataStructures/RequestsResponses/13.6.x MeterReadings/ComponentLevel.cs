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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for component levels.
    /// </summary>
    public static class ComponentLevelExtensions
    {

        /// <summary>
        /// Indicates whether this component level is null or empty.
        /// </summary>
        /// <param name="ComponentLevel">A component level.</param>
        public static Boolean IsNullOrEmpty(this ComponentLevel? ComponentLevel)
            => !ComponentLevel.HasValue || ComponentLevel.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this component level is NOT null or empty.
        /// </summary>
        /// <param name="ComponentLevel">A component level.</param>
        public static Boolean IsNotNullOrEmpty(this ComponentLevel? ComponentLevel)
            => ComponentLevel.HasValue && ComponentLevel.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a component level.
    /// </summary>
    public readonly struct ComponentLevel : IId<ComponentLevel>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this component level is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this component level is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the component level.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new component level based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a component level.</param>
        private ComponentLevel(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a component level.
        /// </summary>
        /// <param name="Text">A text representation of a component level.</param>
        public static ComponentLevel Parse(String Text)
        {

            if (TryParse(Text, out var componentLevel))
                return componentLevel;

            throw new ArgumentException($"Invalid text representation of a component level: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a component level.
        /// </summary>
        /// <param name="Text">A text representation of a component level.</param>
        public static ComponentLevel? TryParse(String Text)
        {

            if (TryParse(Text, out var componentLevel))
                return componentLevel;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ComponentLevel)

        /// <summary>
        /// Try to parse the given text as a component level.
        /// </summary>
        /// <param name="Text">A text representation of a component level.</param>
        /// <param name="ComponentLevel">The parsed component level.</param>
        public static Boolean TryParse(String Text, out ComponentLevel ComponentLevel)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ComponentLevel = new ComponentLevel(Text);
                    return true;
                }
                catch
                { }
            }

            ComponentLevel = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this component level.
        /// </summary>
        public ComponentLevel Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The meter reading applies to the Electric Vehicle (EV) connected to the EVSE.
        /// </summary>
        public static ComponentLevel  EV       { get; }
            = new ("EV");

        /// <summary>
        /// The meter reading applies to a single EVSE.
        /// </summary>
        public static ComponentLevel  EVSE     { get; }
            = new ("EVSE");

        /// <summary>
        /// The meter reading applies to a grouping of EVSEs.
        /// </summary>
        public static ComponentLevel  GROUP    { get; }
            = new ("GROUP");

        #endregion


        #region Operator overloading

        #region Operator == (ComponentLevel1, ComponentLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLevel1">A component level.</param>
        /// <param name="ComponentLevel2">Another component level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ComponentLevel ComponentLevel1,
                                           ComponentLevel ComponentLevel2)

            => ComponentLevel1.Equals(ComponentLevel2);

        #endregion

        #region Operator != (ComponentLevel1, ComponentLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLevel1">A component level.</param>
        /// <param name="ComponentLevel2">Another component level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ComponentLevel ComponentLevel1,
                                           ComponentLevel ComponentLevel2)

            => !ComponentLevel1.Equals(ComponentLevel2);

        #endregion

        #region Operator <  (ComponentLevel1, ComponentLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLevel1">A component level.</param>
        /// <param name="ComponentLevel2">Another component level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ComponentLevel ComponentLevel1,
                                          ComponentLevel ComponentLevel2)

            => ComponentLevel1.CompareTo(ComponentLevel2) < 0;

        #endregion

        #region Operator <= (ComponentLevel1, ComponentLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLevel1">A component level.</param>
        /// <param name="ComponentLevel2">Another component level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ComponentLevel ComponentLevel1,
                                           ComponentLevel ComponentLevel2)

            => ComponentLevel1.CompareTo(ComponentLevel2) <= 0;

        #endregion

        #region Operator >  (ComponentLevel1, ComponentLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLevel1">A component level.</param>
        /// <param name="ComponentLevel2">Another component level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ComponentLevel ComponentLevel1,
                                          ComponentLevel ComponentLevel2)

            => ComponentLevel1.CompareTo(ComponentLevel2) > 0;

        #endregion

        #region Operator >= (ComponentLevel1, ComponentLevel2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentLevel1">A component level.</param>
        /// <param name="ComponentLevel2">Another component level.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ComponentLevel ComponentLevel1,
                                           ComponentLevel ComponentLevel2)

            => ComponentLevel1.CompareTo(ComponentLevel2) >= 0;

        #endregion

        #endregion

        #region IComparable<ComponentLevel> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two component levels.
        /// </summary>
        /// <param name="Object">A component level to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ComponentLevel componentLevel
                   ? CompareTo(componentLevel)
                   : throw new ArgumentException("The given object is not a component level!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ComponentLevel)

        /// <summary>
        /// Compares two component levels.
        /// </summary>
        /// <param name="ComponentLevel">A component level to compare with.</param>
        public Int32 CompareTo(ComponentLevel ComponentLevel)

            => String.Compare(InternalId,
                              ComponentLevel.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ComponentLevel> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two component levels for equality.
        /// </summary>
        /// <param name="Object">A component level to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ComponentLevel componentLevel &&
                   Equals(componentLevel);

        #endregion

        #region Equals(ComponentLevel)

        /// <summary>
        /// Compares two component levels for equality.
        /// </summary>
        /// <param name="ComponentLevel">A component level to compare with.</param>
        public Boolean Equals(ComponentLevel ComponentLevel)

            => String.Equals(InternalId,
                             ComponentLevel.InternalId,
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
