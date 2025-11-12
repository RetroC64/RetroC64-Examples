# RetroC64 Examples

<img align="right" width="160px" height="160px" src="https://raw.githubusercontent.com/RetroC64/RetroC64/main/img/RetroC64.png">

This repository contains examples for the [RetroC64](https://github.com/RetroC64/RetroC64) SDK.

## 🏗️ Building

Make sure you have the [.NET SDK 9.0 or higher](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) installed.

## 🧪 Examples

Ensure that the VICE C64 emulator is [setup correctly](https://retroc64.github.io/docs).

- [HelloBasic](./HelloBasic): A simple example showing how to write BASIC code inline.
- [HelloAsm](./HelloAsm): A simple example showing how to write assembly code inline.
- [HelloSprites](./HelloSprites): An example showing how to use sprites.
- [HelloMusic](./HelloMusic): An example showing how to play music using the built-in SID player.
- [HelloMulti](./HelloMulti): An example showing how to generate multi-disk D64 files.

To run an example, navigate to the example folder and run: `dotnet watch -- run`.

It will build the example, launch the VICE C64 emulator, and deploy the generated PRG file to the emulator, and wait for code changes.

## 🪪 License

This software is released under the [BSD-2-Clause license](https://opensource.org/licenses/BSD-2-Clause). 

## 🤗 Author

Alexandre Mutel aka [xoofx](https://xoofx.github.io).
