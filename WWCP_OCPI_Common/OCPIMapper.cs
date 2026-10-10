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
    /// A delegate which allows you to modify the conversion from WWCP ChargingPool identifications to OCPI Location identifications.
    /// </summary>
    /// <param name="ChargingPoolId">A WWCP ChargingPool identification.</param>
    public delegate Location_Id?             ChargingPoolId_2_LocationId_Delegate               (WWCP.POI.ChargingPool_Id     ChargingPoolId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI Location identifications to WWCP ChargingPool identifications.
    /// </summary>
    /// <param name="PartyId">The party identification of the ChargingPool.</param>
    /// <param name="LocationId">A Location identification.</param>
    public delegate WWCP.POI.ChargingPool_Id?    LocationId_2_ChargingPoolId_Delegate               (Party_Idv3               PartyId,
                                                                                                 Location_Id              LocationId);


    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP ChargingStation identifications to OCPI ChargingStation identifications.
    /// </summary>
    /// <param name="ChargingStationId">A WWCP ChargingStation identification.</param>
    public delegate ChargingStation_Id?      WWCPChargingStationId_2_ChargingStationId_Delegate (WWCP.POI.ChargingStation_Id  ChargingStationId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI ChargingStation identifications to WWCP ChargingStation identifications.
    /// </summary>
    /// <param name="ChargingStationId">An OCPI ChargingStation identification.</param>
    public delegate WWCP.POI.ChargingStation_Id? ChargingStationId_2_WWCPChargingStationId_Delegate (ChargingStation_Id       ChargingStationId);


    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP EVSE identifications to OCPI EVSE unique identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_UId?                WWCPEVSEId_2_EVSEUId_Delegate                      (WWCP.POI.EVSE_Id             EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI EVSE unique identifications to WWCP EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">An EVSE identification.</param>
    public delegate WWCP.POI.EVSE_Id?            EVSEUId_2_WWCPEVSEId_Delegate                      (Party_Idv3               PartyId,
                                                                                                 EVSE_UId                 EVSEUId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP EVSE identifications to OCPI EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">A WWCP EVSE identification.</param>
    public delegate EVSE_Id?                 WWCPEVSEId_2_EVSEId_Delegate                       (WWCP.POI.EVSE_Id             EVSEId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI EVSE identifications to WWCP EVSE identifications.
    /// </summary>
    /// <param name="EVSEId">An EVSE identification.</param>
    public delegate WWCP.POI.EVSE_Id?            EVSEId_2_WWCPEVSEId_Delegate                       (EVSE_Id                  EVSEId);


    /// <summary>
    /// A delegate which allows you to modify the conversion from WWCP connector identifications to OCPI connector identifications.
    /// </summary>
    /// <param name="ConnectorId">A WWCP connector identification.</param>
    public delegate Connector_Id?               WWCPConnectorId_2_ConnectorId_Delegate          (WWCP.POI.ChargingConnector_Id  ConnectorId);

    /// <summary>
    /// A delegate which allows you to modify the conversion from OCPI connector identifications to WWCP connector identifications.
    /// </summary>
    /// <param name="ConnectorId">A connector identification.</param>
    public delegate WWCP.POI.ChargingConnector_Id?  ConnectorId_2_WWCPConnectorId_Delegate          (Party_Idv3                 PartyId,
                                                                                                 Connector_Id               ConnectorId);


    /// <summary>
    /// Helper methods to map OCPI data structures to
    /// WWCP data structures and vice versa.
    /// </summary>
    public static class OCPIMapper
    {

        #region ToOCPI_CountryCode (this CSOId)

        public static CountryCode ToOCPI_CountryCode(this WWCP.POI.ChargingStationOperator_Id CSOId)

            => CountryCode.Parse(CSOId.CountryCode.Alpha2Code);

        #endregion

        #region ToOCPI_PartyId     (this CSOId)

        public static Party_Id ToOCPI_PartyId(this WWCP.POI.ChargingStationOperator_Id CSOId)

            => Party_Id.Parse(CSOId.Suffix);

        #endregion

        #region ToOCPI_PartyIdv3   (this CSOId)

        public static Party_Idv3 ToOCPI_PartyIdv3(this WWCP.POI.ChargingStationOperator_Id CSOId)

            => Party_Idv3.From(
                   CSOId.ToOCPI_CountryCode(),
                   CSOId.ToOCPI_PartyId()
               );

        #endregion


        #region ToOCPI_CountryCode (this EMPId)

        public static CountryCode ToOCPI_CountryCode(this WWCP.POI.EMobilityProvider_Id EMPId)

            => CountryCode.Parse(EMPId.CountryCode.Alpha2Code);

        #endregion

        #region ToOCPI_PartyId     (this EMPId)

        public static Party_Id ToOCPI_PartyId(this WWCP.POI.EMobilityProvider_Id EMPId)

            => Party_Id.Parse(EMPId.Suffix);

        #endregion

        #region ToOCPI_PartyIdv3   (this EMPId)

        public static Party_Idv3 ToOCPI_PartyIdv3(this WWCP.POI.EMobilityProvider_Id EMPId)

            => Party_Idv3.From(
                   EMPId.ToOCPI_CountryCode(),
                   EMPId.ToOCPI_PartyId()
               );

        #endregion


        #region ToWWCP (this EVSEId)

        public static WWCP.POI.EVSE_Id? ToWWCP(this EVSE_Id EVSEId)

            => WWCP.POI.EVSE_Id.TryParse(EVSEId.ToString());

        public static WWCP.POI.EVSE_Id? ToWWCP(this EVSE_Id? EVSEId)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToWWCP()
                   : null;

        #endregion

        #region ToWWCP (this EMSPId)

        public static WWCP.POI.EMobilityProvider_Id? ToWWCP(this EMSP_Id EMSPId)

            => WWCP.POI.EMobilityProvider_Id.TryParse(EMSPId.ToString());

        public static WWCP.POI.EMobilityProvider_Id? ToWWCP(this EMSP_Id? EMSPId)

            => EMSPId.HasValue
                   ? EMSPId.Value.ToWWCP()
                   : null;

        #endregion

        #region ToWWCP (this CPOId)

        public static WWCP.POI.ChargingStationOperator_Id? ToWWCP(this CPO_Id CPOId)

            => WWCP.POI.ChargingStationOperator_Id.TryParse(CPOId.ToString());

        public static WWCP.POI.ChargingStationOperator_Id? ToWWCP(this CPO_Id? CPOId)

            => CPOId.HasValue
                   ? CPOId.Value.ToWWCP()
                   : null;

        #endregion

        #region ToOCPI_EVSEUId(this EVSEId)

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.POI.EVSE_Id               EVSEId,
                                               WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEIdConverter   = null)

            => CustomEVSEIdConverter is not null
                   ? EVSE_UId.TryParse(CustomEVSEIdConverter(EVSEId).ToString())
                   : EVSE_UId.TryParse(EVSEId.ToString());

        public static EVSE_UId? ToOCPI_EVSEUId(this WWCP.POI.EVSE_Id?              EVSEId,
                                               WWCPEVSEId_2_EVSEUId_Delegate?  CustomEVSEIdConverter   = null)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToOCPI_EVSEUId(CustomEVSEIdConverter)
                   : null;

        #endregion

        #region ToOCPI_EVSEId (this EVSEId, CustomEVSEIdConverter = null)

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.POI.EVSE_Id              EVSEId,
                                             WWCPEVSEId_2_EVSEId_Delegate?  CustomEVSEIdConverter   = null)

            => CustomEVSEIdConverter is not null
                   ? EVSE_Id.TryParse(CustomEVSEIdConverter(EVSEId).ToString())
                   : EVSE_Id.TryParse(EVSEId.ToString());

        public static EVSE_Id? ToOCPI_EVSEId(this WWCP.POI.EVSE_Id?             EVSEId,
                                             WWCPEVSEId_2_EVSEId_Delegate?  CustomEVSEIdConverter   = null)

            => EVSEId.HasValue
                   ? EVSEId.Value.ToOCPI_EVSEId(CustomEVSEIdConverter)
                   : null;

        #endregion


        #region ToOCPI(this ChargingPoolId,   ...)

        public static Location_Id? ToOCPI(this WWCP.POI.ChargingPool_Id              ChargingPoolId,
                                          ChargingPoolId_2_LocationId_Delegate?  CustomChargingPoolIdConverter = null)

            => CustomChargingPoolIdConverter is not null
                   ? CustomChargingPoolIdConverter(ChargingPoolId)
                   : Location_Id.TryParse(ChargingPoolId.Suffix);

        #endregion

        #region ToOCPI(this ChargingLocation, ...)

        public static LocationReference? ToOCPI(this WWCP.POI.ChargingLocation?        ChargingLocation,
                                                ChargingPoolId_2_LocationId_Delegate?  CustomChargingPoolIdConverter   = null,
                                                WWCPEVSEId_2_EVSEUId_Delegate?         CustomEVSEIdConverter           = null)
        {

            if (ChargingLocation is null)
                return null;

            var locationId  = ChargingLocation.ChargingPoolId?.ToOCPI(CustomChargingPoolIdConverter);

            if (!locationId.HasValue)
                return null;

            var evseUId     = ChargingLocation.EVSEId?.ToOCPI_EVSEUId(CustomEVSEIdConverter);

            return new LocationReference(
                       LocationId:  locationId.Value,
                       EVSEUIds:    evseUId.HasValue
                                        ? [ evseUId.Value ]
                                        : null
                   );

        }

        #endregion


        #region ToOCPI(this WWCPConnectorType)

        public static ConnectorType? ToOCPI(this WWCP.POI.ChargingConnectorType WWCPConnectorType)
        {

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.CHAdeMO)
                return ConnectorType.CHADEMO;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.CHAOJI)
                return ConnectorType.CHAOJI;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_A)
                return ConnectorType.DOMESTIC_A;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_B)
                return ConnectorType.DOMESTIC_B;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_C)
                return ConnectorType.DOMESTIC_C;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_D)
                return ConnectorType.DOMESTIC_D;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_E_FrenchStandard)
                return ConnectorType.DOMESTIC_E;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_F_SchuKo)
                return ConnectorType.DOMESTIC_F;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_G_BritishStandard)
                return ConnectorType.DOMESTIC_G;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_H)
                return ConnectorType.DOMESTIC_H;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_I)
                return ConnectorType.DOMESTIC_I;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_J_SwissStandard)
                return ConnectorType.DOMESTIC_J;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_K)
                return ConnectorType.DOMESTIC_K;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_L)
                return ConnectorType.DOMESTIC_L;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_M)
                return ConnectorType.DOMESTIC_M;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_N)
                return ConnectorType.DOMESTIC_N;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.DOMESTIC_O)
                return ConnectorType.DOMESTIC_O;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.GBT_AC)
                return ConnectorType.GBT_AC;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.GBT_DC)
                return ConnectorType.GBT_DC;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_60309_2_single_16)
                return ConnectorType.IEC_60309_2_single_16;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_60309_2_three_16)
                return ConnectorType.IEC_60309_2_three_16;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_60309_2_three_32)
                return ConnectorType.IEC_60309_2_three_32;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_60309_2_three_64)
                return ConnectorType.IEC_60309_2_three_64;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_62196_T1)
                return ConnectorType.IEC_62196_T1;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_62196_T1_COMBO)
                return ConnectorType.IEC_62196_T1_COMBO;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_62196_T2)
                return ConnectorType.IEC_62196_T2;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_62196_T2_COMBO)
                return ConnectorType.IEC_62196_T2_COMBO;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_62196_T3A)
                return ConnectorType.IEC_62196_T3A;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.IEC_62196_T3C)
                return ConnectorType.IEC_62196_T3C;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.NEMA_5_20)
                return ConnectorType.NEMA_5_20;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.NEMA_6_30)
                return ConnectorType.NEMA_6_30;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.NEMA_6_50)
                return ConnectorType.NEMA_6_50;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.NEMA_10_30)
                return ConnectorType.NEMA_10_30;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.NEMA_10_50)
                return ConnectorType.NEMA_10_50;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.NEMA_14_30)
                return ConnectorType.NEMA_14_30;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.NEMA_14_50)
                return ConnectorType.NEMA_14_50;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.PANTOGRAPH_BOTTOM_UP)
                return ConnectorType.PANTOGRAPH_BOTTOM_UP;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.PANTOGRAPH_TOP_DOWN)
                return ConnectorType.PANTOGRAPH_TOP_DOWN;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.TESLA_Roadster)
                return ConnectorType.TESLA_R;

            if (WWCPConnectorType == WWCP.POI.ChargingConnectorType.TESLA_ModelS)
                return ConnectorType.TESLA_S;

            throw new ArgumentException($"Unsupported WWCP charging connector type: {WWCPConnectorType}!", nameof(WWCPConnectorType));

        }
        public static ConnectorType? ToOCPI(this WWCP.POI.ChargingConnectorType? ConnectorType)

            => ConnectorType.HasValue
                   ? ConnectorType.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this ConnectorType)

        public static WWCP.POI.ChargingConnectorType? ToWWCP(this ConnectorType OCPIConnectorType)
        {

            if (OCPIConnectorType == ConnectorType.CHADEMO)
                return WWCP.POI.ChargingConnectorType.CHAdeMO;

            if (OCPIConnectorType == ConnectorType.CHAOJI)
                return WWCP.POI.ChargingConnectorType.CHAOJI;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_A)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_A;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_B)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_B;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_C)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_C;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_D)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_D;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_E)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_E_FrenchStandard;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_F)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_F_SchuKo;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_G)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_G_BritishStandard;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_H)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_H;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_I)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_I;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_J)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_J_SwissStandard;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_K)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_K;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_L)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_L;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_M)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_M;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_N)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_N;

            if (OCPIConnectorType == ConnectorType.DOMESTIC_O)
                return WWCP.POI.ChargingConnectorType.DOMESTIC_O;

            if (OCPIConnectorType == ConnectorType.GBT_AC)
                return WWCP.POI.ChargingConnectorType.GBT_AC;

            if (OCPIConnectorType == ConnectorType.GBT_DC)
                return WWCP.POI.ChargingConnectorType.GBT_DC;

            if (OCPIConnectorType == ConnectorType.IEC_60309_2_single_16)
                return WWCP.POI.ChargingConnectorType.IEC_60309_2_single_16;

            if (OCPIConnectorType == ConnectorType.IEC_60309_2_three_16)
                return WWCP.POI.ChargingConnectorType.IEC_60309_2_three_16;

            if (OCPIConnectorType == ConnectorType.IEC_60309_2_three_32)
                return WWCP.POI.ChargingConnectorType.IEC_60309_2_three_32;

            if (OCPIConnectorType == ConnectorType.IEC_60309_2_three_64)
                return WWCP.POI.ChargingConnectorType.IEC_60309_2_three_64;

            if (OCPIConnectorType == ConnectorType.IEC_62196_T1)
                return WWCP.POI.ChargingConnectorType.IEC_62196_T1;

            if (OCPIConnectorType == ConnectorType.IEC_62196_T1_COMBO)
                return WWCP.POI.ChargingConnectorType.IEC_62196_T1_COMBO;

            if (OCPIConnectorType == ConnectorType.IEC_62196_T2)
                return WWCP.POI.ChargingConnectorType.IEC_62196_T2;

            if (OCPIConnectorType == ConnectorType.IEC_62196_T2_COMBO)
                return WWCP.POI.ChargingConnectorType.IEC_62196_T2_COMBO;

            if (OCPIConnectorType == ConnectorType.IEC_62196_T3A)
                return WWCP.POI.ChargingConnectorType.IEC_62196_T3A;

            if (OCPIConnectorType == ConnectorType.IEC_62196_T3C)
                return WWCP.POI.ChargingConnectorType.IEC_62196_T3C;

            if (OCPIConnectorType == ConnectorType.NEMA_5_20)
                return WWCP.POI.ChargingConnectorType.NEMA_5_20;

            if (OCPIConnectorType == ConnectorType.NEMA_6_30)
                return WWCP.POI.ChargingConnectorType.NEMA_6_30;

            if (OCPIConnectorType == ConnectorType.NEMA_6_50)
                return WWCP.POI.ChargingConnectorType.NEMA_6_50;

            if (OCPIConnectorType == ConnectorType.NEMA_10_30)
                return WWCP.POI.ChargingConnectorType.NEMA_10_30;

            if (OCPIConnectorType == ConnectorType.NEMA_10_50)
                return WWCP.POI.ChargingConnectorType.NEMA_10_50;

            if (OCPIConnectorType == ConnectorType.NEMA_14_30)
                return WWCP.POI.ChargingConnectorType.NEMA_14_30;

            if (OCPIConnectorType == ConnectorType.NEMA_14_50)
                return WWCP.POI.ChargingConnectorType.NEMA_14_50;

            if (OCPIConnectorType == ConnectorType.PANTOGRAPH_BOTTOM_UP)
                return WWCP.POI.ChargingConnectorType.PANTOGRAPH_BOTTOM_UP;

            if (OCPIConnectorType == ConnectorType.PANTOGRAPH_TOP_DOWN)
                return WWCP.POI.ChargingConnectorType.PANTOGRAPH_TOP_DOWN;

            if (OCPIConnectorType == ConnectorType.TESLA_R)
                return WWCP.POI.ChargingConnectorType.TESLA_Roadster;

            if (OCPIConnectorType == ConnectorType.TESLA_S)
                return WWCP.POI.ChargingConnectorType.TESLA_ModelS;

            throw new ArgumentException($"Unsupported OCPI connector type: {OCPIConnectorType}!", nameof(OCPIConnectorType));

        }

        public static WWCP.POI.ChargingConnectorType? ToWWCP(this ConnectorType? OCPIConnectorType)

            => OCPIConnectorType.HasValue
                   ? OCPIConnectorType.Value.ToWWCP()
                   : null;

        #endregion



        #region ToOCPI(this CurrentType)

        public static PowerTypes? ToOCPI(this WWCP.POI.CurrentTypes CurrentType)

            => CurrentType switch {
                   WWCP.POI.CurrentTypes.AC_OnePhase     => PowerTypes.AC_1_PHASE,
                   WWCP.POI.CurrentTypes.AC_ThreePhases  => PowerTypes.AC_3_PHASE,
                   WWCP.POI.CurrentTypes.DC              => PowerTypes.DC,
                   _                                 => null
               };

        public static PowerTypes? ToOCPI(this WWCP.POI.CurrentTypes? CurrentType)

            => CurrentType.HasValue
                   ? CurrentType.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this CurrentType)

        public static WWCP.POI.CurrentTypes? ToWWCP(this PowerTypes PowerType)

            => PowerType switch {
                   PowerTypes.AC_1_PHASE  => WWCP.POI.CurrentTypes.AC_OnePhase,
                   PowerTypes.AC_3_PHASE  => WWCP.POI.CurrentTypes.AC_ThreePhases,
                   PowerTypes.DC          => WWCP.POI.CurrentTypes.DC,
                   _                      => null
               };

        public static WWCP.POI.CurrentTypes? ToWWCP(this PowerTypes? PowerType)

            => PowerType.HasValue
                   ? PowerType.Value.ToWWCP()
                   : null;

        #endregion


        #region ToOCPI(this EnergyMeterId)

        public static EnergyMeter_Id? ToOCPI(this WWCP.POI.EnergyMeter_Id EnergyMeterId)

            => EnergyMeter_Id.Parse(EnergyMeterId.ToString());

        public static EnergyMeter_Id? ToOCPI(this WWCP.POI.EnergyMeter_Id? EnergyMeterId)

            => EnergyMeterId.HasValue
                   ? EnergyMeterId.Value.ToOCPI()
                   : null;

        #endregion

        #region ToWWCP(this MeterId)

        public static WWCP.POI.EnergyMeter_Id? ToWWCP(this EnergyMeter_Id MeterId)

            => WWCP.POI.EnergyMeter_Id.Parse(MeterId.ToString());

        public static WWCP.POI.EnergyMeter_Id? ToWWCP(this EnergyMeter_Id? MeterId)

            => MeterId.HasValue
                   ? MeterId.Value.ToWWCP()
                   : null;

        #endregion


    }

}
