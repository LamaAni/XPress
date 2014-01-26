using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Coding;
using XPress.Serialization.Core;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            AddMenuItem(TestNewJsonRefrenceBank);
            AddMenuItem(TestSingleObjectJsonSerialziation);
            AddMenuItem(TestNewJsonBinderSerialization);
            AddMenuItem(TestNewJson);
            MakeMenu();
        }

        #region menuu generation

        static List<Tuple<string, Action>> m_menuList = new List<Tuple<string, Action>>();
        static string m_filename = "testdump.txt";
        static bool WritePrettyJson = true;
        static int m_arraySize = 10;
        static int m_nestSize = 10;

        

        static void MakeMenu()
        {
            string lastmsg = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select an option from the following menu:");
                m_menuList.ForEach((itm, idx) =>
                {
                    Console.Write("\t(");
                    Console.Write(idx.ToString());
                    Console.Write(") ");
                    Console.WriteLine(itm.Item1);
                });

                Console.WriteLine();
                Console.WriteLine("type q or exit to exit.");
                Console.WriteLine();
                if (lastmsg != null)
                {
                    Console.WriteLine(lastmsg);
                    Console.WriteLine();
                }
                lastmsg = null;
                Console.Write("--> ");
                string val = Console.ReadLine();
                int mi = -1;
                if (int.TryParse(val, out mi))
                {
                    if (mi < 0 || mi >= m_menuList.Count)
                    {
                        lastmsg = "Invalid selection, number out of range";
                    }
                    else
                    {
                        m_menuList[mi].Item2();
                    }
                }
                else if (val == "q" || val == "exit")
                {
                    break;
                }
                else
                {
                    lastmsg = "Invalid selection.";
                }
            }
        }

        private static void AddMenuItem(Action<CodeTimer> a)
        {
            AddMenuItem(a, a.Method.Name);
        }

        private static void AddMenuItem(Action<CodeTimer> a, string text)
        {
            AddMenuItem(() =>
            {
                CodeTimer timer = new CodeTimer();
                Console.Clear();
                a(timer);
                timer.Stop();
                Console.Write(timer.ToTraceString("Serialization times"));
                Console.WriteLine();
                Console.WriteLine("Total time: " + timer.TotalTime + " , Press any key....");
                Console.WriteLine();
                Console.ReadKey();
            }, text);
        }

        private static void AddMenuItem(Action a)
        {
            AddMenuItem(a, a.Method.Name);
        }

        private static void AddMenuItem(Action a, string text)
        {
            m_menuList.Add(new Tuple<string, Action>(text, a));
        }

        #endregion

        #region run methods


        static void TestNewJsonRefrenceBank(CodeTimer timer)
        {
            XPress.Serialization.Javascript.JsonStringArrayDataProvider provider = new XPress.Serialization.Javascript.JsonStringArrayDataProvider();
            //XPress.Serialization.IJsonSerializer<string> serializer = XPress.Serialization.Javascript.JsonStringSerializer.Global;
            XPress.Serialization.Reference.JsonRefrenceBank<string> bank
                = new XPress.Serialization.Reference.JsonRefrenceBank<string>(provider);
            int N = 10000;
            Console.WriteLine("Testing json refrence bank storage.");
            for (int i = 0; i < N; i++)
                bank.Anchor(bank.Store(new TestObject()));
            timer.Mark("Single object anchored prepare (" + N + " objects)");
            bank.Update(false);
            timer.Mark("Single object anchored update, found " + bank.Collection.GetAllValueIds().Length + " refrence items");
            bank.CollectGarbage();
            timer.Mark("Single object grabage collection");
            bank.WriteToSource(false, false, true);
            timer.Mark("Single object anchored write");

            string source = provider.RawSource;

            provider = new XPress.Serialization.Javascript.JsonStringArrayDataProvider(source);
            bank = new XPress.Serialization.Reference.JsonRefrenceBank<string>(provider);
            timer.Mark("Created new data provider and bank from source (" + Math.Round(source.Length * 1.0 / 1000) + " Kb)");
            TestObject loadO = bank.Load(0) as TestObject;
            timer.Mark("Read a single object.");
        }

        static void TestSingleObjectJsonSerialziation(CodeTimer timer)
        {
            Console.WriteLine("Testing single object serialization:");
            XPress.Serialization.Javascript.JsonStringSerializer ser = XPress.Serialization.Javascript.JsonStringSerializer.Global;
            TestObject o = new TestObject();
            timer.Mark("Prepare");
            string source = ser.Serialize(o, false, true);
            timer.Mark("Serialize");
            o = ser.Deserialize<TestObject>(source);
            timer.Mark("Deserilaize (" + source.Length + " bytes)");
        }

        static void TestNewJsonBinderSerialization(CodeTimer timer)
        {
            XPress.Serialization.Javascript.JsonStringSerializer ser = XPress.Serialization.Javascript.JsonStringSerializer.Global;
            string source = "";
            Console.WriteLine("Testing binder serialization:");
            // Core value
            string coreVal = "abcd\n\n\t";
            source = ser.SerializeWithBinder(coreVal, true);
            Console.WriteLine("Serialzied data type, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("serialize core value serialization.");
            coreVal = ser.Deserialize<string>(source);
            Console.WriteLine("Serialzied data type, source lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Deserialize core value.");

            var inline = new
            {
                DicTest = new Dictionary<string, string>(),
                ListTest = new List<string>(new string[] { "a", "b", "c" }),
                SmallTest = 12,
            };

            inline.DicTest.Add("a", "a");
            inline.DicTest.Add("b", "b");

            Console.WriteLine("Testing inline object");
            source = ser.SerializeWithBinder(inline, true);
            Console.WriteLine("Serialzied object type, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test single object serialization");
            object inlineResult = ser.Deserialize<object>(source);
            Console.WriteLine("Deserialized object type, source lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test single object deserialization");

            Console.WriteLine("Testing typed object");
            TestObject testo = new TestObject();
            source = ser.SerializeWithBinder(testo, true);
            Console.WriteLine("Serialzied object type, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test single object serialization");
            testo = ser.Deserialize<TestObject>(source);
            Console.WriteLine("Deserialized object type, source lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test single object deserialization");

            Console.WriteLine("Testing array serialization:");
            int N = 10000;
            TestObject[] lst = new int[N].Select(i => new TestObject()).ToArray();
            timer.Mark("Prepare multi object with " + N + " items.");
            source = ser.SerializeWithBinder(lst, true);
            Console.WriteLine("Serialzied object array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + N + " objects serialization");
            lst = ser.Deserialize<TestObject[]>(source);
            Console.WriteLine("Deerialzied object array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + N + " objects deserialization");
            Console.WriteLine("Testing deserialziation:");

            int nestCount = 100;
            Console.WriteLine("Running nested (" + nestCount + ")");
            NestedTestObject nested = NestedTestObject.CreateNested(nestCount);
            source = ser.SerializeWithBinder(nested, true);
            Console.WriteLine("Serialzied nest array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + nestCount + " nested objects serialization");
            nested = ser.Deserialize<NestedTestObject>(source);
            Console.WriteLine("Deerialzied nest object array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + nestCount + " nested objects deserialization");
        }

        static void TestNewJsonSerializtion(CodeTimer timer)
        {
            XPress.Serialization.Javascript.JsonStringSerializer ser = XPress.Serialization.Javascript.JsonStringSerializer.Global;
            string source = "";

            Console.WriteLine("Testing core serialization:");
            // Core value
            string coreVal = "abcd\n\n\t";
            source = ser.Serialize(coreVal, true);
            Console.WriteLine("Serialzied data type, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("serialize core value serialization.");
            coreVal = ser.Deserialize<string>(source);
            Console.WriteLine("Serialzied data type, source lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Deserialize core value.");

            Console.WriteLine("Testing object serialization:");
            TestObject testo = new TestObject();
            source = ser.Serialize(testo, true);
            Console.WriteLine("Serialzied object type, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test single object serialization");
            testo = ser.Deserialize<TestObject>(source);
            Console.WriteLine("Deserialized object type, source lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test single object deserialization");

            Console.WriteLine("Testing array serialization:");
            int N = 10000;
            TestObject[] lst = new int[N].Select(i => new TestObject()).ToArray();
            timer.Mark("Prepare multi object with " + N + " items.");
            source = ser.Serialize(lst, true);
            Console.WriteLine("Serialzied object array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + N + " objects serialization");
            lst = ser.Deserialize<TestObject[]>(source);
            Console.WriteLine("Deerialzied object array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + N + " objects deserialization");
            Console.WriteLine("Testing deserialziation:");

            int nestCount = 100;
            Console.WriteLine("Running nested (" + nestCount + ")");
            NestedTestObject nested = NestedTestObject.CreateNested(nestCount);
            source = ser.Serialize(nested, true);
            Console.WriteLine("Serialzied nest array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + nestCount + " nested objects serialization");
            nested = ser.Deserialize<NestedTestObject>(source);
            Console.WriteLine("Deerialzied nest object array, result lenth: " + source.Length + ", ObjectCount: " + ser.LastProcessTotalNumberOfObjects);
            timer.Mark("Test array of " + nestCount + " nested objects deserialization");
        }

        static void TestNewJson(CodeTimer timer)
        {
            XPress.Serialization.Core.JavscriptJson.JsonStringReader reader = new XPress.Serialization.Core.JavscriptJson.JsonStringReader();
            XPress.Serialization.Core.JavscriptJson.JsonStringWriter writer = new XPress.Serialization.Core.JavscriptJson.JsonStringWriter();
            // string check = File.ReadAllText("resultjson.txt");
            string check = "asdasd\n\t";
            //check = "{^t#1:,v:[^t#12,1,2,3],a:b,c:}";
            int repeats = 1000;
            check = string.Join(check, new string[repeats + 1]);

            IEnumerable<object> stream = reader.GetRecursiveElementStream(check);
            timer.Mark("Read stream from source");
            Console.WriteLine("JSON TESTING: (" + repeats + " repetitions)");
            Console.WriteLine("Stream length: " + check.Length + " (" + (check.Length * 1.0 / 1000).ToString("#.00") + "Kb)");
            Console.WriteLine("# stream items: " + stream.Count());
            Console.WriteLine("# objects: " + (stream.Where(o => o is JsonStreamAction).Cast<JsonStreamAction>().
                Where(a => a == JsonStreamAction.BeginObject).Count()));
            Console.WriteLine("# arrays: " + (stream.Where(o => o is JsonStreamAction).Cast<JsonStreamAction>().
                Where(a => a == JsonStreamAction.BeginArray).Count()));
            Console.WriteLine("# data elements: " + (stream.Where(o => !(o is JsonStreamAction)).Count()));
            timer.Mark();
            XPress.Serialization.Documents.IJsonValue<string> val = reader.ReadStream(stream);
            timer.Mark("Convert stream to value.");
            string notValid = writer.ToJson(val, false, false);
            timer.Mark("Write unvalidated small back to compressed string");
            string written = writer.ToJson(val, false, true);
            timer.Mark("Write validated small back to compressed string");
            string writtenPretty = writer.ToJson(val, true, true);
            timer.Mark("Write small back to pretty string (validation already happend)");

            // doing double read and compare.
            Console.Write("Doing double read and compare...");
            string validateString = writer.ToJson(reader.FromJson(written), false, true);
            if (written == validateString)
            {
                Console.WriteLine(" ok.");
            }
            else Console.WriteLine(" FAIL!!!");
            timer.Mark("Validation.");
        }

        #endregion
    }
}
