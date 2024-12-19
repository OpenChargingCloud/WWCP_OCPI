/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Command 2.0 (the "License");
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
    /// Extension methods for command identifications.
    /// </summary>
    public static class CommandIdExtensions
    {

        /// <summary>
        /// Indicates whether this command identification is null or empty.
        /// </summary>
        /// <param name="CommandId">A command identification.</param>
        public static Boolean IsNullOrEmpty(this Command_Id? CommandId)
            => !CommandId.HasValue || CommandId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this command identification is NOT null or empty.
        /// </summary>
        /// <param name="CommandId">A command identification.</param>
        public static Boolean IsNotNullOrEmpty(this Command_Id? CommandId)
            => CommandId.HasValue && CommandId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a command.
    /// </summary>
    public readonly struct Command_Id : IId<Command_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this command identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this command identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the command identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new command identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a command identification.</param>
        private Command_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) NewRandom(Length = 50)

        /// <summary>
        /// Create a new random command identification.
        /// </summary>
        /// <param name="Length">The expected length of the command identification.</param>
        public static Command_Id NewRandom(Byte Length = 50)

            => new (RandomExtensions.RandomString(Length));

        #endregion

        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a command identification.
        /// </summary>
        /// <param name="Text">A text representation of a command identification.</param>
        public static Command_Id Parse(String Text)
        {

            if (TryParse(Text, out var commandId))
                return commandId;

            throw new ArgumentException($"Invalid text representation of a command identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a command identification.
        /// </summary>
        /// <param name="Text">A text representation of a command identification.</param>
        public static Command_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var commandId))
                return commandId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out CommandId)

        /// <summary>
        /// Try to parse the given text as a command identification.
        /// </summary>
        /// <param name="Text">A text representation of a command identification.</param>
        /// <param name="CommandId">The parsed command identification.</param>
        public static Boolean TryParse(String Text, out Command_Id CommandId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    CommandId = new Command_Id(Text);
                    return true;
                }
                catch
                { }
            }

            CommandId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this command identification.
        /// </summary>
        public Command_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CommandId1, CommandId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandId1">A command identification.</param>
        /// <param name="CommandId2">Another command identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Command_Id CommandId1,
                                           Command_Id CommandId2)

            => CommandId1.Equals(CommandId2);

        #endregion

        #region Operator != (CommandId1, CommandId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandId1">A command identification.</param>
        /// <param name="CommandId2">Another command identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Command_Id CommandId1,
                                           Command_Id CommandId2)

            => !CommandId1.Equals(CommandId2);

        #endregion

        #region Operator <  (CommandId1, CommandId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandId1">A command identification.</param>
        /// <param name="CommandId2">Another command identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Command_Id CommandId1,
                                          Command_Id CommandId2)

            => CommandId1.CompareTo(CommandId2) < 0;

        #endregion

        #region Operator <= (CommandId1, CommandId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandId1">A command identification.</param>
        /// <param name="CommandId2">Another command identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Command_Id CommandId1,
                                           Command_Id CommandId2)

            => CommandId1.CompareTo(CommandId2) <= 0;

        #endregion

        #region Operator >  (CommandId1, CommandId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandId1">A command identification.</param>
        /// <param name="CommandId2">Another command identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Command_Id CommandId1,
                                          Command_Id CommandId2)

            => CommandId1.CompareTo(CommandId2) > 0;

        #endregion

        #region Operator >= (CommandId1, CommandId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandId1">A command identification.</param>
        /// <param name="CommandId2">Another command identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Command_Id CommandId1,
                                           Command_Id CommandId2)

            => CommandId1.CompareTo(CommandId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CommandId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two command identifications.
        /// </summary>
        /// <param name="Object">A command identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Command_Id commandId
                   ? CompareTo(commandId)
                   : throw new ArgumentException("The given object is not a command identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CommandId)

        /// <summary>
        /// Compares two command identifications.
        /// </summary>
        /// <param name="CommandId">A command identification to compare with.</param>
        public Int32 CompareTo(Command_Id CommandId)

            => String.Compare(InternalId,
                              CommandId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CommandId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two command identifications for equality.
        /// </summary>
        /// <param name="Object">A command identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Command_Id commandId &&
                   Equals(commandId);

        #endregion

        #region Equals(CommandId)

        /// <summary>
        /// Compares two command identifications for equality.
        /// </summary>
        /// <param name="CommandId">A command identification to compare with.</param>
        public Boolean Equals(Command_Id CommandId)

            => String.Equals(InternalId,
                             CommandId.InternalId,
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
