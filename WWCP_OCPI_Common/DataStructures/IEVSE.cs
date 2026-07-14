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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// The common interface of all OCPI EVSEs.
    /// </summary>
    public interface IEVSE
    {

        IEnumerable<Capability>          Capabilities           { get; }
        IEnumerable<Connector_Id>        ConnectorIds           { get; }
        GeoCoordinate?                   Coordinates            { get; }
        DateTimeOffset                   Created                { get; }
        JObject                          CustomData             { get; }
        IEnumerable<DisplayText>         Directions             { get; }
        String                           ETag                   { get; }
        EVSE_Id?                         EVSEId                 { get; }
        String?                          FloorLevel             { get; }
        IEnumerable<Image>               Images                 { get; }
        UserDefinedDictionary            InternalData           { get; }
        DateTimeOffset                   LastUpdated            { get; }
        IEnumerable<ParkingRestriction>  ParkingRestrictions    { get; }
        String?                          PhysicalReference      { get; }
        StatusType                       Status                 { get; }
        IEnumerable<StatusSchedule>      StatusSchedule         { get; }
        EVSE_UId                         UId                    { get; }


        //Int32    CompareTo       (IEVSE?        EVSE);
        Int32    CompareTo       (Object?       Object);
        Boolean  ConnectorExists (Connector_Id  ConnectorId);
        //Boolean Equals           (IEVSE?        EVSE);
        Boolean  Equals          (Object?       Object);

        Int32    GetHashCode();
        String   ToString();

    }

}
