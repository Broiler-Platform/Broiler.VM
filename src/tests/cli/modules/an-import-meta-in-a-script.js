// `import.meta` is a module's own metadata, so a script has no object for it to answer with. It is
// refused with the code that says exactly that, and not with the one for an import DECLARATION: a
// reader handed the declaration's code would go looking for an import statement that is not here.
import.meta;
