# dfc-app-discoverskillsandcareers

This project provides questions and records answers. It is designed to be used in conjunction with the Composite UI (Shell application), Service Taxonomy and Job Profiles.

Details of this app may be found on [github](https://github.com/SkillsFundingAgency/dfc-app-discoverskillsandcareers) and on [confluence](https://skillsfundingagency.atlassian.net/wiki/spaces/DFC/pages/2043019322/CUI+discover+your+skills+and+careers).

Details of the Composite UI application may be found on [github](https://github.com/SkillsFundingAgency/dfc-composite-shell) and on [confluence](https://skillsfundingagency.atlassian.net/wiki/spaces/DFC/pages/1878557026/CUI+app+development).

## Getting Started

This is a self-contained Visual Studio 2019 solution containing a number of projects (web application, service and repository layers, with associated unit test and integration test projects).

### Installing

Clone the project and open the solution in Visual Studio 2019.

## List of dependencies

|Item	|Purpose|
|-------|-------|
|Azure Cosmos DB | Document storage |
|[Comp UI App Registry](https://github.com/SkillsFundingAgency/dfc-api-appRegistry) | Running the application | 
|[Comp UI Shell](https://github.com/SkillsFundingAgency/dfc-composite-shell) | Running the application |
|[Content API](https://github.com/SkillsFundingAgency/dfc-api-content) | Load DYSaC data into cache from Service Taxonomy |
|Job Profiles API | Load Job Profile Overview for Results screen |

## Local Config Files

Once you have cloned the public repo you need to rename the appsettings files by removing the -template part from the configuration file names listed below.

| Location | Repo Filename | Rename to |
|-------|-------|-------|
| DFC.App.DiscoverSkillsCareers.UnitTests | appsettings-template.json | appsettings.json |
| DFC.App.DiscoverSkillsCareers | appsettings-template.json | appsettings.json |

## Configuring to run locally

The project contains a number of "appsettings-template.json" files which contain sample appsettings for the web app and the test projects. To use these files, rename them to "appsettings.json" and edit and replace the configuration item values with values suitable for your environment.

By default, the appsettings include a local Azure Cosmos Emulator configuration using the well known configuration values. These may be changed to suit your environment if you are not using the Azure Cosmos Emulator.

### App Registration Document

The app registration document you'll need is...

```
{
    "PartitionKey": "discover-your-skills-and-careers",
    "id": "77674735-ce07-4f83-80b2-68244cec2990",
    "Path": "discover-your-skills-and-careers",
    "TopNavigationText": null,
    "TopNavigationOrder": 2000,
    "Layout": 1,
    "IsOnline": true,
    "OfflineHtml": "<div class=\"govuk-width-container\"><H2>Discover your skills and careers is unavailable, Please try again later</H2></div>",
    "PhaseBannerHtml": "<div  class=\"govuk-phase-banner\"><p class=\"govuk-phase-banner__content\"><strong class=\"govuk-tag govuk-phase-bannertag \">beta</strong> <span class=\"govuk-phase-banner__text\">Complete an independent survey to <a target=\"_blank\" class=\"govuk-link\" href=\"https://online.ipsosinteractive.com/mriweb/mriweb.dll?i.project=S23006601&id=&I.user1=1\">give us feedback</a> about our website.</span> </p></div>",
    "SitemapURL": null,
    "ExternalURL": null,
    "RobotsURL": null,
    "DateOfRegistration": "2020-09-02T10:02:12.0188938+01:00",
    "LastModifiedDate": "2020-09-02T10:02:12.0188938+01:00",
    "Regions": [
        {
            "PageRegion": 1,
            "IsHealthy": true,
            "RegionEndpoint": "https://localhost:44342/head",
            "HealthCheckRequired": false,
            "OfflineHtml": null,
            "DateOfRegistration": "2020-09-02T09:02:12.261465Z",
            "LastModifiedDate": "2020-09-02T09:02:12.2614653Z"
        },
        {
            "PageRegion": 4,
            "IsHealthy": true,
            "RegionEndpoint": "https://localhost:44342/discover-your-skills-and-careers",
            "HealthCheckRequired": false,
            "OfflineHtml": null,
            "DateOfRegistration": "2020-09-02T09:02:12.4636297Z",
            "LastModifiedDate": "2020-09-02T09:02:12.46363Z"
        },
        {
            "PageRegion": 3,
            "IsHealthy": true,
            "RegionEndpoint": "https://localhost:44342/bodytop",
            "HealthCheckRequired": false,
            "OfflineHtml": null,
            "DateOfRegistration": "2020-09-02T09:02:12.6366664Z",
            "LastModifiedDate": "2020-09-02T09:02:12.6366667Z"
        },
        {
            "PageRegion": 7,
            "IsHealthy": true,
            "RegionEndpoint": "https://localhost:44342/bodyfooter",
            "HealthCheckRequired": false,
            "OfflineHtml": null,
            "DateOfRegistration": "2020-09-02T09:02:12.8341235Z",
            "LastModifiedDate": "2020-09-02T09:02:12.8341237Z"
        },
        {
            "PageRegion": 8,
            "IsHealthy": true,
            "RegionEndpoint": "https://localhost:44342/herobanner",
            "HealthCheckRequired": false,
            "OfflineHtml": null,
            "DateOfRegistration": "2020-09-02T09:02:13.0126189Z",
            "LastModifiedDate": "2020-09-02T09:02:13.0126193Z"
        }
    ],
    "Banners": null,
    "PageLocations": null,
    "TraceId": "87a72f1e51dc194ebe8166a11b60dafc",
    "ParentId": "87a72f1e51dc194ebe8166a11b60dafc",
    "_etag": "\"00000000-0000-0000-9563-ddcd924e01d6\"",
    "_rid": "TOMkAMLahD8FAAAAAAAAAA==",
    "_self": "dbs/TOMkAA==/colls/TOMkAMLahD8=/docs/TOMkAMLahD8FAAAAAAAAAA==/",
    "_attachments": "attachments/",
    "_ts": 1601275920
}
```

## Running locally

Before running the app, ensure that the CosmosDB emulator is running.

### Running locally without the shell

This app is designed to be run from within the composite shell. You can run this app outside of the shell, and it will show simple views of the data.

If [running locally without the shell](https://skillsfundingagency.atlassian.net/wiki/spaces/DFC/pages/2025029780/Running+Child+apps+locally+without+the+Shell), you'll have to [set a header](https://skillsfundingagency.atlassian.net/wiki/spaces/DFC/pages/2211217409/3rd+Party+tools+emulators+assets#Chrome-Extensions) for the [session](https://skillsfundingagency.atlassian.net/wiki/spaces/DFC/pages/2026733634/CUI+Session+state).

### Running locally in the shell

To run this product locally, you will need to configure and run the list of dependencies.

Run the solutions in this order...

dfc-api-appregistry
dfc-composite-shell
dfc-api-content
dfc-app-discoverskillscareers

Once configured and the configuration files updated, it should be F5 to run and debug locally. The application can be run using IIS Express or full IIS.

To run the project, start the web application. Once running, browse to the main entrypoint which is the "https://localhost:44342/dysac".

This will allow you start a new assessment or you can return to a existing assessment.

## Deployments

This app will be deployed as an individual deployment for consumption by the Composite UI.

## Assets

CSS, JS, images and fonts used in this site can found in the following repository https://github.com/SkillsFundingAgency/dfc-digital-assets

## Built With

* Microsoft Visual Studio 2019
* .Net Core 3.1
