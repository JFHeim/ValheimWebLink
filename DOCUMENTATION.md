# Documentation
## Controllers
### /execute
Description: Execute known ingame terminal command. Requires authentication. Returns logs of command execution.<br>
HttpMethod: POST<br>
Query parameters
* command<br>
    Description: Command to execute<br>
    Type: string<br>


### /
Description: Returns all controllers info<br>
HttpMethod: GET<br>
Has no query parameters

### /serverinfo
Description: Returns vast server info<br>
HttpMethod: GET<br>
Has no query parameters

### /getfulllog
Description: Returns logs from the log file<br>
HttpMethod: GET<br>
Has no query parameters

### /avaliblecommands
Description: Returns all known ingame console commands<br>
HttpMethod: GET<br>
Has no query parameters

### /findobjects
Description: Searches for objects in the world in given range. To see full data of objects, install WorldObjectsData module.<br>
HttpMethod: GET<br>
Query parameters
* centerpoint<br>
    Description: Center point of the search<br>
    Type: Vector2<br>

* radius<br>
    Description: Radius of the search<br>
    Type: float<br>


### /playerdata/set
Description: Provides way to override player data. Requires authentication. Requires PlayerData module installed.<br>
HttpMethod: POST<br>
Query parameters
* name<br>
    Description: player name<br>
    Type: string<br>

* data<br>
    Description: player data<br>
    Type: json<br>


### /playerdata/get
Description: Returns detail information about the player. Requires authentication. To get more data about the player, install PlayerData module.<br>
HttpMethod: GET<br>
Query parameters
* name<br>
    Description: Name of the player<br>
    Type: string<br>


