// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
using Asm6502;
using RetroC64;
using RetroC64.App;
using RetroC64.Music;
using static RetroC64.C64Registers;

// Program entry: build and run the C64 app containing the music player.
return await C64AppBuilder.Run<HelloMusic>(args);

/// <summary>
/// Simple C64 application that loads a SID tune, installs a raster IRQ to play it,
/// and writes metadata to the screen buffer.
/// </summary>
/// <remarks>
/// Credits for the amazing music "Racing_the_Beam.sid" from Lft (Linus Akesson)
/// https://csdb.dk/release/?id=256179
/// </remarks>
public class HelloMusic : C64AppAsmProgram
{
    protected override void Initialize(C64AppInitializeContext context)
    {
        // Set VICE emulator sound volume (Set it low for .NET Conf live)
        context.Settings.Vice.SoundVolume = 10;
    }

    protected override Mos6502Label Build(C64AppBuildContext context, C64Assembler asm)
    {
        // Load SID file (Racing_the_Beam.sid) from application base directory.
        var sidRawData = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Racing_the_Beam.sid"));

        // Relocate the SID to target address with chosen zero page window.
        var sidFile = context.GetService<IC64SidService>().LoadAndConvertSidFile(context, sidRawData, new SidRelocationConfig()
        {
            TargetAddress = 0x1000,
            ZpLow = 0xF0,
            ZpHigh = 0xFF,
        });

        // Helper to emit init/play routines into assembly.
        var sidPlayer = new SidPlayer(sidFile, asm);

        // -------------------------------------------------------------------------
        // Assembly: code section setup & label declarations
        // -------------------------------------------------------------------------
        asm
            .LabelForward(out var irqMusic)      // Label placeholder for raster IRQ routine
            .LabelForward(out var screenBuffer)  // Label placeholder for screen buffer data
            .Label(out var startOfCode)          // Entry label for the program
            .BeginCodeSection(Name);             // Begin main code section

        // Framework-provided initialization (sets up environment, disables BASIC/KERNAL)
        BeginAsmInit(asm);

        asm
            // Install raster IRQ at line 0xF8 calling irqMusic
            .SetupRasterIrq(irqMusic, 0xF8);

        // Initialize SID player (calls SID init routine)
        sidPlayer!.Initialize();

        // Clear the visible screen using a 1KB block copy (4 * 256 bytes)
        asm
            .CopyMemoryBy256BytesBlock(screenBuffer, VIC2_SCREEN_CHARACTER_ADDRESS_DEFAULT, 4);

        // Hand over to infinite main loop (idle; music driven by IRQ)
        EndAsmInitAndInfiniteLoop(asm);

        // -------------------------------------------------------------------------
        // Raster IRQ routine: acknowledge interrupt and call SID play
        // -------------------------------------------------------------------------
        asm.Label(irqMusic)
            .PushAllRegisters() // Preserve CPU state

            .LDA(VIC2_INTERRUPT)  // Read VIC-II interrupt register (ack)
            .STA(VIC2_INTERRUPT); // Write back to clear IRQ source

        // Call generated SID play routine (advances music)
        sidPlayer.PlayMusic();

        asm
            .PopAllRegisters()           // Restore CPU state
            .RTI()                       // Return from interrupt
            .EndCodeSection();

        // -------------------------------------------------------------------------
        // Prepare screen buffer content: render metadata strings (Author, Title, Released)
        // -------------------------------------------------------------------------
        var musicX = 7;
        var musicY = 10;

        var screenBufferData = new byte[1024];        // Full screen character buffer
        var screenBufferSpan = screenBufferData.AsSpan();
        screenBufferSpan.Fill((byte)' ');             // Initialize with spaces

        // Write SID metadata into buffer at chosen positions
        C64CharSet.StringToPETScreenCode($"   MUSIC: {sidFile!.Author.ToUpperInvariant()}")
            .CopyTo(screenBufferSpan.Slice(40 * (musicY + 2) + musicX));
        C64CharSet.StringToPETScreenCode($"   TITLE: {sidFile.Name.ToUpperInvariant()}")
            .CopyTo(screenBufferSpan.Slice(40 * (musicY + 3) + musicX));
        C64CharSet.StringToPETScreenCode($"RELEASED: {sidFile.Released.ToUpperInvariant()}")
            .CopyTo(screenBufferSpan.Slice(40 * (musicY + 4) + musicX));

        // -------------------------------------------------------------------------
        // Data section: SID music block + initial screen buffer
        // Only the first 256 bytes of screenBufferData are arranged here (matches copy loop).
        // -------------------------------------------------------------------------
        asm
            .BeginDataSection("DemoData")
            .ArrangeBlocks([
                // SID relocated data/code block
                sidPlayer!.GetMusicBlock(),
                // Screen buffer (first 256 bytes used for clearing)
                new(screenBuffer, screenBufferData, 256),
            ])
            .EndDataSection();

        return startOfCode; // Entry point returned to framework.
    }
}
