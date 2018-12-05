# Installing the Sitecore Demandbase module using the Sitecore package

## Acquire the package
The Sitecore Demandbase plugin can be found on the marketplace.

## Install the package
Using preferrably a non-production environment, install the module package using Sitecore's Development Tools > Installation Wizard.
* Upload package to sitecore
* Run installation procedure

There are no sitecore items involved in the module package.  The entirety of the module is self contained through initialize pipeline procedures.  This means if something is accidentally deleted it can be easilly repaired with a simple recyle of the app pool.

## Configure the module
Installing the module will place a configuration file in sitecore at App_Config/Include called SitecoreDemandbase.config.  Inside this config is an Xml node to enter your Demandbase api access key.
