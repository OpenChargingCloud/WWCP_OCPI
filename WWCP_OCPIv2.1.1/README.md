# WWCP OCPI v2.1.1 Implementation

This software implements the [Open Charge Point Interface (OCPI)](https://github.com/ocpi/ocpi)
in C# on .NET6.

## Differences to the official protocol

The following desribes differences of this implementation to the official protocol specification.
Most changes are intended to simplify the daily operations business or to support additional concepts/methods like the *European General Data Protection Regulation (GDPR)* and the *German Calibration Law (Eichrecht)*.

### Access Tokens

Within this implementation access tokens always have an attached **role**. So an access
token might be a **CPO access token** or an **EMSP access token** or anything else. This
will help you to understand and limit the confusion when you look at the list of version
endpoints as they will be filtered by the role of your access token.

### Versions module

This implementation allows also a subset of unauthenticated HTTP requests (e.g. no access
token provided). Unauthenticated requests only see a filtered list of module endpoints
and can also use only a limited set of HTTP requests. This will for example support
**Open Data** initiatives.


### Location module

#### GET ~/locations (CPO)

A CPO might want to expose his locations also to unauthenticated requests
(e.g. no access token provided). This will for example support **Open Data** initiatives.

#### GET ~/locations/country_code/party_id

This HTTP request will allow CPOs to **request all locations** stored within an EMSP.
This will simplify the validation of all stored data.

#### DELETE ~/locations/country_code/party_id

This HTTP request will allow CPOs to **delete all locations** stored within an EMSP.
This will simplify debugging and validation of your software and infrastructure. As
this will *really* delete anything this method should not be used within daily operation
or unless you really know what you are doing and understand all side effects.


### PATCH

[PATCH Method for HTTP]( https://datatracker.ietf.org/doc/rfc5789/ ) is well-known. Yet there
exist two versions for manipulating JSON documents:

- The [JavaScript Object Notation (JSON) Patch]( https://datatracker.ietf.org/doc/rfc6902/ ) describes a PATCH document having its own methods for manipulating the JSON data structure.
- OCPI is using the so called [JSON Merge Patch]( https://datatracker.ietf.org/doc/rfc7396/ ), which is roughly speaking a JSON DIFF document.

The OCPI PATCH specification is again *not safe*. Therefore this implementation adds support for ETAGs and Timestamp


## Your participation

This software is free and Open Source under [**Apache 2.0 license**](LICENSE).
We appreciate your participation in this ongoing project, and your help to
improve it and the e-mobility ICT in general. If you find bugs, want to request
a feature or send us a pull request, feel free to use the normal GitHub
features to do so. For this please read the Contributor License Agreement
carefully and send us a signed copy or use a similar free and open license.
