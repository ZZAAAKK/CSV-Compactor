open System.IO

let DiscoverFiles directory = [ for file in Directory.GetFiles(directory) do if Path.GetExtension(file) = ".csv" then yield file ]

let ReadFilesIntoMemory (files : seq<string>) =
    [ for file in files ->
        [ use sr = new StreamReader(file)
        yield! [ while not sr.EndOfStream do yield sr.ReadLine() ]]]
    
let TryCreateCompactedCSV (data : string list list) =
    data |> Seq.map (fun x -> x.Head) |> Seq.distinct |> Seq.length |> function
    | 1 -> 
        use sw = fsi.CommandLineArgs.[2] |> File.CreateText
        sw.WriteLine(data.Head.Head)
        [[for rows in data do yield! rows.Tail]] |> Seq.concat |> Seq.iter (fun row -> sw.WriteLine(row))
        printfn "New file created successfully!"
    | _ -> printfn "Incompatible headers found - aborted process!"

fsi.CommandLineArgs.[1] |> DiscoverFiles |> ReadFilesIntoMemory |> TryCreateCompactedCSV