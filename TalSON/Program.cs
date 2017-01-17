using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalSON
{
    class Program
    {
        public static int Type_Other = 0;
        public static int Type_Class = 1;
        public static int Type_Array = 2;
        public static int Type_String = 3;
        public static int Type_Double = 4;
        public static int Type_Int = 5;
        public static int Type_Boolean = 6;

        string name;
        string value;
        /**
         * 0 = Other / Unidentified
         * 1 = Class
         * 2 = Array
         * 3 = String
         * 4 = Double
         * 5 = Int
         * 6 = Boolean
         **/
        int type;
        List<Program> children;

        static void Main(string[] args)
        {
            TextReader tr = new StreamReader(@"Test1.JSON");
            string buffer = tr.ReadLine();
            Console.WriteLine(buffer);
            Console.WriteLine();

            Console.WriteLine("Read from file: " + Program.parse(buffer).toText());

            Program json = new Program();
            json.type = 1;
            json.children = new List<Program>();
            Program json2 = new Program();
            json2.type = 5;
            json2.name = "credits";
            json2.value = "8600";
            json.children.Add(json2);
            Program json3 = new Program();
            json3.type = 5;
            json3.name = "duckats";
            json3.value = "150";
            json.children.Add(json3);

            Console.WriteLine("Created in code: "+json.toText());
        }
        public Program()
        {
            this.name = null;
            this.value = null;
            this.type = 0;
            this.children = null;
        }
        public static Program parse(string parseString)
        {
            Program json = new Program();

            int i = 0;
            int index = 0;  //where the current object being read starts
            bool isString = false;  // is 'i' currently in a string
            while (i< parseString.Length)
            {
                if (parseString[i] == '"')
                {
                    isString = !isString;
                }
                else if (!isString)
                {
                    //Class
                    if (parseString[i] == '{' && i!=0)
                    {
                        int tmp = endOfObject(parseString.Substring(i),'{','}');
                        if (json.children == null)
                        {
                            json.children = new List<Program>();
                        }
                        json.children.Add(Program.parse(parseString.Substring(i).Remove(tmp)+"}"));
                    }
                    //Array
                    else if (parseString[i] == '[')
                    {

                    }
                    //Left side = Name, Right side = data
                    else if (parseString[i] == ':')
                    {
                        //Possible errors if i=0....
                        json.name = parseString.Remove(i).Substring(index);
                        index = i + 1;
                    }
                    //Next object
                    else if (parseString[i] == ',')
                    {

                    }
                }

                if (json.value != null)
                {
                    json.type = Program.Type_String;
                    if (json.value[0] == '"' && json.value.Last() == '"')
                    {
                        json.value.Remove(json.value.Length - 1).Substring(1);
                        json.type = Program.Type_String;
                    }
                    
                }
                i++;
            }
            return json;
        }

        public string toText()
        {
            string buffer = "";

            if (this.value != null) //If this isn't a collection
            {
                if (this.name != null)
                {
                    buffer += '"' + this.name + '"'+":";
                }
                if (this.type == 3) //is string
                {
                    buffer += '"' + this.value + '"';
                }
                else
                {
                    buffer += this.value;
                }
            }
            else
            {
                char start = '[';
                char end = ']';

                if (this.type == 1) //is class
                {
                    start = '{';
                    end = '}';
                }

                buffer += start;

                if (this.children != null)
                {
                    foreach (Program child in this.children)
                    {
                        buffer += child.toText();
                        buffer += ",";
                    }
                    if (buffer[buffer.Length - 1] == ',')
                    {
                        buffer = buffer.Remove(buffer.Length - 1);
                    }
                }

                buffer += end;
            }

            return buffer;
        }

        /**
         * Note: String should start with startChar!
         * Return
         *  -1 = end not found
         **/
        public static int endOfObject(string text, char startChar, char endChar)
        {
            int i = 0;
            int level = 0;
            bool isString = false;  // is 'i' currently in a string
            while (i<text.Length)
            {
                if (text[i] == '"')
                {
                    isString = !isString;
                }
                else if (!isString)
                {
                    if (text[i] == startChar)
                    {
                        level++;
                    }
                    else if (text[i] == endChar)
                    {
                        level--;
                        if (level == 0)
                        {
                            return i;
                        }
                    }
                }
                i++;
            }
            return -1;
        }
    }
}
