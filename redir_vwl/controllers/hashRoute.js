import bcrypt from "bcrypt";

export default function hashRoute(req, res) {
  let { input } = req.query;

  if (!input) {
    res.status(400);
    res.json({ error: "No input provided" });
    return;
  }

  const salt = bcrypt.genSaltSync(10);
  const output = bcrypt.hashSync(input, salt);
  res.status(200);
  res.json({ result: output });
}
