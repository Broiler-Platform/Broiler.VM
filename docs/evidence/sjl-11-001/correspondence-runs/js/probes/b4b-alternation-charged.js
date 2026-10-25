// vm-fuel: 10000000
// B4b: (?:a|a)x24 then b /u on a^24 under a fuel allowance of 10,000,000 in Broiler.VM: whether the
// alternation is charged (an exhaustion) or not (a null after all 2^24 paths).
var re = new RegExp('(?:a|a)'.repeat(24) + 'b', 'u');
t('(?:a|a)x24b /u on a^24', function () { var e = elapsed(function () { return re.exec('a'.repeat(24)); }); return R(show(e.r) + ' elapsedMs ' + e.ms); });
