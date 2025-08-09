using QuickPulse.Explains;

namespace QuickAcid.Examples;

[DocFile]
[DocFileHeader("QuickAcid.Examples")]
[DocContent(@"> Zen and the Art of Code Maintenance.")]
public class ReadMe
{
    [Fact]
    [DocHeader("Introduction")]
    [DocContent(
@"**QuickAcid.Examples** is a curated collection of property-based tests designed to show QuickAcid in action.  
Each example breaks something on purpose, sometimes in obvious ways,
sometimes in subtle, stateful, or downright mischievous ones,
so you can see how failures are found, minimized, and explained. 


Think of it as a workshop manual for QuickAcid: part demonstration, part exploration, part philosophy of testing.")]
    public void Now() { Explain.This<ReadMe>("README.md"); }
}