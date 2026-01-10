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

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP EVSE identifications to OCPI EVSE unique identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_UId                 WWCPEVSEId_2_EVSEUId_Delegate                      (WWCP.EVSE_Id             EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI EVSE unique identifications to WWCP EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">An EVSE identification.</param>
    public delegate WWCP.EVSE_Id             EVSEUId_2_WWCPEVSEId_Delegate                      (EVSE_UId                 EVSEUId);


    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP ChargingStation identifications to OCPI ChargingStation identifications.
    /// </summary>
    /// <param name="ChargingStationId">A WWCP ChargingStation identification.</param>
    public delegate ChargingStation_Id       WWCPChargingStationId_2_ChargingStationId_Delegate (WWCP.ChargingStation_Id  ChargingStationId);


    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP EVSE identifications to OCPI EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_Id                  WWCPEVSEId_2_EVSEId_Delegate                       (WWCP.EVSE_Id             EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI EVSE identifications to WWCP EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">An EVSE identification.</param>
    public delegate WWCP.EVSE_Id             EVSEId_2_WWCPEVSEId_Delegate                       (EVSE_Id                  EVSEId);


    /// <summary>
    /// Helper methods to map OCPI data structures to
    /// WWCP data structures and vice versa.
    /// </summary>
    public static class OCPIMapper
    {

        #region ToOCPI_CountryCode(this CSOId)

        public static CountryCode ToOCPI_CountryCode(this WWCP.ChargingStationOperator_Id CSOId)

            => CountryCode.Parse(CSOId.CountryCode.Alpha2Code);

        #endregion

        #region ToOCPI_PartyId(this CSOId)

        public static Party_Id ToOCPI_PartyId(this WWCP.ChargingStationOperator_Id CSOId)

            => Party_Id.Parse(CSOId.Suffix);

        #endregion


        #region ToWWCP (this EVSEId)

        public static WWCP.EVSE_Id? ToWWCP(this EVSE_Id EVSEId)

            => WWCP.EVSE_Id.TryParse(EVSEId.ToString());

        public static WWCP.EVSE_Id? ToWWCP(this EVSE_Id? EVSEId)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToWWCP()
                   : null;

        #endregion

        #region ToWWCP (this EMSPId)

        public static WWCP.EMobilityProvider_Id? ToWWCP(this EMSP_Id EMSPId)

            => WWCP.EMobilityProvider_Id.TryParse(EMSPId.ToString());

        public static WWCP.EMobilityProvider_Id? ToWWCP(this EMSP_Id? EMSPId)

            => EMSPId.HasValue
                   ? EMSPId.Value.ToWWCP()
                   : null;

        #endregion

        #region ToWWCP (this CPOId)

        public static WWCP.ChargingStationOperator_Id? ToWWCP(this CPO_Id CPOId)

            => WWCP.ChargingStationOperator_Id.TryParse(CPOId.ToString());

        public static WWCP.ChargingStationOperator_Id? ToWWCP(this CPO_Id? CPOId)

            => CPOId.HasValue
                   ? CPOId.Value.ToWWCP()
                   : null;

        #endregion

        #region ToOCPI_EVSEUId(this EVSEId)

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id               EVSEId,
                                               WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEIdConverter   = null)

            => CustomEVSEIdConverter is not null
                   ? EVSE_UId.TryParse(CustomEVSEIdConverter(EVSEId).ToString())
                   : EVSE_UId.TryParse(EVSEId.ToString());

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.EVSE_Id?              EVSEId,
                                               WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEIdConverter   = null)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToOCPI_EVSEUId(CustomEVSEIdConverter)
                   : null;

        #endregion

        #region ToOCPI_EVSEId (this EVSEId, CustomEVSEIdConverter = null)

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id              EVSEId,
                                             WWCPEVSEId_2_EVSEId_Delegate?  CustomEVSEIdConverter   = null)

            => CustomEVSEIdConverter is not null
                   ? EVSE_Id.TryParse(CustomEVSEIdConverter(EVSEId).ToString())
                   : EVSE_Id.TryParse(EVSEId.ToString());

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.EVSE_Id?             EVSEId,
                                             WWCPEVSEId_2_EVSEId_Delegate?  CustomEVSEIdConverter   = null)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToOCPI_EVSEId(CustomEVSEIdConverter)
                   : null;

        #endregion


    }

}
