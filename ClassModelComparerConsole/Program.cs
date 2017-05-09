using System;
using System.Xml.Linq;
using static System.Console;

namespace ClassModelComparerConsole
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            var configDoc = XDocument.Load("Config.xml");
            var classModelComparer = new ClassModelComparer.ClassModelComparer(configDoc);
            var classModelComparison = classModelComparer.CompareClassModels();
            WriteLine("DONE!");
			WriteLine("Press the \"Enter\" key to continue.");
            Console.ReadLine();
        }
    }
}
