using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Text;

namespace lab1
{
    public class FileEditor
    {
        enum Commands
        {
            Error,
            InitializeFile,
            Init,
            Status,
            Help,
            Exit,
            Delete,
            Close,
            Compress,
            DeCompress
        }
        enum Attributes
        {
            Info,
            I,
            Open,
            O,
            Create,
            C,
            IgnoreWarnings,
            Iw,
            All
        }
        static Dictionary<Commands, List<Attributes>> AttributesOfCommand = new Dictionary<Commands, List<Attributes>>();
        class Command
        {
            public Commands MainCommand;
            public HashSet<Attributes> CommandAttributes;
            public Command()
            {
                MainCommand = Commands.Error;
                CommandAttributes = new HashSet<Attributes>();
                //CommandAttributes = new Dictionary<Attributes, bool>();
                //foreach (Attributes Attribute in Enum.GetValues(typeof(Attributes)))
                //{
                //    CommandAttributes.Add(Attribute, false);
                //}
            }
            public static void ExpandAttribute(ref Attributes attribute)
            {
                switch (attribute)
                {
                    case Attributes.I:
                        attribute = Attributes.Info;
                        break;
                    case Attributes.C:
                        attribute = Attributes.Create;
                        break;
                    case Attributes.O:
                        attribute = Attributes.Open;
                        break;
                    case Attributes.Iw:
                        attribute = Attributes.IgnoreWarnings;
                        break;
                }
            }

        }
        static Command ParseUserCommand(string userCommand)
        {
            var result = new Command();
            try
            {
                var Data = userCommand.Split().Where(x => x.Length != 0).Select(x => x.Trim());
                result.MainCommand = (Commands)Enum.Parse(typeof(Commands), Data.First(), true);
                if (result.MainCommand == Commands.Init)
                    result.MainCommand = Commands.InitializeFile;
                Data = Data.Skip(1);
                foreach (var str in Data)
                {
                    if (str[0] != '-') throw new Exception("Attribute \'" + str + "\' does not have leading dash");
                    var attribute = (Attributes)Enum.Parse(typeof(Attributes), str.Substring(1), true);
                    Command.ExpandAttribute(ref attribute);
                    if (result.CommandAttributes.Contains(attribute)) throw new Exception("Duplication of attributes forbidden");
                    else
                        result.CommandAttributes.Add(attribute);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result.MainCommand = Commands.Error;
            }

            return result;
        }
        static bool UserAgree()
        {
            char c;
            while (true)
            {
                Console.WriteLine("(Y)es or (N)o");
                c = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (c == 'y' || c == 'Y') return true;
                else
                    if (c == 'n' || c == 'N') return false;
                else
                    Console.WriteLine("Try again");
            }
        }
        static FileStream InitializeFile(HashSet<Attributes> attributes)
        {
            string path;
            FileStream myFile = null;
            bool firstEnter = true;
            while (true)
            {
                try
                {
                    if (!firstEnter)
                    {
                        Console.WriteLine("Do you want to try to initialize the file again?");
                        if (!UserAgree())
                            break;
                    }
                    firstEnter = false;
                    Console.Clear();
                    Console.WriteLine("Enter path, name and extension of your file in format \"Disk:\\path\\nameOfYourFile.extension\"");
                    path = Console.ReadLine();
                    FileInfo tempFile = new FileInfo(path);
                    if (tempFile.Extension == "" && !attributes.Contains(Attributes.IgnoreWarnings))
                    {
                        Console.WriteLine("*WARNING* File does not have an extension, do you want to define an extension?");
                        if (UserAgree())
                        {
                            Console.WriteLine("Enter an extension");
                            Console.Write(tempFile.Name + ".");
                            string extension = Console.ReadLine();
                            tempFile = new FileInfo(path + "." + extension);
                        }
                    }


                    if (attributes.Contains(Attributes.Create))
                    {
                        if (myFile != null)
                            myFile.Close();
                        myFile = tempFile.Create();
                        Console.Clear();
                        Console.WriteLine("File succesfully created on path \n{0}", myFile.Name);
                        break;
                    }
                    else
                    if (!tempFile.Exists)
                    {
                        if (attributes.Contains(Attributes.Open))
                        {
                            Console.WriteLine("File does not exist");
                            continue;
                        }

                        Console.WriteLine("File does not exist, do you want create new file on path \n{0}?", tempFile.FullName);
                        if (UserAgree())
                        {
                            if (myFile != null)
                                myFile.Close();
                            myFile = tempFile.Create();
                            Console.Clear();
                            Console.WriteLine("File succesfully created on path \n{0}", myFile.Name);
                            break;
                        }
                    }
                    else
                    {
                        myFile = tempFile.Open(FileMode.Open);
                        Console.Clear();
                        Console.WriteLine("File succesfully opened on path \n{0}", myFile.Name);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
            return myFile;
        }
        static void ShowHelp(HashSet<Attributes> attributes)
        {
            string GetFormattedAttributes(List<Attributes> attributes)
            {
                var ans = new StringBuilder();
                foreach (var attribute in attributes)
                {
                    ans.Append('-' + attribute.ToString() + ", ");
                }
                if (ans.Length == 0) return "";
                else
                    return '(' + ans.ToString().Substring(0, ans.Length - 2) + ')';
            }

            Console.Clear();
            if (!attributes.Contains(Attributes.All))
            {
                Console.WriteLine("Possible commands with their optional attributes:");
                foreach (var e in AttributesOfCommand)
                {
                    Console.WriteLine("{0} {1}", e.Key.ToString(), GetFormattedAttributes(e.Value));
                }
                Console.WriteLine("\nFor more information use command Help -All");
            }
            else
            {
                Console.WriteLine(
                    "command InitializeFile (shortly init) let you initialize file by entering path of the file\n" +
                    "possible attributes are:\n" +
                    "-Create (shortly -C) let you rewrite an existing file\n" +
                    "-Open (shortly -O) let you skip question about opening file, also can be used with -Create attribute\n" +
                    "-IgnoreWarnings (shortly -IW) let you skip some questions\n" +
                    "\n" +
                    "command Status let you find out path and size of your file and with\n" +
                    "attribute -All a lot of additional information like time of creating file,\n" +
                    "local directory files, extension of your file and etc.\n" +
                    "\n" +
                    "command Compress will compress your file in .zip archieve can be used with attributes\n" +
                    "-IgnoreWarnings (-IW) to skip some questions\n" +
                    "-Info (-I) to examine compressed file\n" +
                    "-All to get more information by attribute -Info\n" +
                    "\n" +
                    "command DeCompress will deArchieve your file for .zip archieve if it was compressed by this programm\n" +
                    "can be used with atrributes -Info and -All result same as Compress\n" +
                    "\n" +
                    "command Delete will delete the opened file from computer\n" +
                    "\n" +
                    "command Close will close the opened file (same goal can be achieved by using command Init)\n" +
                    "\n" +
                    "command Exit will shutdown the programm with closing file\n" +
                    "\n" +
                    "command ");
            }
        }
        static void ShowStatus(HashSet<Attributes> attributes, FileInfo fileInf)
        {
            Console.WriteLine("The file successfully opened on the path:");
            Console.WriteLine(fileInf.FullName);
            Console.WriteLine("Size (Bytes): {0}", fileInf.Length);

            if (attributes.Contains(Attributes.All))
            {
                Console.WriteLine("Directory name: {0}", fileInf.DirectoryName);
                Console.WriteLine("Name: {0}", fileInf.Name);
                Console.WriteLine("Extension: {0}", fileInf.Extension);
                Console.WriteLine("Attributes: {0}", fileInf.Attributes);
                Console.WriteLine("Creation time: {0}", fileInf.CreationTime);
                Console.WriteLine("Last access time: {0}", fileInf.LastAccessTime);
                Console.WriteLine("Last write time: {0}", fileInf.LastWriteTime);
                int count = 0;
                foreach (var e in fileInf.Directory.GetFiles())
                    count++;
                foreach (var e in fileInf.Directory.GetDirectories())
                    count++;

                if (count > 5)
                    Console.WriteLine("The number of local files and directories is {0}, should we show them?", count);

                if (count <= 5 || UserAgree())
                {
                    Console.WriteLine("The number of local directory files and directories is {0}:", count);
                    foreach (var e in fileInf.Directory.GetFiles())
                        Console.WriteLine(e.Name);
                    foreach (var e in fileInf.Directory.GetDirectories())
                        Console.WriteLine(e.Name);
                }
            }
        }
        static bool IsValidAttributes(Command command)
        {
            foreach (var attribute in command.CommandAttributes)
                if (!AttributesOfCommand[command.MainCommand].Contains(attribute))
                    return false;

            return true;
        }
        static void CompressFile(HashSet<Attributes> attributes, ref FileStream file, ref FileInfo fileInf)
        {
            if ((fileInf.Extension == ".zip" || fileInf.Extension == ".rar" || fileInf.Extension == ".gz") && !attributes.Contains(Attributes.IgnoreWarnings))
            {
                Console.WriteLine("*WARNING* It's look like file is already compressed");
                Console.WriteLine("Do you want to compress anyway?");
                if (!UserAgree())
                    return;
            }
            string nameBeforeCompression = fileInf.Name;
            string hren = DateTime.Now.Ticks.ToString();
            DirectoryInfo directoryInf = new DirectoryInfo(fileInf.DirectoryName + '\\' + hren);
            directoryInf.Create();
            file.Close();
            fileInf.MoveTo(directoryInf.FullName + '\\' + fileInf.Name);
            ZipFile.CreateFromDirectory(directoryInf.FullName, fileInf.DirectoryName + ".zip");
            foreach (var oneFile in directoryInf.GetFiles())
                oneFile.Delete();
            directoryInf.Delete();
            fileInf = new FileInfo(fileInf.DirectoryName + ".zip");
            fileInf.MoveTo(fileInf.DirectoryName + '\\' + nameBeforeCompression + ".zip", true);
            file = fileInf.Open(FileMode.Open);
            Console.WriteLine("File compressed successfully");
            if (attributes.Contains(Attributes.Info))
                ShowStatus(attributes, fileInf);
        }
        static void DeCompressFile(HashSet<Attributes> attributes, ref FileStream file, ref FileInfo fileInf)
        {
            try
            {
                file.Close();
                ZipFile.ExtractToDirectory(fileInf.FullName, fileInf.DirectoryName, true);
                fileInf.Delete();
                fileInf = new FileInfo(fileInf.DirectoryName + '\\' + fileInf.Name.Substring(0, fileInf.Name.Length - 4));
                file = fileInf.Open(FileMode.Open);
                Console.WriteLine("File decompressed successfully");
                if (attributes.Contains(Attributes.Info))
                    ShowStatus(attributes, fileInf);
            }
            catch
            {
                Console.WriteLine("File was not compressed");
            }
        }
        static void InitializeDict() //TODO 
        {

            Attributes[] temp1 = { Attributes.Create, Attributes.IgnoreWarnings, Attributes.Open };
            AttributesOfCommand.Add(Commands.InitializeFile, new List<Attributes>(temp1));

            Attributes[] temp2 = { Attributes.All };
            AttributesOfCommand.Add(Commands.Status, new List<Attributes>(temp2));

            Attributes[] temp3 = { Attributes.Info, Attributes.IgnoreWarnings, Attributes.All };
            AttributesOfCommand.Add(Commands.Compress, new List<Attributes>(temp3));

            Attributes[] temp4 = { Attributes.Info, Attributes.All };
            AttributesOfCommand.Add(Commands.DeCompress, new List<Attributes>(temp4));

            Attributes[] temp5 = { Attributes.All };
            AttributesOfCommand.Add(Commands.Help, new List<Attributes>(temp5));

            AttributesOfCommand.Add(Commands.Delete, new List<Attributes>());
            AttributesOfCommand.Add(Commands.Exit, new List<Attributes>());
            AttributesOfCommand.Add(Commands.Close, new List<Attributes>());
        }
        static public void Start()
        {
            Console.SetWindowSize(Console.LargestWindowWidth - 24, Console.LargestWindowHeight - 10);
            Console.SetWindowPosition(0, 0);
            Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, args) => Console.WriteLine("Successfull shutdown (using {0})", args.SpecialKey));
            FileStream myFile = null;
            FileInfo fileInf = null;
            bool flagExit = false;
            InitializeDict();
            while (!flagExit)
            {
                Console.WriteLine("\nType Help to receive available command list");
                Console.Write("> ");
                var command = ParseUserCommand(Console.ReadLine()); //additional attributes will be skiped   

                if (command.MainCommand == Commands.Error)
                    continue;

                if (myFile == null)
                    if (command.MainCommand != Commands.InitializeFile && command.MainCommand != Commands.Help)
                    {
                        Console.Clear();
                        Console.WriteLine("File was not opened, use command InitializeFile");
                        continue;
                    }

                if (!IsValidAttributes(command))
                {
                    Console.WriteLine("*WARNING* Some attributes are redundant");
                    Console.WriteLine("should we skip them?");
                    if (!UserAgree())
                    {
                        Console.Clear();
                        continue;
                    }
                }

                try
                {
                    switch (command.MainCommand)
                    {
                        case Commands.Help:
                            ShowHelp(command.CommandAttributes);
                            break;
                        case Commands.InitializeFile:
                            myFile = InitializeFile(command.CommandAttributes) ?? myFile;
                            if (myFile != null)
                                fileInf = new FileInfo(myFile.Name);
                            break;
                        case Commands.Status:
                            Console.Clear();
                            ShowStatus(command.CommandAttributes, fileInf);
                            break;
                        case Commands.Exit:
                            Console.WriteLine("Successfull shutdown");
                            flagExit = true;
                            break;
                        case Commands.Delete:
                            Console.Clear();
                            myFile.Close();
                            fileInf.Delete();
                            myFile = null;
                            fileInf = null;
                            Console.WriteLine("File deleted successfully");
                            break;
                        case Commands.Close:
                            Console.Clear();
                            myFile.Close();
                            myFile = null;
                            fileInf = null;
                            Console.WriteLine("File closed successfully");
                            break;
                        case Commands.Compress:
                            Console.Clear();
                            CompressFile(command.CommandAttributes, ref myFile, ref fileInf);
                            break;
                        case Commands.DeCompress:
                            Console.Clear();
                            DeCompressFile(command.CommandAttributes, ref myFile, ref fileInf);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }




            }


        }


    }
}
