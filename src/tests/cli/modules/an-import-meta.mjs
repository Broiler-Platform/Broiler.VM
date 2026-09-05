// `import.meta` is one object per module, ordinary and extensible, and this composition puts
// nothing in it: a host populates it with what it knows about where the module came from, and this
// one has decided that a guest learns nothing about the filesystem it was read from.
const first = import.meta;
first.mine = "kept";

typeof import.meta + " " +
  (import.meta === first) + " " +
  Object.getPrototypeOf(import.meta) + " " +
  import.meta.mine;
