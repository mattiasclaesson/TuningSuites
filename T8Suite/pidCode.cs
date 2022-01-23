using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CommonSuite;

namespace T8SuitePro
{
    public class PidHelper
    {
        private int _address = 0;
        public int FileAddress
        {
            get { return _address; }
            set { _address = value; }
        }

        private int _index = 0;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private string _pid = string.Empty;
        public string PID
        {
            get { return _pid; }
            set { _pid = value; }
        }

        private byte _packed = 0;
        public byte PackedFlags
        {
            get { return _packed; }
            set { _packed = value; }
        }

        public int ReadFlag
        {
            get { return (_packed >> 6) & 3; }
            set { _packed = (byte)((_packed & 0x3f) | ((value & 3) << 6)); }
        }

        public int WriteFlag
        {
            get { return (_packed >> 4) & 3; }
            set { _packed = (byte)((_packed & 0xcf) | ((value & 3) << 4)); }
        }

        public int ControlFlag
        {
            get { return (_packed >> 2) & 3; }
            set { _packed = (byte)((_packed & 0xf3) | ((value & 3) << 2)); }
        }

        private int _symindex = 0;
        public int SymbolIndex
        {
            get { return _symindex; }
            set { _symindex = value; }
        }

        private int _symsize = 0;
        public int SymbolSize
        {
            get { return _symsize; }
            set { _symsize = value; }
        }

        private int _symaddress = 0;
        public int SymbolAddress
        {
            get { return _symaddress; }
            set { _symaddress = value; }
        }

        private int _symtype = 0;
        public int SymbolType
        {
            get { return _symtype; }
            set { _symtype = value; }
        }

        private string _description = string.Empty;
        public string SymbolDescription
        {
            get { return _description; }
            set { _description = value; }
        }

        private bool _protect = false;
        public bool IsProtected
        {
            get { return _protect; }
            set { _protect = value; }
        }

        public PidHelper CopyOf()
        {
            PidHelper ph = new PidHelper();
            ph.FileAddress = _address;
            ph.Index = _index;
            ph.PID = _pid;
            ph.PackedFlags = _packed;
            ph.SymbolIndex = _symindex;
            ph.SymbolSize = _symsize;
            ph.SymbolAddress = _symaddress;
            ph.SymbolType = _symtype;
            ph.SymbolDescription = _description;
            ph.IsProtected = _protect;
            return ph;
        }
    }

    public class PidCollection : SortableCollectionBase, ICustomTypeDescriptor
    {
        #region CollectionBase implementation

        public PidCollection()
        {
            base.SortObjectType = typeof(PidHelper);
        }

        public PidCollection CopyOf()
        {
            PidCollection nc = new PidCollection();
            foreach (PidHelper ph in this)
            {
                nc.Add(ph.CopyOf());
            }
            return nc;
        }

        public PidHelper this[int index]
        {
            get
            {
                return ((PidHelper)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(PidHelper value)
        {
            return (List.Add(value));
        }

        public int IndexOf(PidHelper value)
        {
            return (List.IndexOf(value));
        }

        public bool Contains(PidHelper value)
        {
            // If value is not of type Int16, this will return false.
            return (List.Contains(value));
        }

        /////////////////////////////////
        // Table size is hardcoded so this list should never, UNDER ANY CIRCUMSTANCE, change count!
        public void Insert(int index, PidHelper value) { }
        public void Remove(PidHelper value) { }
        /////////////////////////////////
        // Are these even needed?
        protected override void OnInsert(int index, Object value) {}
        protected override void OnRemove(int index, Object value) {}
        protected override void OnSet(int index, Object oldValue, Object newValue) {}
        protected override void OnValidate(Object value) {}
        #endregion
        
        [TypeConverter(typeof(SIDICollectionConverter))]
        public PidCollection pids
        {
            get { return this; }
        }
        internal class SymbolConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
            {
                /*
                if (destType == typeof(string) && value is PidHelper)
                {
                    // Cast the value to an Employee type
                    PidHelper pp = (PidHelper)value;

                    // chriva
                    // return pp.Symbol + ", " + pp.AddressSRAM + ", " + pp.Value;
                    // return "Fixme";
                    return pp.PID.ToString("X4");
                }*/
                return base.ConvertTo(context, culture, value, destType);
            }
        }

        // This is a special type converter which will be associated with the EmployeeCollection class.
        // It converts an EmployeeCollection object to a string representation for use in a property grid.
        internal class SIDICollectionConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
            {
                if (destType == typeof(string) && value is PidCollection)
                {
                    // Return department and department role separated by comma.
                    return "Symbols";
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }

        #region ICustomTypeDescriptor impl

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++)
            {
                // Create a property descriptor for the employee item and add to the property descriptor collection
                PidCollectionPropertyDescriptor pd = new PidCollectionPropertyDescriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection
            return pds;
        }
        #endregion

        public class PidCollectionPropertyDescriptor : PropertyDescriptor
        {
            private PidCollection collection = null;
            private int index = -1;

            public PidCollectionPropertyDescriptor(PidCollection coll, int idx)
                : base("#" + idx.ToString(), null)
            {
                this.collection = coll;
                this.index = idx;
            }

            public override AttributeCollection Attributes
            {
                get
                {
                    return new AttributeCollection(null);
                }
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get
                {
                    return this.collection.GetType();
                }
            }

            public override object GetValue(object component)
            {
                return this.collection[index];
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return this.collection[index].GetType(); }
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            public override void ResetValue(object component) {}
            public override void SetValue(object component, object value) {}

        }
    }

    class pidCode
    {
    }
}
