// The source of a `for … of` is a new expression position. A generator function stays refused by
// name inside it.
for (const value of function* () {}) {
}
