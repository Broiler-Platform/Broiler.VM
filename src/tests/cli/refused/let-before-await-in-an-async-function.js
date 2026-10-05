// `let` before `await` begins a declaration, and `await` is no binding name in an async function,
// so this is an early error rather than `let;` and `await 0;`. Until 2026-10-03 it ran (JSC-253).
async function f() {
    let
    await 0;
}
