// The dependency of the default-async-generator row. The anonymous form is the one that was
// refused, so it is the one written here.
export default async function* () {
  yield await Promise.resolve("awaited");
  yield "plain";
}
