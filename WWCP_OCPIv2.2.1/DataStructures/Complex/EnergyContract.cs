/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The energy delivery contract.
    /// </summary>
    public readonly struct EnergyContract : IEquatable<EnergyContract>,
                                            IComparable<EnergyContract>,
                                            IComparable
    {

        #region Properties

        /// <summary>
        /// The name of the energy supplier for this token.
        /// </summary>
        [Mandatory]
        public String             SupplierName    { get; }

        /// <summary>
        /// The optional contract identification at the energy supplier, that belongs
        /// to the owner of this token.
        /// </summary>
        [Mandatory]
        public EnergyContract_Id  ContractId      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new energy delivery contracts for an EV driver.
        /// </summary>
        /// <param name="SupplierName">A name of the energy supplier for this token.</param>
        /// <param name="ContractId">An optional contract identification at the energy supplier, that belongs to the owner of this token.</param>
        public EnergyContract(String             SupplierName,
                              EnergyContract_Id  ContractId)
        {

            this.SupplierName  = SupplierName;
            this.ContractId    = ContractId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnergyContractParser = null)

        /// <summary>
        /// Parse the given JSON representation of an energy contract.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergyContractParser">A delegate to parse custom energy contract JSON objects.</param>
        public static EnergyContract Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<EnergyContract>?  CustomEnergyContractParser   = null)
        {

            if (TryParse(JSON,
                         out var energyContract,
                         out var errorResponse,
                         CustomEnergyContractParser))
            {
                return energyContract;
            }

            throw new ArgumentException("The given JSON representation of an energy contract is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomEnergyContractParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a energy contract.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergyContractParser">A delegate to parse custom energy contract JSON objects.</param>
        public static EnergyContract? TryParse(JObject                                       JSON,
                                               CustomJObjectParserDelegate<EnergyContract>?  CustomEnergyContractParser   = null)
        {

            if (TryParse(JSON,
                         out var energyContract,
                         out var errorResponse,
                         CustomEnergyContractParser))
            {
                return energyContract;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out EnergyContract, out ErrorResponse, CustomEnergyContractParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an energy contract.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyContract">The parsed energy contract.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out EnergyContract  EnergyContract,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out EnergyContract,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an energy contract.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyContract">The parsed energy contract.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergyContractParser">A delegate to parse custom energy contract JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out EnergyContract                            EnergyContract,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<EnergyContract>?  CustomEnergyContractParser   = null)
        {

            try
            {

                EnergyContract = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse SupplierName    [mandatory]

                if (!JSON.ParseMandatoryText("supplier_name",
                                             "energy supplier name",
                                             out String SupplierName,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ContractId      [mandatory]

                if (!JSON.ParseMandatory("contract_id",
                                         "energy contract identification",
                                         EnergyContract_Id.TryParse,
                                         out EnergyContract_Id ContractId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                EnergyContract = new EnergyContract(SupplierName,
                                                    ContractId);


                if (CustomEnergyContractParser is not null)
                    EnergyContract = CustomEnergyContractParser(JSON,
                                                                EnergyContract);

                return true;

            }
            catch (Exception e)
            {
                EnergyContract  = default;
                ErrorResponse   = "The given JSON representation of an energy contract is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEnergyContractSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EnergyContract>? CustomEnergyContractSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("supplier_name",  SupplierName),

                           new JProperty("contract_id",    ContractId.ToString())

                       );

            return CustomEnergyContractSerializer is not null
                       ? CustomEnergyContractSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public EnergyContract Clone()

            => new (new String(SupplierName.ToCharArray()),
                    ContractId.Clone);

        #endregion


        #region Operator overloading

        #region Operator == (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => EnergyContract1.Equals(EnergyContract2);

        #endregion

        #region Operator != (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => !EnergyContract1.Equals(EnergyContract2);

        #endregion

        #region Operator <  (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EnergyContract EnergyContract1,
                                          EnergyContract EnergyContract2)

            => EnergyContract1.CompareTo(EnergyContract2) < 0;

        #endregion

        #region Operator <= (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => EnergyContract1.CompareTo(EnergyContract2) <= 0;

        #endregion

        #region Operator >  (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EnergyContract EnergyContract1,
                                          EnergyContract EnergyContract2)

            => EnergyContract1.CompareTo(EnergyContract2) > 0;

        #endregion

        #region Operator >= (EnergyContract1, EnergyContract2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyContract1">An energy delivery contract.</param>
        /// <param name="EnergyContract2">Another energy delivery contract.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EnergyContract EnergyContract1,
                                           EnergyContract EnergyContract2)

            => EnergyContract1.CompareTo(EnergyContract2) >= 0;

        #endregion

        #endregion

        #region IComparable<EnergyContract> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two energy delivery contracts.
        /// </summary>
        /// <param name="Object">An energy delivery contract to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EnergyContract energyContract
                   ? CompareTo(energyContract)
                   : throw new ArgumentException("The given object is not a energy delivery contract!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyContract)

        /// <summary>
        /// Compares two energy delivery contracts.
        /// </summary>
        /// <param name="EnergyContract">An energy delivery contract to compare with.</param>
        public Int32 CompareTo(EnergyContract EnergyContract)
        {

            var c = SupplierName.CompareTo(EnergyContract.SupplierName);

            if (c == 0)
                c = ContractId.  CompareTo(EnergyContract.ContractId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnergyContract> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy delivery contracts for equality.
        /// </summary>
        /// <param name="Object">An energy delivery contract to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnergyContract energyContract &&
                   Equals(energyContract);

        #endregion

        #region Equals(EnergyContract)

        /// <summary>
        /// Compares two energy delivery contracts for equality.
        /// </summary>
        /// <param name="EnergyContract">An energy delivery contract to compare with.</param>
        public Boolean Equals(EnergyContract EnergyContract)

            => SupplierName.Equals(EnergyContract.SupplierName) &&
               ContractId.  Equals(EnergyContract.ContractId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return SupplierName.GetHashCode() * 3 ^
                       ContractId.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   SupplierName,
                   " (",
                   ContractId,
                   ")"

               );

        #endregion

    }

}
