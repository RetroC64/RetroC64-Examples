// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
using Asm6502;
using RetroC64;
using RetroC64.App;
using static RetroC64.C64Registers;

return await C64AppBuilder.Run<HelloAsm>(args);

public class HelloAsm : C64AppAsmProgram
{
    private bool FlickerColors { get; set; } = false;

    protected override Mos6502Label Build(C64AppBuildContext context, C64Assembler asm)
    {
        asm
            .Label(out var start)
            .BeginCodeSection("Main");

        if (FlickerColors)
        {
            asm
                .CLC()
                .LDX_Imm(0)
                .Label(out var loop)
                .STX(VIC2_BG_COLOR0)
                .STX(VIC2_BORDER_COLOR)
                .INX()
                .BCC(loop); // infinite loop
        }
        else
        {
            asm
                .LDA_Imm(COLOR_RED)
                .STA(VIC2_BG_COLOR0)
                .LDA_Imm(COLOR_GREEN)
                .STA(VIC2_BORDER_COLOR)
                .InfiniteLoop();
        }

        asm.EndCodeSection();
        return start;
    }
}
