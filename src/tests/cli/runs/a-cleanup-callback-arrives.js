// A FINALIZATION REGISTRY'S CLEANUP CALLBACK ARRIVES (phase F4, JSD-0029 D03-a, JSC-263). This host
// sweeps its registries when it drains the job queue after the script, and collects once before that
// drain; the callbacks then run as ordinary jobs, in registration order, with their held values. A
// target the script still holds is never reported, and one whose token was unregistered never is.
var arrived = [];
var registry = new FinalizationRegistry(function (held) {
  arrived.push(held);
  print('arrived ' + arrived.join(','));
});

(function () {
  for (var i = 0; i < 3; i++) {
    registry.register({ index: i }, 'h' + i);
  }
})();

var kept = {};
registry.register(kept, 'kept');

var token = {};
(function () { registry.register({}, 'unregistered', token); })();

[registry.unregister(token), arrived.length].join(' ');
