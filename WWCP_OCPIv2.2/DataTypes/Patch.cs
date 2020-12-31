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

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    //public static class PatchObject
    //{

    //    public static JObject Apply(JObject oldJObject, JObject patch)
    //    {

    //        foreach (var property in patch)
    //        {

    //            if (property.Value is null)
    //            {
    //                oldJObject.Remove(property.Key);
    //            }

    //            else if (property.Value is JObject subObject)
    //            {

    //                if (oldJObject[property.Key] is JObject oldSubObject)
    //                    oldJObject[property.Key] = Apply(oldSubObject, subObject);

    //                else
    //                    oldJObject[property.Key] = subObject;

    //            }

    //            //else if (property.Value is JArray subArray)
    //            //{
    //            //}

    //            else
    //                oldJObject[property.Key] = property.Value;

    //        }

    //        return oldJObject;

    //    }

    //}

}
