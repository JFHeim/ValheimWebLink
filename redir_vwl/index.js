import express from "express";
import { timeoutFetch } from "./utils.js";
import bcrypt from "bcrypt";
import cors from "cors";

const app = express();
app.use(express.json());
app.use(cors());
const salt = bcrypt.genSaltSync(10);
const port = 5555;

app.get("/hash", async (req, res) => {
  let { input } = req.query;

  if (!input) {
    res.status(400);
    res.json({ error: "No input provided" });
    return;
  }

  const output = bcrypt.hashSync(input, salt);
  res.status(200);
  res.json({ result: output });
});

app.get("/redir", async (req, res) => {
  let { urlToFetch, timeout } = req.query;
  timeout = Number(timeout) || 5000;

  if (!urlToFetch) {
    res.status(400);
    res.json({ error: "No url provided" });
    return;
  }

  console.log(`Got /redir ${urlToFetch} with timeout ${timeout}`);

  /** @type {Response} */
  let response = null;
  let error = null;
  try {
    response = await timeoutFetch(timeout, `http://${urlToFetch}`);
  } catch (e) {
    error = e;
  }

  console.log(`error = `, error);
  console.log(`response = `, response);

  if (error === null) {
    const result = await response.json();
    console.log(`result = `, result);

    let json = {
      urlFetched: urlToFetch,
      status: response.status,
      result: result,
    };

    res.status(response.status);
    res.json(json);

    console.log(`json = `, json);
  } else {
    res.status(response?.status || 504);
    res.json({
      urlFetched: urlToFetch,
      status: response?.status || 504, //внешний сервер не ответил
      error: error.toString(),
    });
  }
});

app.listen(port, null, () => {
  console.log(`Server running on http://localhost:${port}\n`);
});
