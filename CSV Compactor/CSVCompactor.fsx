open System.IO
open System.Data
open System.Linq

let DiscoverFiles directory = [ for file in Directory.GetFiles(directory) do if Path.GetExtension(file) = ".csv" then yield file ]

let ReadFilesIntoMemory files =
    [ for file in files do
        let dt = new DataTable(file)
        printfn $"Loading {dt.TableName}"
        use sr = new StreamReader(file)
        dt.Columns.AddRange([| for header in sr.ReadLine().Split(',') do yield new DataColumn(header) |])
        while not sr.EndOfStream do
            let sb = new System.Text.StringBuilder()
            for s in sr.ReadLine().Split(',') do sb.Append($"{s},") |> ignore
            dt.Rows.Add(sb.ToString()) |> ignore
        yield dt ]

let CompareHeaders (dataSet : DataTable list) =
    let rec Compare (data : DataTable list) = 
        match data with
        | [_] -> true
        | h::t -> Enumerable.SequenceEqual([ for c in h.Columns do yield c.ColumnName ], [for c in t.Head.Columns do yield c.ColumnName ]) || Compare t
        | _ -> false
    Compare dataSet, dataSet
    
let CreateCompactedCSV matchingHeaders (dataSet : DataTable list) =
    if matchingHeaders then
        use sw = fsi.CommandLineArgs.[2] |> File.CreateText
        for column in dataSet.[0].Columns do sw.Write($"{column.ColumnName},")
        sw.WriteLine()
        for table in dataSet do
            for row in table.Rows do
                for item in row.ItemArray do 
                    sw.Write($"{item}")
                sw.WriteLine()
        printfn "New file created successfully!"
    else printfn "Incompatible headers found - aborted process!"

fsi.CommandLineArgs.[1]
|> DiscoverFiles
|> ReadFilesIntoMemory
|> CompareHeaders
||> CreateCompactedCSV