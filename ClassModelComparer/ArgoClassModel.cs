using System;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Ionic.Zip;

namespace ClassModelComparer
{
    public class ArgoClassModel
    {
		XDocument argoModelXmiDoc;
		XDocument argoTypesXmiDoc;
		List<UmlClass> classList = new List<UmlClass>();

        public ArgoClassModel(string argoFilepath)
        {
			ClassList = new List<UmlClass>();
			LoadArgoTypesDoc();
			LoadArgoModelXmiDoc(argoFilepath);
			var namespaceManager = new XmlNamespaceManager(new NameTable());
			namespaceManager.AddNamespace("UML", "org.omg.xmi.namespace.UML");
            foreach (var classElement in argoModelXmiDoc.XPathSelectElements("//UML:Class[@name]", namespaceManager))
            {
				var umlClass = new UmlClass();
				ClassList.Add(umlClass);
				umlClass.Name = classElement.Attribute("name").Value;
                foreach (var propertyElement in classElement.XPathSelectElements("UML:Classifier.feature/UML:Attribute", namespaceManager))
                {
					var umlClassProperty = new UmlClassProperty();
					umlClass.Properties.Add(umlClassProperty);
					umlClassProperty.Name = propertyElement.Attribute("name").Value;
                    umlClassProperty.MinOccurs = propertyElement.XPathSelectElement("UML:StructuralFeature.multiplicity/UML:Multiplicity/UML:Multiplicity.range/UML:MultiplicityRange", namespaceManager).Attribute("lower").Value;
					umlClassProperty.MaxOccurs = propertyElement.XPathSelectElement("UML:StructuralFeature.multiplicity/UML:Multiplicity/UML:Multiplicity.range/UML:MultiplicityRange", namespaceManager).Attribute("upper").Value;
                    umlClassProperty.MaxOccurs = umlClassProperty.MaxOccurs.Replace("-1", "unbounded");
                    XElement typeReferenceElement;
                    var propertyTypeName = "";
                    var propertyTypeClassId = "";
                    typeReferenceElement = propertyElement.XPathSelectElement("UML:StructuralFeature.type/UML:Class", namespaceManager);
                    if (typeReferenceElement != null)
                    {
                        propertyTypeClassId = typeReferenceElement.Attribute("xmi.idref").Value;
                        propertyTypeName = argoModelXmiDoc.XPathSelectElement($"/XMI/XMI.content/UML:Model/UML:Namespace.ownedElement/UML:Class[@xmi.id='{propertyTypeClassId}']", namespaceManager).Attribute("name").Value;
                    }
					typeReferenceElement = propertyElement.XPathSelectElement("UML:StructuralFeature.type/UML:DataType", namespaceManager);
					if (typeReferenceElement != null)
					{
                        if (typeReferenceElement.Attribute("href") != null)
                        {
							propertyTypeClassId = typeReferenceElement.Attribute("href").Value;
							propertyTypeClassId = propertyTypeClassId.Substring(propertyTypeClassId.IndexOf("#") + 1);
							propertyTypeName = argoTypesXmiDoc.XPathSelectElement($"//*[@xmi.id='{propertyTypeClassId}']").Attribute("name").Value;
						}
                        else if (typeReferenceElement.Attribute("xmi.idref") != null)
                        {
							propertyTypeClassId = typeReferenceElement.Attribute("xmi.idref").Value;
							//propertyTypeName = argoModelXmiDoc.XPathSelectElement($"/XMI/XMI.content/UML:Model/UML:Namespace.ownedElement/UML:Class[@xmi.id='{propertyTypeClassId}']", namespaceManager).Attribute("name").Value;
							propertyTypeName = argoModelXmiDoc.XPathSelectElement($"/XMI/XMI.content/UML:Model/UML:Namespace.ownedElement/*[@xmi.id='{propertyTypeClassId}']", namespaceManager).Attribute("name").Value;
						}
					}
					typeReferenceElement = propertyElement.XPathSelectElement("UML:StructuralFeature.type/UML:Enumeration", namespaceManager);
                    if (typeReferenceElement != null)
                    {
						if (typeReferenceElement.Attribute("href") != null)
						{
							propertyTypeClassId = typeReferenceElement.Attribute("href").Value;
							propertyTypeClassId = propertyTypeClassId.Substring(propertyTypeClassId.IndexOf("#") + 1);
							propertyTypeName = argoTypesXmiDoc.XPathSelectElement($"//*[@xmi.id='{propertyTypeClassId}']").Attribute("name").Value;
						}
						else if (typeReferenceElement.Attribute("xmi.idref") != null)
						{
							propertyTypeClassId = typeReferenceElement.Attribute("xmi.idref").Value;
							propertyTypeName = argoModelXmiDoc.XPathSelectElement($"/XMI/XMI.content/UML:Model/UML:Namespace.ownedElement/UML:Enumeration[@xmi.id='{propertyTypeClassId}']", namespaceManager).Attribute("name").Value;
						}
					}
					umlClassProperty.DataType = propertyTypeName;
                }
            }
        }

		private void LoadArgoModelXmiDoc(string argoFilepath)
		{
			using (ZipFile zip = ZipFile.Read(argoFilepath))
			{
				FileInfo fi = new FileInfo(argoFilepath);
				string xmiFilename = fi.Name.Replace("zargo", "xmi");
				ZipEntry e = zip[xmiFilename];
				using (var ms = new MemoryStream())
				{
					e.Extract(ms);
					ms.Position = 0;
					var sr = new StreamReader(ms);
					argoModelXmiDoc = XDocument.Parse(sr.ReadToEnd());
				}
			}
		}

		private void LoadArgoTypesDoc()
		{
			var assembly = Assembly.GetExecutingAssembly();
			// This file comes from http://argoUML:tigris.org//profiles/uml14/default-uml14.xmi
			// which is referenced in the main model XMI file.
			var resourceName = "ClassModelComparer.default-uml14.xmi";
			string argoTypeDocString;
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
				argoTypeDocString = reader.ReadToEnd();
			argoTypesXmiDoc = XDocument.Parse(argoTypeDocString);
		}

		public List<UmlClass> ClassList { get; set; }
    }
}
