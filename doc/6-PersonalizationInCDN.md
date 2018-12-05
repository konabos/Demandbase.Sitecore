# Utiilzing personalization through a CDN

## Understanding CDNs
What it boils down to is a CDN caches the HTML output and whenever anyone requests a page it simply returns a flat HTML version of your page.
This is amazingly powerful when it comes to getting fast response times, however it's almost completely incompatable with any form of dynamic content with a few exceptions.

### Pass through
CDNs will give the ability to white list pages that will always retrieve content from the source, in our case Sitecore.  This requires foresight from content authors as well as cooperation from the team managing the CDN to white list any pages that are going to be personalized before the personalization is live.

It's very important that you make sure the white list of a page is intact before setting up personalization, otherwise when the CDN refreshes the page cache for a particular user it faces the risk of caching personalized content.  At which point you could be delivering content for a specific target audience to everyone.

### Javascript
While significantly more complex, you can utilize javascript to dynamically load personalized content. 
Sitecore doesn't provide a solution for this out of the box, but if needed the steps to accomplish this would be.
* Create an endpoint for personalization
* white list this endpoint to pass through the CDN
* Patch into Sitecore's conditional rendering engine to place an identifier instead of resolving the personalized placeholder
* On page load detect these identifiers and send them to your endpoint including the page guid and placeholder attempting to render
* Your endpoint will take the page id, as well as placeholder and return personalized html
* Your javascript will then replace the identifiers with the HTML returned from the endpoint

### Recommendation
due to the simplicity, we would recommend the pass through of personalized pages approach.
