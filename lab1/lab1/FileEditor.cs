using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            Delete
        }
        enum Attributes
        {
            Info,
            I,
            Open,
            O,
            Create,
            C,
            IgnorWarnings,
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
                        attribute = Attributes.IgnorWarnings;
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
                    if (tempFile.Extension == "" && !attributes.Contains(Attributes.IgnorWarnings))
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

        }
        static void ShowStatus(HashSet<Attributes> attributes, FileInfo fileInf)
        {
            Console.Clear();
            if (fileInf == null)
            {
                Console.WriteLine("File was not opened, use command \"InitializeFile\"");
                return;
            }

            Console.WriteLine("Full name: {0}", fileInf.FullName);
            Console.WriteLine("Size (Bytes): {0}", fileInf.Length);
            Console.WriteLine("Creation time: {0}", fileInf.CreationTime);

            if (attributes.Contains(Attributes.All))
            {
                Console.WriteLine("Directory name: {0}", fileInf.DirectoryName);
                Console.WriteLine("Name: {0}", fileInf.Name);
                Console.WriteLine("Extension: {0}", fileInf.Extension);
                Console.WriteLine("Attributes: {0}", fileInf.Attributes);
                Console.WriteLine("Last access time: {0}", fileInf.LastAccessTime);
                Console.WriteLine("Last write time: {0}", fileInf.LastWriteTime);
            }
        }
        static void DeleteFile(HashSet<Attributes> attributes, FileInfo fileInf)
        {
            Console.Clear();
            if (fileInf == null)
            {
                Console.WriteLine("File was not opened, use command \"InitializeFile\"");
                return;
            }
        }
        static bool IsValidAttributes(Command command)
        {
            foreach (var attribute in command.CommandAttributes)
                if (!AttributesOfCommand[command.MainCommand].Contains(attribute))
                    return false;

            return true;
        }
        static void InitializeDict() //TODO 
        {
            Attributes[] temp1 = { Attributes.Create, Attributes.IgnorWarnings, Attributes.Open };
            AttributesOfCommand.Add(Commands.InitializeFile, new List<Attributes>(temp1));

            Attributes[] temp2 = { Attributes.All };
            AttributesOfCommand.Add(Commands.Status, new List<Attributes>(temp2));

            Attributes[] temp3 = { };
            AttributesOfCommand.Add(Commands.Delete, new List<Attributes>(temp3));            

            Attributes[] temp4 = { };
            AttributesOfCommand.Add(Commands.Exit, new List<Attributes>(temp4));

            Attributes[] temp5 = { };
            AttributesOfCommand.Add(Commands.Help, new List<Attributes>(temp5));
        }
        static public void Start()
        {
            FileStream myFile = null;
            FileInfo fileInf = null;
            bool flagExit = false;
            InitializeDict();
            while (!flagExit)
            {
                Console.WriteLine("\nType \"Help\" to receive available command list");
                var command = ParseUserCommand(Console.ReadLine()); //additional attributes will be skiped   

                if (command.MainCommand == Commands.Exit)
                    continue;

                if (!IsValidAttributes(command))
                {
                    Console.WriteLine("*WARNING* Some attributes are redundant");
                    Console.WriteLine("should we skip them?");
                    if (!UserAgree())
                        continue;
                }

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
                        ShowStatus(command.CommandAttributes, fileInf);
                        break;
                    case Commands.Exit:
                        Console.WriteLine("Successfull shutdown");
                        flagExit = true;
                        break;
                    case Commands.Delete:
                        DeleteFile(command.CommandAttributes, fileInf);
                        break;


                }


            }


        }
    }
}
