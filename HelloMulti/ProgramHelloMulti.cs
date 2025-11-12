// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
using Asm6502;
using RetroC64;
using RetroC64.App;
using static RetroC64.C64Registers;

return await C64AppBuilder.Run<HelloMulti>(args);

/// <summary>
/// Represents a multi-disk Commodore 64 application that demonstrates basic 
/// and assembly program examples across multiple disks.
/// </summary>
/// <remarks>This class adds two disks to the application, each containing sample 
/// programs. The first disk includes BASIC programs that print messages, while 
/// the second disk includes additional BASIC programs and an assembly program 
/// that modifies display colors.
/// </remarks>
internal class HelloMulti : C64App
{
    protected override void Initialize(C64AppInitializeContext context)
    {
        Add(new HelloDisk1());
        Add(new HelloDisk2());
    }

    private class HelloDisk1 : C64AppDisk
    {
        protected override void Initialize(C64AppInitializeContext context)
        {
            Add(new HelloBasic("10 PRINT \"HELLO WORLD 1") { Name = "HelloWorld1" });
            Add(new HelloBasic("10 PRINT \"HELLO WORLD 2") { Name = "HelloWorld2" });
        }
    }

    private class HelloDisk2 : C64AppDisk
    {
        protected override void Initialize(C64AppInitializeContext context)
        {
            Add(new HelloBasic("10 PRINT \"HELLO WORLD 3") { Name = "HelloWorld3" });
            Add(new HelloBasic("10 PRINT \"HELLO WORLD 4") { Name = "HelloWorld4" });
            Add(new HelloAsm());
        }
    }

    private class HelloBasic : C64AppBasic
    {
        public HelloBasic(string text) => Text = text;
    }

    private class HelloAsm : C64AppAsmProgram
    {
        protected override Mos6502Label Build(C64AppBuildContext context, C64Assembler asm)
        {
            asm
                .Label(out var start)
                .BeginCodeSection("Main");

            asm
                .LDA_Imm(COLOR_RED)
                .STA(VIC2_BG_COLOR0)
                .LDA_Imm(COLOR_GREEN)
                .STA(VIC2_BORDER_COLOR)
                .InfiniteLoop();

            asm.EndCodeSection();
            return start;
        }
    }
}