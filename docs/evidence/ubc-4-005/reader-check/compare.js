// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// THE READER CHECK'S SECOND HALF: each module extract.py cut out, encoded by wabt 1.0.39 with every post-1.0
// feature off, compared byte for byte with the reader's encoding. Run where node_modules/wabt resolves:
//
//   node compare.js <modules.json> <the reader's --encode-to directory>
//
// ONE MODULE IS GIVEN TO WABT RESPELLED: elem.wast's first, whose (elem $t ...) names a table in the 1.0 text
// format and a segment in wabt's, which follows 2.0. It is handed to wabt as (elem (table $t) ...), which means in
// 2.0 what (elem $t ...) means in 1.0; the reader's encoding is compared with the result unchanged.
const fs = require('fs'), path = require('path');
const [modulesFile, encDir] = process.argv.slice(2);
require(require.resolve('wabt', { paths: [process.cwd()] }))().then(wabt => {
  const mods = JSON.parse(fs.readFileSync(modulesFile, 'utf8'));
  const features = {};
  let same = 0, differ = 0, wabtRefused = 0; const out = [];
  for (const m of mods) {
    const mine = fs.readFileSync(path.join(encDir, `${m.file}.${m.ordinal}.wasm`));
    let theirs;
    try {
      let src = Buffer.from(m.b64, 'base64'); if (m.file==='elem.wast' && m.ordinal===1) src = Buffer.from(src.toString('latin1').split('(elem $t (').join('(elem (table $t) ('), 'latin1'); const mod = wabt.parseWat(`${m.file}.${m.ordinal}`, new Uint8Array(src), features);
      mod.resolveNames();
      theirs = Buffer.from(mod.toBinary({ log: false, canonicalize_lebs: true, relocatable: false, write_debug_names: false }).buffer);
      mod.destroy();
    } catch (e) { wabtRefused++; out.push(`wabt-refused ${m.file}:${m.ordinal} ${String(e.message).split('\n')[0]}`); continue; }
    if (Buffer.compare(mine, theirs) === 0) same++; else { differ++; out.push(`differ ${m.file}:${m.ordinal} reader=${mine.toString('hex')} wabt=${theirs.toString('hex')}`); }
  }
  out.forEach(l => console.log(l));
  console.log(`# ${mods.length} modules: ${same} byte-identical, ${differ} differ, ${wabtRefused} refused by wabt`);
});
