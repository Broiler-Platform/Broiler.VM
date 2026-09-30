// A `use strict` LATER IN A PROLOGUE MAKES THE WHOLE PROLOGUE STRICT, so an entry before it holding
// a legacy octal escape is refused though it was read while the body was still sloppy.
function strictAfterwards() {
  '\1';
  'use strict';
}
