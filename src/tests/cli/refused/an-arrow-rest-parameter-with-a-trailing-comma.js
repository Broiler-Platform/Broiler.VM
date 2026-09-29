// AN ARROW'S PARAMETERS ARE READ BY THE SAME LIST A FUNCTION'S ARE, so its rest parameter is
// refused by name too, trailing comma included. Until 2026-09-29 this was answered as "`)` was
// expected".
var collect = (...rest,) => rest;
