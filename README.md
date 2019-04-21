# Theatreers Show API

Show API used as part of the Theatreers Project

| CI Pipeline                                                                                                                                                                                                                       | Nightly Pipeline                                                                                                                                                                                                                       |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [![Build Status](https://dev.azure.com/theatreers/Theatreers/_apis/build/status/Application/Theatreers.Show%20CI?branchName=master)](https://dev.azure.com/theatreers/Theatreers/_build/latest?definitionId=46&branchName=master) | [![Build Status](https://dev.azure.com/theatreers/Theatreers/_apis/build/status/Application/Theatreers.Show%20Nightly?branchName=master)](https://dev.azure.com/theatreers/Theatreers/_build/latest?definitionId=55&branchName=master) |
|                                                                                                                                                                                                                                   | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Theatreers.Show&metric=alert_status)](https://sonarcloud.io/dashboard?id=Theatreers.Show)                                                             |

## Getting Started

This is the codebase for the Theatreers Show API. THe Theatreers project is split into a number of Microservices, This particular microservices deals with shows as part of the solution, and any associated downstream processing.

### Building

You can leverage the YAML Files in the pipelines folder. This will allow you to build the code and generate a zip file.

You will need to make some changes for your own local pipelines, and change the SonarCloud Project and Key, if you decide to track using that route.

## Deployment

Once you have the zip file, you will be able to push this to Azure Functions.

## Contributing

Please read [CONTRIBUTING.md](contributing.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Christian Reddington** - [Christian Reddington](https://github.com/christianreddington)
