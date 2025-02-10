namespace Yggdrasil.GoogleSpreadsheet
{
    public interface IProgressHandler
    {
        void Show(string title, string message);

        void Hide();
    }
}