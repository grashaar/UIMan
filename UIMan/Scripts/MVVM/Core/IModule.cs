namespace UnuGames.MVVM
{
    public interface IModule
    {
        object OriginalData
        {
            get;
            set;
        }

        ViewModelBehaviour ViewModel { get; }
    }
}