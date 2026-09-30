// EVERY PART OF A CLASS IS STRICT CODE, its element names as much as its method bodies, so a string
// key holding a legacy octal escape is refused. The comparison engine admits this one: it checks a
// class's string keys as it checks the sloppy code outside the class, and only the bodies of its
// methods as strict. This host follows the specification's "all parts of a ClassDeclaration or a
// ClassExpression are strict mode code", and a conformance runner scores the SyntaxError.
class Named {
  '\101'() {
    return 'A';
  }
}
