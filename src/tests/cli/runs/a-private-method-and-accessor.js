// THE FOUR KINDS OF PRIVATE ELEMENT, each of which is stored differently and refuses differently.
// A field is writable; a method is NOT, so writing one is a TypeError in a class body that is
// strict code by definition; an accessor is a pair that `get #a` and `set #a` build together, so
// the two halves must reach one element rather than two; and a static private member lives on the
// constructor, where an instance cannot see it.
class Account {
  #balance = 100;

  #fee() { return 5; }

  get #net() { return this.#balance - this.#fee(); }
  set #net(value) { this.#balance = value + this.#fee(); }

  static #rate = 2;

  net() { return this.#net; }
  setNet(value) { this.#net = value; return this.#balance; }
  step() { this.#balance += 1; return this.#balance; }
  writeMethod() { this.#fee = 1; }
  static rate() { return Account.#rate; }
  static holdsRate(candidate) { return #rate in candidate; }
}

var account = new Account();
var refusal;

try {
  account.writeMethod();
  refusal = "none";
} catch (error) {
  refusal = error.constructor.name;
}

account.net() + " " + account.setNet(200) + " " + account.step() + " / " +
  Account.rate() + " " + Account.holdsRate(Account) + " " + Account.holdsRate(account) + " / " +
  refusal;
