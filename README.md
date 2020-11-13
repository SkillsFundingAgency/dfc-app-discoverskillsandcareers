# dfc-app-discoverskillsandcareers
This project provides questions and records answers. It is designed to be used in connjunction with the Composite UI (Shell application), Service Taxonomy and Job Profiles

Details of the this app may be found here https://github.com/SkillsFundingAgency/dfc-app-discoverskillsandcareers

Details of the Composite UI application may be found here https://github.com/SkillsFundingAgency/dfc-composite-shell

## Getting Started

This is a self-contained Visual Studio 2019 solution containing a number of projects (web application, service and repository layers, with associated unit test and integration test projects).

### Installing

Clone the project and open the solution in Visual Studio 2019.

## List of dependencies

|Item	|Purpose|
|-------|-------|
|Azure Cosmos DB | Document storage |
|Comp UI App Registry | Running the application | 
|Comp UI Shell | Running the application |
|Content API | Load DYSaC data into cache from Service Taxonomy |
|Job Profiles API | Load Job Profile Overview for Results screen |

## Local Config Files

Once you have cloned the public repo you need to rename the appsettings files by removing the -template part from the configuration file names listed below.

| Location | Repo Filename | Rename to |
|-------|-------|-------|
| DFC.App.DiscoverSkillsCareers.UnitTests | appsettings-template.json | appsettings.json |
| DFC.App.DiscoverSkillsCareers | appsettings-template.json | appsettings.json |

## Configuring to run locally

The project contains a number of "appsettings-template.json" files which contain sample appsettings for the web app and the test projects. To use these files, rename them to "appsettings.json" and edit and replace the configuration item values with values suitable for your environment.

By default, the appsettings include a local Azure Cosmos Emulator configuration using the well known configuration values. These may be changed to suit your environment if you are not using the Azure Cosmos Emulator. In addition, Sitefinity configuration settings will need to be edited.

## Running locally

To run this product locally, you will need to configure the list of dependencies. Once configured and the configuration files updated, it should be F5 to run and debug locally. The application can be run using IIS Express or full IIS.

To run the project, start the web application. Once running, browse to the main entrypoint which is the "https://localhost:44342/dysac". 

This will list allow you start a new assessment or you can return to a existing assessment.

This app is designed to be run from within the composite app. However running this app outside of the other apps will only show simple views of the data.

## Deployments

This app will be deployed as an individual deployment for consumption by the Composite UI.

## Assets

CSS, JS, images and fonts used in this site can found in the following repository https://github.com/SkillsFundingAgency/dfc-digital-assets

## Built With

* Microsoft Visual Studio 2019
* .Net Core 3.1

## References

Please refer to https://github.com/SkillsFundingAgency/dfc-digital for additional instructions on configuring individual components like Sitefinity and Cosmos.