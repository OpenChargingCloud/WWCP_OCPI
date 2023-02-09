/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

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
        /// <param name="Name">The offical (multi-language) name of the roaming provider.</param>
        /// 
        /// <param name="EVSE2EVSEDataRecord">A delegate to process an EVSE data record, e.g. before pushing it to the roaming provider.</param>
        /// 
        /// <param name="DefaultOperator">An optional Charging Station Operator, which will be copied into the main OperatorID-section of the OCPI SOAP request.</param>
        /// <param name="OperatorNameSelector">An optional delegate to select an Charging Station Operator name, which will be copied into the OperatorName-section of the OCPI SOAP request.</param>
        /// <param name="IncludeEVSEs">Only include the EVSEs matching the given delegate.</param>
        /// <param name="ServiceCheckEvery">The service check intervall.</param>
        /// <param name="StatusCheckEvery">The status check intervall.</param>
        /// 
        /// <param name="DisablePushData">This service can be disabled, e.g. for debugging reasons.</param>
        /// <param name="DisablePushStatus">This service can be disabled, e.g. for debugging reasons.</param>
        /// <param name="DisableAuthentication">This service can be disabled, e.g. for debugging reasons.</param>
        /// <param name="DisableSendChargeDetailRecords">This service can be disabled, e.g. for debugging reasons.</param>
        /// 
        /// <param name="OCPIConfigurator">An optional delegate to configure the new OCPI roaming provider after its creation.</param>
        /// <param name="Configurator">An optional delegate to configure the new roaming provider after its creation.</param>
        public static OCPIv2_1_1.HTTP.OCPICSOAdapter?

            CreateOCPIv2_1_CSOAdapter(this RoamingNetwork                                      RoamingNetwork,
                                      EMPRoamingProvider_Id                                    Id,
                                      I18NString                                               Name,
                                      I18NString                                               Description,

                                      HTTPServer                                               HTTPServer,
                                      HTTPPath                                                 HTTPPathPrefix,

                                      OCPIv2_1_1.WWCPEVSEId_2_EVSEId_Delegate?                 CustomEVSEIdConverter                = null,
                                      OCPIv2_1_1.WWCPEVSE_2_EVSE_Delegate?                     CustomEVSEConverter                  = null,
                                      OCPIv2_1_1.WWCPEVSEStatusUpdate_2_StatusType_Delegate?   CustomEVSEStatusUpdateConverter      = null,
                                      OCPIv2_1_1.WWCPChargeDetailRecord_2_CDR_Delegate?        CustomChargeDetailRecordConverter    = null,

                                      //IChargingStationOperator?                                DefaultOperator                      = null,
                                      //ChargingStationOperatorNameSelectorDelegate?             OperatorNameSelector                 = null,

                                      IncludeEVSEIdDelegate?                                   IncludeEVSEIds                       = null,
                                      IncludeEVSEDelegate?                                     IncludeEVSEs                         = null,
                                      IncludeChargingPoolIdDelegate?                           IncludeChargingPoolIds               = null,
                                      IncludeChargingPoolDelegate?                             IncludeChargingPools                 = null,
                                      ChargeDetailRecordFilterDelegate?                        ChargeDetailRecordFilter             = null,

                                      TimeSpan?                                                ServiceCheckEvery                    = null,
                                      TimeSpan?                                                StatusCheckEvery                     = null,
                                      TimeSpan?                                                CDRCheckEvery                        = null,

                                      Boolean                                                  DisablePushData                      = false,
                                      Boolean                                                  DisablePushStatus                    = false,
                                      Boolean                                                  DisableAuthentication                = false,
                                      Boolean                                                  DisableSendChargeDetailRecords       = false,

                                      Action<OCPIv2_1_1.HTTP.OCPICSOAdapter>?                  OCPIConfigurator                     = null,
                                      Action<IEMPRoamingProvider>?                             Configurator                         = null,

                                      String                                                   EllipticCurve                        = "P-256",
                                      ECPrivateKeyParameters?                                  PrivateKey                           = null,
                                      PublicKeyCertificates?                                   PublicKeyCertificates                = null)

        {

            #region Initial checks

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given roaming provider name must not be null or empty!");

            #endregion

            var newRoamingProvider = new OCPIv2_1_1.HTTP.OCPICSOAdapter(

                                         Id,
                                         Name,
                                         Description,
                                         RoamingNetwork,

                                         HTTPServer,
                                         HTTPPathPrefix,

                                         CustomEVSEIdConverter,
                                         CustomEVSEConverter,
                                         CustomEVSEStatusUpdateConverter,
                                         CustomChargeDetailRecordConverter,

                                         IncludeEVSEIds,
                                         IncludeEVSEs,
                                         IncludeChargingPoolIds,
                                         IncludeChargingPools,
                                         ChargeDetailRecordFilter,

                                         ServiceCheckEvery,
                                         StatusCheckEvery,
                                         CDRCheckEvery,

                                         DisablePushData,
                                         DisablePushStatus,
                                         DisableAuthentication,
                                         DisableSendChargeDetailRecords

                                     );

            OCPIConfigurator?.Invoke(newRoamingProvider);

            return RoamingNetwork.
                       CreateNewRoamingProvider(newRoamingProvider,
                                                Configurator) as OCPIv2_1_1.HTTP.OCPICSOAdapter;

        }

    }

}
