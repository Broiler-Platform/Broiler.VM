// A live binding: the importer reads the counter after asking the exporter to change it,
// and the value it reads is the new one. A copy taken at instantiation would answer 0.
export let counter = 0;

export function bump() {
  counter = counter + 1;
}

export default 40;
