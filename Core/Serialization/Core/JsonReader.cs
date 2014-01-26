using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    /// <summary>
    /// Implements a json stream reader that reads json from a value stream. 
    /// </summary>
    /// <typeparam name="C">The stream char type</typeparam>
    public abstract class JsonReader<C, T>
        where T : IEnumerable<C>
    {
        /// <summary>
        /// creates a stream reader that applies the
        /// </summary>
        /// <param name="beginObject"></param>
        /// <param name="beginArray"></param>
        /// <param name="pairSeperator"></param>
        public JsonReader(LanguageDefinitions<C> language, JsonDefinitions<T> def)
        {
            Language = language;
            Definitions = def;

            Dictionary<C, JsonStreamAction> actions = new Dictionary<C, JsonStreamAction>();
            actions[Language.BeginArray] = JsonStreamAction.BeginArray;
            actions[Language.BeginObject] = JsonStreamAction.BeginObject;
            actions[Language.EndObject] = JsonStreamAction.EndObject;
            actions[Language.EndArray] = JsonStreamAction.EndArray;
            actions[Language.DataMarker] = JsonStreamAction.DataMarker;
            actions[Language.DataPairSeperator] = JsonStreamAction.DataPairSeperator;
            actions[Language.DataValueSeperator] = JsonStreamAction.DataValueSeperator;
            actions[Language.EscapeNextSymble] = JsonStreamAction.EscapeNextSymbol;
            actions[Language.DirectiveMarker] = JsonStreamAction.Directive;
            Actions=actions;

            TrimChars = new HashSet<C>(Language.TrimChars);
        }

        /// <summary>
        /// A collection of chars to trim from before and after the value
        /// </summary>
        public HashSet<C> TrimChars { get; private set; }

        /// <summary>
        /// A collection of actions to be used when reading the json.
        /// </summary>
        public Dictionary<C, JsonStreamAction> Actions { get; private set; }

        /// <summary>
        /// The collection of definitions on how to handle data.
        /// </summary>
        public JsonDefinitions<T> Definitions { get; set; }

        /// <summary>
        /// The definition of the language.
        /// </summary>
        public LanguageDefinitions<C> Language { get; private set; }


        /// <summary>
        /// Reads the value into the stream.
        /// </summary>
        /// <returns>The json value</returns>
        public Documents.IJsonValue<T> FromJson(T source)
        {
            return ReadStream(GetRecursiveElementStream(source));
        }

        enum ReadMode { Object, Array, Data };

        void GetCurData(object val, out ElementData<T> data, out JsonStreamAction action)
        {
            data = val as ElementData<T>;
            if (data == null)
                action = (JsonStreamAction)val;
            else action = JsonStreamAction.Data;
        }

        /// <summary>
        /// Reads the value into the stream.
        /// </summary>
        /// <returns>The json value</returns>
        public Documents.IJsonValue<T> ReadStream(IEnumerable<object> stream)
        {
            if (stream.IsEmpty())
                return null;

            JsonArray<T> root = new JsonArray<T>();
            DoRecursiveArrayConvert(stream.GetEnumerator(), root);
            return root.Count == 0 ? null : (root.Count == 1 ? root.First() : root); // the root parent.
        }

        void DoRecursiveArrayConvert(IEnumerator<object> streamEnumr, JsonArray<T> ar)
        {
            while (streamEnumr.MoveNext())
            {
                ElementData<T> dat = streamEnumr.Current as ElementData<T>;
                if (dat != null)
                {
                    ar.AddRawValue(new JsonData<T>(dat, Definitions), dat.IsDirective);
                }
                else
                {
                    switch ((JsonStreamAction)streamEnumr.Current)
                    {
                        case JsonStreamAction.BeginObject:
                            JsonObject<T> co = new JsonObject<T>();
                            DoRecursiveObjectConvert(streamEnumr, co);
                            ar.AddRawValue(co, false);
                            break;
                        case JsonStreamAction.BeginArray:
                            JsonArray<T> car = new JsonArray<T>();
                            DoRecursiveArrayConvert(streamEnumr, car);
                            ar.AddRawValue(car, false);
                            break;
                        case JsonStreamAction.EndArray:
                            return; // no need to continue reading this array.
                        case JsonStreamAction.EndObject:
                            throw new Exception("Found end object while reading array without begin object.");
                            break;
                    }
                }
            }
        }

        void DoRecursiveObjectConvert(IEnumerator<object> streamEnumr, JsonObject<T> o)
        {
            while (streamEnumr.MoveNext())
            {
                IJsonValue<T> key = null;
                bool isDirective = false;
                ElementData<T> dat = streamEnumr.Current as ElementData<T>;
                if (dat == null)
                {
                    if ((JsonStreamAction)streamEnumr.Current == JsonStreamAction.EndObject)
                        return;
                    switch ((JsonStreamAction)streamEnumr.Current)
                    {
                        case JsonStreamAction.BeginObject:
                            JsonObject<T> co = new JsonObject<T>();
                            DoRecursiveObjectConvert(streamEnumr, co);
                            isDirective = false;
                            key = co;
                            break;
                        case JsonStreamAction.BeginArray:
                            JsonArray<T> car = new JsonArray<T>();
                            DoRecursiveArrayConvert(streamEnumr, car);
                            isDirective = false;
                            key = car;
                            break;
                        case JsonStreamAction.EndObject:
                            return;
                            break;
                        case JsonStreamAction.EndArray:
                            throw new Exception("Found end array inside an object without begin array command.");
                            break;
                    }
                }
                else
                {
                    key = new JsonData<T>(dat, Definitions);
                    isDirective = dat.IsDirective;
                }

                if (!streamEnumr.MoveNext())
                    throw new Exception("Cannot find value for object, while value is expected.");

                dat= streamEnumr.Current as ElementData<T>;
                if (dat != null)
                {
                    o.AddRawValue(key, new JsonData<T>(dat, Definitions), isDirective);
                }
                else switch ((JsonStreamAction)streamEnumr.Current)
                    {
                        case JsonStreamAction.BeginObject:
                            JsonObject<T> co = new JsonObject<T>();
                            DoRecursiveObjectConvert(streamEnumr, co);
                            o.AddRawValue(key, co, isDirective);
                            break;
                        case JsonStreamAction.BeginArray:
                            JsonArray<T> car = new JsonArray<T>();
                            DoRecursiveArrayConvert(streamEnumr, car);
                            o.AddRawValue(key, car, isDirective);
                            break;
                        case JsonStreamAction.EndObject:
                            throw new Exception("No object value was found for key when trying to end object.");
                            break;
                        case JsonStreamAction.EndArray:
                            throw new Exception("Found end array inside an object without begin array command.");
                            break;
                    }
            }
        }

        /* OLD Reading.
        /// <summary>
        /// Returns the element stream for the source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [Obsolete]
        public IEnumerable<object> __GetElementStream(T source)
        {
            int charLength = source.Count();
            Stack<bool> readingObject = new Stack<bool>();
            readingObject.Push(false);

            List<object> stream = new List<object>();
            Dictionary<C, JsonStreamAction> actions = new Dictionary<C, JsonStreamAction>();
            actions[Language.BeginArray] = JsonStreamAction.BeginArray;
            actions[Language.BeginObject] = JsonStreamAction.BeginObject;
            actions[Language.EndObject] = JsonStreamAction.EndObject;
            actions[Language.EndArray] = JsonStreamAction.EndArray;
            actions[Language.DataMarker] = JsonStreamAction.DataMarker;
            actions[Language.DataPairSeperator] = JsonStreamAction.DataPairSeperator;
            actions[Language.DataValueSeperator] = JsonStreamAction.DataValueSeperator;
            actions[Language.EscapeNextSymble] = JsonStreamAction.EscapeNextSymbol;
            int index = 0;
            int lastIndex = -1;
            bool isInDataRead = false;
            bool prevWasEscpae = false;
            bool resetLastIndex = false;
            bool addValue = false;
            bool endCurrent = false;
            bool isWaitingForPairMarker = false;
            bool isInValueReadMode = true;

            while (charLength > index)
            {
                C c = IndexSource(source, index);
                JsonStreamAction action = JsonStreamAction.Data;
                action = actions.TryGetValue(c, out action) ? action : JsonStreamAction.Data;

                if (!isInDataRead)
                {
                    if (!isWaitingForPairMarker)
                    {
                        switch (action)
                        {
                            case JsonStreamAction.BeginArray:
                                readingObject.Push(false);
                                stream.Add(JsonStreamAction.BeginArray);
                                resetLastIndex = true;
                                isInValueReadMode = true;
                                break;
                            case JsonStreamAction.BeginObject:
                                isInValueReadMode = true;
                                readingObject.Push(true);
                                isWaitingForPairMarker = true;
                                stream.Add(JsonStreamAction.BeginObject);
                                resetLastIndex = true;
                                break;
                            case JsonStreamAction.EndArray:
                                // end of last marker.
                                addValue = isInValueReadMode;
                                isInValueReadMode = false;
                                resetLastIndex = true;
                                endCurrent = true;
                                break;
                            case JsonStreamAction.EndObject:
                                addValue = isInValueReadMode;
                                isInValueReadMode = false;
                                resetLastIndex = true;
                                endCurrent = true;
                                break;
                            case JsonStreamAction.DataValueSeperator:
                                resetLastIndex = true;
                                addValue = isInValueReadMode;
                                isInValueReadMode = true;
                                isWaitingForPairMarker = readingObject.Peek();
                                break;
                        }
                    }
                    else if (action == JsonStreamAction.DataPairSeperator)
                    {
                        addValue = isInValueReadMode;
                        isInValueReadMode = true;
                        resetLastIndex = true;
                        isWaitingForPairMarker = false;
                    }

                    if (addValue)
                    {
                        ElementData<T> data = CreateElementData(source, lastIndex + 1, index);
                        stream.Add(data);
                        addValue = false;
                    }

                    if (endCurrent)
                    {
                        stream.Add(readingObject.Peek() ? JsonStreamAction.EndObject : JsonStreamAction.EndArray);
                        readingObject.Pop();
                        endCurrent = false;
                    }

                    if (resetLastIndex)
                    {
                        resetLastIndex = false;
                        lastIndex = index;
                    }
                }

                if (isInValueReadMode && action== JsonStreamAction.DataMarker)
                {
                    if(!prevWasEscpae && isInDataRead)
                    {
                        isInDataRead = false;
                    }
                    else
                    {
                        isInDataRead = true;
                    }
                }

                // checking for escaped prev.
                if (isInDataRead && !prevWasEscpae && action == JsonStreamAction.EscapeNextSymbol)
                {
                    prevWasEscpae = true;
                }
                else prevWasEscpae = false;

                index += 1;
            }

            if (lastIndex + 1 < index)
            {
                // need to add one final value.
                stream.Add(CreateElementData(source, lastIndex + 1, index));
            }

            return stream;
        }

        */

        public IEnumerable<object> GetRecursiveElementStream(T source)
        {
            List<object> stream = new List<object>();

            InternalJsonReaderStream<C, T> strm = new InternalJsonReaderStream<C, T>(this);
            strm.ReadSource(source);

            return strm;
        }

        /// <summary>
        /// Get the char at the index.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal abstract C IndexSource(T source, int index);

        /// <summary>
        /// Get the source count.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal abstract int GetSourceCount(T source);
    }

    [Flags]
    public enum JsonStreamAction
    {
        BeginObject = 1,
        EndObject = 2,
        BeginArray = 4,
        EndArray = 8,
        DataMarker = 16,
        DataPairSeperator = 32,
        DataValueSeperator = 64,
        EscapeNextSymbol = 128,
        Data = 256,
        Directive = 512,
        JsonStopReadValue = BeginObject | EndObject | BeginArray | EndArray | DataValueSeperator,
        JsonStopReadKey = DataPairSeperator | JsonStopReadValue,
    }

    /// <summary>
    /// Holdes information about a single specific read (of a specific stream).
    /// The reader allows for specific reading of values into the stream (recursive).
    /// </summary>
    /// <typeparam name="C">The char type</typeparam>
    /// <typeparam name="T">The source type</typeparam>
    class InternalJsonReaderStream <C,T> : List<object>
        where T:IEnumerable<C>
    {
        public InternalJsonReaderStream(JsonReader<C, T> reader)
        {
            Reader = reader;
        }

        public JsonDefinitions<T> Definitions { get { return Reader.Definitions; } }

        public JsonReader<C,T> Reader { get; private set; }

        protected T Source { get; private set; }

        protected CharIndexedEnumerator<C> Items { get; private set; }

        public Dictionary<C, JsonStreamAction> Actions { get { return Reader.Actions; } }

        public HashSet<C> TrimChars { get { return Reader.TrimChars; } }

        /// <summary>
        /// Reads the source into the stream.
        /// </summary>
        /// <param name="source"></param>
        public void ReadSource(T source)
        {
            Source = source;
            Items = new CharIndexedEnumerator<C>(Source.GetEnumerator(), TrimChars, (idx) => Reader.IndexSource(source, idx));
            DoRecursiveArrayRawRead();
        }

        /// <summary>
        /// Reads array from the source
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        protected void DoRecursiveArrayRawRead()
        {
            bool objectEnded = false;
            while (!Items.Ended)
            {
                bool directive = false;
                bool lastWasObjectEnd = objectEnded;
                objectEnded = false;
                Items.MarkStartIndex(true);
                JsonStreamAction stopAction = this.ReadUntilReached(JsonStreamAction.JsonStopReadValue, out directive, true, JsonStreamAction.DataValueSeperator);
                switch (stopAction)
                {
                    case JsonStreamAction.Data:
                    case JsonStreamAction.DataValueSeperator:
                        // this is actual data and now we need to load the next data value.
                        if (Items.CharCount > 0 || !lastWasObjectEnd)
                            this.Add(new ElementData<T>(Source, Items.MarkedStartIndex, Items.MarkedEndIndex, Definitions, directive));
                    break;
                    case JsonStreamAction.BeginObject:
                        // no need to add the data, since this is now an object until the next data read.
                        this.Add(JsonStreamAction.BeginObject);
                        DoRecursiveObjectRawRead();
                        this.Add(JsonStreamAction.EndObject);
                        objectEnded = true;
                        break;
                    case JsonStreamAction.BeginArray:
                        // no need to add the data, since this is now an object until the next data read.
                        this.Add(JsonStreamAction.BeginArray);
                        DoRecursiveArrayRawRead();
                        this.Add(JsonStreamAction.EndArray);
                        objectEnded = true;
                        break;
                    case JsonStreamAction.EndObject:
                        throw new Exception("Found end object when expecting end array. At index " + Items.Index);
                        break;
                    case JsonStreamAction.EndArray:
                        // ends the current array.
                        if (Items.CharCount > 0)
                            this.Add(new ElementData<T>(Source, Items.MarkedStartIndex, Items.MarkedEndIndex, Definitions, directive));
                        return; // the array has ended.
                        break;
                }
            }
        }


        /// <summary>
        /// Reads object from the source.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        protected void DoRecursiveObjectRawRead()
        {
            bool objectEnded = false;
            while (!Items.Ended)
            {
                bool directive = false;
                // reading the key.
                Items.MarkStartIndex(true);
                JsonStreamAction stopAction = ReadUntilReached(JsonStreamAction.JsonStopReadKey, out directive, true,
                     JsonStreamAction.DataPairSeperator | JsonStreamAction.DataValueSeperator);

                switch (stopAction)
                {
                    case JsonStreamAction.DataValueSeperator:
                        if (Items.CharCount==0)
                            continue;
                        else throw new Exception("Cannot have data value sperator inside a key. At " + Items.Index);
                        break;
                    case JsonStreamAction.EndObject:
                        // the key is ingored here since it cannot be here. only a value can end an object. Therefore we may assume that something was padded.
                        return;
                        break;
                    case JsonStreamAction.DataPairSeperator:
                        this.Add(new ElementData<T>(Source, Items.MarkedStartIndex, Items.MarkedEndIndex, Definitions, directive));
                        break;
                    case JsonStreamAction.BeginObject:
                        // no need to add the data, since this is now an object until the next data read.
                        this.Add(JsonStreamAction.BeginObject);
                        DoRecursiveObjectRawRead();
                        this.Add(JsonStreamAction.EndObject);
                        break;
                    case JsonStreamAction.BeginArray:
                        // no need to add the data, since this is now an array until the next data read.
                        this.Add(JsonStreamAction.BeginArray);
                        DoRecursiveArrayRawRead();
                        this.Add(JsonStreamAction.EndArray);
                        break;
                    default:
                        if (Items.Ended)
                        {
                            if (Items.CharCount>0)
                                throw new Exception("Found key but value was not found.");
                            return; // nothing to do.
                        }
                        else throw new Exception("Invalid char inside a key. Only non array/object/datavalue chars are allowed. At " + Items.Index);
                        break;
                }

                // reading the value.
                Items.MarkStartIndex(true);
                stopAction = ReadUntilReached(JsonStreamAction.JsonStopReadValue, out directive, false);
                switch (stopAction)
                {
                    case JsonStreamAction.Data:
                    case JsonStreamAction.DataValueSeperator:
                        // this is actual data and now we need to load the next data value.
                        this.Add(new ElementData<T>(Source, Items.MarkedStartIndex, Items.MarkedEndIndex, Definitions, directive));
                        break;
                    case JsonStreamAction.BeginObject:
                        // no need to add the data, since this is now an object until the next data read.
                        this.Add(JsonStreamAction.BeginObject);
                        DoRecursiveObjectRawRead();
                        this.Add(JsonStreamAction.EndObject);
                        break;
                    case JsonStreamAction.BeginArray:
                        // no need to add the data, since this is now an object until the next data read.
                        this.Add(JsonStreamAction.BeginArray);
                        DoRecursiveArrayRawRead();
                        this.Add(JsonStreamAction.EndArray);
                        break;
                    case JsonStreamAction.EndObject:
                        // ends the current array.
                        this.Add(new ElementData<T>(Source, Items.MarkedStartIndex, Items.MarkedEndIndex, Definitions, directive));
                        return; // the array has ended.
                        break;
                    case JsonStreamAction.EndArray:
                        throw new Exception("Found end array when expecting end object. At index " + Items.Index);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a data value into the builder until the value has ended.
        /// </summary>
        protected JsonStreamAction ReadUntilReached(JsonStreamAction until, out bool directive, bool allowDirectives = true, 
            JsonStreamAction readDirectiveUntil =  JsonStreamAction.DataValueSeperator)
        {
            directive = false;
            bool inDataReadMode = false;
            bool lastWasEscape = false;
            while (Items.MoveNext())
            {
                C c = Items.Current;
                JsonStreamAction action;
                bool isAction = Actions.TryGetValue(c, out action);

                if (!lastWasEscape)
                {
                    if (isAction)
                    {
                        bool dataMarker = action == JsonStreamAction.DataMarker;
                        if (inDataReadMode)
                        {
                            if (dataMarker)
                                inDataReadMode = false;
                            else if (action == JsonStreamAction.EscapeNextSymbol)
                                lastWasEscape = true;
                        }
                        else
                        {
                            if (allowDirectives && action == JsonStreamAction.Directive)
                            {
                                // reset the current location and mark this as a directive.
                                Items.MarkStartIndex(true);
                                bool outdummy = false;
                                directive = true;
                                return ReadUntilReached(until, out outdummy, false, readDirectiveUntil);
                            }
                            else if (dataMarker)
                                inDataReadMode = true;
                            if (until.HasFlag(action))
                            {
                                Items.MarkEndIndex();
                                Items.TrimLeadingAndEndingChars();
                                return action; // ended.
                            }
                        }
                    }
                }
                else lastWasEscape = false;
            }

            Items.MarkEndIndex();
            Items.TrimLeadingAndEndingChars();
            // means that the responce is data.
            return JsonStreamAction.Data;
        }
    }

    class CharIndexedEnumerator<C> : IEnumerator<C>
    {
        /// <summary>
        /// Resets the enumerator!!
        /// </summary>
        /// <param name="baseEnum"></param>
        public CharIndexedEnumerator(IEnumerator<C> baseEnum, HashSet<C> trimChars, Func<int,C> getCharFromSource)
        {
            BaseEnumerator = baseEnum;
            TrimChars = trimChars;
            GetCharFromSource = getCharFromSource;
            Reset();
        }

        public Func<int, C> GetCharFromSource { get; private set; }

        /// <summary>
        /// A collection of chars to trim from before and after the value.
        /// </summary>
        public HashSet<C> TrimChars { get; private set; }

        public IEnumerator<C> BaseEnumerator { get; private set; }

        /// <summary>
        /// The marked value of the start index.
        /// </summary>
        public int MarkedStartIndex { get; private set; }

        /// <summary>
        /// The marked value of the end index.
        /// </summary>
        public int MarkedEndIndex { get; private set; }

        /// <summary>
        /// The current index.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The current char count for the current index and marked location.
        /// </summary>
        public int CharCount { get { return MarkedEndIndex - MarkedStartIndex; } }

        /// <summary>
        /// True when the move next has reaced its end.
        /// </summary>
        public bool Ended { get; private set; }

        /// <summary>
        /// Marks the current location as the parameter MarkedLocation
        /// </summary>
        public void MarkStartIndex(bool markedBeforeReadStarted)
        {
            MarkedStartIndex = markedBeforeReadStarted ? Index + 1 : Index;
        }

        /// <summary>
        /// Marks the end index of the current value.
        /// </summary>
        public void MarkEndIndex()
        {
            MarkedEndIndex =Index;
        }

        /// <summary>
        /// Trimms the end and start index to fit the char values.
        /// </summary>
        public void TrimLeadingAndEndingChars()
        {
            if (TrimChars.Count == 0)
                return;

            while (MarkedStartIndex < MarkedEndIndex && CharCount > 0 && TrimChars.Contains(GetCharFromSource(MarkedStartIndex)))
            {
                MarkedStartIndex += 1;
            }

            while (MarkedStartIndex < MarkedEndIndex && CharCount > 0 && TrimChars.Contains(GetCharFromSource(MarkedEndIndex-1)))
            {
                MarkedEndIndex -= 1;
            }
        }

        #region IEnumerator<C> Members

        public C Current
        {
            get { return BaseEnumerator.Current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            BaseEnumerator.Dispose();
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if(BaseEnumerator.MoveNext())
            {
                Index += 1;
                return true;
            }
            if (!Ended)
            {
                Index += 1;
                Ended = true;
            }
            return false;
        }

        public void Reset()
        {
            BaseEnumerator.Reset();
            Index = -1;
            MarkedStartIndex = int.MinValue;
        }

        #endregion
    }
}
