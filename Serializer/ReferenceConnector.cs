using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tyy996Utilities.Zip.Serializer
{
    public sealed class ReferenceSerializer
    {
        private Dictionary<object, Guid> references;

        public ReferenceSerializer()
        {
            references = new Dictionary<object, Guid>();
        }

        public Guid AddReference(object obj)
        {
            Guid referenceID;
            if (references.TryGetValue(obj, out referenceID))
                return referenceID;

            var id = new Guid();
            references.Add(obj, id);
            return id;
        }

        //public Guid GetReferenceID(object obj)
        //{

        //}
    }

    public sealed class ReferenceDeserializer
    {
        private Dictionary<Guid, object> references;
        private Queue<ReferenceField> fieldQueue;

        public ReferenceDeserializer()
        {
            references = new Dictionary<Guid, object>();
            fieldQueue = new Queue<ReferenceField>();
        }

        public void AddReferenceable(Guid id, object reference)
        {
            references.Add(id, reference);
        }

        public void Push(object obj, FieldInfo field, Guid referenceID)
        {
            fieldQueue.Enqueue(new ReferenceField(obj, field, referenceID));
        }

        public void Finish()
        {
            while (fieldQueue.Count > 0)
            {
                var referenceField = fieldQueue.Dequeue();

                object reference;
                if (references.TryGetValue(referenceField.ReferenceID, out reference))
                    referenceField.SetValue(reference);
            }
        }

        private struct ReferenceField
        {
            public object Object { get; private set; }
            public FieldInfo Info { get; private set; }
            public Guid ReferenceID { get; private set; }

            public ReferenceField(object obj, FieldInfo info, Guid referenceID)
            {
                Object = obj;
                Info = info;
                ReferenceID = referenceID;
            }

            public void SetValue(object value)
            {
                Info.SetValue(Object, value);
            }
        }
    }

    //public class ReferenceConnector
    //{
    //    private Dictionary<Guid, IReferenceable> references;
    //    private Queue<ReferenceField> fieldQueue;

    //    public ReferenceConnector()
    //    {
    //        references = new Dictionary<Guid, IReferenceable>();
    //        fieldQueue = new Queue<ReferenceField>();
    //    }

    //    public void AddReferenceable(Guid id, IReferenceable reference)
    //    {
    //        references.Add(id, reference);
    //    }

    //    public void Push(object obj, FieldInfo field, Guid referenceID)
    //    {
    //        fieldQueue.Enqueue(new ReferenceField(obj, field, referenceID));
    //    }

    //    public void ConnectAll()
    //    {
    //        while (fieldQueue.Count > 0)
    //        {
    //            var referenceField = fieldQueue.Dequeue();

    //            IReferenceable reference;
    //            if (references.TryGetValue(referenceField.ReferenceID, out reference))
    //                referenceField.SetValue(reference as object);
    //        }
    //    }

    //    private struct ReferenceField
    //    {
    //        public object Object { get; private set; }
    //        public FieldInfo Info { get; private set; }
    //        public Guid ReferenceID { get; private set; }

    //        public ReferenceField(object obj, FieldInfo info, Guid referenceID)
    //        {
    //            Object = obj;
    //            Info = info;
    //            ReferenceID = referenceID;
    //        }

    //        public void SetValue(object value)
    //        {
    //            Info.SetValue(Object, value);
    //        }
    //    }
    //}
}
