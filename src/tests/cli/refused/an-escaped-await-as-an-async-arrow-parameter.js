// An async arrow reserves `await` in its parameter, however it is spelled: the escaped spelling bound
// it until 2026-10-04 (JSC-256).
var f = async aw\u0061it => 1;
