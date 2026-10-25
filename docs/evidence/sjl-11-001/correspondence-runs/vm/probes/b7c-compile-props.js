// vm-fuel: 200000
// B7c: [\p{L} x50] under vi, fifty references to the largest general category.
// B7b-B7e are four programs alike but for the class's atom, each atom the same length, so a
// program's fuel differs from its pair's only by what compiling the class charges. bisect-vm-fuel.sh
// finds each one's smallest completing allowance in Broiler.VM.
var p = '[' + '\\p{L}'.repeat(50) + ']';
t('compile', function () { new RegExp(p, 'vi'); return R('compiled'); });
