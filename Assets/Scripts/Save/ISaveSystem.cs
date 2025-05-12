namespace Save
{
    public interface ISaveSystem
    {
        string Save(BaseSaveData saveData, string fileName);
        BaseSaveData Load(string fileName);
        string GetLatestSaveFile();
        bool Delete(string fileName);
        string[] GetSaveFiles();
        public bool HasAnySave();
        public bool SaveExists(string fileName);
    }
}