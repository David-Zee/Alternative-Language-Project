//David Uzan
//COP 4020
//Alternative Language Project
//April 21, 2024

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


class Program
{
    static void Main(string[] args)
    {
        List<Cell> cellPhones = ReadCSV("cells.csv"); // Read the CSV file and store the data in a List of Cell objects

        //Testing for types of variables in cleaned cellPhones cell
        //Everything is initially a string, the tests will be done on variables that should be changed

        //Console.WriteLine(cellPhones[18].Launch_announced.GetType()); //Should be int
        //Console.WriteLine(cellPhones[18].Body_weight.GetType()); //Should be float
        //Console.WriteLine(cellPhones[18].Display_size.GetType()); //Should be float

        Console.WriteLine("Welcome to the Alt Language Program.\n");

        string user_input = "0"; //Initialize to unused value

        while (user_input != "-1") //Create menu
        {
            Console.WriteLine("Please enter a command from the list:\n");
            Console.WriteLine("1. Average weight of phones\n"); //Uses AverageWeight
            Console.WriteLine("2. Print phones released in different year than announced\n"); //Uses DifferentYears
            Console.WriteLine("3. Print number of phones with only one feature sensor\n"); //Uses OneFeature
            Console.WriteLine("4. Print year with most phone launches after 1999\n"); //Uses MostLaunchesInYear
            Console.WriteLine("5. Add phone information\n"); //Uses AddPhone
            Console.WriteLine("6. List all cells from a specific OEM\n"); //Uses OEMSearch 
            Console.WriteLine("7. List the mean weigh of phones from a specific year\n"); //Uses YearWeight

            Console.WriteLine("-1. Exit\n");
            user_input = Console.ReadLine();

            if (user_input == "1")
            {
                AverageWeight(cellPhones);
            }
            else if (user_input == "2")
            {
                DifferentYears(cellPhones);
            }
            else if (user_input == "3")
            {
                OneFeature(cellPhones);
            }
            else if (user_input == "4")
            {
                MostLaunchesInYear(cellPhones);
            }
            else if (user_input == "5")
            {
                AddPhone(cellPhones);
            }
            else if (user_input == "6")
            {
                OEMSearch(cellPhones);
            }
            else if (user_input == "7")
            {
                YearWeight(cellPhones);
            }
        }

    }

