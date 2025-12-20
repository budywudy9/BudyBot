# BudyBot
Custom Twitch chat integration. Runs via user client "chatting" as either the user or a custom bot account.

Decided I want to create my own Twitch chat bot to have full control over my own integration.
I plan on continually updating this and adding new and more complex features as I go.

Feel free to use this code to create and run your own bot, though I wouldn't recommend it. Currently it requires manual setup of a MariaDB database and I have not implemented a method of adding credentials without hardcoding.

## Current Features
- it runs
- basically just that
- greets first-time chatters*
- allows "quotes" to be saved and randomly sent again
  - `!quote` - replies with a random saved quote
  - `!quote <user>` - replies with a random saved quote from `<user>` or an error message if none exist. 
    - Replies in the format `"<quote>" - @<user>, <date>`
  - `!quote <user> <message>` - save a quote from `<user>`
- tracks total messages sent by the user*<br>
<sub><sup>*for messages sent while the bot is active as there is no way, that I currently know of, to grab and process historic chat data of a user/ in general.</sup></sub>

## Future Features
- Migration of my NightBot commands
  - !13k, !slay, !hairflip, !banned, !kpop, !jdplus, !avatar, !emotes, !commands, !rules
- Automatically say promo etc. every x minutes
- Song request queue
- Differentiate for osu! SRs vs other games e.g. Just Dance, Muse Dash
  - May be best to use a system that finds the exact map/ map ID for osu! since I may not have the map and there may be multiple with the same name
- Integrate with OBS to visually display SR queue including who requested it
- Extend integration with additional features e.g. keybinds for switching custom overlays, sound effects, visual effects, etc.
  - This could easily be its own standalone thing and very well may become one if I get to it since its kinda really just the a virtual Stream Deck...
- Rudimentary UI
- Rework it to monitor multiple channels and keep it running on a personal server in case I ever add it to any other channels
- Whatever other features I think up knowing full well I probably won't even finish half of this lmao

