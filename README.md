Sprint 4 Changes:
- Added Google Gemini AI integration for symptom checker
- Updated UI on symptoms page
- Updated UI on graph page
- Improved popup on settings page
- Updated legend on the map to provide users with additional information about what the colors mean
- Reduced the overall number of database calls down to two on the map screen when appearing
- Improved stats page to display current default ZIP code's reported illness when going to it
- Added a feedback page for users to submit feedback regarding Mapidemic

Sprint 3 Changes:
- Added proper logic when rendering circles and corresponding color-severity. Colors and severity are determined by a percentage of reports against the population :smile: :smile: :smile:
- Added try-catch blocks around every database call to ensure a network error does not crash the app if the database cannot be reached :smile: :smile: :smile:
- Added building blocks for OpenAI to be used for symptom checking, if Dr. Rogers can get us the funding to make the api calls :smile:
- Added syncfusion ListView to display stats report to the users instead of button clicks. Updated Meet the Team page to a tab view to display team information better instead of button clicks also. :smile: :smile: :smile:
- Improved graph page functionality by adding a time range picker, as well as a locality picker to reduce confusion, as well as resolving a bug where switching app themes would not apply it to the graph. :smile: :smile: :smile:

Sprint 2 changes:
- Added color coded circles to the map showing illness density in each postal code :smile: :smile: :smile:
- Graphing functionality added to display histogram of illness data, both locally and nationally :smile: :smile: :smile:
- Report functionality added to display illness data in plain English :smile: :smile: :smile:
- Updated menu pages that conform to the users requested app theme :smile:
- Updated settings page to display the correct postal code as the entry default text :smile:

# Mapidemic
Mapidemic is a .NET MAUI mobile application that helps communities understand how illness is spreading in their area — in real time, anonymously, and without creepy tracking.

Users can:
- Report what they're sick with (e.g. flu, strep, etc.)
- Specify their ZIP code (not their exact address)
- See a live illness heatmap showing concentration of reported cases in nearby areas

Our goal is simple: give people early awareness of local outbreaks so they can make better decisions (mask up, maybe skip that crowded event, sanitize the cart handle, etc.) before they get hit.

## Table of Contents
- [Problem We're Solving](#problem-were-solving)
- [Core Features](#core-features)
- [How It Works](#how-it-works)
- [Privacy and Safety](#privacy-and-safety)
- [Setup / Running Locally](#setup-/-running-locally)
- [Planned / Future Work](#planned-/-future-work)
- [Contributors](#contributors)
- [License](#license)

## Problem We're Solving
People only find out "there's something going around" when:
- Half the office calls in sick, or
- Their kid's school emails them 4 days too late.
There's no fast, community-level view of what's spreading _right now_ in your area unless public health authorities announce it, and that's usually delayed and high-level ("flu levels are statewide").

Mapidemic solves that by letting regular people self-report illness symptoms/diagnoses and instantly visualizing that data geographically. You get hyperlocal awareness with low friction:
- Local, not national.
- Current, not last month.
- Anonymous, not invasive.

## Core Features
### 1. Heatmap / Outbreak View

The map shows color-coded "bubbles" at ZIP-level to indicate report density:
- Yellow → a few reports
- Orange → moderate activity
- Red → heavy activity
- Black → severe activity

Circles are drawn with a fixed radius (miles). We never render a tiny pinpoint. Multiple reports in the same ZIP turn that ZIP's bubble more severe.

### 2. Report Illness

User's select an illness from a searchable list, entering their ZIP code before submission. The app will validate the ZIP and submit the report to our backend. We do **not** store names, emails, device IDs, or precise GPS at submission.

