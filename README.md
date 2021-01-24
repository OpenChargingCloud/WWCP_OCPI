WWCP OCPI v2.2
==============

This software will allow the communication between World Wide Charging
Protocol (WWCP) entities and entities implementing the
[Open Charge Point Interface (OCPI)](https://github.com/ocpi/ocpi) which
is defined and used by the [EVRoaming Foundation](https://evroaming.org).
The focus of this protocol is a scalable, automated EV roaming setup
between Charge Point Operators (EVSE Operators) and e-Mobility Service
Providers. The focus of this protocol is a **peer-to-peer** operation, but
since version 2.2 also roaming hubs are defined.

## Content

- [Implementation Details and Differences](WWCP_OCPIv2.2/README.md) to the official protocol specification. The OCPI specification has unfortunatelly many flaws and security issues. This implementation provides extentions and work-arounds for most of these issues to simplify the daily operations business, high availability or to support additional concepts/methods like *European General Data Protection Regulation (GDPR)*  and the *German Calibration Law (Eichrecht)*.

- We at Open Charging Cloud love Open Source and [Open Data](OpenData.md). Therefore we provide an extention to the OCPI specification allowing Charge Point Operators (Charging Stations Operators) to provide their charging locations as Open Data.

### Your participation

This software is free and Open Source under [**Apache 2.0 license**](LICENSE).
We appreciate your participation in this ongoing project, and your help to
improve it and the e-mobility ICT in general. If you find bugs, want to request
a feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
