namespace Save
{
    public interface ISaveSystem
    {
        void Save(BaseSaveData saveData);
        BaseSaveData Load();
    }
}