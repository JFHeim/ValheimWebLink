import express from "express";
import cors from "cors";
import hashRoute from "./controllers/hashRoute.js";
import redirRoute from "./controllers/redirRoute.js";
import registerServer from "./controllers/registerServer.js";
import updateServerInfo from "./controllers/updateServerInfo.js";

const app = express();
app.use(express.json());
app.use(cors());
const port = 5555;

app.get("/hash", hashRoute);
app.get("/redir", redirRoute);
app.get("/registerServer", registerServer);
app.get("/updateServerInfo", updateServerInfo);

app.listen(port, null, () => {
  console.log(`Server running on http://localhost:${port}\n`);
});
