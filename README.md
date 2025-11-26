WWCP OCPI
=========

This software will allow the communication between World Wide Charging Protocol (WWCP) entities and entities implementing the [Open Charge Point Interface (OCPI)](https://github.com/ocpi/ocpi) which is defined and used by the [EVRoaming Foundation](https://evroaming.org). The focus of this protocol is a scalable, automated EV roaming setup between Charge Point Operators (EVSE Operators) and e-Mobility Service Providers. The focus of this protocol is communication within a **peer-to-peer** topology of CPOs, EMPs and other actors, but since v2.2 also roaming hubs are defined.


## Implementation Details and Differences

The following desribes differences of this implementation to the official protocol specification.
Most changes are intended to simplify the daily operations business or to support additional concepts/methods like the *European General Data Protection Regulation (GDPR)*, the *German Calibration Law (Eichrecht)*, the *Alternative Fuels Infrastructure Regulation (AFIR)* or the *Public Charge Point Regulations 2023* within the United Kingdom.

- The [OCPI v2.1.x ](WWCP_OCPIv2.1.1/README.md) focuses on simple *"one peer connected to one other peer"*-topologies, but besides its simplicity the specification has unfortunatelly some design flaws. This implementation provides extensions and work-arounds for most of these issues to simplify the daily operations business, high availability or to support additional concepts/methods like *European General Data Protection Regulation (GDPR)* and the *German Calibration Law (Eichrecht)*.

- The [OCPI v2.2.x ](WWCP_OCPIv2.2.1/README.md) extends its focus on many *"CPOs/EMPs on one peer"*-topologies and introduces a *"hub*"-concept like OICP. This specification is unfortunatelly more a hack, has many design flaws and security issues. This implementation provides extensions and work-arounds for most of these issues to simplify the daily operations business, high availability or to support additional concepts/methods like *European General Data Protection Regulation (GDPR)* and the *German Calibration Law (Eichrecht)*.

- The [OCPI v2.3](WWCP_OCPIv2.3/README.md) is a simple data structures extension to v2.2.1. So the same problems exist and are still unsolved. This implementation provides extensions and work-arounds for most of these issues to simplify the daily operations business, high availability or to support additional concepts/methods like *European General Data Protection Regulation (GDPR)*, the *German Calibration Law (Eichrecht)*, *EU Alternative Fuels Infrastructure Regulation (AFIR)* and the *UK's Public Charge Point Regulations 2023*.

- The [OCPI v3.0](WWCP_OCPIv2.0/README.md) is very different to the v2.x versions of the protocol specification, closer to WWCP, but still has many (new) design flaws and security issues. This implementation provides extensions and work-arounds for most of these issues to simplify the daily operations business, high availability or to support additional concepts/methods like *European General Data Protection Regulation (GDPR)*, the *German Calibration Law (Eichrecht)*, *EU Alternative Fuels Infrastructure Regulation (AFIR)* and the *UK's Public Charge Point Regulations 2023*.    
**Note:** OCPI v3.0 has not been officially released yet. The latest draft, *"OCPI v3.0-2 review2, 2024-02-22"*, still contains numerous issues. As a result, this implementation should be considered a technology preview rather than a production-ready solution.


## 2nd-Secret Authentication via Time-based One-Time Passwords (TOTPs)

The OCPI security model is fundamentally fragile. In practice it boils down to a single static token plus the legacy CREDENTIALS module, a combination that has been known to be unsafe since OCPI v2.2. This implementation addresses that weakness by introducing a second, independent secret derived from a Time-based One-Time Password (TOTP).

Instead of relying on a long-lived access token alone, an additional HTTP header carries a short-lived TOTP value that rotates (for example) every 30 seconds. The static OCPI token effectively becomes the identifier (similar to a login or username), while the TOTP functions as the ephemeral time-bound secret or password.

Together this yields a *two-secret* handshake on every OCPI request, removes the single-secret failure mode, and enables proper replay-protection and token-theft mitigation without breaking OCPI interoperability.

*Note:* This is a *Two-Secret Scheme*, not a real *2nd, Two-Factor (2FA) or Multi-Factor Authentication Scheme (MFA)*, as the TOTP is not based on a different factor class *(“something you know”, “something you have”, “something you are”)*. This TOTP-based two-secret scheme delivers a pragmatic, interoperable, CRA-aligned hardening of OCPI that directly mitigates the most critical weaknesses of the existing token-only model, without waiting for an unlikely consensus on mutual TLS (mTLS) adoption.

### Alignment with EU CRA and EU NIS2

This TOTP extension directly supports several technical obligations under the upcoming *EU Cyber Resilience Act (CRA)* and the *NIS2 Directive*, especially for manufacturers and operators of EV charging infrastructure:

- **CRA Article 10: “Security by Design”**
The native OCPI token model provides no replay protection and no mitigation against token theft. Adding per-request TOTP implements time-bound authentication, a recognized ***state-of-the-art security-by-design*** measure. Long-lived single static tokens are explicitly called out in *ENISA* and *BSI guidance* as not meeting state-of-the-art criteria.

- **CRA Article 15: “Vulnerability Handling & Mitigation”**
Static tokens are single points of failure. With TOTPs, a stolen token is insufficient to access the system, reducing the severity of credential-leak vulnerabilities and improving post-incident containment.

- **CRA Annex I, Essential Requirement 1 & 2**
These require protection against unauthorized access and prevention of common attack vectors. Short-lived TOTPs reduce the attack surface dramatically without requiring changes to OCPI message schemas.

- **NIS2: Risk-based Security Controls for Essential/Important Entities**
CPO/EMSP systems increasingly fall under NIS2 supervision (energy sector). Deploying short-lived, replay-resistant request authentication is aligned with required security hardening and incident-minimization strategies.

- **NIS2 Article 21(2)(d): Strong Authentication**
Article 21 mandates risk-based cybersecurity measures including “access control policies based on state-of-the-art authentication mechanisms” and “protection against stolen or compromised credentials”.
This TOTP authentication is a compliant step towards stronger authentication mechanisms by removing static credentials from the threat landscape.    
National NIS2 implementations (Germany: KRITIS-DA, Netherlands: NEN-7510 extensions, France: ANSSI référentiel) explicitly cite TOTP, short-lived tokens, or hardware-based second factors as acceptable measures.

- **CRA/NIS2: Operational Monitoring and Auditability**
Every couple of seconds all requests contain a fresh TOTP. This simplifies anomaly detection (invalid TOTPs, repeated attempts, replay patterns) and aligns with CRA/NIS2 expectations for continuous monitoring of security-relevant events.


## Open Data by Default

This OCPI implementation includes an optional feature to enable anonymous, unauthenticated access to the VERSIONS, LOCATIONS, and TARIFFS endpoints via a simple WebAPI. By activating this feature, you can ensure compliance with regulatory requirements such as the *EU Alternative Fuels Infrastructure Regulation (AFIR)* and the *UK's Public Charge Point Regulations 2023*.

This capability facilitates the provision of open and transparent access to essential data about charging station availability, locations, and pricing, aligning with the mandates for publicly accessible information to promote interoperability and user convenience. Additionally, this feature simplifies integration with third-party platforms and services that rely on open access to charging infrastructure data, supporting a seamless user experience while adhering to legal standards.


### Your participation

This software is free and Open Source under [GNU Affero General Public License (AGPL)](LICENSE).
We appreciate your participation in this ongoing project, and your help to
improve it and the e-mobility ICT in general. If you find bugs, want to request
a feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
