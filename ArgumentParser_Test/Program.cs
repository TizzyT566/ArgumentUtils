using static ArgumentUtils.ArgumentParser;

// SetArgs(["-l", "9843496413", "-i", "64536", "364", "35.373", "Pokémon", "567", "-p", "437547", "5413.85k1361"]);

Dictionary<int, string> dict = new()
{
    [567] = "Pokémon"
};

using (Option("-p").Param(out long p).Param(out double d, 111.111))
{
    Console.WriteLine($"{p}, {d}");
}

if (Option("-i", required: false)
    .Param(out int result)
    .Param(out int result2)
    .Param(out double test)
    .Param(out string meh, "\"\"")
    .Map(dict, out string s, "Digimon"))
{
    Console.WriteLine($"Success: {result}, {result2}, {test}, {meh}, {s}");
}
else
{
    Console.WriteLine($"Failed: {result}, {result2}, {test}, {meh}, {s}");
}

if (Option("-l", required: true)
    .Param(out long l))
{
    Console.WriteLine($"Success: {l}");
}
else
{
    Console.WriteLine($"Failed: {l}");
}