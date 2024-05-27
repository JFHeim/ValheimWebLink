import { addServer } from "../db.js";

export default function registerServer(req, res) {
  let { id, ip, port, name, description, users } = req.query;

  const serverInfo = {
    id: id,
    ip: ip,
    port: port,
    name: name,
    description: description,
    users: users,
  };

  if (!id) {
    res.status(400);
    res.json({ error: "No id provided" });
    return;
  }

  if (!ip) {
    res.status(400);
    res.json({ error: "No ip provided" });
    return;
  }

  if (!port) {
    res.status(400);
    res.json({ error: "No port provided" });
    return;
  }

  if (!name) {
    res.status(400);
    res.json({ error: "No name provided" });
    return;
  }

  if (!description) {
    res.status(400);
    res.json({ error: "No description provided" });
    return;
  }

  if (!users) {
    res.status(400);
    res.json({ error: "No users array provided" });
    return;
  }

  if (users.length === 0) {
    res.status(400);
    res.json({ error: "Users array is empty" });
    return;
  }

  const rootUser = serverInfo.users.find((user) => user.username === "root");
  if (!rootUser) {
    res.status(400);
    res.json({ error: "No root user provided" });
    return;
  }

  if (!rootUser.password) {
    res.status(400);
    res.json({ error: "No password provided for root user" });
    return;
  }

  for (let i = 0; i < users.length; i++) {
    let user = users[i];
    if (!user.username) {
      res.status(400);
      res.json({ error: "No username provided in user object" });
      return;
    }
    if (!user.password) {
      res.status(400);
      res.json({ error: `No password provided for user ${user.username}` });
      return;
    }

    if (!user.permissions) {
      res.status(400);
      res.json({ error: `No permissions provided for user ${user.username}` });
      return;
    }
  }

  const dbResponse = addServer(serverInfo);

  if (dbResponse.ok) res.status(200);
  else {
    res.status(500);
    res.json({ error: dbResponse.error });
  }
}
