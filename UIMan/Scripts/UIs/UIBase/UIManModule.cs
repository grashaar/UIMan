using UnuGames.MVVM;

namespace UnuGames
{
    public class UIManModule<T> : ViewModelBehaviour, IModule where T : new()
    {
        private T dataInstance = new T();

        [UIManProperty]
        public virtual T DataInstance
        {
            get
            {
                return this.dataInstance;
            }

            set
            {
                this.dataInstance = value;
                NotifyModelPropertyChange(nameof(this.DataInstance), this.DataInstance);
            }
        }

        public object OriginalData
        {
            get
            {
                return this.DataInstance;
            }

            set
            {
                this.DataInstance = (T)value;
            }
        }

        public ViewModelBehaviour ViewModel
        {
            get
            {
                return this;
            }
        }
    }
}