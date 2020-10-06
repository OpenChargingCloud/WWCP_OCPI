using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.WWCP;
/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// 
    /// </summary>
    public static class OICPMapper
    {


        public static IEnumerable<Location> ToOCPI(this IEnumerable<ChargingPool>  ChargingPools)

            => ChargingPools.SafeSelect(pool => {

                                            try
                                            {

                                                return new Location(CountryCode.Parse("DE"),
                                                                    Party_Id.Parse("GEF"),
                                                                    Location_Id.Parse("..."),
                                                                    true,
                                                                    pool.Address.Street + pool.Address.HouseNumber,
                                                                    pool.Address.City.FirstText(),
                                                                    pool.Address.Country.Alpha3Code,
                                                                    pool.GeoLocation ?? default,
                                                                    "timezone",
                                                                    null,
                                                                    "name",
                                                                    pool.Address.PostalCode,
                                                                    pool.Address.Country.CountryName.FirstText(),
                                                                    null,
                                                                    null,
                                                                    new EVSE[0],
                                                                    new DisplayText[] { new DisplayText(Languages.eng, "directions") },
                                                                    new BusinessDetails(pool.Operator.Name.FirstText(),
                                                                                        pool.Operator.Homepage,
                                                                                        null),
                                                                    null,
                                                                    null,
                                                                    new Facilities[0],
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    null,
                                                                    DateTime.UtcNow);

                                            }
                                            catch (Exception e)
                                            {
                                                return null;
                                            }
                                        }).

                             Where(pool => pool != null);

    }

}
