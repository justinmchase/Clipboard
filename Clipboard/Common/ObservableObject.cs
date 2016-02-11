using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace TaoOfLeo.Clipboard.Common
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		private Queue<Func<Task>> _work = new Queue<Func<Task>>();
		private bool _isBusy;

		public bool IsBusy
		{
			get { return _isBusy; }
			set { Set(ref _isBusy, value, () => IsBusy); }
		}

		public void Queue(Func<Task> work)
		{
			if (work == null)
				throw new ArgumentNullException("work");

			this.IsBusy = true;
			_work.Enqueue(work);
			RunNext();
		}

		private async void RunNext()
		{
			if (_work.Any())
			{
				var work = _work.Dequeue();
				await work();
				RunNext();
			}
			else
			{
				this.IsBusy = false;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged = (o, e) => { };

		protected void Set<T>(ref T field, T value, params Expression<Func<object>>[] propertyReferences)
		{
			if (!object.ReferenceEquals(field, value))
			{
				field = value;
                OnPropertyChanged(propertyReferences);
			}
		}

        protected void OnPropertyChanged(params Expression<Func<object>>[] properties)
        {
            var names = properties.Select(p => ((MemberExpression)p.Body).Member.Name).ToArray();
            OnPropertyChanged(names);
        }

        protected void OnPropertyChanged(params string[] properties)
        {
            foreach (var name in properties)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected void UpdateAll()
        {
            this.OnPropertyChanged(this.GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Select(p => p.Name)
                .ToArray());
        }
	}
}
