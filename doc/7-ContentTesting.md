# Testing content
There are a few things to keep in mind while testing content that is personalized by Demandbase attributes

## Testing attribute based personalizations
There is a widget included in preview mode to simulate a particular ip.  With this we can render the page as it would be seen from a particular consumers ip.
This works by setting a cookie in your browser, once you have the cookie set you will impersonate that ip until you erase it from the widget or clear your cookies.
However an important note is that in order to test personalization you need to be in normal rendering mode.
You can enter normal mode by changing the sc_mode query string parameter to sc_mode=normal.
