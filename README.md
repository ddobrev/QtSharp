This repository is obsolete. For new code and binary releases please visit https://gitlab.com/ddobrev/QtSharp.
=======


QtSharp
=======

Mono/.NET bindings for Qt

This project aims to create Mono/.NET libraries that wrap Qt (https://qt-project.org/) thus enabling its usage through C#.
It relies on the excellent CppSharp (https://github.com/mono/CppSharp).
It is a generator that expects the include and library directories of a Qt set-up and then generates and compiles the wrappers.
While still in development, it should work with any Qt version when complete.
There is no Qt included in the repository, users have to download and install Qt themselves.
For now, Qt MinGW for Windows has been the only tested version.
Qt for OS X and Linux are planned, Qt for VC++ has not been planned for now.

The source code is separated into a library that contains the settings and passes the generator needs, and a command-line client.
In the future a GUI client, constructed with Qt# itself, is planned as well.

The are binary releases for Windows and Qt MinGW at https://github.com/ddobrev/QtSharp/releases. They are in an alpha stage.
As they get more stable, binaries for other operating systems will be added as well.

## Getting started

You need to deploy Qt itself by following http://doc.qt.io/qt-5/windows-deployment.html#application-dependencies .
In addition, for each Qt module you use you also need Qt<module>-inlines.dll deployed alongside your executable.
You can use QtSharp with any C# IDE, including Visual Studio, but make sure your executable is 32-bit by either using the
x86 configuration or AnyCPU with "Prefer 32-bit" checked.

## Examples

You can find examples at https://github.com/dsoronda/QtSharpDemos and https://github.com/grbd/QtSharp.TestApps .
Thanks to https://github.com/dsoronda and https://github.com/grbd.

## Documentation

1. [Building - QtSharp](https://github.com/ddobrev/QtSharp/blob/master/Docs/1.%20Building%20-%20QtSharp.md)
2. [Running - QTSharp CLI](https://github.com/ddobrev/QtSharp/blob/master/Docs/2.%20Running%20-%20QtSharp.CLI.md)
3. [Running - NUnit Tests](https://github.com/ddobrev/QtSharp/blob/master/Docs/3.%20Running%20-%20NUnit%20Tests.md)
4. [Building - CppSharp](https://github.com/ddobrev/QtSharp/blob/master/Docs/4.%20Building%20-%20CppSharp.md)

## Coverage

QtSharp has been tested only with Qt for MinGW, and with Qt's built-in MinGW set-up, so far.

## Funding

In order to speed up the development of the project, I've been looking for funding.
There are 2 ways for that. The first one is sponsoring Qt# itself.
The second way would be paid assignments related to CppSharp - for example bindings for other C++ libraries.
Either way is going to immensely benefit Qt#.
