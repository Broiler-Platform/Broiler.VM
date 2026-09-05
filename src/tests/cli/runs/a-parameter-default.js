// A default runs only when the argument is `undefined` - not when it is merely absent, and not
// when it is `null` - and it is evaluated left to right in a scope where the earlier parameters
// are already bound. `length` stops counting at the first one.
function label(prefix, separator = "-", suffix = prefix + separator) {
  return prefix + separator + suffix;
}

label("a") + " " + label("a", "+") + " " + label("a", undefined, "z") + " " + label.length;
