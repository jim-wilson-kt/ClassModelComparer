using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;

namespace ClassModelComparer
{
    public class ClassComparison
    {
        private XDocument configDoc;

        private ClassComparison() { } // Need this for serialization

        public ClassComparison(XDocument configDoc)
        {
            this.configDoc = configDoc;
            InitializeClass();
        }

        public string ClassName
        {
            get;
            set; // Would make this private, but must be public for serialization
        }

        public ClassExistenceEnum Existence
        {
            get;
            set; // Would make this private, but must be public for serialization
		}

        public List<PropertyComparison> PropertyComparisonList
        {
            get;
            set; // Would make this private, but must be public for serialization
		}

        public bool HasArgoOnlyProperties
        {
            get;
            set; // Would make this private, but must be public for serialization
		}

        public bool HasDotNetOnlyProperties
        {
            get;
            set; // Would make this private, but must be public for serialization
		}

        public bool HasIncompatibleProperties // e.g., data types or multiplicity
        {
            get;
            set; // Would make this private, but must be public for serialization
		}

        private UmlClass ac;
        public UmlClass ArgoClass
        {
            get => ac;
            set
            {
                ac = value;
                SetClassExistencePropertyValue();
                if (ac != null) ClassName = ac.Name;
                if (DotNetClass != null) CompareProperties();
            }
        }

        private UmlClass dnc;
        public UmlClass DotNetClass
        {
            get => dnc;
            set
            {
                dnc = value;
                SetClassExistencePropertyValue();
                if (dnc != null) ClassName = dnc.Name;
                if (ArgoClass != null) CompareProperties();
            }
        }

        public enum ClassExistenceEnum
        {
            Both,
            ArgoOnly,
            DotNetOnly,
            Neither // This shouldn't happen
        }

        private void SetClassExistencePropertyValue()
        {
            Existence = ClassExistenceEnum.Both; // Default
            if (ArgoClass != null)
            {
                if (DotNetClass == null)
                    Existence = ClassExistenceEnum.ArgoOnly;
            }
            else if (DotNetClass != null)
            {
                if (ArgoClass == null)
                    Existence = ClassExistenceEnum.DotNetOnly;
            }
            else
            {
                Existence = ClassExistenceEnum.Neither; // This shouldn't happen
            }
        }

        private void CompareProperties()
        {
            SetClassExistencePropertyValue();
            if (Existence != ClassExistenceEnum.Both)
            {
                // This shouldn't happen
                return;
            }
            foreach (var argoProperty in ArgoClass.Properties)
            {
                var propertyComparison = new PropertyComparison(configDoc);
                PropertyComparisonList.Add(propertyComparison);
                propertyComparison.ArgoProperty = argoProperty;
                var dotNetProperty = DotNetClass.Properties.Where(x => x.Name == propertyComparison.PropertyName).FirstOrDefault();
                if (dotNetProperty != null) propertyComparison.DotNetProperty = dotNetProperty;
            }
            foreach (var dotNetProperty in DotNetClass.Properties)
            {
                if (PropertyComparisonList.Where(x => x.PropertyName == dotNetProperty.Name).FirstOrDefault() == null)
                {
                    var propertyComparison = new PropertyComparison(configDoc);
                    PropertyComparisonList.Add(propertyComparison);
                    propertyComparison.DotNetProperty = dotNetProperty;
                }
            }
            HasArgoOnlyProperties = PropertyComparisonList.Where(x => x.Existence == PropertyComparison.PropertyExistenceEnum.ArgoOnly).Count() > 0;
            HasDotNetOnlyProperties = PropertyComparisonList.Where(x => x.Existence == PropertyComparison.PropertyExistenceEnum.DotNetOnly).Count() > 0;
            HasIncompatibleProperties = PropertyComparisonList.Where(x => x.DataTypesAreCompatible == false).Count() > 0;
        }

        private bool AlreadyInInitializeClass;
        private void InitializeClass()
        {
            if (AlreadyInInitializeClass)
            {
                return;
            }
            else
            {
                AlreadyInInitializeClass = true;
                HasArgoOnlyProperties = false;
                HasDotNetOnlyProperties = false;
                HasIncompatibleProperties = false;
                PropertyComparisonList = new List<PropertyComparison>();
                ClassName = "";
                AlreadyInInitializeClass = false;
                return;
            }
        }
    }
}