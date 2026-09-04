// `static` is a modifier, a member name and the head of a static block, and only the token
// after it says which. The block is the one this manifest does not admit.
class Registered {
  static {
    Registered.ready = true;
  }
}
