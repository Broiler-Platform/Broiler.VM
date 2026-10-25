// vm-fuel: 200000
// B7e: [\p{RGI_Emoji} x20] under v, twenty references to a property of strings.
// B7b-B7e are four programs alike but for the class's atom, each atom the same length, so a
// program's fuel differs from its pair's only by what compiling the class charges. bisect-vm-fuel.sh
// finds each one's smallest completing allowance in Broiler.VM.
var p = '[' + '\\p{RGI_Emoji}'.repeat(20) + ']';
t('compile', function () { new RegExp(p, 'v'); return R('compiled'); });
