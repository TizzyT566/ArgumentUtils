using static ArgumentUtils.ArgumentParser;

SetArgs(["-l", "9843496413", "-i", "64536", "364", "35.373", "Pokémon", "567", "-p", "437547", "5413.851361"]);

Dictionary<int, string> dict = new()
{
    [567] = "Pokémon"
};

if (Arg("-p", required: false)
    .Param(out long p)
    .Param(out double d))
{
    Console.WriteLine($"Success: {p} {d}");
}
else
{
    Console.WriteLine($"FAILED: {p} {d}");
}

if (Arg("-i", required: true)
    .Param(out int result)
    .Param(out int result2)
    .Param(out double test)
    .Param(out string meh, "\"\"")
    .Map(dict, out string s, "Digimon"))
{
    Console.WriteLine($"Success: {result} {result2} {test} {meh} {s}");
}
else
{
    Console.WriteLine($"Failed: {result} {result2} {test} {meh} {s}");
}


if (Arg("-l", required: true)
    .Param(out long l))
{
    Console.WriteLine($"Success: {l}");
}
else
{
    Console.WriteLine($"Failed: {l}");
}