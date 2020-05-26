# Injecting demandbase attributes into rich text
This Step is optional, it allows you to inject whatever demandbase attributes are associated with the current user into rich text. Through integration with another 3rd party module Token Manager

## Token Manager
The Sitecore [Tokenmanager](https://github.com/JeffDarchuk/SCTokenManager) is a Sitecore Module that allows content authors to inject dynamic or managed content into a rich text field.
This is particularly useful for Demandbase as it allows us to inject demandbase data into our content.

## Installing Token Manager using Sitecore Package
Token Manager is available to install via Sitecore package available with Demandabase, that you can Install following these steps:
Using preferrably a non-production environment, install the module package using Sitecore's Development Tools > Installation Wizard.
* Upload package to sitecore
* Run installation procedure

## Installing Token Manager using Nuget
Token Manager is available to install via [nuget](https://www.nuget.org/packages/TokenManager) using visual studio or the package manager console command: Install-Package Token Manager.
Install into your web solution and make sure the dll is in the bin folder and configuration file (tokens.config) is under App_Config/Include.

## Integrating with Demandbase
There is nothing to do here, Tokenmanager will detect the token definitions which are contained within the Demandbase module and wire up the tokens for use automatically.  So simply install Token Manager and you're done.

## Injecting Content
After installing there will be a new button in the rich text editor that looks like an orange lightning bold.  If you click this button you will be shown a form to fill out for which attribute you would like to inject onto the page.
During edit mode this will show up as a blue highlighted placeholder reporting what attribute will be returned.  Once outside of edit mode it will be resolved into a value for the particular attribute based on the user viewing the page.
