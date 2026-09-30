// A LABEL IS IN FORCE ONLY INSIDE THE STATEMENT IT LABELS, and not inside a function written there:
// two statements one after the other may carry the same label, and a function inside a labelled
// statement starts with no label in force.
var trace = [];

a: {
  trace.push('first');
  break a;
}

a: {
  trace.push('second');
  var inner = function () {
    a: for (var index = 0; index < 3; index++) {
      if (index === 1) {
        break a;
      }
      trace.push('inner ' + index);
    }
  };
  inner();
}

trace.join(', ');
