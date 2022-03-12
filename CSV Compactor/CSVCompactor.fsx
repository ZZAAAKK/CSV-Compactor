open System.IO

let ReadFilesIntoMemory directory =
    if Directory.Exists directory then 
        [ for file in [ for file in Directory.GetFiles(directory) do if Path.GetExtension(file) = ".csv" then printfn "Found file %s" file; yield file ] ->
            [ use sr = new StreamReader(file)
            yield! [ while not sr.EndOfStream do yield sr.ReadLine() ]]]
    else printfn "The specified input directory could not be found!"; []
    
let TryCreateCompactedCSV (data : string list list) =
    data |> Seq.length |> function
    | 0 -> printfn "No files found!"
    | _ as n -> printfn "Comparing headers for %i files." n
                data |> Seq.map (fun x -> x.Head) |> Seq.distinct |> Seq.length |> function
                | 1 -> 
                    use sw = fsi.CommandLineArgs.[2] |> File.CreateText
                    sw.WriteLine(data.Head.Head)
                    [[for rows in data do yield! rows.Tail]] |> Seq.concat |> Seq.iter (fun row -> sw.WriteLine(row))
                    printfn "New file created successfully!"
                | _ -> printfn "Incompatible headers found - aborted process!"

fsi.CommandLineArgs.[1] |> ReadFilesIntoMemory |> TryCreateCompactedCSV