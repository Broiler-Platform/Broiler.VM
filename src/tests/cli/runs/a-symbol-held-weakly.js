// A SYMBOL `Symbol.for` DID NOT MAKE CAN BE HELD WEAKLY (JSP-5, JSC-237), which is the language's
// `CanBeHeldWeakly` since ES2023: a WeakMap key, a WeakSet member, a WeakRef target and a registry's
// target and token. A registered Symbol cannot, because `Symbol.for` answers it again whenever it is
// asked for. Every Symbol was refused.
var symbol = Symbol('held');
var answers = [];

var weakMap = new WeakMap([[symbol, 'value']]);
answers.push(weakMap.get(symbol), weakMap.has(Symbol.iterator), weakMap.delete(symbol), weakMap.has(symbol));

var weakSet = new WeakSet();
weakSet.add(Symbol.iterator);
answers.push(weakSet.has(Symbol.iterator));

answers.push(new WeakRef(symbol).deref() === symbol);

var registry = new FinalizationRegistry(function () {});
registry.register(symbol, 'held', Symbol.asyncIterator);
answers.push(registry.unregister(Symbol.asyncIterator));

for (var attempt of [
  function () { new WeakMap().set(Symbol.for('registered'), 1); },
  function () { new WeakSet().add(Symbol.for('registered')); },
  function () { new WeakRef(Symbol.for('registered')); },
  function () { registry.register(Symbol.for('registered'), 1); },
]) {
  try {
    attempt();
    answers.push('held');
  } catch (error) {
    answers.push(error.constructor.name);
  }
}

answers.join(' / ');
