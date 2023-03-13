# headlines
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=PetrBilek1_headlines&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=PetrBilek1_headlines)

**Headlines is a web application for tracking articles and their headlines.**

Can be seen here: https://titulkovac.pbilek.eu/

### Technologies used:
**Frontend:**
 - **Vue.js**
 - WebSockets

**Backend:**
 - **ASPNET Core**
 - **Entity Framework Core**
 - **Microsoft SQL**
 - **RabbitMQ** + MassTransit
 - **Redis**
 - **Cloud Object Storage**

**Testing:**
 - [XUnit](https://github.com/xunit/xunit)
 - [Testcontainers](https://github.com/testcontainers/testcontainers-dotnet) - library to support tests with throwaway instances of Docker containers
 - [FluentAssertions](https://github.com/fluentassertions/fluentassertions) - enhanced assertion methods
 - [Moq](https://github.com/moq/moq4) - mocking framework
 - [Bogus](https://github.com/bchavez/Bogus) - fake data generator
 - [Stryker](https://github.com/stryker-mutator/stryker-net) - mutation testing

**Dev Ops:**
 - **Docker**
 - **Kubernetes**
