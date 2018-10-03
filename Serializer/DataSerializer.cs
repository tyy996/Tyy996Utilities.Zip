using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Tyy996Utilities.Zip.Serializer
{
    public static class DataSerializer
    {
        private static BindingFlags bindingFlags { get { return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetField; } }

        public static PrimitiveData SerializeIntoData(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Object was null.");

            ReferenceSerializer references = new ReferenceSerializer();
            return serializeIntoData(obj, typeof(object), references);
        }

        public static PrimitiveData SerializeIntoData(object obj, Type peak)
        {
            if (obj == null)
                throw new ArgumentNullException("Object was null.");

            ReferenceSerializer references = new ReferenceSerializer();
            return serializeIntoData(obj, peak, references);
        }

        private static PrimitiveData serializeIntoData(object obj, Type peak, ReferenceSerializer references)
        {
            var type = obj.GetType();

            //if (obj == null)
            //    yield return new PrimitiveData();

            if (type.IsSerializable && PrimitiveData.IsAcceptable(obj))
                return new PrimitiveData(obj);

            if (obj is IEnumerable || obj is ICollection)
                return serializeArray(obj, references);

            return serializeObject(obj, peak, references);

            //Dictionary<string, PrimitiveData> objectDictionary = new Dictionary<string, PrimitiveData>();
            //foreach (var field in type.GetAllFields(peak, bindingFlags))
            //{
            //    var subObj = field.GetValue(obj);

            //    if (subObj != null && (subObj.GetType().IsValueType || subObj is string || subObj.GetType().HasParameterlessConstructor()))
            //    {
            //        var data = serializeIntoData(subObj, typeof(object));
            //        if (data.Type == PrimitiveDataType.Invaild)
            //            throw new InvalidOperationException(string.Format("Object ({0}) could not be serialized.", subObj.GetType().Name));

            //        objectDictionary.Add(field.Name, data);
            //    }
            //}

            //return new PrimitiveData(objectDictionary);
        }

        private static PrimitiveData serializeArray(object obj, ReferenceSerializer references)
        {
            var type = obj.GetType();

            var enumerable = obj as ICollection;

            List<PrimitiveData> array = new List<PrimitiveData>();
            foreach (var element in enumerable)
            {
                if (element != null && (element.GetType().IsValueType || element is string || element.GetType().HasParameterlessConstructor()))
                {
                    var data = serializeIntoData(element, typeof(object), references);
                    if (data.Type == PrimitiveDataType.Invaild)
                        throw new InvalidOperationException(string.Format("Object ({0}) could not be serialized.", element.GetType().Name));

                    array.Add(data);
                }
            }

            return new PrimitiveData(array);
        }

        private static PrimitiveData serializeObject(object obj, Type peak, ReferenceSerializer references)
        {
            var type = obj.GetType();

            if (type.HasAttribute<ReferenceAttribute>())
                return new PrimitiveData(null, type, references.AddReference(obj));

            Dictionary<string, PrimitiveData> objectDictionary = new Dictionary<string, PrimitiveData>();
            foreach (var field in type.GetAllFields(peak, bindingFlags))
            {
                var subObj = field.GetValue(obj);

                if (subObj != null && !field.HasAttribute<NonSerializedAttribute>() && (subObj.GetType().IsValueType || subObj is string || subObj.GetType().HasParameterlessConstructor()))
                {
                    var data = serializeIntoData(subObj, typeof(object), references);
                    if (data.Type == PrimitiveDataType.Invaild)
                        throw new InvalidOperationException(string.Format("Object ({0}) could not be serialized.", subObj.GetType().Name));

                    objectDictionary.Add(field.Name, data);
                }
            }

            if (type.HasAttribute<ReferenceableAttribute>())
                return new PrimitiveData(objectDictionary, type, references.AddReference(obj));

            return new PrimitiveData(objectDictionary, type);
        }

        public static T DeserializeFromData<T>(PrimitiveData data)
        {
            return DeserializeFromData<T>(data, typeof(object));
        }

        public static T DeserializeFromData<T>(PrimitiveData data, Type peak)
        {
            ReferenceDeserializer references = new ReferenceDeserializer();
            return (T)deserializeFromData(typeof(T), data, peak, references);
        }

        public static void DeserializeFromData<T>(ref T instance, PrimitiveData data)
        {
            DeserializeFromData(ref instance, data, typeof(object));
        }

        public static void DeserializeFromData<T>(ref T instance, PrimitiveData data, Type peak)
        {
            var obj = instance as object;
            ReferenceDeserializer references = new ReferenceDeserializer();
            deserializeFromData(ref obj, data, peak, references);
        }

        public static object DeserializeFromData(Type type, PrimitiveData data, Type peak)
        {
            ReferenceDeserializer references = new ReferenceDeserializer();
            return deserializeFromData(type, data, peak, references);
        }

        private static object deserializeFromData(Type type, PrimitiveData data, Type peak, ReferenceDeserializer references)
        {
            if (data.Type == PrimitiveDataType.Null || data.Type == PrimitiveDataType.Invaild)
                throw new InvalidOperationException("Data was null or invalid.");

            //var peakABoo = PrimitiveData.GetType()

            if (data.Type == PrimitiveDataType.Object)
            {
                return deserializeDataObject(type, data, peak, references);
            }
            else if (data.Type == PrimitiveDataType.Array)
            {
                return deserializeDataArray(type, data, references);
            }
            else
            {
                return data.ToObject(type);
            }
        }

        private static void deserializeFromData(ref object instance, PrimitiveData data, Type peak, ReferenceDeserializer references)
        {
            if (data.Type == PrimitiveDataType.Null || data.Type == PrimitiveDataType.Invaild)
                throw new InvalidOperationException("Data was null or invalid.");

            if (data.Type == PrimitiveDataType.Object)
            {
                deserializeDataObject(ref instance, data, peak, references);
            }
            else if (data.Type == PrimitiveDataType.Array)
            {
                var collection = instance as ICollection;
                deserializeDataArray(ref collection, data, references);
            }
            else
            {
                data.ToObject(instance.GetType());
            }
        }

        private static object deserializeDataObject(Type type, PrimitiveData data, Type peak, ReferenceDeserializer references)
        {
            var instance = Activator.CreateInstance(data.OriginalType);
            deserializeDataObject(ref instance, data, peak, references);

            return instance;
            //var objectDictionary = data.ToDataObject();

            //foreach (var field in type.GetAllFields(peak, bindingFlags))
            //{
            //    PrimitiveData subData;
            //    if (objectDictionary.TryGetValue(field.Name, out subData))
            //        field.SetValue(instance, deserializeFromData(field.FieldType, subData, typeof(object)));
            //}

            //return instance;
        }

        private static void deserializeDataObject(ref object instance, PrimitiveData data, Type peak, ReferenceDeserializer references)
        {
            var objectDictionary = data.ToDataObject();

            if (instance.GetType().HasAttribute<ReferenceableAttribute>())
                references.AddReferenceable(data.ReferenceID, instance);

            foreach (var field in instance.GetType().GetAllFields(peak, bindingFlags))
            {
                PrimitiveData subData;

                if (objectDictionary.TryGetValue(field.Name, out subData))
                {
                    if (field.HasAttribute<ReferenceAttribute>())
                        references.Push(instance, field, subData.ReferenceID);
                    else
                        field.SetValue(instance, deserializeFromData(field.FieldType, subData, typeof(object), references));
                }
            }
        }

        private static object deserializeDataArray(Type type, PrimitiveData data, ReferenceDeserializer references)
        {
            var instance = Activator.CreateInstance(type) as ICollection;
            deserializeDataArray(ref instance, data, references);

            return instance;
        }

        private static void deserializeDataArray(ref ICollection instance, PrimitiveData data, ReferenceDeserializer references)
        {
            var array = data.ToList();
            //var elementType = GetElementType(instance.GetType());
            var elementType = instance.GetType().GetGenericArguments()[0];

            //var collection = makeCollection(elementType);
            var collection = instance;
            //Result.GetType().GetMethod("Add").Invoke(Result, new[] { objTemp });

            //var list = typeof(ICollection<>).MakeGenericType(elementType);

            foreach (var element in array)
            {
                var obj = deserializeFromData(elementType, element, typeof(object), references);
                collection.GetType().GetMethod("Add").Invoke(collection, new[] { obj });
            }
        }

        private static object makeCollection(Type elementType)
        {
            var IListRef = typeof(List<>);
            Type[] IListParam = { elementType };
            return Activator.CreateInstance(IListRef.MakeGenericType(IListParam));
        }

        //private static ICollection<TItem> createCollection<TItem>()
        //{
        //    Type listType = GetGenericListType<TItem>();
        //    ICollection<TItem> list = (ICollection<TItem>)Activator.CreateInstance(listType);
        //    return list;
        //}

        //private static Type GetGenericListType(Type type)
        //{
        //    var defaultListType = typeof(List<>);
        //    Type[] itemTypes = { type };
        //    Type listType = defaultListType.MakeGenericType(itemTypes);
        //    return listType;
        //}

        /// <summary>
        /// Fetches the element type for objects inside of the collection.
        /// </summary>
        private static Type GetElementType(Type objectType)
        {
            if (objectType.HasElementType)
                return objectType.GetElementType();

            throw new Exception("Element Error");

            //Type enumerableList = (objectType as IEnumerable)
            //if (enumerableList != null) return enumerableList.GetGenericArguments()[0];

            //return typeof(object);
        }

        //private static MethodInfo GetAddMethod(Type type)
        //{
        //    // There is a really good chance the type will extend ICollection{}, which will contain
        //    // the add method we want. Just doing type.GetFlattenedMethod() may return the incorrect one --
        //    // for example, with dictionaries, it'll return Add(TKey, TValue), and we want
        //    // Add(KeyValuePair<TKey, TValue>).
        //    Type collectionInterface = fsReflectionUtility.GetInterface(type, typeof(ICollection<>));
        //    if (collectionInterface != null)
        //    {
        //        MethodInfo add = collectionInterface.GetDeclaredMethod("Add");
        //        if (add != null) return add;
        //    }

        //    // Otherwise try and look up a general Add method.
        //    return
        //        type.GetFlattenedMethod("Add") ??
        //        type.GetFlattenedMethod("Push") ??
        //        type.GetFlattenedMethod("Enqueue");
        //}
    }
}
