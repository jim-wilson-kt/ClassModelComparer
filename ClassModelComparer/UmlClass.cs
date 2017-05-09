using System;
using System.Collections.Generic;

namespace ClassModelComparer
{
    public class UmlClass
    {
		// We definitely don't need all these properties for the Argo model
		// and probably don't need the for DotNet, but it doesn't hurt to have
		// them for now.

		public UmlClass()
        {
			Properties = new List<UmlClassProperty>();
        }

		public string Name
		{
			get;
			set;
		}

		public string BaseType
		{
			get;
			set;
		}

		public string FullName
		{
			get;
			set;
		}

		public string Namespace
		{
			get;
			set;
		}

		public bool IsEnum
		{
			get;
			set;
		}

        public List<UmlClassProperty> Properties
        {
            get;
            set;
        }
    }
}
