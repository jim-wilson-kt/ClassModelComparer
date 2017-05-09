using System;
using System.Reflection;
using System.Collections.Generic;

namespace ClassModelComparer
{
	public class DotNetClassModel
	{
		public DotNetClassModel(string assemblyFilepath)
		{
			ClassList = new List<UmlClass>();
			Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFilepath);
			Console.WriteLine(assembly.ImageRuntimeVersion);
			Console.WriteLine(assembly.GetName().Version);
			foreach (Type type in assembly.GetTypes())
			{
				var umlClass = new UmlClass();
				ClassList.Add(umlClass);
				umlClass.FullName = type.FullName;
				umlClass.Name = type.Name;
				umlClass.Namespace = type.Namespace;
				umlClass.IsEnum = type.IsEnum;
				umlClass.BaseType = type.BaseType?.FullName ?? "NONE";
				foreach (var prop in assembly.GetType(type.FullName).GetProperties())
				{
					var umlClassProperty = new UmlClassProperty();
					umlClass.Properties.Add(umlClassProperty);
					umlClassProperty.Name = RemoveTickMarkContent(prop.Name);
					var propType = "";

					if (type.ContainsGenericParameters)
					{
						propType = "GENERIC";
					}
					else
					{
						propType = RemoveTickMarkContent(prop.PropertyType.Name);
						if (propType == "List")
						{
							var listOfType = RemoveTickMarkContent(prop.PropertyType.GetGenericArguments()[0].Name);
							propType = $"List of {listOfType}";
							umlClassProperty.MinOccurs = "0";
							umlClassProperty.MaxOccurs = "unbounded";
						}
						if (propType == "Nullable")
						{
							var nullibleType = RemoveTickMarkContent(prop.PropertyType.GetGenericArguments()[0].Name);
							propType = $"{nullibleType} (nullable)";
							umlClassProperty.MinOccurs = "0";
							umlClassProperty.MaxOccurs = "1";
						}
						if (propType == "IEnumerable")
						{
							var enumerableType = RemoveTickMarkContent(prop.PropertyType.GetGenericArguments()[0].Name);
							propType = $"IEnumerable of {enumerableType}";
							umlClassProperty.MinOccurs = "0";
							umlClassProperty.MaxOccurs = "unbounded";
						}
					}
					umlClassProperty.DataType = propType;
				}
			}
		}

		string RemoveTickMarkContent(string text)
		{
			if (text.Contains("`"))
				text = text.Substring(0, text.IndexOf("`"));
			return text;
		}

		public List<UmlClass> ClassList { get; set; }
	}
}