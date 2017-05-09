using System;

namespace ClassModelComparer
{
    public class UmlClassProperty
    {
        public UmlClassProperty()
        {
			MinOccurs = "?";
			MaxOccurs = "?";
        }

        public string Name
        {
            get;
            set;
        }
        public string DataType
        {
            get;
            set;
        }
        public string MinOccurs
        {
            get;
            set;
        }
        public string MaxOccurs
        {
            get;
            set;
        }
    }
}
