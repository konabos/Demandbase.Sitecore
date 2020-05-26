# Configuring Demandbase
There are a few configuration options available.  Many of which will never need to be modified, however it is imporant to understand what the options do
```xml
<?xml version="1.0"?>
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
	<sitecore>
		<!-- 
			Uncomment model node for Xdb storage of demandbase data
		-->
		<model>
			<elements>
				<element interface="SitecoreDemandbase.Data.Interface.IXdbFacetDemandbaseData, SitecoreDemandbase" implementation="SitecoreDemandbase.Data.XdbFacetDemandbaseData, SitecoreDemandbase" />
			</elements>
			<entities>
				<contact>
					<facets>
						<facet name="Demandbase Data" contract="SitecoreDemandbase.Data.XdbFacetDemandbaseData, SitecoreDemandbase" />
					</facets>
				</contact>
			</entities>
		</model>
		<pipelines>
			<httpRequestBegin>
				<processor patch:after="*[@type='Sitecore.Pipelines.HttpRequest.EnsureServerUrl, Sitecore.Kernel']" type="SitecoreDemandbase.Pipeline.HttpRequestBegin.ValidateUser, SitecoreDemandbase" />
			</httpRequestBegin>
			<initialize>
				<processor type="SitecoreDemandbase.Pipeline.Initialize.InitializeDemandbase, SitecoreDemandbase" >
					<param name="restApi">http://api.demandbase.com/api/v2/ip.json</param>
					<param name="key">Enter_your_demandbase_key_here</param>
					<param name="demandbaseIp">4.16.87.224</param>
					<!-- 
						Uncomment following for session storage of demandbase data
					-->
					<!--<UserService type="SitecoreDemandbase.Data.SessionUserData, SitecoreDemandbase">
						<Timeout>500</Timeout>
					</UserService>-->
					<!-- 
						Uncomment following for Xdb storage of demandbase data
					-->
					<UserService type="SitecoreDemandbase.Data.XdbUserData, SitecoreDemandbase">
						<Timeout>500</Timeout>
					</UserService>
					<!--
						You can remove the following two XML nodes to manually manage Demandbase Rules.  They are located here /sitecore/system/Settings/Rules/Definitions/Demandbase
					-->
					<!-- this is where the Demandbase attributes are defined, omitted for space concerns -->
				</processor>
			</initialize>
		</pipelines>
		<commands>
			<command name="demandbase:mockip" type="SitecoreDemandbase.Commands.MockIp, SitecoreDemandbase" />
		</commands>
	</sitecore>
</configuration>
```

## Misc Settings
Params
* RestApi - The version of the rest api from Demandbase being used to gather data on the user.
* Key - The Demandbase api access key **Very important to set this up right away**.
* demandbaseIp - The IP address to Demandbase, this is utilized to dynamically determine account settings.

## Utilizing XDB (default)
There are two optional configuration nodes that are enabled by default to utilize XDB as a storage medium for Demandbase data.
* Uncomment Model node at the top of the file.
* Comment out UserService node of type SitecoreDemandbase.Data.SessionUserData
* Uncomment UserService node of type SitecoreDemandbase.Data.XdbUserData.

## Utilizing SessionUserData
To enable Session for environments without XDB 
* Comment out the Model node at the top of file.
* Comment out UserService node with the type SitecoreDemandbase.Data.XdbUserData.
* Uncomment out UserService node with the type SitecoreDemandbase.Data.SessionUserData.
