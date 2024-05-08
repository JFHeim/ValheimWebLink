# ValheimWebLink

Everything listed tested and works nice. Even tested on real linux server with public ip. 
For authorization I'm using basic (username and password). They are stored in auth.json in server root dir


## Http requests
* GET /avaliblecommands - Returns all known ingame console commands
* POST /execute - Execute known ingame terminal command. Requires authorization
* GET /getfulllog - Returns logs from the log file
* GET / - Returns all controllers info
* GET /serverinfo - Returns vast server info

## In terminal commands
This commands you can run right from terminal, I mean real one, not ingame one.
* vwl help - help message
* vwl clear - clear the console
* vwl stop - stops listening for requests
* vwl getport - prints the port number
* vwl restart - restarts the http server