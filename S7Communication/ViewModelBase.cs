using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace S7Communication
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel
    {
        [XmlIgnore]
        [field: NonSerialized]
        public CompositeDisposable Disposables { get; set; } = new CompositeDisposable();


        #region IActivatableViewModel
        [field: NonSerialized]
        public ViewModelActivator Activator { get; }

        public ViewModelBase() => Activator = new ViewModelActivator();
        #endregion

        #region CreateOrGet
        private Dictionary<string, object> m_DictCommandActions = new Dictionary<string, object>();

        public ReactiveCommand<Unit, Unit> CreateOrGet(Action execute, [CallerMemberName] string callerMemberName = null)
        {
            if (string.IsNullOrEmpty(callerMemberName))
            {
                return ReactiveCommand.Create(execute);
            }

            if (m_DictCommandActions.TryGetValue(callerMemberName, out object result))
            {
                return result as ReactiveCommand<Unit, Unit>;
            }

            ReactiveCommand<Unit, Unit> command = ReactiveCommand.Create(execute);
            m_DictCommandActions.Add(callerMemberName, command);
            return command;
        }

        public ReactiveCommand<Unit, TResult> CreateOrGet<TResult>(Func<TResult> execute, [CallerMemberName] string callerMemberName = null)
        {
            if (string.IsNullOrEmpty(callerMemberName))
            {
                return ReactiveCommand.Create(execute);
            }

            if (m_DictCommandActions.TryGetValue(callerMemberName, out object result))
            {
                return result as ReactiveCommand<Unit, TResult>;
            }

            ReactiveCommand<Unit, TResult> command = ReactiveCommand.Create(execute);
            m_DictCommandActions.Add(callerMemberName, command);
            return command;
        }

        public ReactiveCommand<TParam, Unit> CreateOrGet<TParam>(Action<TParam> execute,
            [CallerMemberName] string callerMemberName = null)
        {
            if (string.IsNullOrEmpty(callerMemberName))
            {
                return ReactiveCommand.Create(execute);
            }

            if (m_DictCommandActions.TryGetValue(callerMemberName, out object result))
            {
                return result as ReactiveCommand<TParam, Unit>;
            }

            ReactiveCommand<TParam, Unit> command = ReactiveCommand.Create(execute);
            m_DictCommandActions.Add(callerMemberName, command);
            return command;
        }

        public ReactiveCommand<TParam, TResult> CreateOrGet<TParam, TResult>(Func<TParam, TResult> execute, [CallerMemberName] string callerMemberName = null)
        {
            if (string.IsNullOrEmpty(callerMemberName))
            {
                return ReactiveCommand.Create<TParam, TResult>(execute);
            }

            if (m_DictCommandActions.TryGetValue(callerMemberName, out object result))
            {
                return result as ReactiveCommand<TParam, TResult>;
            }

            ReactiveCommand<TParam, TResult> command = ReactiveCommand.Create<TParam, TResult>(execute);
            m_DictCommandActions.Add(callerMemberName, command);
            return command;
        }
        #endregion

    }
}
