using XPress.Serialization.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Strings;
using XPress.Serialization;

namespace XPress.Serialization.Javascript
{
    public class JsonStringArrayDataProvider : JsonMemoryArrayDataProvider<string>
    {
        public JsonStringArrayDataProvider(string source=null, IJsonSerializer<string> serializer = null)
            : base(serializer == null ? Javascript.JsonStringSerializer.Global : serializer)
        {
            ReadSource(source);
        }

        /// <summary>
        /// The object delimiter associated with the string source.
        /// </summary>
        public static string ObjectDelimiter = "☺";

        /// <summary>
        /// The spacer for pretty json.
        /// </summary>
        public static string PrettySpacer = "\n";

        /// <summary>
        /// The raw source associated with the data provider.
        /// </summary>
        public string RawSource { get; private set; }

        /// <summary>
        /// Validates that a raw value exists.
        /// </summary>
        /// <param name="id"></param>
        void ValidateSourceValue(uint id, bool isPretty)
        {
            if (!SourceValues.ContainsKey(id))
            {
                SourceValues[id] = Serializer.ToJsonRepresentation(JsonValues[id], isPretty);
            }
        }

        public override void UpdateSource(bool isPretty = false)
        {
            Coding.CodeTimer timer = new Coding.CodeTimer();
            // writing to source.
            StringBuilder builder = new StringBuilder();
            builder.Append(CurId.ToString());
            timer.Mark("curid");
            AddObjectDilimeter(isPretty, builder);
            builder.Append(string.Join(",", Anchors.Select(a => a.ToString())));
            timer.Mark("anchors");
            AddObjectDilimeter(isPretty, builder);
            bool isFirst = true;
            ChildReferences.ForEach(kvp =>
            {
                if (!isFirst)
                {
                    builder.Append(",");
                }
                isFirst = false;
                builder.Append(kvp.Key);
                builder.Append(":");
                builder.Append(string.Join("|", kvp.Value.Select(v => v.ToString())));
            });
            timer.Mark("refrences");
            AddObjectDilimeter(isPretty, builder);
            builder.Append(Serializer.Serialize(Binder, true, isPretty).EscapeForDelimiter(ObjectDelimiter));
            timer.Mark("binder");

            uint[] allIds = JsonValues.Keys.Union(SourceValues.Keys).ToArray();

            allIds.ForEach(id => ValidateSourceValue(id, isPretty));
            timer.Mark("Id validation");

            foreach (uint id in allIds)
            {
                AddObjectDilimeter(isPretty, builder);
                builder.Append(id.ToString());
                AddObjectDilimeter(isPretty, builder);
                builder.Append(SourceValues[id].EscapeForDelimiter(ObjectDelimiter));
            }
            timer.Mark("objects");
            RawSource = builder.ToString();
            timer.Mark("tosource");
        }

        private static void AddObjectDilimeter(bool isPretty, StringBuilder builder)
        {
            if (isPretty)
                builder.Append(PrettySpacer);
            builder.Append(ObjectDelimiter);
            if (isPretty)
                builder.Append(PrettySpacer);
        }

        /// <summary>
        /// Reads the source for the data provider.
        /// </summary>
        /// <param name="source">If null, then assume empty.</param>
        public void ReadSource(string source = null)
        {
            Coding.ProcessTimer timer = new Coding.ProcessTimer();
            RawSource = source;
            JsonValues = new Dictionary<uint, Documents.IJsonValue<string>>();
            SourceValues = new Dictionary<uint, string>();

            if (source == null)
            {
                Anchors = new HashSet<uint>();
                ChildReferences = new Dictionary<uint, HashSet<uint>>();
                Binder = new SerializationTypeBinder<string>();
                return;
            }

            timer.Mark("Processing defaults");
            // splitting the source value and rewriting the source.
            string[] objs = source.SplitByDelimiter(ObjectDelimiter).Select(v => v.UnescapeForDelimiter(ObjectDelimiter).Trim(new char[] { ' ', '\n' })).ToArray();
            timer.Mark("Reading sources");
            if (objs.Length < 4)
                throw new Exception("Incorrect format when reading source.");

            // reading definitions.
            CurId = uint.Parse(objs[0]);
            Anchors = new HashSet<uint>(objs[1].Split(',').Select(id => uint.Parse(id)));
            timer.Mark("Anchors");
            //ChildReferences = Serializer.Deserialize<string, Dictionary<uint, HashSet<uint>>>(objs[2]);
            ChildReferences = new Dictionary<uint, HashSet<uint>>();
            foreach (string crcol in objs[2].Split(','))
            {
                string[] kvp = crcol.Split(':');
                if (kvp.Length != 2)
                    continue;
                ChildReferences[uint.Parse(kvp[0])] = new HashSet<uint>(kvp[1].Split('|').Select(v => uint.Parse(v)));
            }
            timer.Mark("Child refrences");
            Binder = Serializer.Deserialize<string, SerializationTypeBinder<string>>(objs[3]);
            timer.Mark("Binder");

            // reading the object values.
            for (int i = 4; i < objs.Length; i += 2)
            {
                uint id = uint.Parse(objs[i]);
                string val = objs[i + 1];
                SourceValues[id] = val;
            }
            timer.Mark("objects");
        }

        public override string GetSource()
        {
            return RawSource;
        }
    }
}
