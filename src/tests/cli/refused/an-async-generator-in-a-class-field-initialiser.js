// A NEW POSITION, OPENED BY THE FAMILY BESIDE IT. Until a class FIELD was admitted there was no
// field initialiser for an async generator to be written in, so this source was refused for the
// field and the async generator inside it was never reached. Admitting the field is what makes the
// initialiser a position of its own, and a construct still outside the manifest has to answer for
// itself there exactly as it does at the top level.
class Streamed {
  source = async function* () {
    yield 1;
  };
}
