// A trailing comma is a valid object literal and not a valid JSON text, and a JSON module is read
// as JSON: the load is refused, and nothing in the graph runs.
import data from "./not-json.json" with { type: "json" };

data;
