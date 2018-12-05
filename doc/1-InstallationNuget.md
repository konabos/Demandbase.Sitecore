# Installing the Sitecore Demandbase module Using the nuget feed

## Acquire the package
The Sitecore Demandbase nuget package can be found here __TODO__
Or installed via the nuget package manager using the command __TODO__

## Install the package
Using preferrably a non-production environment, install the nuget package using Visual Studio (or other like tool)

There are no sitecore items to be installed from a package.  The entirety of the module is self contained through initialize pipeline procedures.  This means if something is accidentally deleted it can be easilly repaired with a simple recyle of the app pool.

## Configure the module
Installing the nuget package will place a configuration file in sitecore at App_Config/Include called SitecoreDemandbase.config.  Inside this config is an Xml node to enter your Demandbase api access key.
