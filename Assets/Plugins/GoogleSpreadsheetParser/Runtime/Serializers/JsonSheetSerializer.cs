using Newtonsoft.Json.Linq;

namespace Yggdrasil.GoogleSpreadsheet
{
    public class JsonSheetSerializer : ISheetSerializer
    {
        string ISheetSerializer.Serialize(SheetData data)
        {
            var jObject = SerializeToJObject(data);
            return jObject.ToString();
        }

        private JObject SerializeToJObject(SheetData data)
        {
            var result = new JObject();

            if (data.Values != null)
            {
                foreach (var (key, value) in data.Values)
                {
                    result[key] = JToken.FromObject(value);
                }
            }

            if (data.Children != null)
            {
                for (var i = 0; i < data.Children.Count; i++)
                {
                    var child = data.Children[i];
                    var key = child.Key ?? (i + 1).ToString();
                    result[key] = SerializeToJObject(child);
                }
            }

            return result;
        }
    }
}