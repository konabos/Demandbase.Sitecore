# Configuring Analytics
Sitecore out of the box doesn't provide an easy way to track metrics on personalization value.  
Part of this module is a system to generate a metatag on a page that's dynamically generated to match the personalization context.
This metatag is then usable by any tag management tool such as Google Tag Manager to track value of visits based on the personalization state.

## Personalization Context
A static class provides a method of outputting a dynamic metatag to represent a state of personalization.
Here is an example usage in a cshtml context.
```cs
@Html.Raw(PersonalizationContext.GenerateMetaTags())
```
Which will then generate a meta tag like the following
```xml
<meta name="sitecorePersonalization" content="B2B,High Traffic">
```
In the above tag, this is a representation of a page that has two personalization blocks that evaluated to true, "B2B" and "High Traffic".
These names are assigned when the user is setting up a personalization state for a component.

For example, to set up the values in this meta tag make sure you note the process for setting up component personalization
* Choose a component to personalize
* Open personalization form
* Add a personalization state
* Give this state a name **this is the field that ends up in the meta tag**
* Add rules

> **Note that this meta tag isn't unique to demandbase rules, but applies to all personalization rules in sitecore**
