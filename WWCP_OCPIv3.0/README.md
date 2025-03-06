# WWCP OCPI v3.0 Implementation

This software implements the [Open Charge Point Interface (OCPI)](https://github.com/ocpi/ocpi)
in C# on .NET9++.

## Differences to the official protocol

The following desribes differences of this implementation to the official protocol specification.
Most changes are intended to improve the overall security or simplify the daily operations business
or to support additional concepts/methods like the *EU Cyber Resilience Act (CRA)*, the *European General Data Protection Regulation (GDPR)*, the *German Calibration Law (Eichrecht)* or the *New European Measuring instruments Directive (MID)*

### 1. Crypto agility with keyGroupId, NotBefore and NotAfter in Certificate Signing Requests

In the process of a Certificate Signing Request (CSR) from Party A to Party B (UC: 01.01), Party A has the option to include option to include the following optional attributes in the CSR:
1. *keyGroupId*: A unique identification (`UTF8String`) for the cryptographic key used in the CSR.
2. *notBefore* Date: Specifies the earliest point in time when the certificate, if issued, should become valid.
3. *notAfter* Date: Specifies the expiration date after which the certificate will no longer be valid.

These attributes enhance security, availability and crypto agility by enabling Party A to manage multiple valid cryptographic keys simultaneously and ensure seamless lifecycle management of keys and certificates. Service interruptions especially by during key rotations or certificate updates can be avoided by supporting overlapping validity periods. When Party A submits a new CSR with an existing KeyId, Party B can recognize it as a request to replace the old CSR or certificate associated with that key. The old certificate should be invalidated and replaced with the new one.


### 2. Party Identifications in Certificate Signing Requests and Certificates

In the process of a Certificate Signing Request (CSR) from Party A to Party B (UC: 01.01), Party A has the option to include an optional list of all (sub-)party, CPO, or EMSP identifications as attribute data within the CSR. This information serves to provide Party B with additional context and clarity about the requesting entity and its affiliations, enabling Party B to make more informed decisions regarding whether to sign or reject the incoming CSR and thus helps to ***prevent impersonation attacks***.    
These attributes, if included, will also be copied directly into the issued X.509 Client Certificate, ensuring that the information about (sub-)party, CPO, or EMSP identifications is available during subsequent TLS handshake operations. This allows both the certificate owner and verifying parties to access this metadata at the point of establishing secure connections, streamlining trust validation processes.

The identification data is encoded using ASN.1 structures such as `SEQUENCE OF UTF8String`, allowing flexibility in representing multiple identifications in a structured and standardized format. The chosen ASN.1 OIDs for the lists of (sub-)party, CPO or EMSP identifications is still subject
of change.


### 3. Identity Keys, User Roles and Micro-PKIs within Micro Service Architectures

tba.


### 4. Signed Locations, Tariffs, Charge Details Records, ...

tba.


### 5. Energy Meters on Locations, Stations and EVSEs

tba.


### 6. Open Data Access

tba.


## Your participation

This software is free and Open Source under [**Apache 2.0 license**](LICENSE).
We appreciate your participation in this ongoing project, and your help to
improve it and the e-mobility ICT in general. If you find bugs, want to request
a feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
