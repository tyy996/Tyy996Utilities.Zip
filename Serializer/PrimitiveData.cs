using System;
using System.Collections.Generic;

namespace Tyy996Utilities.Zip.Serializer
{
    [Serializable]
    public struct PrimitiveData
    {
        private object value;
        private Guid referenceID;
        private Type originalType;

        public PrimitiveDataType Type { get { return GetType(value); } }
        public Guid ReferenceID { get { return referenceID; } }
        internal Type OriginalType { get { return originalType; } }

        public PrimitiveData(object value) :
            this(value, value.GetType())
        {
        }

        public PrimitiveData(object value, Type originalType) :
            this(value, originalType, Guid.Empty)
        {
        }

        public PrimitiveData(object value, Type originalType, Guid referenceID)
        {
            this.value = value;
            this.originalType = originalType;
            this.referenceID = referenceID;
        }

        public T ToGeneric<T>()
        {
            if (IsTypeMatch(typeof(T)))
                return (T)value;
            else
                return default(T);
        }

        public Dictionary<string, PrimitiveData> ToDataObject()
        {
            return (Dictionary<string, PrimitiveData>)value;
        }

        public List<PrimitiveData> ToList()
        {
            return (List<PrimitiveData>)value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public object ToObject(object obj)
        {
            return ToObject(obj.GetType());
        }

        public object ToObject(Type type)
        {
            if (IsTypeMatch(type))
                return value;
            else
                return null;
        }

        public bool IsTypeMatch(object obj)
        {
            return IsTypeMatch(obj.GetType());
        }

        public bool IsTypeMatch(Type type)
        {
            if (value == null)
                return false;

            return value.GetType() == type;
        }

        //public static PrimitiveData Make(object obj)
        //{
        //    PrimitiveData record;
        //    TryMake(obj, out record);
        //    return record;
        //}

        //public static bool TryMake(object obj, out PrimitiveData record)
        //{
        //    if (obj == null)
        //    {
        //        record = new PrimitiveData();
        //        return true;
        //    }
        //    else if (IsAcceptable(obj))
        //    {
        //        record = new PrimitiveData(obj);
        //        return true;
        //    }
        //    else
        //    {
        //        record = new PrimitiveData();
        //        return false;
        //    }
        //}

        public static bool IsAcceptable(object obj)
        {
            return GetType(obj) != PrimitiveDataType.Invaild;
        }

        public static PrimitiveDataType GetType(object obj)
        {
            if (obj == null)
                return PrimitiveDataType.Null;
            if (obj is double || obj is float || obj is decimal)
                return PrimitiveDataType.Double;
            if (obj is int || obj is uint || obj is long || obj is ulong || obj is short || obj is ushort || obj is byte || obj is sbyte)
                return PrimitiveDataType.Int;
            if (obj is bool)
                return PrimitiveDataType.Boolean;
            if (obj is string || obj is char)
                return PrimitiveDataType.String;
            if (obj is List<PrimitiveData>)
                return PrimitiveDataType.Array;
            if (obj is Dictionary<string, PrimitiveData>)
                return PrimitiveDataType.Object;
            else
                return PrimitiveDataType.Invaild;
        }
    }

    public enum PrimitiveDataType
    {
        Null,
        Array,
        Double,
        Int,
        Boolean,
        String,
        Invaild,
        Object
    }
}
