// A rest parameter is a dense Array and never the `arguments` object, it is empty rather than
// undefined when the caller passed nothing, and `length` does not count it.
function tail(first, ...rest) {
  return first + ":" + rest.length + ":" + rest.join(",") + ":" + Array.isArray(rest);
}

tail(1) + " " + tail(1, 2, 3) + " " + tail.length;
