using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace ClassModelComparer
{
	public class ClassModelComparer
	{
		ArgoClassModel argoClassModel;
		DotNetClassModel dotNetClassModel;
		XDocument configDoc;

		public ClassModelComparer(XDocument configDoc)
		{
			this.configDoc = configDoc;
			string argoModelFilepath = configDoc.XPathSelectElement("/Config/ArgoUmlFilepath").Value;
            string dotNetAssemblyFilepath = configDoc.XPathSelectElement("/Config/DotNetAssemblyFilepath").Value;
			argoClassModel = new ArgoClassModel(argoModelFilepath);
			dotNetClassModel = new DotNetClassModel(dotNetAssemblyFilepath);
            CompareClassModels();
		}

        private void SerializeClassModelComparison()
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<ClassComparison>));
			using (StringWriter sww = new StringWriter())
			{
				using (XmlWriter writer = XmlWriter.Create(sww))
				{
                    ser.Serialize(writer, ClassComparisonList);
                    string serializationResult = sww.ToString();
					serializationResult = serializationResult.Replace("</ArrayOfClassComparison>", configDoc.Root.ToString() + "</ArrayOfClassComparison>");
                    var doc = XDocument.Parse(serializationResult);
                    doc.Root.Name = "ClassComparison";
                    doc.Save(GetOutputFilepath("Model_Comparison", "xml"));
				} 
			}
        }

        public List<ClassComparison> CompareClassModels()
        {
            if (ClassComparisonList != null)
                return ClassComparisonList;
			ClassComparisonList = new List<ClassComparison>();

			foreach (var argoClass in ArgoClassList)
            {
                var classComparison = new ClassComparison(configDoc);
                ClassComparisonList.Add(classComparison);
                classComparison.ArgoClass = argoClass;
                var dotNetClass = DotNetClassList.Where(x => x.Name == classComparison.ClassName).FirstOrDefault();
                if (dotNetClass != null)
                    classComparison.DotNetClass = dotNetClass;
            }
            foreach (var dotNetClass in DotNetClassList)
            {
                if (ClassComparisonList.Where(x => x.ClassName == dotNetClass.Name).FirstOrDefault() == null)
                {
					var classComparison = new ClassComparison(configDoc);
					ClassComparisonList.Add(classComparison);
                    classComparison.DotNetClass = dotNetClass;
				}
            }
            SerializeClassModelComparison();
            ProduceHtmlReport();
            ProduceIssueLog();
            return ClassComparisonList;
        }

        private string GetOutputFilepath(string fileType, string extension)
        {
			string outputPath = configDoc.XPathSelectElement("/Config/OutputDirectoryPath").Value;
			string outputFilenamePrefix = configDoc.XPathSelectElement("/Config/OutputFilenamePrefix").Value;
            string dateTimePart = DateTime.UtcNow.ToString("yyyy-MM-ddTHH.mm.ss");
            string filename = $"{outputFilenamePrefix}_{fileType}_{dateTimePart}.{extension}";
            return Path.Combine(outputPath, filename);
        }

        private void ProduceHtmlReport()
        {
            // ToDo: Implement this
        }

        private void AppendSectionTitle(string sectionTitle, StringBuilder sb, bool omitLeadingCrLf = false)
        {
            if (!omitLeadingCrLf) sb.AppendLine("");
            sb.AppendLine("=========================================");
            sb.AppendLine("  " + sectionTitle);
			sb.AppendLine("=========================================");
            sb.Append("");
		}

        private void ProduceIssueLog()
        {
            var sb = new StringBuilder();

            AppendSectionTitle("Argo-Only Classes", sb, true);
            foreach (var classComparison in ClassComparisonList.Where(x=>x.Existence==ClassComparison.ClassExistenceEnum.ArgoOnly))
                sb.AppendLine(classComparison.ClassName);

            AppendSectionTitle("DotNet-Only Classes", sb);
            foreach (var classComparison in ClassComparisonList.Where(x => x.Existence == ClassComparison.ClassExistenceEnum.DotNetOnly))
				sb.AppendLine(classComparison.ClassName);

			// Yes, it would be faster to iterate over ClassComparisonList list only once,
			// but this is fast already and the code is clean. So good enough.
			AppendSectionTitle("Argo-Only Properties", sb);
			foreach (var classComparison in ClassComparisonList.Where(x => x.Existence == ClassComparison.ClassExistenceEnum.Both))
                foreach (var propertyComparison in classComparison.PropertyComparisonList.Where(x=>x.Existence==PropertyComparison.PropertyExistenceEnum.ArgoOnly))
                    sb.AppendLine($"{classComparison.ClassName}.{propertyComparison.PropertyName}");

			AppendSectionTitle("DotNet-Only Properties", sb);
			foreach (var classComparison in ClassComparisonList.Where(x => x.Existence == ClassComparison.ClassExistenceEnum.Both))
                foreach (var propertyComparison in classComparison.PropertyComparisonList.Where(x => x.Existence == PropertyComparison.PropertyExistenceEnum.DotNetOnly))
					sb.AppendLine($"{classComparison.ClassName}.{propertyComparison.PropertyName}");

            AppendSectionTitle("Incompatible Data Types (Argo/Dotnet)", sb);
			foreach (var classComparison in ClassComparisonList.Where(x => x.Existence == ClassComparison.ClassExistenceEnum.Both))
                foreach (var propertyComparison in classComparison.PropertyComparisonList.Where(x => !x.DataTypesAreCompatible))
                    sb.AppendLine($"{classComparison.ClassName}.{propertyComparison.PropertyName}: {propertyComparison.ArgoProperty.DataType}/{propertyComparison.DotNetProperty.DataType}");

			AppendSectionTitle("MinOccurs Mismatch (Argo/Dotnet)", sb);
			foreach (var classComparison in ClassComparisonList.Where(x => x.Existence == ClassComparison.ClassExistenceEnum.Both))
                foreach (var propertyComparison in classComparison.PropertyComparisonList.Where(x => !x.MinOccursAreSame))
                    sb.AppendLine($"{classComparison.ClassName}.{propertyComparison.PropertyName}: {propertyComparison.ArgoProperty.MinOccurs}/{propertyComparison.DotNetProperty.MinOccurs}");

			AppendSectionTitle("MaxOccurs Mismatch (Argo/Dotnet)", sb);
			foreach (var classComparison in ClassComparisonList.Where(x => x.Existence == ClassComparison.ClassExistenceEnum.Both))
				foreach (var propertyComparison in classComparison.PropertyComparisonList.Where(x => !x.MaxOccursAreSame))
					sb.AppendLine($"{classComparison.ClassName}.{propertyComparison.PropertyName}/{propertyComparison.ArgoProperty.MaxOccurs}/{propertyComparison.DotNetProperty.MaxOccurs})");

            // Write to file
            File.WriteAllText(GetOutputFilepath("Discrepancy_Log", "txt"), sb.ToString());
        }

        public List<ClassComparison> ClassComparisonList
        {
            private set;
            get;
        }

		public List<UmlClass> ArgoClassList
		{
			get { return argoClassModel.ClassList; }
		}

		public List<UmlClass> DotNetClassList
		{
			get { return dotNetClassModel.ClassList; }
		}
	}
}
