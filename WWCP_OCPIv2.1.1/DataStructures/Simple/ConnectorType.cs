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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Extension methods for connector types.
    /// </summary>
    public static class ConnectorTypeExtensions
    {

        /// <summary>
        /// Indicates whether this connector type is null or empty.
        /// </summary>
        /// <param name="ConnectorType">A connector type.</param>
        public static Boolean IsNullOrEmpty(this ConnectorType? ConnectorType)
            => !ConnectorType.HasValue || ConnectorType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this connector type is NOT null or empty.
        /// </summary>
        /// <param name="ConnectorType">A connector type.</param>
        public static Boolean IsNotNullOrEmpty(this ConnectorType? ConnectorType)
            => ConnectorType.HasValue && ConnectorType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The type of a connector.
    /// </summary>
    public readonly struct ConnectorType : IId<ConnectorType>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this connector type is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this connector type is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the connector type.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connector type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a connector type.</param>
        private ConnectorType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        public static ConnectorType Parse(String Text)
        {

            if (TryParse(Text, out var connectorType))
                return connectorType;

            throw new ArgumentException($"Invalid text representation of a connector type: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        public static ConnectorType? TryParse(String Text)
        {

            if (TryParse(Text, out var connectorType))
                return connectorType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ConnectorType)

        /// <summary>
        /// Try to parse the given text as a connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        /// <param name="ConnectorType">The parsed connector type.</param>
        public static Boolean TryParse(String Text, out ConnectorType ConnectorType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ConnectorType = new ConnectorType(Text);
                    return true;
                }
                catch
                { }
            }

            ConnectorType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this connector type.
        /// </summary>
        public ConnectorType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The connector type is CHAdeMO, DC.
        /// </summary>
        public static ConnectorType CHADEMO
            => new ("CHADEMO");

        /// <summary>
        /// Standard/Domestic household, type "A", NEMA 1-15, 2 pins
        /// </summary>
        public static ConnectorType DOMESTIC_A
            => new ("DOMESTIC_A");

        /// <summary>
        /// Standard/Domestic household, type "B", NEMA 5-15, 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_B
            => new("DOMESTIC_B");

        /// <summary>
        /// Standard/Domestic household, type "C", CEE 7/17, 2 pins
        /// </summary>
        public static ConnectorType DOMESTIC_C
            => new ("DOMESTIC_C");

        /// <summary>
        /// Standard/Domestic household, type "D", 3 pin
        /// </summary>
        public static ConnectorType DOMESTIC_D
            => new ("DOMESTIC_D");

        /// <summary>
        /// Standard/Domestic household, type "E", CEE 7/5 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_E
            => new ("DOMESTIC_E");

        /// <summary>
        /// Standard/Domestic household, type "F", CEE 7/4, Schuko, 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_F
            => new ("DOMESTIC_F");

        /// <summary>
        /// Standard/Domestic household, type "G", BS 1363, Commonwealth, 3
        /// </summary>
        public static ConnectorType DOMESTIC_G
            => new ("DOMESTIC_G");

        /// <summary>
        /// Standard/Domestic household, type "H", SI-32, 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_H
            => new ("DOMESTIC_H");

        /// <summary>
        /// Standard/Domestic household, type "I", AS 3112, 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_I
            => new ("DOMESTIC_I");

        /// <summary>
        /// Standard/Domestic household, type "J", SEV 1011, 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_J
            => new ("DOMESTIC_J");

        /// <summary>
        /// Standard/Domestic household, type "K", DS 60884-2-D1, 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_K
            => new ("DOMESTIC_K");

        /// <summary>
        /// Standard/Domestic household, type "L", CEI 23-16-VII, 3 pins
        /// </summary>
        public static ConnectorType DOMESTIC_L
            => new ("DOMESTIC_L");

        /// <summary>
        /// IEC 60309-2 Industrial Connector single phase 16 Amperes (usually blue)
        /// </summary>
        public static ConnectorType IEC_60309_2_single_16
            => new ("IEC_60309_2_single_16");

        /// <summary>
        /// IEC 60309-2 Industrial Connector three phase 16 Amperes (usually red)
        /// </summary>
        public static ConnectorType IEC_60309_2_three_16
            => new ("IEC_60309_2_three_16");

        /// <summary>
        /// IEC 60309-2 Industrial Connector three phase 32 Amperes (usually red)
        /// </summary>
        public static ConnectorType IEC_60309_2_three_32
            => new ("IEC_60309_2_three_32");

        /// <summary>
        /// IEC 60309-2 Industrial Connector three phase 64 Amperes (usually red)
        /// </summary>
        public static ConnectorType IEC_60309_2_three_64
            => new ("IEC_60309_2_three_64");

        /// <summary>
        /// IEC 62196 Type 1 "SAE J1772".
        /// </summary>
        public static ConnectorType IEC_62196_T1
            => new ("IEC_62196_T1");

        /// <summary>
        /// Combo Type 1 based, DC
        /// </summary>
        public static ConnectorType IEC_62196_T1_COMBO
            => new ("IEC_62196_T1_COMBO");

        /// <summary>
        /// IEC 62196 Type 2 "Mennekes"
        /// </summary>
        public static ConnectorType IEC_62196_T2
            => new ("IEC_62196_T2");

        /// <summary>
        /// Combo Type 2 based, DC
        /// </summary>
        public static ConnectorType IEC_62196_T2_COMBO
            => new ("IEC_62196_T2_COMBO");

        /// <summary>
        /// IEC 62196 Type 3A
        /// </summary>
        public static ConnectorType IEC_62196_T3A
            => new ("IEC_62196_T3A");

        /// <summary>
        /// IEC 62196 Type 3C "Scame"
        /// </summary>
        public static ConnectorType IEC_62196_T3C
            => new ("IEC_62196_T3C");

        /// <summary>
        /// On-board Bottom-up-Pantograph typically for bus charging.
        /// </summary>
        public static ConnectorType PANTOGRAPH_BOTTOM_UP
            => new ("PANTOGRAPH_BOTTOM_UP");

        /// <summary>
        /// Off-board Top-down-Pantograph typically for bus charging.
        /// </summary>
        public static ConnectorType PANTOGRAPH_TOP_DOWN
            => new ("PANTOGRAPH_TOP_DOWN");

        /// <summary>
        /// Tesla Connector "Roadster"-type (round, 4 pin)
        /// </summary>
        public static ConnectorType TESLA_R
            => new ("TESLA_R");

        /// <summary>
        /// Tesla Connector "Model-S"-type (oval, 5 pin)
        /// </summary>
        public static ConnectorType TESLA_S
            => new ("TESLA_S");

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => ConnectorType1.Equals(ConnectorType2);

        #endregion

        #region Operator != (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => !ConnectorType1.Equals(ConnectorType2);

        #endregion

        #region Operator <  (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConnectorType ConnectorType1,
                                          ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) < 0;

        #endregion

        #region Operator <= (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) <= 0;

        #endregion

        #region Operator >  (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConnectorType ConnectorType1,
                                          ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) > 0;

        #endregion

        #region Operator >= (ConnectorType1, ConnectorType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorType1">A connector type.</param>
        /// <param name="ConnectorType2">Another connector type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConnectorType ConnectorType1,
                                           ConnectorType ConnectorType2)

            => ConnectorType1.CompareTo(ConnectorType2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConnectorType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two connector types.
        /// </summary>
        /// <param name="Object">A connector type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConnectorType connectorType
                   ? CompareTo(connectorType)
                   : throw new ArgumentException("The given object is not a connector type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConnectorType)

        /// <summary>
        /// Compares two connector types.
        /// </summary>
        /// <param name="ConnectorType">A connector type to compare with.</param>
        public Int32 CompareTo(ConnectorType ConnectorType)

            => String.Compare(InternalId,
                              ConnectorType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ConnectorType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two connector types for equality.
        /// </summary>
        /// <param name="Object">A connector type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConnectorType connectorType &&
                   Equals(connectorType);

        #endregion

        #region Equals(ConnectorType)

        /// <summary>
        /// Compares two connector types for equality.
        /// </summary>
        /// <param name="ConnectorType">A connector type to compare with.</param>
        public Boolean Equals(ConnectorType ConnectorType)

            => String.Equals(InternalId,
                             ConnectorType.InternalId,
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
