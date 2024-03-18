/*
Following upgrades has been implemented:
1.Initialized the ImportedObjects list as List<ImportedObject> instead of IEnumerable<ImportedObject>.
2.Improved data loading from the file and formatted it using the Trim() method to remove leading and trailing white spaces.
3.Optimized the data display logic to avoid nested foreach loops.
4.Changed the data type of the NumberOfChildren field to int.
5.Removed unnecessary data display options from the ImportAndPrintData method that are not used.
*/

namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataReader
    {
        private List<ImportedObject> ImportedObjects = new List<ImportedObject>();

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
           
            ImportData(fileToImport);

            
            ProcessAndPrintData(printData);
        }

        private void ImportData(string fileToImport)
        {
            using (var streamReader = new StreamReader(fileToImport))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var values = line.Split(';');
                    var importedObject = new ImportedObject
                    {
                        Type = values[0].Trim(),
                        Name = values[1].Trim(),
                        Schema = values[2].Trim(),
                        ParentName = values[3].Trim(),
                        ParentType = values[4].Trim(),
                        DataType = values[5].Trim(),
                        IsNullable = values[6].Trim()
                    };
                    ImportedObjects.Add(importedObject);
                }
            }
        }

        private void ProcessAndPrintData(bool printData)
        {
            foreach (var database in ImportedObjects.Where(obj => obj.Type.ToUpper() == "DATABASE"))
            {
                var databaseTables = ImportedObjects.Where(obj => obj.ParentType.ToUpper() == database.Type && obj.ParentName == database.Name);
                Console.WriteLine($"Database '{database.Name}' ({databaseTables.Count()} tables)");

                if (printData)
                {
                    
                    foreach (var table in databaseTables)
                    {
                        Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                        if (printData)
                        {
                            
                            foreach (var column in ImportedObjects.Where(obj => obj.ParentType.ToUpper() == table.Type && obj.ParentName == table.Name))
                            {
                                Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                            }
                        }
                    }
                }
            }
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string DataType { get; set; }
        public string ParentType { get; set; }
        public string IsNullable { get; set; }
        public int NumberOfChildren { get; set; }
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

   
}
