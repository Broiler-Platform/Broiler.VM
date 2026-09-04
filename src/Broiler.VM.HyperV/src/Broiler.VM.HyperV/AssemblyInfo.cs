// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System.Runtime.InteropServices;

// Every native dependency of this component is a Windows system library, so the loader is told
// to look only in System32. That closes the door on a DLL of the same name being picked up from
// the application directory or the working directory.
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
