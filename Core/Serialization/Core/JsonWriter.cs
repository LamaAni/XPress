using XPress.Serialization.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Core
{
    /// <summary>
    /// Writes json objects into json.
    /// </summary>
    /// <typeparam name="C">The 'char' type. the makes up the collection. (For example, char)</typeparam>
    /// <typeparam name="T">The source array. (For example string)</typeparam>
    public abstract class JsonWriter<C, T>
        where T : IEnumerable<C>
    {
        /// <summary>
        /// Returns the element data associated with a value.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="getElementData"></param>
        public JsonWriter(LanguageDefinitions<C> language, JsonDefinitions<T> definitions)
        {
            Definitions = definitions;
            Language = language;
        }

        #region members

        /// <summary>
        /// The collection of definitions that determine the behaviur of the writer.
        /// </summary>
        public JsonDefinitions<T> Definitions { get; set; }

        /// <summary>
        /// The language defintions for the writer.
        /// </summary>
        public LanguageDefinitions<C> Language { get; private set; }

        #endregion

        #region Write methods

        /// <summary>
        /// Converts the json value to a char collection.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public StreamBuilder<C, T> ToStream(IJsonValue<T> val, bool convertRawValuesToData =false)
        {
            StreamBuilder<C, T> builder = CreateBuilder();
            DoRecursiveWrite(val, builder, convertRawValuesToData);
            return builder;
        }

        /// <summary>
        /// Returns the json value for the specific stream.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="isPetty">If true, the writer will attempt to write the value as a pretty json. (Dose not apply to all json language types).</param>
        /// <returns></returns>
        public virtual T ToJson(IJsonValue<T> val, bool isPretty = false, bool convertRawValuesToData = false)
        {
            StreamBuilder<C, T> strm = ToStream(val, convertRawValuesToData);
            return isPretty ? strm.ToPrettyValue() : strm.ToValue();
        }

        void DoRecursiveWrite(IJsonValue<T> val, StreamBuilder<C, T> builder, bool convertRawValuesToData = false)
        {
            if(val==null)
            {
                // adding the null value.
                builder.Append(Definitions.NullValue);
                return;
            }

            JsonDirective<T> directive = val as JsonDirective<T>;
            if (directive != null)
            {
                builder.Append(Language.DirectiveMarker);
                builder.Append(Definitions.DirectiveConverter.ToRaw(directive));
                return;
            }

            JsonData<T> dat = val as JsonData<T>;
            if (dat != null)
            {
                WriteJsonData(builder, convertRawValuesToData, dat);
                return;
            }
            JsonArray<T> ar = val as JsonArray<T>;
            if (ar != null)
            {
                DoRecursiveWriteArray(builder, convertRawValuesToData, ar);
                return;
            }
            JsonObject<T> ob = val as JsonObject<T>;
            if (ob != null)
            {
                DoRecursiveWriteObject(builder, convertRawValuesToData, ob);
                return;
            }
        }

        private void WriteJsonData(StreamBuilder<C, T> builder, bool convertRawValuesToData, JsonData<T> dat)
        {
            if (!convertRawValuesToData && !dat.HasValue)
            {
                if (dat.RawData != null)
                    builder.Append(dat.RawData.GetRawData());
                else builder.Append(default(C));
            }
            else builder.Append(Definitions.GetValue(dat.Value));
        }

        private void DoRecursiveWriteArray(StreamBuilder<C, T> builder, bool convertRawValuesToData, JsonArray<T> ar)
        {
            builder.Append(Language.BeginArray);
            bool isFirst = true;
            if (ar.HasDirectives)
                ar.Directives.ForEach(d =>
                {
                    if (!isFirst)
                        builder.Append(Language.DataValueSeperator);
                    isFirst = false;
                    DoRecursiveWrite(d, builder, convertRawValuesToData);
                });
            ar.ForEach(v =>
            {
                if (!isFirst)
                    builder.Append(Language.DataValueSeperator);
                isFirst = false;
                DoRecursiveWrite(v, builder, convertRawValuesToData);
            });
            builder.Append(Language.EndArray);
        }

        private void DoRecursiveWriteObject(StreamBuilder<C, T> builder, bool convertRawValuesToData, JsonObject<T> ob)
        {
            builder.Append(Language.BeginObject);
            bool isFirst = true;
            if (ob.HasDirectives)
                ob.Directives.ForEach(d =>
                {
                    if (!isFirst)
                        builder.Append(Language.DataValueSeperator);
                    isFirst = false;
                    DoRecursiveWrite(d, builder, convertRawValuesToData);
                    builder.Append(Language.DataPairSeperator);
                    builder.Append(Definitions.NullValue);
                });
            ob.ForEach(p =>
            {
                if (!isFirst)
                    builder.Append(Language.DataValueSeperator);
                isFirst = false;
                DoRecursiveWrite(p.Key, builder, convertRawValuesToData);
                builder.Append(Language.DataPairSeperator);
                DoRecursiveWrite(p.Value, builder, convertRawValuesToData);
            });
            builder.Append(Language.EndObject);
        }

        public abstract StreamBuilder<C,T> CreateBuilder();

        #endregion
    }


    /// <summary>
    /// A stream building mechanisem.
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class StreamBuilder<C, T>
    {
        public StreamBuilder(LanguageDefinitions<C> language)
        {
            Language = language;
        }

        public LanguageDefinitions<C> Language { get; private set; }

        /// <summary>
        /// Returns the value this builder has built.
        /// </summary>
        /// <returns></returns>
        public abstract T ToValue();

        /// <summary>
        /// Returns the prettified value of the stream object.
        /// </summary>
        /// <returns></returns>
        public abstract T ToPrettyValue();

        /// <summary>
        /// Appends a value to the collection
        /// </summary>
        /// <param name="val"></param>
        public abstract void Append(T val);

        /// <summary>
        /// Appends a value to the collection
        /// </summary>
        /// <param name="val"></param>
        public abstract void Append(C val);
    }

}
