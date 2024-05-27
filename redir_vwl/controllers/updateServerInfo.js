import { updateServer } from "../db.js";

export default function updateServerInfo(req, res) {
  const { id, serverInfo } = req.query;

  if (!id) {
    res.status(400);
    res.json({ error: "No id provided" });
    return;
  }

  if (!serverInfo) {
    res.status(400);
    res.json({ error: "No ip provided" });
    return;
  }

  if (!serverInfo.port) {
    res.status(400);
    res.json({ error: "No port provided" });
    return;
  }

  if (!serverInfo.name) {
    res.status(400);
    res.json({ error: "No name provided" });
    return;
  }

  if (!serverInfo.description) {
    res.status(400);
    res.json({ error: "No description provided" });
    return;
  }

  if (!serverInfo.users) {
    res.status(400);
    res.json({ error: "No users array provided" });
    return;
  }

  if (serverInfo.users.length === 0) {
    res.status(400);
    res.json({ error: "Users array is empty" });
    return;
  }

  if (serverInfo.users.find((user) => user.username === "root")) {
    res.status(400);
    res.json({ error: "Username 'root' is reserved, you can not change it" });
    return;
  }

  for (let i = 0; i < serverInfo.users.length; i++) {
    let user = serverInfo.users[i];
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
      res.json({
        error: `No permissions provided for user ${user.username}`,
      });
      return;
    }
  }

  const dbResponse = updateServer(id, serverInfo);

  if (dbResponse.ok) res.status(200);
  else {
    res.status(500);
    res.json({ error: dbResponse.error });
  }
}
