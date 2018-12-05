# Demandbase Data in sitecore
This document will go over the details of how demandbase data is integrated into Sitecore as well as how you can have access to this data as part of a users session.

## Demandbase context
Installing the module comes with a context singleton object.
```cs
			DemandbaseContext.Attributes; //A list of demandbase attributes 
			DemandbaseContext.AccountWatch; //A list of watch list attributes
			DemandbaseContext.DemandbaseIp; //IP address for demandbase
			DemandbaseContext.Key; //Demandbase access key
			DemandbaseContext.Levels; //List of headquarter heirarchy levels
			DemandbaseContext.NoHq; //Boolean for if no hierarchy levels exist
			DemandbaseContext.RestApi; //Rest api to get user attributes

			DemandbaseContext.User.GetFullObject(); //Get a dynamic object representing the entirety of the Demandbase payload
			DemandbaseContext.User.GetValue<T>(AttributeId); //Get the value of a particular demandbase attribute without a headquarter heirarchy
			DemandbaseContext.User.GetSecondTeirValue<T>(HqLevel, AttributeId); //Get the value of a particular demandbase attribute with a headquarter heirarchy
```
This singleton can be utilized anywhere inside the context of a web request.

## Attribute definitions
The definition for the available Demandbase attributes are set up in the Demandbase.config file.
```xml
			<attribute customizable="0">
				<type>string</type>
				<name>Audience</name>
				<id>audience</id>
				<values>
					<value>Enterprise Business</value>
					<value>Mid-Market Business</value>
					<value>SMB</value>
					<value>Government</value>
					<value>Education</value>
					<value>Hospitality</value>
					<value>Residential</value>
					<value>Wireless</value>
					<value>Bot</value>
					<value>SOHO</value>
					<value>Obscured</value>
				</values>
			</attribute>
```
Using the attribute definintions defined within this configuration file the system automatically generates rules for Sitecore personalization.
If there are values in the configuration file that you would never care about, you can feel free to remove them to clean up the authoring experience.

Node definitions:
* values tag - Set of default values, creates a picklist for authors to choose particular values when setting up rules.
* id tag - Attribute id from Demandbase.
* name tag - User friendly name for the Demandbase attribute.
* type tag - Data type of attribute value *int, string, or bool*.
* Customizable attribute - If this is set to 1 the attribute will be included in the free form value entry rules where you define a particular string value for an attribute.  If it is set to 0 and the attribute has default values, it will be excluded from the free form rule.
