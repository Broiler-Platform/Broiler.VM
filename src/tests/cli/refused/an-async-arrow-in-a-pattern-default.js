// A pattern element's default is a new expression position. An async arrow stays refused by
// name inside it - the scan that recognises one has to run here too.
var [a = async () => 1] = [];
