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
                NotifyModelChange(this.DataInstance);
            }
        }

        public object OriginalData
        {
            get
            {
                return (object)this.DataInstance;
            }
            set
            {
                this.DataInstance = (T)value;
            }
        }

        public ViewModelBehaviour VM
        {
            get
            {
                return this;
            }
        }
    }
}