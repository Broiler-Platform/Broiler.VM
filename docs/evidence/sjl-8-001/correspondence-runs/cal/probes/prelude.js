// Shared by every probe: one line per operation, "label => answer" or "label => throws Name".
function show(v) { return v === undefined ? 'undefined' : String(v); }
function t(label, fn) {
  var r;
  try { r = show(fn()); } catch (e) { r = 'throws ' + (e && e.name); }
  emit(label + ' => ' + r);
}
