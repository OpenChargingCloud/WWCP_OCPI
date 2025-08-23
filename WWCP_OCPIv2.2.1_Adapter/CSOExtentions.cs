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

using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.WWCP
{

    /// <summary>
    /// Extensions methods for the WWCP wrapper for OCPI roaming clients for charging station operators.
    /// </summary>
    public static class CSOExtensions
    {

        /// <summary>
        /// Create and register a new electric vehicle roaming provider
        /// using the OCPI protocol and having the given unique electric
        /// vehicle roaming provider identification.
        /// </summary>
        /// 
        /// <param name="RoamingNetwork">A WWCP roaming network.</param>
        /// <param name="Id">The unique identification of the roaming provider.</param>
        /// <param name="Name">The official (multi-language) name of the roaming provider.</param>
        /// 
        /// <param name="EVSE2EVSEDataRecord">A delegate to process an EVSE data record, e.g. before pushing it to the roaming provider.</param>
        /// 
        /// <param name="IncludeEVSEs">Only include the EVSEs matching the given delegate.</param>
        /// <param name="ServiceCheckEvery">The service check interval.</param>
        /// <param name="StatusCheckEvery">The status check interval.</param>
        /// 
        /// <param name="DisablePushData">This service can be disabled, e.g. for debugging reasons.</param>
        /// <param name="DisablePushStatus">This service can be disabled, e.g. for debugging reasons.</param>
        /// <param name="DisableAuthentication">This service can be disabled, e.g. for debugging reasons.</param>
        /// <param name="DisableSendChargeDetailRecords">This service can be disabled, e.g. for debugging reasons.</param>
        /// 
        /// <param name="OCPIConfigurator">An optional delegate to configure the new OCPI roaming provider after its creation.</param>
        /// <param name="Configurator">An optional delegate to configure the new roaming provider after its creation.</param>
        public static OCPIv2_2_1.OCPICSOAdapter?

            CreateOCPIv2_2_1_CSOAdapter(this RoamingNetwork                                      RoamingNetwork,
                                        CSORoamingProvider_Id                                    Id,
                                        I18NString                                               Name,
                                        I18NString                                               Description,

                                        OCPIv2_2_1.HTTP.CommonAPI                                CommonAPI,

                                        OCPIv2_2_1.GetTariffIds_Delegate?                        GetTariffIds                         = null,

                                        OCPI.      WWCPEVSEId_2_EVSEUId_Delegate?                CustomEVSEUIdConverter               = null,
                                        OCPI.      WWCPEVSEId_2_EVSEId_Delegate?                 CustomEVSEIdConverter                = null,
                                        OCPIv2_2_1.WWCPEVSE_2_EVSE_Delegate?                     CustomEVSEConverter                  = null,
                                        OCPIv2_2_1.WWCPEVSEStatusUpdate_2_StatusType_Delegate?   CustomEVSEStatusUpdateConverter      = null,
                                        OCPIv2_2_1.WWCPChargeDetailRecord_2_CDR_Delegate?        CustomChargeDetailRecordConverter    = null,

                                        IncludeEVSEIdDelegate?                                   IncludeEVSEIds                       = null,
                                        IncludeEVSEDelegate?                                     IncludeEVSEs                         = null,
                                        IncludeChargingStationIdDelegate?                        IncludeChargingStationIds            = null,
                                        IncludeChargingStationDelegate?                          IncludeChargingStations              = null,
                                        IncludeChargingPoolIdDelegate?                           IncludeChargingPoolIds               = null,
                                        IncludeChargingPoolDelegate?                             IncludeChargingPools                 = null,
                                        IncludeChargingStationOperatorIdDelegate?                IncludeChargingStationOperatorIds    = null,
                                        IncludeChargingStationOperatorDelegate?                  IncludeChargingStationOperators      = null,
                                        ChargeDetailRecordFilterDelegate?                        ChargeDetailRecordFilter             = null,

                                        TimeSpan?                                                ServiceCheckEvery                    = null,
                                        TimeSpan?                                                StatusCheckEvery                     = null,
                                        TimeSpan?                                                CDRCheckEvery                        = null,

                                        Boolean                                                  DisablePushData                      = false,
                                        Boolean                                                  DisablePushStatus                    = false,
                                        Boolean                                                  DisablePushAdminStatus               = false,
                                        Boolean                                                  DisablePushEnergyStatus              = false,
                                        Boolean                                                  DisableAuthentication                = false,
                                        Boolean                                                  DisableSendChargeDetailRecords       = false,

                                        Action<OCPIv2_2_1.OCPICSOAdapter>?                       OCPIConfigurator                     = null,
                                        Action<ICSORoamingProvider>?                             Configurator                         = null,

                                        String                                                   EllipticCurve                        = "P-256",
                                        ECPrivateKeyParameters?                                  PrivateKey                           = null,
                                        PublicKeyCertificates?                                   PublicKeyCertificates                = null,

                                        Boolean?                                                 IsDevelopment                        = null,
                                        IEnumerable<String>?                                     DevelopmentServers                   = null,
                                        Boolean?                                                 DisableLogging                       = null,
                                        String?                                                  LoggingPath                          = null,
                                        String?                                                  LoggingContext                       = null,
                                        String?                                                  LogfileName                          = null,
                                        OCPILogfileCreatorDelegate?                              LogfileCreator                       = null,

                                        String?                                                  ClientsLoggingPath                   = null,
                                        String?                                                  ClientsLoggingContext                = null,
                                        OCPILogfileCreatorDelegate?                              ClientsLogfileCreator                = null,
                                        IDNSClient?                                              DNSClient                            = null)

        {

            #region Initial checks

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given roaming provider name must not be null or empty!");

            #endregion

            var newRoamingProvider = new OCPIv2_2_1.OCPICSOAdapter(

                                         Id,
                                         Name,
                                         Description,
                                         RoamingNetwork,

                                         CommonAPI,

                                         GetTariffIds,

                                         CustomEVSEUIdConverter,
                                         CustomEVSEIdConverter,
                                         CustomEVSEConverter,
                                         CustomEVSEStatusUpdateConverter,
                                         CustomChargeDetailRecordConverter,

                                         IncludeChargingStationOperatorIds,
                                         IncludeChargingStationOperators,
                                         IncludeChargingPoolIds,
                                         IncludeChargingPools,
                                         IncludeChargingStationIds,
                                         IncludeChargingStations,
                                         IncludeEVSEIds,
                                         IncludeEVSEs,
                                         ChargeDetailRecordFilter,

                                         ServiceCheckEvery,
                                         StatusCheckEvery,
                                         CDRCheckEvery,

                                         DisablePushData,
                                         DisablePushAdminStatus,
                                         DisablePushStatus,
                                         DisablePushEnergyStatus,
                                         DisableAuthentication,
                                         DisableSendChargeDetailRecords,

                                         EllipticCurve,
                                         PrivateKey,
                                         PublicKeyCertificates,

                                         IsDevelopment,
                                         DevelopmentServers,
                                         DisableLogging,
                                         LoggingPath,
                                         LoggingContext,
                                         LogfileName,
                                         LogfileCreator is not null
                                             ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator       (loggingPath, null, context, logfileName)
                                             : null,

                                         ClientsLoggingPath,
                                         ClientsLoggingContext,
                                         ClientsLogfileCreator is not null
                                             ? (loggingPath, remotePartyId, context, logfileName) => ClientsLogfileCreator(loggingPath, null, context, logfileName)
                                             : null,
                                         DNSClient

                                     );

            OCPIConfigurator?.Invoke(newRoamingProvider);

            return RoamingNetwork.
                       CreateCSORoamingProvider(newRoamingProvider,
                                                Configurator) as OCPIv2_2_1.OCPICSOAdapter;

        }

    }

}
