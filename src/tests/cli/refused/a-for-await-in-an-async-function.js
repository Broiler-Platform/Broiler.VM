// `for await` WAS ONLY EVER REACHABLE FROM A PLACE THIS MANIFEST REFUSED FIRST, and admitting
// `async` is what put it inside a body the front end now parses. It is refused BY NAME, before the
// parenthesis - after it the head reads as an ordinary one and the diagnostic would name whatever
// token followed `await` instead of naming the construct.
async function drain(source) {
  for await (const page of source) {
    print(page);
  }
}
