using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lab1
{
    class Program
    {

        enum Commands
        {
            Error,
            InitializeFile,
            Init,
            Status,
            Help
        }
        enum Attributes
        {
            info,
            i,
            open,
            o,
            create,
            c,
            ignor_warnings,
            iw,

        }
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
                    case Attributes.i:
                        attribute = Attributes.info;
                        break;
                    case Attributes.c:
                        attribute = Attributes.create;
                        break;
                    case Attributes.o:
                        attribute = Attributes.open;
                        break;
                    case Attributes.iw:
                        attribute = Attributes.ignor_warnings;
                        break;
                }
            }

        }
        static Command ParseUserCommand(string UserCommand)
        {
            var Result = new Command();
            try
            {
                var Data = UserCommand.Split().Where(x => x.Length != 0).Select(x => x.Trim());
                Result.MainCommand = (Commands)Enum.Parse(typeof(Commands), Data.First(), true);
                Data = Data.Skip(1);
                foreach (var str in Data)
                {
                    if (str[0] != '-') throw new Exception("Attribute \'" + str + "\' does not have leading dash");
                    var attribute = (Attributes)Enum.Parse(typeof(Attributes), str.Substring(1), true);
                    Command.ExpandAttribute(ref attribute);
                    if (Result.CommandAttributes.Contains(attribute)) throw new Exception("Duplication of attributes forbidden");
                    else
                        Result.CommandAttributes.Add(attribute);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Result.MainCommand = Commands.Error;
            }

            return Result;
        }
        static bool UserAgree()
        {
            char c = '*';//?
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
            string Path;
            FileStream MyFile = null;
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
                    Path = Console.ReadLine();
                    FileInfo TempFile = new FileInfo(Path);
                    if (TempFile.Extension == "" && !attributes.Contains(Attributes.ignor_warnings))
                    {
                        Console.WriteLine("*WARNING* File does not have an extension, do you want to define an extension?");
                        if (UserAgree())
                        {
                            Console.WriteLine("Enter an extension");
                            Console.Write(TempFile.Name + ".");
                            string extension = Console.ReadLine();
                            TempFile = new FileInfo(Path + "." + extension);
                        }
                    }


                    if (attributes.Contains(Attributes.create))
                    {
                        MyFile = TempFile.Create();
                        Console.Clear();
                        Console.WriteLine("File succesfully created on path \n{0}", MyFile.Name);
                        break;
                    }
                    else
                    if (!TempFile.Exists)
                    {
                        if (attributes.Contains(Attributes.open))
                        {
                            Console.WriteLine("File does not exist");
                            continue;
                        }

                        Console.WriteLine("File does not exist, do you want create new file on path \n{0}?", TempFile.FullName);
                        if (UserAgree())
                        {
                            MyFile = TempFile.Create();
                            Console.Clear();
                            Console.WriteLine("File succesfully created on path \n{0}", MyFile.Name);
                            break;
                        }
                    }
                    else
                    {
                        MyFile = TempFile.Open(FileMode.Open);
                        Console.Clear();
                        Console.WriteLine("File succesfully opened on path \n{0}", MyFile.Name);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
            return MyFile;
        }
        static void Help(HashSet<Attributes> attributes)
        {

        }
        static void Status(HashSet<Attributes> attributes, FileInfo FileInf)
        {
            if (FileInf == null)
            {
                Console.WriteLine("File was not opened, use command \"InitializeFile\"");
            }
            else
            {
                Console.WriteLine("File opened on the path: {0}", FileInf.FullName);
                Console.WriteLine("Size: {0}", FileInf.Length);
                Console.WriteLine("Name: {0}", FileInf.Name);
                Console.WriteLine("Creation time: {0}", FileInf.CreationTime);
                Console.WriteLine("Directory name: {0}", FileInf.DirectoryName);
            }
        }
        static void Main(string[] args)
        {
            FileStream MyFile = null;
            FileInfo FileInf = null;
            while (true)
            {
                Console.WriteLine("\nType \"Help\" to receive available command list");
                var Command = ParseUserCommand(Console.ReadLine()); //additional attributes will be skiped   


                switch (Command.MainCommand)
                {
                    case Commands.Help:
                        Help(Command.CommandAttributes);
                        break;
                    case Commands.Init:
                    case Commands.InitializeFile:
                        MyFile = InitializeFile(Command.CommandAttributes) ?? MyFile;
                        if (MyFile != null)
                            FileInf = new FileInfo(MyFile.Name);
                        break;
                    case Commands.Status:
                        Status(Command.CommandAttributes, FileInf);
                        break;


                }


            }


        }
    }
}
