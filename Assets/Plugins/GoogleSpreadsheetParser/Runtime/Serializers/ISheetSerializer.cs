namespace Yggdrasil.GoogleSpreadsheet
{
    public interface ISheetSerializer
    {
        string Serialize(SheetData data);
    }
}