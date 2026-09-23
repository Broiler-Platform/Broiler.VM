// `await` IS RESERVED IN A CLASS STATIC BLOCK, and a `using` declaration binds a name like `let`
// does. So `using await` there is the reserved word as a binding and not a missing semicolon: the
// same code `let await` gets in the same place (JSeal VM-FIX-H).
class C {
  static {
    using await = null;
  }
}
