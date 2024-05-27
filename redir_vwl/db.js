import fs from "fs";

let data = {
  servers: [
    {
      id: "unique_guid",
      ip: "127.0.0.1",
      port: 2456,
      name: "My Server",
      description: "My server description",
      users: [
        {
          username: "root",
          password: "root",
          permissions: [
            "READ_generalinfo",
            "WRITE_generalinfo",
            "READ_log",
            "READ_adminlist",
            "WRITE_adminlist",
            "DELETE_adminlist",
            "READ_banlist",
            "WRITE_banlist",
            "DELETE_banlist",
            "READ_modlist",
            "READ_players",
            "READ_objects",
            "WRITE_objects",
            "CommandExecute",
          ],
        },
      ],
    },
  ],
};
data = { servers: [] };

fs.readFile("db.json", "utf8", (err, data) => {
  if (err) {
    console.error(err);
    return;
  }

  try {
    data = JSON.parse(data);
    console.log(data);
  } catch (error) {
    console.error("Error parsing JSON:", error);
  }
});

setInterval(() => {
  if (!data) return;
  fs.writeFile("db.json", JSON.stringify(data), (err) => {
    if (err) console.error(`Error writing to db.json: ${err}`);
  });
}, 5000);

export const getServers = () => data.servers;

export const getServerByName = (name) =>
  data.servers.find((server) => server.name === name);

export const getServerByIp = (ip, port) =>
  data.servers.find((server) => server.ip === ip && server.port === port);

export const getServerById = (id) =>
  data.servers.find((server) => server.id === id);

export const addServer = (serverInfo) => {
  if (!data)
    return {
      ok: false,
      error: "Database is empty, this should not happen",
    };
  if (!serverInfo.id)
    return {
      ok: false,
      error: "No id provided",
    };
  if (getServerById(serverInfo.id))
    return {
      ok: false,
      error: `Server with id ${serverInfo.id} already exists`,
    };
  if (getServerByIp(serverInfo.ip, serverInfo.port))
    return {
      ok: false,
      error: `Server with ip ${serverInfo.ip} and port ${serverInfo.port} already exists`,
    };
  if (getServerByName(serverInfo.name))
    return {
      ok: false,
      error: `Server with name ${serverInfo.name} already exists`,
    };

  console.log(`Added server ${serverInfo.id}: ${JSON.stringify(serverInfo)}`);
  data.servers.push(serverInfo);
  return {
    ok: true,
    error: "",
  };
};

export const updateServer = (id, serverInfo) => {
  if (serverInfo.id) delete serverInfo.id;
  if (!data)
    return {
      ok: false,
      error: "Database is empty, this should not happen",
    };
  let server = getServerById(id);
  if (!server)
    return {
      ok: false,
      error: `Server with id ${id} not found`,
    };

  const oldServerInfo = { ...server };
  server = { ...server, ...serverInfo };

  console.log(
    `Updated server ${id}: 
    ${JSON.stringify(oldServerInfo)} -> ${JSON.stringify(server)}`
  );

  return {
    ok: true,
    error: "",
  };
};