    static List<Cell> ReadCSV(string filePath)
    {
        List<Cell> cellPhones = new List<Cell>(); //Empty list of Cell objects

        try
        {
            if (File.Exists(filePath)) //Check is file is empty
            {
                //Get the length of the file
                long length = new FileInfo(filePath).Length;

                //Close program if empty file
                if (length == 0)
                {
                    Console.WriteLine("The file you are trying to read is empty.\n Exiting program.");
                    Environment.Exit(0);
                }
            }

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null) //Reads through every line of csv file until EOF
                {
                    CleanUp(line, cellPhones); //Cleans up the line and changes variable types to desired
                }
            }
        }
        catch (Exception e) //Catch if file is not in the same directory
        {
            Console.WriteLine($"Error reading CSV: {e.Message}");
        }

        return cellPhones;
    }

    static int? Announced(string input)
    {
        string yearString = "";
        foreach (char c in input) //Cycles through first 4 characters of string
        {
            if (char.IsDigit(c)) //If character is a digit, add to string
            {
                yearString += c;
            }
            else if (yearString.Length > 0)
            {
                break;
            }
        }

        //Attempt to convert the string of digits as a year
        if (!string.IsNullOrEmpty(yearString) && int.TryParse(yearString, out int year) && (yearString.Length == 4))
        {
            return year;
        }
        else
        {
            return null;
        }
    }

    static object Status(string input)
    {
        Regex regex = new Regex(@"\b\d{4}\b"); //Regex to check if string contains a 4 digit year, surrounded by non-number characters
        Match match = regex.Match(input);

        if (match.Success)
        {
            int year;
            if (int.TryParse(match.Value, out year)) //If the year is a valid integer, return the year
            {
                return year;
            }
        }
        else if (input.Equals("Cancelled"))
        {
            return "Cancelled";
        }
        else if (input.Equals("Discontinued"))
        {
            return "Discontinued";
        }

        return null;
    }

    static float? Weight(string input)
    {
        Regex regex = new Regex(@"\b(\d*\.?\d+) *g\b"); //Regex to check if string contains a number followed by "g"
        Match match = regex.Match(input);

        if (match.Success)
        {
            float weight;
            if (float.TryParse(match.Groups[1].Value, out weight)) //If the weight is a valid float, return the weight
            {
                return weight;
            }
        }

        return null;
    }

    static string SimClean(string input)
    {
        if (input.Equals("Yes"))
        {
            return null;
        }
        else if (input.Equals("No"))
        {
            return null;
        }

        return input;
    }

    static float? DisplaySize(string input)
    {
        Regex regex = new Regex(@"(\d*\.?\d+)\s*inches"); //Regex to check if string contains a number followed by "inches"
        Match match = regex.Match(input);

        if (match.Success)
        {
            float size;
            if (float.TryParse(match.Groups[1].Value, out size)) //If the size is a valid float, return the size
            {
                return size;
            }
        }

        return null;
    }

    static string Sensors(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        int intValue;
        float floatValue;
        //If the input is a number by itself, return null
        if (int.TryParse(input, out intValue) || float.TryParse(input, out floatValue))
        {
            return null;
        }

        return input;
    }

    static string Platform(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        int intValue;
        float floatValue;
        //If the input is a number by itself, return null
        if (int.TryParse(input, out intValue) || float.TryParse(input, out floatValue))
        {
            return null;
        }

        int commaIndex = input.IndexOf(',');
        if (commaIndex >= 0)
        {
            return input.Substring(0, commaIndex); //Using a ',' as a delimeter, return the first part of the string
        }

        return input;
    }

    static void CleanUp(string line, List<Cell> cellPhones)
    {

        //Regex to split the line into parts based on the delimeter ',' ignoring cases inside of quotations of each cell
        string[] parts = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        string oem = parts[0];

        string model = parts[1];
        string launch_announced = parts[2];
        string launch_status = parts[3];
        string body_dimensions = parts[4];
        string body_weight = parts[5];
        string body_sim = parts[6];
        string display_type = parts[7];
        string display_size = parts[8];
        string display_resolution = parts[9];
        string features_sensors = parts[10];
        string platform_os = parts[11];

        // THE TRANSFORMATION SHOULD HAPPEN BEFORE ADDING TO LIST
        int? launch_announced_c = Announced(launch_announced);
        object launch_status_c = Status(launch_status);
        float? body_weight_c = Weight(body_weight);
        string body_sim_c = SimClean(body_sim);
        float? display_size_c = DisplaySize(display_size);
        string features_sensors_c = Sensors(features_sensors);
        string platform_os_c = Platform(platform_os);

        cellPhones.Add(new Cell(oem, model, launch_announced_c, launch_status_c, body_dimensions, body_weight_c, body_sim_c, display_type, display_size_c, display_resolution, features_sensors_c, platform_os_c));
    }

    static void OEMSearch(List<Cell> cellPhones)
    {
        Console.WriteLine("Please enter the OEM you would like to search for: ");
        string OEM = Console.ReadLine();

        int count = 0; //Keeps track of instances of OEM
        foreach (Cell cell in cellPhones)
        {
            if (cell.OEM == OEM)
            {
                Console.WriteLine(cell.toString());
                count++;
            }
        }

        if (count == 0)
        {
            Console.WriteLine("That OEM has not been found in the list.\n");
        }
    }

    static void AddPhone(List<Cell> cellPhones)
    {
        Console.WriteLine("Please enter the phones information in the format below:\n");
        Console.WriteLine("OEM, Model, Launch Announced, Launch Status, Body Dimensions, Body Weight, Body Sim, Display Type, Display Size, Display Resolution, Features/Sensors, Platform OS");

        string phone = Console.ReadLine();

        try
        {
            CleanUp(phone, cellPhones); //Clean information to fit the scheme
        }
        catch (Exception)
        {
            Console.WriteLine("Invalid arguments, feel free to try again.\n");
        }
    }

    static void DifferentYears(List<Cell> cellPhones)
    {
        int count = 0;
        foreach (Cell cell in cellPhones)
        {
            if (cell.Launch_announced is int && cell.Launch_status is int) //If announced and status are both integers
            {
                int announced = (int)cell.Launch_announced;
                int status = (int)cell.Launch_status;

                if (announced != status) //If announced and status are not the same year
                {
                    Console.WriteLine($"OEM is {cell.OEM} and the Model is {cell.Model}");
                    count++;
                }
            }
        }
        Console.WriteLine($"The total number of phones released in different years than announced are {count}\n");
    }

    static void AverageWeight(List<Cell> cellPhones)
    {
        List<string> providers = new List<string>(); //Empty list of all OEMs

        foreach (Cell cell in cellPhones)
        {
            providers.Add(cell.OEM);
        }
        var uniqueProviders = providers.Distinct().ToList(); //List of unique OEMs

        List<float> weights = new List<float>(); //Empty list of weights

        foreach (var provider in uniqueProviders)
        {
            int count = 0;
            float weight = 0;
            foreach (Cell cell in cellPhones) //Cycles through all phone OEMs and totals their weights
            {
                if (cell.OEM == provider)
                {
                    try
                    {
                        weight += (float)cell.Body_weight;
                        count++;
                    }
                    catch (Exception) //Ignore null cases
                    {
                        continue;
                    }
                }
            }
            float avg_weight = weight / count;
            weights.Add(avg_weight);
        }

        object[,] combined_arr = CombineListsIntoArray(uniqueProviders, weights); //Combines lists into a 2D array for easier computation

        float highest_avg = 0;
        string highest_provider = "";

        for (int x = 0; x < combined_arr.GetLength(0); x++) //Cycles through column of OEMs
        {
            if ((float)combined_arr[x, 1] > highest_avg)
            {
                highest_avg = (float)combined_arr[x, 1];
                highest_provider = (string)combined_arr[x, 0];
            }
        }

        Console.WriteLine($"The highest average weight of a phone is {highest_avg}g from {highest_provider}\n");

    }

    static object[,] CombineListsIntoArray(List<string> list1, List<float> list2)
    {
        object[,] array = new object[list1.Count, 2]; //Creates a 2D array with the length of the first list as the rows and 2 columns (OEM, Weight)

        for (int i = 0; i < list1.Count; i++)
        {
            array[i, 0] = list1[i];
            array[i, 1] = list2[i];
        }

        return array;
    }

    static void OneFeature(List<Cell> cellPhones)
    {
        int count = 0;
        foreach (Cell cell in cellPhones)
        {
            //If the features/sensors contain a ")," or "," , it is a multi-feature phone
            if (cell.Features_sensors.Contains("),") || cell.Features_sensors.Contains(","))
            {
                continue;
            }
            else { count++; }
        }
        Console.WriteLine($"The total number of phones with only one feature sensor are {count}\n");
    }

    static void MostLaunchesInYear(List<Cell> cellPhones)
    {
        int[,] arr = new int[21, 2]; //Creates a 2D array with 21 rows and 2 columns (Year, Count)

        for (int i = 0; i < 21; i++) //Initializes array
        {
            arr[i, 0] = i + 2000;
            arr[i, 1] = 0;
        }

        foreach (Cell cell in cellPhones)
        {
            try
            {
                int announced = (int)cell.Launch_announced; //I USED LAUNCH ANNOUNCED INSTEAD OF LAUNCH STATUS BECAUSE MOST CASES ARE THE SAME, ALSO A PHONE COULD BE RELEASED IN A CERTAIN YEAR BUT DISCONTINUED AND THE YEAR NOT PROVIDED
                arr[announced - 2000, 1]++;
            }
            catch (Exception) //Ignore null cases
            {
                continue;
            }
        }

        int max = 0;
        int max_year = 0;
        for (int i = 0; i < 21; i++) //Finding max value
        {
            if (arr[i, 1] > max)
            {
                max = arr[i, 1];
                max_year = arr[i, 0];
            }
        }

        Console.WriteLine($"The year {max_year} had the most phones launched with {max} phones\n");
    }

    static void YearWeight(List<Cell> cellPhones)
    {
        Console.WriteLine("Please enter the year you would like to search for: ");
        string user_input = Console.ReadLine();
        int year;
        if (int.TryParse(user_input, out year)) //If the year is a valid integer, return the year
        {
            int count = 0;
            float weight = 0;
            foreach (Cell cell in cellPhones)
            {
                try
                {
                    if (cell.Launch_announced == year)
                    {
                        weight += (float)cell.Body_weight;
                        count++;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            float avg_weight = weight / count;
            Console.WriteLine($"The average weight of phones launched in {year} is {avg_weight}g\n");
        }
        else
        {
            Console.WriteLine("The year entered is not a valid year,\n");
        }
    }
}


class Cell
{
    public string OEM { get; set; }
    public string Model { get; set; }
    public int? Launch_announced { get; set; }
    public object Launch_status { get; set; }
    public string Body_dimensions { get; set; }
    public float? Body_weight { get; set; }
    public string Body_sim { get; set; }
    public string Display_type { get; set; }
    public float? Display_size { get; set; }
    public string Display_resolution { get; set; }
    public string Features_sensors { get; set; }
    public string Platform_os { get; set; }

    public Cell(string oem, string model, int? launch_announced, object launch_status, string body_dimensions, float? body_weight, string body_sim, string display_type, float? display_size, string display_resolution, string features_sensors, string platform_os)
    {
        OEM = oem;
        Model = model;
        Launch_announced = launch_announced;
        Launch_status = launch_status;
        Body_dimensions = body_dimensions;
        Body_weight = body_weight;
        Body_sim = body_sim;
        Display_type = display_type;
        Display_size = display_size;
        Display_resolution = display_resolution;
        Features_sensors = features_sensors;
        Platform_os = platform_os;
    }

    public string toString()
    {
        return $"OEM: {OEM}, Model: {Model}, Launch_announced: {Launch_announced}, Launch_status: {Launch_status}, Body_dimensions: {Body_dimensions}, Body_weight: {Body_weight}, Body_sim: {Body_sim}, Display_type: {Display_type}, Display_size: {Display_size}, Display_resolution: {Display_resolution}, Features_sensors: {Features_sensors}, Platform_os: {Platform_os} \n";
    }
}
