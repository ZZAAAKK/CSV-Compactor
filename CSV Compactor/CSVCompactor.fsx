open System.IO
open System.Data
open System.Text

let GetOutputFile outputFilePath =
    if not (outputFilePath |> File.Exists) then
        outputFilePath
        |> File.CreateText // if your output folder is synced to a file share this may cause issues (OneDrive etc.) If that happens, just run it again once the file is synced.
        |> ignore 
    outputFilePath

let CreateCompactedCSV matchingHeaders (dataSet : DataTable list) =
    if matchingHeaders then
        use sw = new StreamWriter(fsi.CommandLineArgs.[2] |> GetOutputFile)
        let sb = new StringBuilder()
        for i in [0..dataSet.[0].Columns.Count-1] do
            sb.Append($"{dataSet.[0].Columns.[i].ColumnName},") |> ignore
        sw.WriteLine(sb.ToString())
        sb.Clear() |> ignore
        for i in [0..dataSet.Length - 1] do
            for j in [0..dataSet.[i].Rows.Count-1] do
                for k in [0..dataSet.[i].Rows.[j].ItemArray.Length - 1] do
                    sb.Append($"{dataSet.[i].Rows.[j].ItemArray.[k]},") |> ignore
                sw.WriteLine(sb.ToString())
                sb.Clear() |> ignore
        printfn "New file created successfully!"
    else
        printfn "Incompatible headers found - aborted process!"

let CompareHeaders (dataSet : DataTable list) =
    let mutable matchingHeaders = true
    for dt in dataSet do
        for dtComp in dataSet do
            if matchingHeaders then
                if not (dt.TableName = dtComp.TableName) then
                    if dt.Columns.Count = dtComp.Columns.Count then
                        for i in [0..dt.Columns.Count - 1] do
                            matchingHeaders <- dt.Columns.[i].ColumnName = dtComp.Columns.[i].ColumnName
                    else matchingHeaders <- false
    matchingHeaders, dataSet

let ReadFilesIntoMemory (files : seq<string>) =
    let mutable dataSet = []
    for file in files do
        let dt = new DataTable(file)
        printfn $"{dt.TableName}"
        use sr = new StreamReader(file)
        for header in sr.ReadLine().Split(',') do
            dt.Columns.Add(header) |> ignore
            printf $"{header} "
        printfn $""
        while not sr.EndOfStream do
            let dr = dt.NewRow()
            let rowValues = sr.ReadLine().Split(',')
            for i in [0..dt.Columns.Count-1] do 
                dr.[i] <- rowValues.[i]
                printf $"{rowValues.[i]} "
            dt.Rows.Add(dr)
            printfn $""
        dataSet <- dt :: dataSet
    dataSet

let DiscoverFiles directory = [ for file in Directory.GetFiles(directory) do if Path.GetExtension(file) = ".csv" then yield file ]

let args = fsi.CommandLineArgs
args.[1] // input folder
|> DiscoverFiles
|> ReadFilesIntoMemory
|> CompareHeaders
||> CreateCompactedCSV