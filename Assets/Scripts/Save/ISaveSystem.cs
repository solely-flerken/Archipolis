namespace Save
{
    public interface ISaveSystem
    {
        string Save(BaseSaveData saveData, string fileName);
        BaseSaveData Load(string fileName);
        BaseSaveData LoadLatest();
        bool Delete(string fileName);
    }
}