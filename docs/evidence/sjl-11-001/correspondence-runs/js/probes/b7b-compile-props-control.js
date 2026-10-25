// vm-fuel: 200000
// B7b: the control for B7c: [\x41a x50] under vi, a class of two code points and no property.
// B7b-B7e are four programs alike but for the class's atom, each atom the same length, so a
// program's fuel differs from its pair's only by what compiling the class charges. bisect-vm-fuel.sh
// finds each one's smallest completing allowance in Broiler.VM.
var p = '[' + '\\x41a'.repeat(50) + ']';
t('compile', function () { new RegExp(p, 'vi'); return R('compiled'); });
