using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;

namespace ClassModelComparer
{
    public class PropertyComparison
    {
        private XDocument configDoc;

        private PropertyComparison() { } // Need this for serialization

        public PropertyComparison(XDocument configDoc)
        {
            this.configDoc = configDoc;
            InitializeClass();
        }

        public string PropertyName
        {
            get;
            set; // Would make this private, but must be public for serialization
        }

        public PropertyExistenceEnum Existence
        {
            get;
            set; // Would make this private, but must be public for serialization
        }

        public bool DataTypesAreCompatible
        {
            get;
            set; // Would make this private, but must be public for serialization
        }

        public bool MinOccursAreSame
        {
            get;
            set; // Would make this private, but must be public for serialization
        }

        public bool MaxOccursAreSame
        {
            get;
            set; // Would make this private, but must be public for serialization
        }

        private UmlClassProperty ap;
        public UmlClassProperty ArgoProperty
        {
            get => ap;
            set
            {
                ap = value;
                if (ap != null) PropertyName = ap.Name;
                SetPropertyExistencePropertyValue();
                if (DotNetProperty != null) ComparePropertyDefinitions();
            }
        }

        private UmlClassProperty dnp;
        public UmlClassProperty DotNetProperty
        {
            get => dnp;
            set
            {
                dnp = value;
                if (dnp != null) PropertyName = dnp.Name;
                SetPropertyExistencePropertyValue();
                if (ArgoProperty != null) ComparePropertyDefinitions();
            }
        }

        private void ComparePropertyDefinitions()
        {
            
            if (Existence != PropertyExistenceEnum.Both)
            {
                // This shouldn't happen
                InitializeClass();
                return;
            }

            // MinOccurs
            if (ArgoProperty.MinOccurs == DotNetProperty.MinOccurs || DotNetProperty.MinOccurs == "?")
                MinOccursAreSame = true;
            else
                MinOccursAreSame = false;

            // MaxOccurs
            if (ArgoProperty.MaxOccurs == DotNetProperty.MaxOccurs || DotNetProperty.MaxOccurs == "?")
                MaxOccursAreSame = true;
            else
                MaxOccursAreSame = false;

			// Data Type Compatibility
            var dotNetTypeMod = DotNetProperty.DataType.Replace(" (nullable)", "");
            DataTypesAreCompatible = TypesAreEquivalent(ArgoProperty.DataType, dotNetTypeMod);
            if (!DataTypesAreCompatible)
            {
                if (dotNetTypeMod.Contains("List of ") || dotNetTypeMod.Contains("IEnumerable of "))
                {
                    // This is an inexact process, but probably close enough.
                    DataTypesAreCompatible = TypesAreEquivalent(ArgoProperty.DataType, dotNetTypeMod);
                    if (!DataTypesAreCompatible)
                    {
                        // Might want to drop in a pluralization library.
                        dotNetTypeMod = dotNetTypeMod.Replace("List of ", "");
                        dotNetTypeMod = dotNetTypeMod.Replace("IEnumerable of ", "");
                        DataTypesAreCompatible = TypesAreEquivalent(ArgoProperty.DataType, dotNetTypeMod);
                        if (!DataTypesAreCompatible)
                        {
                            var argoTypeMod = ArgoProperty.DataType + "s"; // One pluralization approach
                            DataTypesAreCompatible = TypesAreEquivalent(argoTypeMod, dotNetTypeMod);
                            if (!DataTypesAreCompatible)
                            {
                                argoTypeMod = ArgoProperty.DataType + "es"; // Another pluralization approach
                                DataTypesAreCompatible = TypesAreEquivalent(argoTypeMod, dotNetTypeMod);
                            }
                        }
                    }
                }
            }
        }

        private bool TypesAreEquivalent(string argoType, string dotNetType)
        {
            if (argoType == dotNetType)
                return true;
            else if (configDoc.XPathSelectElement($"/Config/TypeEquivalency[Argo='{argoType}' and DotNet='{dotNetType}']") != null)
                return true;
            else
                return false;
        }

        public enum PropertyExistenceEnum
        {
            Both,
            ArgoOnly,
            DotNetOnly,
            Neither // This shouldn't happen
        }

        private void SetPropertyExistencePropertyValue()
        {
            Existence = PropertyExistenceEnum.Both; // Default
            if (ArgoProperty != null)
            {
                if (DotNetProperty == null)
                    Existence = PropertyExistenceEnum.ArgoOnly;
            }
            else if (DotNetProperty != null)
            {
                if (ArgoProperty == null)
                    Existence = PropertyExistenceEnum.DotNetOnly;
            }
            else
            {
                Existence = PropertyExistenceEnum.Neither; // This shouldn't happen
            }
        }

        private void InitializeClass()
        {
            Existence = PropertyExistenceEnum.Neither;
            DataTypesAreCompatible = true;
            MinOccursAreSame = true;
            MaxOccursAreSame = true;
            PropertyName = "";
        }
    }
}
