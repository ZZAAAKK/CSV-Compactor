# CSV-Compactor
## About:
This is a simple script which takes in any number of csv files, compares them to check for compatibility, and concatenates them into a single output CSV file.
## How to use:
1) Download the file 'CSVCompactor.fsx'
2) Open your command line/terminal
3) cd into the folder where you saved the above script
4) Run the following command, replacing where necessary:
<code>dotnet fsi CSVCompactor.fsx [input folder] [full output file path, including .csv extension]</code>
## Troubleshooting:
- If you can't run the file at all, and get a dotnet error, or 'dotnet is not a recognised...' error, then you need to install either dotnet, or F# - I'm not going into the details on how to install those here as it's well documented elsewhere.
- If you get error FS0087 then you're in the wrong folder.
- Any other issues, please raise an issue via the issues tab.
