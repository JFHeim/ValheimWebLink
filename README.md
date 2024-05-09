# ValheimWebLink
 
Valheim mod that allows http comunication with valheim server.
Also gives terminal commands.

Some commands are public and some require authentication.
For authorization I'm using basic auth - username and password.
By default username is `root` and password is `root`.
You must change it before running the server.
Username and password will be updated automatically when `auth.json` file changes.
They are stored in `auth.json` in server root directory.

Mod is server side, but in than way it doesn't give too many data.
In order to get them you need to install modules.
They are little client-side + server-side mods
that gives server access to lots of client-side data with read and write access.<br>
For example `WorldObjectsData` module makes `/findobjects` request
give information about items in chests, portal names and more.<br>
`PlayerData` module makes you able to use `/playerdata/set` to change player data
and also `/playerdata/get` will give you more data about player, like inventory items, food and so on.

Mod is in early development stage, so modules do not exist yet.<br>
And if you want to use mod now, you need to write me in discord.<br>
Also if you have any questions or suggestions, write me.

Look at [Documentation](DOCUMENTATION.md) page for all requests.

## Contact
Discord: `justafrogger`