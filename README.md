QtSharp
=======

Mono/.NET bindings for Qt

This project aims to create Mono/.NET libraries that wrap Qt (https://qt-project.org/) thus enabling its usage through C#.
It is a generator that expects the include and library directories of a Qt set-up and then generates and compiles the wrappers. While still in development, it should work with any Qt version when complete. There is no Qt included in the repository, users have to download and install Qt themselves. For now, Qt MinGW for Windows has been the only tested version. Qt for OS X and Linux are planned, Qt for VC++ is not been planned for now.

The source code is separated into a library that contains the settings and passes the generator needs, and a command-line client. In the future a GUI client, constructed with Qt# itself, is planned as well.

When the generated bindings are stable, binary releases will be uploaded.


QtSharp has been tested only with Qt for MinGW, and with Qt's built-in MinGW set-up, so far.