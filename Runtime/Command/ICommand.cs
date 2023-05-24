namespace GameHelperSDK
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
