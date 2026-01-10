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

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for module identifications.
    /// </summary>
    public static class ModuleIdExtensions
    {

        /// <summary>
        /// Indicates whether this module identification is null or empty.
        /// </summary>
        /// <param name="ModuleId">A module identification.</param>
        public static Boolean IsNullOrEmpty(this Module_Id? ModuleId)
            => !ModuleId.HasValue || ModuleId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this module identification is NOT null or empty.
        /// </summary>
        /// <param name="ModuleId">A module identification.</param>
        public static Boolean IsNotNullOrEmpty(this Module_Id? ModuleId)
            => ModuleId.HasValue && ModuleId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a module.
    /// Custom module identifications might occur!
    /// string(255)
    /// </summary>
    public readonly struct Module_Id : IId<Module_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this module identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this module identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the module identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new module identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a module identification.</param>
        private Module_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a module identification.
        /// </summary>
        /// <param name="Text">A text representation of a module identification.</param>
        public static Module_Id Parse(String Text)
        {

            if (TryParse(Text, out var moduleId))
                return moduleId;

            throw new ArgumentException($"Invalid text representation of a module identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a module identification.
        /// </summary>
        /// <param name="Text">A text representation of a module identification.</param>
        public static Module_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var moduleId))
                return moduleId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ModuleId)

        /// <summary>
        /// Try to parse the given text as a module identification.
        /// </summary>
        /// <param name="Text">A text representation of a module identification.</param>
        /// <param name="ModuleId">The parsed module identification.</param>
        public static Boolean TryParse(String Text, out Module_Id ModuleId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ModuleId = new Module_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ModuleId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this module identification.
        /// </summary>
        public Module_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// The credentials & registration module.
        /// </summary>
        public static Module_Id Credentials
            => new ("credentials");


        /// <summary>
        /// The CDRs module.
        /// </summary>
        public static Module_Id CDRs
            => new ("cdrs");

        /// <summary>
        /// The Charging Profiles module.
        /// </summary>
        public static Module_Id ChargingProfiles
            => new ("chargingprofiles");

        /// <summary>
        /// The Commands module.
        /// </summary>
        public static Module_Id Commands
            => new ("commands");

        /// <summary>
        /// The EVSEStatuses module.
        /// </summary>
        public static Module_Id EVSEStatuses
            => new ("evsestatuses");

        /// <summary>
        /// The Invoice Reconciliation module.
        /// </summary>
        public static Module_Id InvoiceReconciliation
            => new ("irrs");

        /// <summary>
        /// The Locations module.
        /// </summary>
        public static Module_Id Locations
            => new ("locations");

        /// <summary>
        /// The Sessions module.
        /// </summary>
        public static Module_Id Sessions
            => new ("sessions");

        /// <summary>
        /// The Power Regulation module.
        /// </summary>
        public static Module_Id PowerRegulation
            => new ("meterreadings");

        /// <summary>
        /// The Tariffs module.
        /// </summary>
        public static Module_Id Tariffs
            => new ("tariffs");

        /// <summary>
        /// The Tariff Assocations module.
        /// </summary>
        public static Module_Id TariffAssocations
            => new ("tariffassociations");

        /// <summary>
        /// The Tokens module.
        /// </summary>
        public static Module_Id Tokens
            => new ("tokens");

        #endregion


        #region Operator overloading

        #region Operator == (ModuleId1, ModuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ModuleId1">A module identification.</param>
        /// <param name="ModuleId2">Another module identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Module_Id ModuleId1,
                                           Module_Id ModuleId2)

            => ModuleId1.Equals(ModuleId2);

        #endregion

        #region Operator != (ModuleId1, ModuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ModuleId1">A module identification.</param>
        /// <param name="ModuleId2">Another module identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Module_Id ModuleId1,
                                           Module_Id ModuleId2)

            => !ModuleId1.Equals(ModuleId2);

        #endregion

        #region Operator <  (ModuleId1, ModuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ModuleId1">A module identification.</param>
        /// <param name="ModuleId2">Another module identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Module_Id ModuleId1,
                                          Module_Id ModuleId2)

            => ModuleId1.CompareTo(ModuleId2) < 0;

        #endregion

        #region Operator <= (ModuleId1, ModuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ModuleId1">A module identification.</param>
        /// <param name="ModuleId2">Another module identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Module_Id ModuleId1,
                                           Module_Id ModuleId2)

            => ModuleId1.CompareTo(ModuleId2) <= 0;

        #endregion

        #region Operator >  (ModuleId1, ModuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ModuleId1">A module identification.</param>
        /// <param name="ModuleId2">Another module identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Module_Id ModuleId1,
                                          Module_Id ModuleId2)

            => ModuleId1.CompareTo(ModuleId2) > 0;

        #endregion

        #region Operator >= (ModuleId1, ModuleId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ModuleId1">A module identification.</param>
        /// <param name="ModuleId2">Another module identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Module_Id ModuleId1,
                                           Module_Id ModuleId2)

            => ModuleId1.CompareTo(ModuleId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ModuleId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two module identifications.
        /// </summary>
        /// <param name="Object">A module identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Module_Id moduleId
                   ? CompareTo(moduleId)
                   : throw new ArgumentException("The given object is not a module identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ModuleId)

        /// <summary>
        /// Compares two module identifications.
        /// </summary>
        /// <param name="ModuleId">A module identification to compare with.</param>
        public Int32 CompareTo(Module_Id ModuleId)

            => String.Compare(InternalId,
                              ModuleId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ModuleId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two module identifications for equality.
        /// </summary>
        /// <param name="Object">A module identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Module_Id moduleId &&
                   Equals(moduleId);

        #endregion

        #region Equals(ModuleId)

        /// <summary>
        /// Compares two module identifications for equality.
        /// </summary>
        /// <param name="ModuleId">A module identification to compare with.</param>
        public Boolean Equals(Module_Id ModuleId)

            => String.Equals(InternalId,
                             ModuleId.InternalId,
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
